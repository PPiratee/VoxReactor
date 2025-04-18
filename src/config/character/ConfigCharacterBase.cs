
namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBase : ConfigInstanceStorable
    {
        public readonly ConfigCharacterClothing clothingConfig;
        public readonly ConfigCharacterEmotions emotionConfig;
        public ConfigCharacterBase(string idPrefix, MVRScript mVRScript): base(idPrefix) {
            clothingConfig = new ConfigCharacterClothing(mVRScript, this);
            emotionConfig = new ConfigCharacterEmotions(mVRScript, this);
        }
        
        
    }
}
