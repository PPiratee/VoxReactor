
namespace PPirate.VoxReactor
{
    internal class ConfigHandJob : ConfigInstanceStorable
    {
        public readonly JSONStorableBool handJobEnabled;

        public readonly ConfigDirtyTalk dirtyTalkConfig;


        public ConfigHandJob(string idPrefix, ConfigInstanceStorable parent) : base(idPrefix, parent) {
            handJobEnabled = new JSONStorableBool(GetStorableId("HandJobEnabled"), true);
            Main.singleton.RegisterBool(handJobEnabled);


            dirtyTalkConfig = new ConfigDirtyTalk(this, "DirtyTalkConfig",
                "talking dirty how big his cock is.," +
                "talking dirty about how hard {{ user }} is.," +
                "about how she wants him to cum on her tits.," +
                "pervy dirty talk about wanting to be used.," +
                "be dirty talk about how wet she is.," +
                "asking if he likes her giving him a hand job."
            );
            
        }
        public ConfigHandJob(string idPrefix) : base(idPrefix)
        {
            handJobEnabled = new JSONStorableBool(GetStorableId("HandJobEnabled"), true);
            Main.singleton.RegisterBool(handJobEnabled);


            dirtyTalkConfig = new ConfigDirtyTalk(this, "DirtyTalkConfig_",
                "talking dirty how big his cock is.," +
                "talking dirty about how hard {{ user }} is.," +
                "about how she wants him to cum on her tits.," +
                "pervy dirty talk about wanting to be used.," +
                "be dirty talk about how wet she is.," +
                "asking if he likes her giving him a hand job."
            );

        }

    }
}
