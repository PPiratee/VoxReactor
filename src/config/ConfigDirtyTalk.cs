
namespace PPirate.VoxReactor
{
    internal class ConfigDirtyTalk : ConfigInstanceStorable
    {
        public readonly JSONStorableBool dirtyTalkEnabled;
        public readonly JSONStorableString dirtyTalkLines;

        public ConfigDirtyTalk(string idPrefix, string lines): base(idPrefix) {
            dirtyTalkEnabled = new JSONStorableBool(GetStorableId("DirtyTalkEnabled"), true);
            Main.singleton.RegisterBool(dirtyTalkEnabled);

            dirtyTalkLines = new JSONStorableString(GetStorableId("DirtyTalkLines"), lines);
            Main.singleton.RegisterString(dirtyTalkLines);
        }

        public ConfigDirtyTalk(ConfigInstanceStorable parent, string idPrefix, string lines) : base(idPrefix, parent)
        {
            dirtyTalkEnabled = new JSONStorableBool(GetStorableId("DirtyTalkEnabled"), true);
            Main.singleton.RegisterBool(dirtyTalkEnabled);

            dirtyTalkLines = new JSONStorableString(GetStorableId("DirtyTalkLines"), lines);
            Main.singleton.RegisterString(dirtyTalkLines);
        }

        



    }
}
