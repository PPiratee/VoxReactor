
namespace PPirate.VoxReactor
{
    public class ConfigVoxReactor
    {
        public readonly ConfigCharacterBase char1Config;
        public ConfigVoxReactor(MVRScript mVRScript) { 
            char1Config = new ConfigCharacterBase("ch1_", mVRScript);
            //char2Config = new ConfigCharacterBase("ch2_", mVRScript);
        }
         

    }
}
