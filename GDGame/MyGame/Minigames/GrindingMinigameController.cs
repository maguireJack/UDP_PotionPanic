using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GDGame.MyGame.Actors;

namespace GDGame.MyGame.Controllers
{
    public class GrindingMinigameController : Minigame
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private UITextureObject ui;
        private int count;
        private bool keyA;

        public GrindingMinigameController(string id, ActorType actorType, StatusType statusType,
            KeyboardManager keyboardManager, GamePadManager gamePadManager, UITextureObject ui)
            : base(id, actorType, statusType)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.count = 0;
            this.keyA = true;
            this.ui = ui;
        }

        public override void Start()
        {
            ui.StatusType = StatusType.Drawn;
            StatusType = StatusType.Update;
        }

        public override bool IsComplete()
        {
            if(count >= 20)
            {
                count = 0;
                keyA = true;
                ui.StatusType = StatusType.Off;
                StatusType = StatusType.Off;
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
            }
            else if (!keyA && keyboardManager.IsFirstKeyPress(Keys.D))
            {
                count++;
                keyA = true;
            }
        }
    }
}