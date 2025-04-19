

namespace PPirate.VoxReactor
{
    //using static EmotionManager;
    internal class ConfigCharacterSpecificEmotion : ConfigInstanceStorable
    {

        public readonly JSONStorableBool emotionEnabled;
        public readonly string emotionName;

       // public readonly JsonStorableBool emotionsEnabled;

        public ConfigCharacterSpecificEmotion(string emotionName, ConfigInstanceStorable parent): base(emotionName, parent) {
            this.emotionName = emotionName;
            //this.emotionType = emotionType;
            emotionEnabled = new JSONStorableBool(GetStorableId("EmotionEnabled"), true);
            Main.singleton.RegisterBool(emotionEnabled);
        }
    }
}
