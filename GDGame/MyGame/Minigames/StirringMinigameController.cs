using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.MyGame.Controllers
{
    public class StirringMinigameController : Minigame
    {
        private MouseManager mouseManager;
        private UITextureObject background;
        private float radius;
        private float startAngle;
        private float angle;

        private UITextureObject ball;

        public StirringMinigameController(string id, ActorType actorType, StatusType statusType,
            MouseManager mouseManager, UITextureObject background, float radius,
            UITextureObject ball)
            : base(id, actorType, statusType)
        {
            this.mouseManager = mouseManager;
            this.background = background;
            this.radius = radius;
            this.ball = ball;
            this.startAngle = MathHelper.ToRadians(180);
            this.angle = 0;
        }

        public override void Start()
        {
            background.StatusType = StatusType.Drawn;
            ball.StatusType = StatusType.Drawn;
            StatusType = StatusType.Update;
        }

        public override bool IsComplete()
        {
            if (MathHelper.ToDegrees(angle) > 357)
            {
                ball.StatusType = StatusType.Off;
                background.StatusType = StatusType.Off;
                StatusType = StatusType.Off;
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            //Check if mouse intersects ball, if it does move it along its circular path
            if (mouseManager.Bounds.Intersects(ball.Transform2D.Bounds))
            {
                angle += MathHelper.ToRadians(2);
            }

            //Translate the ball according to it's angle
            ball.Transform2D.Translation = new Vector2(
                GameConstants.screenCentre.X + (float)Math.Cos(angle + startAngle) * radius,
                GameConstants.screenCentre.Y + (float)Math.Sin(angle + startAngle) * radius);

            base.Update(gameTime);
        }
    }
}
