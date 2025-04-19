

namespace PPirate.VoxReactor
{
    //using static EmotionManager;
    internal class ConfigCharacterSpecificEmotion : ConfigInstanceStorable
    {

        public readonly JSONStorableBool emotionEnabled;
        private readonly string emotionName;

        public ConfigCharacterSpecificEmotion(string emotionName, ConfigInstanceStorable parent): base(emotionName, parent) {
            this.emotionName = emotionName;
            //this.emotionType = emotionType;
            // emotionsEnabled = new JSONStorableBool(GetStorableId("EmotionsEnabled"), true);
            //Main.singleton.RegisterBool(emotionsEnabled);
        }
    }
}
