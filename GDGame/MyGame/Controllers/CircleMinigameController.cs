using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.MyGame.Controllers
{
    public class CircleMinigameController : Controller
    {
        private MouseManager mouseManager;
        private UITextureObject background;
        private float radius;
        private float startAngle;
        private float angle;

        public CircleMinigameController(string id, ControllerType controllerType,
            MouseManager mouseManager, UITextureObject background, float radius)
            : base(id, controllerType)
        {
            this.mouseManager = mouseManager;
            this.background = background;
            this.radius = radius;
            this.startAngle = MathHelper.ToRadians(180);
            this.angle = 0;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (mouseManager.Bounds.Intersects(drawnActor.Transform2D.Bounds))
            {
                angle += MathHelper.ToRadians(2);
            }

            drawnActor.Transform2D.Translation = new Vector2(
                GameConstants.screenCentre.X + (float)Math.Cos(angle + startAngle) * radius,
                GameConstants.screenCentre.Y + (float)Math.Sin(angle + startAngle) * radius);

            if(MathHelper.ToDegrees(angle) > 357)
            {
                drawnActor.StatusType = StatusType.Off;
                background.StatusType = StatusType.Off;
                //publish event
            }

            base.Update(gameTime, actor);
        }

    }
}
