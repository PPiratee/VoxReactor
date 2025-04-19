

namespace PPirate.VoxReactor
{
    internal class ConfigCharacterClothing: ConfigInstanceStorable
    {
        public readonly JSONStorableBool clothingContextEnabled;
        public ConfigCharacterClothing(ConfigInstanceStorable parent): base(parent) {
            clothingContextEnabled = new JSONStorableBool(GetStorableId("EnableClothingContext"), true);
            Main.singleton.RegisterBool(clothingContextEnabled);
        }
    }
}
