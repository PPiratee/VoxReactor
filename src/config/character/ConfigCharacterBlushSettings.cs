

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBlushSettings : ConfigInstanceStorable
    {
        public readonly JSONStorableBool blushEnabled;
        public readonly JSONStorableFloat blushDurationMin;
        public readonly JSONStorableFloat blushDurationMax;


        public ConfigCharacterBlushSettings(ConfigInstanceStorable parent): base(parent) {
            blushEnabled = new JSONStorableBool(GetStorableId("BlushEnabled"), true);
            Main.singleton.RegisterBool(blushEnabled);

            blushDurationMin = new JSONStorableFloat(GetStorableId("BlushDurationMin"), 7f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMin);

            blushDurationMax = new JSONStorableFloat(GetStorableId("BlushDurationMax"), 20f, 0, 60, true, true);
            Main.singleton.RegisterFloat(blushDurationMax);
        }
    }
}
