using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UIMouseController : Controller
    {
        private MouseManager mouseManager;

        public UIMouseController(string id,
            ControllerType controllerType, MouseManager mouseManager) : base(id, controllerType)
        {
            this.mouseManager = mouseManager;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                if (drawnActor.Transform2D.Bounds.Contains(this.mouseManager.Bounds))
                {
                    //     if (this.mouseManager.IsLeftButtonClicked())
                    {
                        drawnActor.Color = Color.Red;
                    }
                }
                else
                    drawnActor.Color = Color.White;
            }

            base.Update(gameTime, actor);
        }
    }
}