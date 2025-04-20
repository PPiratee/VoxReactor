

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBlushSettings : ConfigInstanceStorable
    {
        public readonly JSONStorableBool blushEnabled;

        public readonly JSONStorableBool emotionEmbarrasedSetsMinBlush;
        public readonly JSONStorableBool emotionHornySetsMinBlush;
        public readonly JSONStorableBool bodyLanguageStimulationSetsMinBlush;

        public readonly JSONStorableFloat blushSpeed;
        public readonly JSONStorableFloat deBlushSpeed;

        public readonly JSONStorableFloat blushDurationMin;
        public readonly JSONStorableFloat blushDurationMax;


        public ConfigCharacterBlushSettings(ConfigInstanceStorable parent): base(parent) {
            blushEnabled = new JSONStorableBool(GetStorableId("BlushEnabled"), true);
            Main.singleton.RegisterBool(blushEnabled);

            emotionEmbarrasedSetsMinBlush = new JSONStorableBool(GetStorableId("EmotionEmbarrasedSetsMinBLush"), true);
            Main.singleton.RegisterBool(emotionEmbarrasedSetsMinBlush);

            emotionHornySetsMinBlush = new JSONStorableBool(GetStorableId("EmotionHornySetsMinBLush"), false);
            Main.singleton.RegisterBool(emotionHornySetsMinBlush);

            bodyLanguageStimulationSetsMinBlush = new JSONStorableBool(GetStorableId("BodyLanguageStimulationSetsMinBlush"), true);
            Main.singleton.RegisterBool(bodyLanguageStimulationSetsMinBlush);

            blushDurationMin = new JSONStorableFloat(GetStorableId("BlushDurationMin"), 7, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMin);

            blushDurationMax = new JSONStorableFloat(GetStorableId("BlushDurationMax"), 20, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMax);


            blushSpeed = new JSONStorableFloat(GetStorableId("blushSpeed"), 1.2f, 0, 2, true, true);
            Main.singleton.RegisterFloat(blushSpeed);

            deBlushSpeed = new JSONStorableFloat(GetStorableId("deBlushSpeed"), 0.25f, 0, 2, true, true);
            Main.singleton.RegisterFloat(deBlushSpeed);
        }
    }
}
