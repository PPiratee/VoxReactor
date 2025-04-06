using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using MVR;
using MeshVR;
using System.Linq;
using Leap.Unity;
using MVR.FileManagementSecure;
using System.Security.AccessControl;


namespace PPirate.VoxReactor
{
	public class ClothingPluginManager : MVRScript {

		string fileExt = "clothingplugins";

		DAZDynamic dazDynamic;
		DAZDynamicItem dazDynamicItem;
		MVRPluginManager pluginManager;
		PresetManager presetManager;
		Destructor destruct;

		bool needsReload = false;

		JSONClass sceneJson = null;
		bool lastRestoreWasSceneRestore = false;

		public override void Init() {
			WalkAndGetParentItems();
			if(containingAtom == null) throw new Exception("Unable to determine containing atom.");
			if(pluginManager == null) throw new Exception("Unable to find plugin manager.");
			if(dazDynamicItem == null) throw new Exception("Unable to find dynamic item.");
			if(dazDynamic == null) throw new Exception("Unable to find dazDynamic item.");

			this.overrideId = $"{dazDynamic.uid}:{this.storeId}";

			DAZCharacterSelector selector = containingAtom.GetStorableByID("geometry") as DAZCharacterSelector;
			if(selector == null) throw new Exception("Unable to find CharacterSelector");


			pluginManager.containingAtom = containingAtom;
			pluginManager.gameObject.SetActive(true);
			pluginManager.exclude = true;

			presetManager = dazDynamic.GetComponentInChildren<PresetManager>();
			List<PresetManager.SpecificStorable> specificStorables = presetManager.specificStorables.ToList();

			PresetManager.SpecificStorable specificStorable = null;
			foreach(var storable in specificStorables) {
				if(storable.specificKey == "ClothingPluginManager") {
					specificStorable = storable;
					specificStorable.specificStorableBucket = this.transform;
				}
			}
			if(specificStorable == null) {
				specificStorable = new PresetManager.SpecificStorable();
				specificStorable.specificKey = "ClothingPluginManager";
				specificStorable.storeId = this.storeId;
				specificStorable.specificStorableBucket = this.transform;
				specificStorable.includeChildren = false;
			}
			
			presetManager.specificStorables = specificStorables.ToArray();
			presetManager.RefreshStorables();

			// Add the destructor to unload mods if we do a force reload
			if(dazDynamicItem != selector.femaleClothingCreatorItem && dazDynamicItem != selector.maleClothingCreatorItem) {
				destruct = dazDynamic.binaryStorableBucket.gameObject.AddComponent<Destructor>();
				destruct.pluginManager = pluginManager;
				destruct.storedContainingAtom = containingAtom;
			}


			// If this isn't set on the plugin manager by the time we reach here, then
			// this plugin will not show up in the UI. In that case we'll want to reload the plugins.
			if(pluginManager.pluginPanelPrefab == null) {
				needsReload = true;
			}
			// Copy over all of the prefabs
			MVRPluginManager containingAtomPluginManager = containingAtom.GetStorableByID("PluginManager") as MVRPluginManager;
			if(containingAtomPluginManager != null){
				pluginManager.pluginPanelPrefab = containingAtomPluginManager.pluginPanelPrefab;
				pluginManager.scriptControllerPanelPrefab = containingAtomPluginManager.scriptControllerPanelPrefab;
				pluginManager.scriptUIPrefab = containingAtomPluginManager.scriptUIPrefab;
				pluginManager.configurableSliderPrefab = containingAtomPluginManager.configurableSliderPrefab;
				pluginManager.configurableTogglePrefab = containingAtomPluginManager.configurableTogglePrefab;
				pluginManager.configurableColorPickerPrefab = containingAtomPluginManager.configurableColorPickerPrefab;
				pluginManager.configurableButtonPrefab = containingAtomPluginManager.configurableButtonPrefab;
				pluginManager.configurablePopupPrefab = containingAtomPluginManager.configurablePopupPrefab;
				pluginManager.configurableScrollablePopupPrefab = containingAtomPluginManager.configurableScrollablePopupPrefab;
				pluginManager.configurableFilterablePopupPrefab = containingAtomPluginManager.configurableFilterablePopupPrefab;
				pluginManager.configurableTextFieldPrefab = containingAtomPluginManager.configurableTextFieldPrefab;
				pluginManager.configurableSpacerPrefab = containingAtomPluginManager.configurableSpacerPrefab;
			} else {
				throw new Exception("Unable to find plugin manager for main atom.");
			}

			// If the Daz clothing item needs a post-load restore, then we are still in scene restore
			// In that case we want to load the defaults after the item loads, but before the post-load
			// restore (which will apply scene settings)
			if(!dazDynamicItem.enabled && dazDynamicItem.needsPostLoadJSONRestore) {
				dazDynamicItem.onLoadedHandlers += PostLoad;
			} else if (needsReload) {
				StartCoroutine("PostLoadAfterDelay");
			}

			StartCoroutine("AddPluginTabToUI");
		}

