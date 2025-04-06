

namespace PPirate.VoxReactor
{
    public class ConfigCharacterClothing: ConfigInstanceStorable
    {
        public readonly JSONStorableBool clothingContextEnabled;
        public ConfigCharacterClothing(MVRScript mVRScript, ConfigInstanceStorable parent): base(parent) {
            clothingContextEnabled = new JSONStorableBool(GetStorableId("EnableClothingContext"), true);
            mVRScript.RegisterBool(clothingContextEnabled);
        }
    }
}
