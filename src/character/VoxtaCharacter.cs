using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class VoxtaCharacter
    {


        public readonly Main main;
        public readonly Atom atom;
        public readonly string name;
        public readonly int characterNumber;
        public readonly string role;


        public readonly VoxtaService voxtaService;

        public readonly PersonPluginsHolder plugins;

        public readonly ObserverRegistry actionObserverRegistry;

        public readonly StateManager stateManager;
        public readonly EmotionManager emotionManager;
        public readonly GazeManager gazeManager;
        public readonly VoiceManager voiceManager;
        public readonly TouchManager touchManager;
        public readonly LookingAtManager lookingAtManager;
        public readonly SexManager sexManager;
        public readonly HornyManager hornyManager;
        public readonly DirtyTalkManager dirtyTalkManager;
        public readonly ShakeManager shakeManager;
        public readonly BlowJobManager blowjobManager;
        public readonly ClothingManager clothingManager;


        public readonly EventShowTits eventShowTits;



        private Logger logger;
        public VoxtaCharacter(int characterNumber, string name, Atom atom, Main main, VoxtaService voxtaService)
        {

            
            try
            {

                logger = new Logger("VoxtaCharacter:Char#" + characterNumber);
                logger.StartMethod("Constructor"); 
                logger.DEBUG("VoxtaCharacter - I am constructed, my name is " + name);
                this.characterNumber = characterNumber;
                this.name = name;
                this.atom = atom;
                this.main = main;
                this.voxtaService = voxtaService;


                actionObserverRegistry = new ObserverRegistry();

                stateManager = new StateManager(this);
                plugins = new PersonPluginsHolder(this);
                gazeManager = new GazeManager(this);
                emotionManager = new EmotionManager( this);//depends on gazeManager being set
                voiceManager = new VoiceManager(this);
                touchManager = new TouchManager(this);
                lookingAtManager = new LookingAtManager(this);
                sexManager = new SexManager(this);
                hornyManager = new HornyManager(this);
                dirtyTalkManager = new DirtyTalkManager(this);
                shakeManager = new ShakeManager(this);
                blowjobManager = new BlowJobManager(this);
                clothingManager = new ClothingManager(this);

                eventShowTits = new EventShowTits(this);

            }
            catch (Exception e)
            {
                logger.ERR("FAILURE  Constructor: " + e);
            }
        }
        public VoxtaCharacter(int characterNumber, string name)//todod remove this
        {
            this.characterNumber = characterNumber;
            this.name = name;
        }
    }
}
