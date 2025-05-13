
namespace PPirate.VoxReactor
{
    internal class ConfigVoxReactor
    {
        public readonly ConfigCharacterBase char1Config;
        public readonly ConfigDirtyTalk globalDirtyTalkConfig;
        public readonly ConfigHandJob globalHandJobConfig;
        public static ConfigVoxReactor singeton;
        public ConfigVoxReactor(MVRScript mVRScript) {
            singeton = this;
            char1Config = new ConfigCharacterBase("ch1_");
            globalDirtyTalkConfig = new ConfigDirtyTalk("global_dt_",
                "talking dirty about the situation.," +
                "asking if he likes her sexually servicing him.," +
                "dirty talk about how she is his slut.," +
                "pervy dirty talk about wanting to be used.," +
                "be dirty talk about how wet she is."
            );
            globalHandJobConfig = new ConfigHandJob("global_hj_");
            //char2Config = new ConfigCharacterBase("ch2_", mVRScript);
        }
        public ConfigCharacterBase GetCharacterConfig(VoxtaCharacter character) {
            if (character.characterNumber == 1) { 
                return char1Config;
            }
            throw new System.Exception("unable to get character config for: " + character.characterNumber);
        }
        public ConfigCharacterBase GetCharacterConfig(int characterNumber)
        {
            if (characterNumber == 1)
            {
                return char1Config;
            }
            throw new System.Exception("unable to get character config for character: " + characterNumber);
        }


    }
}
