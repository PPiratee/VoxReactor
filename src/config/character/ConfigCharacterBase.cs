
namespace PPirate.VoxReactor
{
    internal class ConfigCharacterBase : ConfigInstanceStorable
    {
        public readonly ConfigCharacterClothing clothingConfig;
        public readonly ConfigCharacterEmotions emotionConfig;
        public ConfigCharacterBase(string idPrefix): base(idPrefix) {
            clothingConfig = new ConfigCharacterClothing(this);
            emotionConfig = new ConfigCharacterEmotions(this);
        }
        
        
    }
}
