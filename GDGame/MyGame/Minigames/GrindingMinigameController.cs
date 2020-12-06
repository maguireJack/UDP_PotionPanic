using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GDGame.MyGame.Actors;
using System.Collections.Generic;

namespace GDGame.MyGame.Controllers
{
    public class GrindingMinigameController : Minigame
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private Dictionary<string, UITextureObject> uiPanels;
        private int count;
        private bool keyA;

        public GrindingMinigameController(string id, ActorType actorType, StatusType statusType,
            KeyboardManager keyboardManager, GamePadManager gamePadManager,
            Dictionary<string, UITextureObject> uiPanels)
            : base(id, actorType, statusType)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.count = 0;
            this.keyA = true;
            this.uiPanels = uiPanels;
        }

        public override void Start()
        {
            uiPanels["grinding_A"].StatusType = StatusType.Drawn;
            StatusType = StatusType.Update;
        }

        public override bool IsComplete()
        {
            if(count >= 20)
            {
                count = 0;
                keyA = true;
                StatusType = StatusType.Off;
                foreach(UITextureObject uiPanel in uiPanels.Values)
                    uiPanel.StatusType = StatusType.Off;
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            if (capabilities.IsConnected)
                HandleControlerMashing();
            else HandleMashing();

            base.Update(gameTime);
        }

        private void HandleControlerMashing()
        {
            if (keyA && gamePadManager.IsFirstButtonPress(0, Buttons.A)) 
            {
                count++;
                keyA = false;
            }
            else if(gamePadManager.IsFirstButtonPress(0, Buttons.B))
            {
                count++;
                keyA = true;
            }
        } 

        private void HandleMashing()
        {
            if (keyA && keyboardManager.IsFirstKeyPress(Keys.A))
            {
                count++;
                keyA = false;
                uiPanels["grinding_A"].StatusType = StatusType.Off;
                uiPanels["grinding_D"].StatusType = StatusType.Drawn;
            }
            else if (!keyA && keyboardManager.IsFirstKeyPress(Keys.D))
            {
                count++;
                keyA = true;
                uiPanels["grinding_A"].StatusType = StatusType.Drawn;
                uiPanels["grinding_D"].StatusType = StatusType.Off;
            }
        }
    }
}