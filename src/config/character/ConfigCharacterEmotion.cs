

namespace PPirate.VoxReactor
{
    public class ConfigCharacterEmotion : ConfigInstanceStorable
    {
        public readonly JSONStorableBool emotionsEnabled;
        public ConfigCharacterEmotion(MVRScript mVRScript, ConfigInstanceStorable parent): base(parent) {
            emotionsEnabled = new JSONStorableBool(GetStorableId("EmotionsEnabled"), true);
            mVRScript.RegisterBool(emotionsEnabled);
        }
    }
}
