using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Controllers
{
    class SinusoidalController : Controller
    {
        public SinusoidalController(string id, ControllerType controllerType) : base(id, controllerType)
        {

        }
        /// <summary>
        /// Is called to rotate a handheld pickup when it hasn't been picked up yet
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        /// <param name="actor">Actor the movement acts upon</param>
        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
            DrawnActor3D actor3d = actor as DrawnActor3D;
            //actor3d.Transform3D.TranslateBy(new Vector3(0, 10, 0));
            actor3d.Transform3D.TranslateBy(new Vector3(0, System.MathF.Sin((float)(1f * gameTime.TotalGameTime.TotalSeconds / 4) * 20f), 0));
            actor3d.Transform3D.RotateAroundUpBy(.5f);
            
        }
    }
}
