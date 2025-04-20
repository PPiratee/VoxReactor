

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBlushSettings : ConfigInstanceStorable
    {
        public readonly JSONStorableBool blushEnabled;
        public readonly JSONStorableBool emotionEmbarrasedSetsMinimumBLush;
        public readonly JSONStorableBool emotionHornySetsMinimumBlush;
        public readonly JSONStorableFloat blushDurationMin;
        public readonly JSONStorableFloat blushDurationMax;


        public ConfigCharacterBlushSettings(ConfigInstanceStorable parent): base(parent) {
            blushEnabled = new JSONStorableBool(GetStorableId("BlushEnabled"), true);
            Main.singleton.RegisterBool(blushEnabled);

            emotionEmbarrasedSetsMinimumBLush = new JSONStorableBool(GetStorableId("EmotionEmbarrasedSetsMinimumBLush"), true);
            Main.singleton.RegisterBool(emotionEmbarrasedSetsMinimumBLush);

            emotionHornySetsMinimumBlush = new JSONStorableBool(GetStorableId("EmotionHornySetsMinimumBLush"), false);
            Main.singleton.RegisterBool(emotionHornySetsMinimumBlush);

            blushDurationMin = new JSONStorableFloat(GetStorableId("BlushDurationMin"), 0f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMin);

            blushDurationMax = new JSONStorableFloat(GetStorableId("BlushDurationMax"), 0f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMax);
        }
    }
}
