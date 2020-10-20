using GDLibrary;
using Microsoft.Xna.Framework;

namespace GD_IntroToMonoGame.GDLibrary.Controller.Camera
{
    //rotate around the up axis from (-60, +60)
    //hint: gameTime.TotalGameTime.Seconds, Math.Sin(), MathHelper.ToRadians()
    //remember float rotAngle = 60 * sin (MathHelper.ToRadians(wT + phi))
    //phi = how much rotation we start with e.g. 45 degrees
    //w = omega = angular speed = some float that controller speed try both 0.2f and 2f and see results
    //now rotate look vector using Vector3.Transform(originalLook, rotAngle)
    public class PanController : IController
    {

        public PanController()
        {

        }

        public void Update(GameTime gameTime, IActor actor)
        {
            //see FirstPersonCameraController::Update()
        }

        public object Clone()
        {
            return null;
        }
    }
}
