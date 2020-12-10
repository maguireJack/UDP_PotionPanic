using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDGame.MyGame.Minigames
{
    public class SlidingMinigameController : Minigame
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private UITextureObject background;
        private UITextureObject target;
        private UITextureObject safeZone;
        private UITextureObject progressBar;

        private float progress;
        private float bottom;
        private float top;
        private float maxProgressWidth;
        private float angle;

        public SlidingMinigameController(string id, ActorType actorType, StatusType statusType,
            KeyboardManager keyboardManager, GamePadManager gamePadManager,
            UITextureObject background, UITextureObject target, UITextureObject safeZone, UITextureObject progressBar)
            : base(id, actorType, statusType)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.background = background;
            this.target = target;
            this.safeZone = safeZone;
            this.progressBar = progressBar;

            this.progress = 30;
            this.bottom = safeZone.Transform2D.Translation.Y;
            this.top = bottom - 250;
            this.maxProgressWidth = progressBar.Transform2D.Bounds.Width;
            this.angle = 0;
        }

        public override void Start()
        {
            SendLockEvent();
            safeZone.Transform2D.Translation = new Vector2(GameConstants.screenCentre.X, bottom);
            target.Transform2D.Translation = GameConstants.screenCentre;
            angle = 0;
            progress = 30;

            background.StatusType = StatusType.Drawn;
            target.StatusType = StatusType.Drawn;
            safeZone.StatusType = StatusType.Drawn;
            progressBar.StatusType = StatusType.Drawn;
            StatusType = StatusType.Update;
        }

        public override bool IsComplete()
        {
            if(progress >= 100)
            {
                SendUnlockEvent();

                background.StatusType = StatusType.Off;
                target.StatusType = StatusType.Off;
                safeZone.StatusType = StatusType.Off;
                progressBar.StatusType = StatusType.Off;
                StatusType = StatusType.Off;

                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            angle++;
            HandleKeyboard();

            if (safeZone.Transform2D.Bounds.Intersects(target.Transform2D.Bounds))
                progress += 0.4f;
            else if(progress > 0)
                progress -= 0.2f;

            target.Transform2D.Translation = new Vector2(
                target.Transform2D.Translation.X,
                ((float)Math.Sin(MathHelper.ToRadians(-angle)) * 160) + GameConstants.screenCentre.Y + 25);

            //update progress visual (width and position)
            progressBar.Transform2D.Scale = new Vector2(progress/100f * maxProgressWidth, progressBar.Transform2D.Scale.Y);
            progressBar.Transform2D.Translation = new Vector2(
                GameConstants.screenCentre.X - (maxProgressWidth - progressBar.Transform2D.Scale.X)/2,
                progressBar.Transform2D.Translation.Y);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            if(keyboardManager.IsKeyDown(Keys.Space)
                && safeZone.Transform2D.Translation.Y > top)
            {
                safeZone.Transform2D.Translation += new Vector2(0, -3);
            }
            else if(!keyboardManager.IsKeyDown(Keys.Space)
                && safeZone.Transform2D.Translation.Y < bottom)
            {
                safeZone.Transform2D.Translation += new Vector2(0, 3);
            }
        }
    }
}
