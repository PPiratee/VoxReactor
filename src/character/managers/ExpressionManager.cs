using System;
using PPirate.VoxReactor;


namespace PPirate.VoxReactormanagers
{
    internal class ExpressionManager
    {
        private readonly VoxtaCharacter character;
        private readonly ExpressionRouterPlugin expressionPlugin;

        public ExpressionManager(VoxtaCharacter character)
        {
            this.character = character;

            expressionPlugin = character.plugins.expressionRouterPlugin;
        }
        string lastMood = "";
        public void LoadExpression(string mood) {
            if (lastMood == mood) {
                return;
            }
            lastMood = mood;

           // expressionPlugin.StopAll();
            expressionPlugin.LoadMood(mood);

           // AtomUtils.RunAfterDelay(1f, ()=> {
               // expressionPlugin.LoadMood(mood);
               // expressionPlugin.StartAll();
           // });
            //AtomUtils.RunAfterDelay(5f, () => {
                //expressionPlugin.LoadMood(mood);
               // expressionPlugin.StartAll();
          //  });
        }
    }
}
