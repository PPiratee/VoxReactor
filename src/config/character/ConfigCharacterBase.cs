
namespace PPirate.VoxReactor
{
    public class ConfigCharacterBase : ConfigInstanceStorable
    {
        public readonly ConfigCharacterClothing clothingConfig;
        public ConfigCharacterBase(string idPrefix, MVRScript mVRScript): base(idPrefix) {
            clothingConfig = new ConfigCharacterClothing(mVRScript, this);
        }
        
        
    }
}