		public void Start() {
			// UI 

			// Preset code was adapted from AcidBubbles' Collider Editor
			var savePath = dazDynamic.GetStoreFolderPath(false);

			
			var savePresetUI = CreateButton("Save Plugin Presets");
			savePresetUI.button.onClick.AddListener(() =>
			{
				FileManagerSecure.CreateDirectory(savePath);
				var fileBrowserUI = SuperController.singleton.fileBrowserUI;
				fileBrowserUI.SetTitle("Save Plugin Presets");
				fileBrowserUI.fileRemovePrefix = null;
				fileBrowserUI.hideExtension = false;
				fileBrowserUI.keepOpen = false;
				fileBrowserUI.fileFormat = fileExt;
				fileBrowserUI.defaultPath = savePath;
				fileBrowserUI.showDirs = true;
				fileBrowserUI.shortCuts = null;
				fileBrowserUI.browseVarFilesAsDirectories = false;
				fileBrowserUI.SetTextEntry(true);
				fileBrowserUI.Show(SavePreset);
				fileBrowserUI.fileEntryField.text = GetDefaultFileName() + "." + fileExt;
				fileBrowserUI.ActivateFileNameField();
			});
			var loadPresetUI = CreateButton("Load Plugin Presets");
			loadPresetUI.button.onClick.AddListener(() =>
			{
				FileManagerSecure.CreateDirectory(savePath);
				var shortcuts = FileManagerSecure.GetShortCutsForDirectory(savePath);
				SuperController.singleton.GetMediaPathDialog(LoadPreset, fileExt, savePath, false, true, false, null, false, shortcuts);
			});
			
			this.exclude = false;
		}

		/*
			There might be a more elegant way to do this but this works for now.
		*/
		private void WalkAndGetParentItems(){
			GameObject parent = this.gameObject;
			while(this.containingAtom == null || parent != this.containingAtom.gameObject){
				if(parent.transform.parent == null){
					return;
				}
				if(pluginManager == null){
					pluginManager = parent.GetComponent<MVRPluginManager>();
				}
				if(dazDynamic == null){
					dazDynamic = parent.GetComponent<DAZDynamic>();
				}
				if(dazDynamicItem == null){
					dazDynamicItem = parent.GetComponent<DAZDynamicItem>();
				}
				this.containingAtom = parent.GetComponent<Atom>();
				parent = parent.transform.parent.gameObject;
			}
		}
		
		private void PostLoad() {
			SyncClothSimSettings();

			string namedDefaultFile = $"{dazDynamic.GetStoreFolderPath(true)}{GetDefaultFileName()}.{fileExt}";
			string defaultFile = $"{dazDynamic.GetStoreFolderPath(true)}default.{fileExt}";
			
			if(lastRestoreWasSceneRestore && sceneJson != null && sceneJson.Count > 0){
				LoadFromJSON(sceneJson);
			} else if(FileManagerSecure.FileExists(namedDefaultFile)){
				LoadPreset(namedDefaultFile);
			} else if (FileManagerSecure.FileExists(defaultFile)){
				LoadPreset(defaultFile);
			} else if (needsReload) {
				// If there is no default, we may still want to re-load plugins once now that the 
				// plugin manager is fully set up. This is mostly to make sure that this plugin
				// shows up in the UI. 
				pluginManager.LateRestoreFromJSON(pluginManager.GetJSON());
			}
		}

