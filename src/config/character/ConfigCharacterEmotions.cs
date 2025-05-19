

using System.Collections.Generic;

namespace PPirate.VoxReactor
{
    //using static EmotionManager;

    internal class ConfigCharacterEmotions : ConfigInstanceStorable
    {

        public readonly JSONStorableBool emotionsEnabled;
        public readonly ConfigCharacterSpecificEmotion hornienessConfig;
        public readonly ConfigCharacterSpecificEmotion happynessConfig;
        public readonly ConfigCharacterSpecificEmotion sadnessConfig;
        public readonly ConfigCharacterSpecificEmotion angerConfig;
        public readonly ConfigCharacterSpecificEmotion embarrassmentConfig;

        private readonly List<ConfigCharacterSpecificEmotion> configs = new List<ConfigCharacterSpecificEmotion>();

        public ConfigCharacterEmotions(ConfigInstanceStorable parent): base(parent) {
            emotionsEnabled = new JSONStorableBool(GetStorableId("EmotionsEnabled"), true);
            Main.singleton.RegisterBool(emotionsEnabled);

            hornienessConfig = new ConfigCharacterSpecificEmotion(Hornieness.hornieNessName, this)
                .SetAngerMultiplier(-1)
                .SetSadnessMultiplier(-1);
            configs.Add(hornienessConfig);
         

            happynessConfig = new ConfigCharacterSpecificEmotion(Happyness.happynessName, this)
                .SetAngerMultiplier(-1)
                .SetSadnessMultiplier(-1)
                .SetEmbarrassmentMultiplier(-0.75f);
            configs.Add(happynessConfig);


            sadnessConfig = new ConfigCharacterSpecificEmotion(Sadness.sadnessName, this)
                .SetHornynessMultiplier(-1)
                .SetHappynessMultiplier(-1)
                .SetAngerMultiplier(-1);
            configs.Add(sadnessConfig);


            angerConfig = new ConfigCharacterSpecificEmotion(Anger.angerName, this)
                .SetHornynessMultiplier(-1)
                .SetHappynessMultiplier(-1)
                .SetSadnessMultiplier(-1);
            configs.Add(angerConfig);


            embarrassmentConfig = new ConfigCharacterSpecificEmotion(Embarrassment.embarrassmentName, this)
                .SetHornynessMultiplier(0.5f)
                .SetAngerMultiplier(-1.5f);
            configs.Add(embarrassmentConfig);



        }

        public ConfigCharacterSpecificEmotion GetEmotionConfigByName(string emotionName) {
            foreach (var item in configs)
            {
                if (item.emotionName == emotionName) { 
                    return item;
                }
            }
            throw new System.Exception("unable to get emotion config for " + emotionName);
        }
    }
}
