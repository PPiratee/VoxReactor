

namespace PPirate.VoxReactor
{
    //using static EmotionManager;

    internal class ConfigCharacterEmotions : ConfigInstanceStorable
    {

        public readonly JSONStorableBool emotionsEnabled;
        public readonly ConfigCharacterSpecificEmotion hornienessConfig;
        public readonly ConfigCharacterSpecificEmotion happynessConfig;

        public ConfigCharacterEmotions(ConfigInstanceStorable parent): base(parent) {
            emotionsEnabled = new JSONStorableBool(GetStorableId("EmotionsEnabled"), true);
            Main.singleton.RegisterBool(emotionsEnabled);

            hornienessConfig = new ConfigCharacterSpecificEmotion(Hornieness.hornieNessName, this);
            happynessConfig = new ConfigCharacterSpecificEmotion(Happyness.happynessName, this);
        }
    }
}
