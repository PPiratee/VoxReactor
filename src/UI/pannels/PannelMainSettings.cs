using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelMainSettings : UIPannel
    {
        public JSONStorableBool testBool = new JSONStorableBool("TestBoolMainSettings", true);

        public PannelMainSettings(UIPannel parentPannel) : base("Main Settings",  parentPannel) {
            mvrScript.RegisterBool(testBool);
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();
            testBool.CreateUI(uiElements, false);
           // mvrScript.RegisterBool(testBool);
        }
    }
}
