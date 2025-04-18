

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterEmotions : ConfigInstanceStorable
    {
        public readonly JSONStorableBool emotionsEnabled;
        public ConfigCharacterEmotions(ConfigInstanceStorable parent): base(parent) {
            emotionsEnabled = new JSONStorableBool(GetStorableId("EmotionsEnabled"), true);
            Main.singleton.RegisterBool(emotionsEnabled);
        }
    }
}
