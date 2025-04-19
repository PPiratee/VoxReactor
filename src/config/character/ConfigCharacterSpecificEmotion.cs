

namespace PPirate.VoxReactor
{
    //using static EmotionManager;
    internal class ConfigCharacterSpecificEmotion : ConfigInstanceStorable
    {

        public readonly string emotionName;
        public readonly JSONStorableBool emotionEnabled;
        public readonly JSONStorableFloat hornynessMultiplier;
        public readonly JSONStorableFloat happynessMultiplier;
        public readonly JSONStorableFloat sadnessMultiplier;
        public readonly JSONStorableFloat angerMultiplier;
        public readonly JSONStorableFloat embarrassmentMultiplier;





        public ConfigCharacterSpecificEmotion(string emotionName, ConfigInstanceStorable parent): base(emotionName, parent) {

            this.emotionName = emotionName;
  
            emotionEnabled = new JSONStorableBool(GetStorableId("EmotionEnabled"), true);
            Main.singleton.RegisterBool(emotionEnabled);

            this.hornynessMultiplier = new JSONStorableFloat(GetStorableId("HornynessMultiplier"), 0, -10, 10);
            Main.singleton.RegisterFloat(this.hornynessMultiplier);

            this.happynessMultiplier = new JSONStorableFloat(GetStorableId("HappynessMultiplier"), 0, -10, 10);
            Main.singleton.RegisterFloat(this.happynessMultiplier);

            this.sadnessMultiplier = new JSONStorableFloat(GetStorableId("SadnessMultiplier"), 0, -10, 10);
            Main.singleton.RegisterFloat(this.sadnessMultiplier);

            this.angerMultiplier = new JSONStorableFloat(GetStorableId("AngerMultiplier"), 0, -10, 10);
            Main.singleton.RegisterFloat(this.angerMultiplier);

            this.embarrassmentMultiplier = new JSONStorableFloat(GetStorableId("EmbarrasementMultiplier"), 0, -10, 10);
            Main.singleton.RegisterFloat(this.embarrassmentMultiplier);

        }

        public ConfigCharacterSpecificEmotion SetHornynessMultiplier(float value)
        {
            hornynessMultiplier.val = value;
            hornynessMultiplier.SetDefaultFromCurrent();
            return this;
        }

        public ConfigCharacterSpecificEmotion SetHappynessMultiplier(float value)
        {
            happynessMultiplier.val = value;
            happynessMultiplier.SetDefaultFromCurrent();
            return this;
        }

        public ConfigCharacterSpecificEmotion SetSadnessMultiplier(float value)
        {
            sadnessMultiplier.val = value;
            sadnessMultiplier.SetDefaultFromCurrent();
            return this;
        }

        public ConfigCharacterSpecificEmotion SetAngerMultiplier(float value)
        {
            angerMultiplier.val = value;
            angerMultiplier.SetDefaultFromCurrent();
            return this;
        }

        public ConfigCharacterSpecificEmotion SetEmbarrassmentMultiplier(float value)
        {
            embarrassmentMultiplier.val = value;
            embarrassmentMultiplier.SetDefaultFromCurrent();
            return this;
        }

    }
}