		// From the dotnet runtime, since plugins don't have access to System.IO.Path
		private static char[] GetInvalidFileNameChars() => new char[] {
			'\"', '<', '>', '|', '\0',
			(char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
			(char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
			(char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
			(char)31, ':', '*', '?', '\\', '/'
		};

		private string GetDefaultFileName(){
			return String.Join("_", dazDynamicItem.displayName.Split(GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries) ).TrimEnd('.') + ".default";
		}

		/* 
			VAM has an issue on subsequent forced reloads where the ClothSimControl
			may get out of sync with the ClothSettings. This fixes that.
		*/
		private void SyncClothSimSettings(){
			ClothSimControl clothSimControl = dazDynamicItem.GetComponentInChildren<ClothSimControl>();
			if(clothSimControl != null) {
				SyncStorable(clothSimControl, "simEnabled");
				SyncStorable(clothSimControl, "integrateEnabled");
				SyncStorable(clothSimControl, "collisionEnabled");
				SyncStorable(clothSimControl, "collisionRadius");
				SyncStorable(clothSimControl, "drag");
				SyncStorable(clothSimControl, "weight");
				SyncStorable(clothSimControl, "distanceScale");
				SyncStorable(clothSimControl, "stiffness");
				SyncStorable(clothSimControl, "compressionResistance");
				SyncStorable(clothSimControl, "friction");
				SyncStorable(clothSimControl, "staticMultiplier");
				SyncStorable(clothSimControl, "collisionPower");
				SyncStorable(clothSimControl, "gravityMultiplier");
				SyncStorable(clothSimControl, "iterations");
				SyncStorable(clothSimControl, "allowDetach");
				SyncStorable(clothSimControl, "detachThreshold");
				SyncStorable(clothSimControl, "jointStrength");
				SyncStorable(clothSimControl, "force");
			}
		}

		private void SyncStorable(JSONStorable parent, string name) {
			var storable = parent.GetParam(name);
			var storableBool = storable as JSONStorableBool;
			if(storableBool != null && storableBool.setCallbackFunction != null) {
				storableBool.setCallbackFunction(storableBool.val);
			}
			var storableFloat = storable as JSONStorableFloat;
			if(storableFloat != null && storableFloat.setCallbackFunction != null) {
				storableFloat.setCallbackFunction(storableFloat.val);
			}
			var storableVector = storable as JSONStorableVector3;
			if(storableVector != null && storableVector.setCallbackFunction != null) {
				storableVector.setCallbackFunction(storableVector.val);
			}
		}

		private IEnumerator PostLoadAfterDelay() {
			yield return new WaitForEndOfFrame();

			PostLoad();	
			yield break;
		}

		public IEnumerator AddPluginTabToUI() {
			MVRPluginManager containingAtomPluginManager = containingAtom.GetStorableByID("PluginManager") as MVRPluginManager;
			if(containingAtomPluginManager == null) throw new Exception("Cannot find plugin manager for parent atom.");
			
			// Wait for the Person's plugin manager UI to be loaded.
			// This should happen when the Person's UI is first opened.
            Transform pluginManagerUITransform = null;
			while(pluginManagerUITransform == null) {
				yield return null;
				pluginManagerUITransform = containingAtomPluginManager.UITransform;
			}

			// Wait for the clothing item's customization UI to be created
			// This should happen when the user clicks 'Customize' on the clothing
			Transform customizationUI = dazDynamicItem.customizationUI;
			while(customizationUI == null) {
				yield return null;
				customizationUI = dazDynamicItem.customizationUI;
			}

			// Wait for the customization UI to be active
			while(!customizationUI.gameObject.activeInHierarchy) {
				yield return null;
			}
			
			// This should have been initialized with the PluginManager's IO but I've seen it null here. 
			// Race conditions? Me being dumb? Who knows. Adding wait loop.
			MVRPluginManagerUI containingAtomPluginManagerUI = pluginManagerUITransform.GetComponentInChildren<MVRPluginManagerUI>(true);
			while(containingAtomPluginManagerUI == null) {
				yield return new WaitForSeconds(0.1f);
				containingAtomPluginManagerUI = pluginManagerUITransform.GetComponentInChildren<MVRPluginManagerUI>(true);
			}

			UIConnectorMaster uiConnectorMaster = customizationUI.GetComponent<UIConnectorMaster>();
			TabbedUIBuilder tabbedUIBuilder = uiConnectorMaster.GetComponentInChildren<TabbedUIBuilder>();

			foreach(var child in tabbedUIBuilder.selector.toggleContainer.GetChildren()) {
				if(child.name == "Plugins") {
					yield break;
				}
			}
			
			Transform pluginTabPrefab = UnityEngine.Object.Instantiate<Transform>(containingAtomPluginManagerUI.transform,tabbedUIBuilder.selector.transform);

			// Remove any plugin UIs we inadvertently copied over
			foreach(var script in pluginTabPrefab.GetComponentsInChildren<MVRScriptUI>(true)) {
				Destroy(script.gameObject);
			}

			foreach(var script in pluginTabPrefab.GetComponentsInChildren<MVRPluginUI>(true)) {
				Destroy(script.gameObject);
			}

			
			TabbedUIBuilder.Tab pluginsTab = new TabbedUIBuilder.Tab{name = "Plugins", prefab = pluginTabPrefab, color = Color.blue};
			tabbedUIBuilder.AddTab(pluginsTab);

			pluginManager.UITransform = pluginTabPrefab.transform;
			pluginManager.InitUI();
			
			yield break;
		}

		public void OnEnable() {
			if(containingAtom != null) {
				StartCoroutine("AddPluginTabToUI");
			}
		}

		public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false) {
			
			JSONClass jc = pluginManager.GetJSON(true,true,true);
			jc["id"] = this.storeId;

			JSONArray storables = new JSONArray();
			foreach(MVRScript script in pluginManager.GetComponentsInChildren<MVRScript>(true)) {
				if(script != this) {
					storables.Add(script.GetJSON());
				}
			}
			
			jc["storables"] = storables;

			this.needsStore = true;
		
			return jc;
		}


		public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true){
			if(this.isPresetRestore) {
				if(jc.Count > 0) {
					LoadFromJSON(jc);
				}								
			} else {
				sceneJson = jc;
				lastRestoreWasSceneRestore = SuperController.singleton.isLoading;
			}
        }

		private void LoadPreset(string path) {
			if (string.IsNullOrEmpty(path))
				return;
			
			var presetJson = (JSONClass)LoadJSON(path);
			
			LoadFromJSON(presetJson);
		}

		private void LoadFromJSON(JSONClass jc) {
			pluginManager.LateRestoreFromJSON(jc, true, true, false);

			if(jc.HasKey("storables")) {
				JSONArray storables = jc["storables"].AsArray;

				foreach(MVRScript script in pluginManager.GetComponentsInChildren<MVRScript>(true)) {
					foreach(JSONClass storable in storables) {
						string id = storable["id"];

						if(id == script.storeId) {
							script.RestoreFromJSON(storable);
							script.LateRestoreFromJSON(storable);
						}
					}
				}
			}
		}
		
		private void SavePreset(string path) {
			SuperController.singleton.fileBrowserUI.fileFormat = null;
			if (string.IsNullOrEmpty(path)) {
				return;
			}

			if (!path.ToLower().EndsWith($".{fileExt}")) {
				path += $".{fileExt}";
			}

			SaveJSON(GetJSON(true,true,true), path);
		}

		/*
			A forced reload of the parent clothing will only clear specific
			types of components. MaterialOptions are one of them, and should 
			otherwise be inert. 

			Using this to detect that the clothing is reloading and clearing
			the plugins. 

			Not doing this causes the plugin managers and plugins to accumulate.
		*/
		public class Destructor : MaterialOptions {
			public MVRPluginManager pluginManager = null;
			public Atom storedContainingAtom = null;

			public Destructor() {
				this.overrideId = "ClothingPluginDestructor";
				this.paramMaterialSlots = new int[0];
			}

			public void OnDestroy(){
				if(pluginManager != null) {
					foreach(var manager in pluginManager.gameObject.GetComponents<MVRPluginManager>()) {
						if(manager != pluginManager) {
							SuperController.LogError("Multiple clothing plugin managers detected on deletion. This generally shouldn't happen. Please file a bug report.");
						}
						manager.containingAtom = storedContainingAtom;
						manager.RemoveAllPlugins();
						Destroy(manager);
					}
				}
			}

			public override void SetUI(Transform t) {
				// No-op
			}
		}
    }
}
