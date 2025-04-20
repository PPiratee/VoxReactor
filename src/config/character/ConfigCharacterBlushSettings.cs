

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBlushSettings : ConfigInstanceStorable
    {
        public readonly JSONStorableBool blushEnabled;
        public readonly JSONStorableBool emotionEmbarrasedSetsMinimumBLush;
        public readonly JSONStorableFloat blushDurationMin;
        public readonly JSONStorableFloat blushDurationMax;


        public ConfigCharacterBlushSettings(ConfigInstanceStorable parent): base(parent) {
            blushEnabled = new JSONStorableBool(GetStorableId("BlushEnabled"), true);
            Main.singleton.RegisterBool(blushEnabled);

            emotionEmbarrasedSetsMinimumBLush = new JSONStorableBool(GetStorableId("EmotionEmbarrasedSetsMinimumBLush"), true);
            Main.singleton.RegisterBool(emotionEmbarrasedSetsMinimumBLush);

            blushDurationMin = new JSONStorableFloat(GetStorableId("BlushDurationMin"), 0f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMin);

            blushDurationMax = new JSONStorableFloat(GetStorableId("BlushDurationMax"), 0f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMax);
        }
    }
}
