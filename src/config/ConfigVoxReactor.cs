
namespace PPirate.VoxReactor
{
    internal class ConfigVoxReactor
    {
        public readonly ConfigCharacterBase char1Config;
        public static ConfigVoxReactor singeton;
        public ConfigVoxReactor(MVRScript mVRScript) {
            singeton = this;
            char1Config = new ConfigCharacterBase("ch1_");
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
