using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    //rotate around the up axis from (-60, +60)
    //hint: gameTime.TotalGameTime.Seconds, Math.Sin(), MathHelper.ToRadians()
    //remember float rotAngle = 60 * sin (MathHelper.ToRadians(wT + phi))
    //phi = how much rotation we start with e.g. 45 degrees
    //w = omega = angular speed = some float that controller speed try both 0.2f and 2f and see results
    //now rotate look vector using Vector3.Transform(originalLook, rotAngle)
    public class PanController : IController
    {
        private Vector3 rotationAxis;
        private float maxAmplitude, angularSpeed, phaseAngle;

        public PanController(Vector3 rotationAxis, 
            float maxAmplitude, float angularSpeed, float phaseAngle)
        {
            this.rotationAxis = rotationAxis;
            this.angularSpeed = angularSpeed;
            this.maxAmplitude = maxAmplitude;
            this.phaseAngle = phaseAngle;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            float time = (float)gameTime.TotalGameTime.TotalSeconds%360;

            // y = A * Sin(wT + phaseAngle)
            float rotAngle = this.maxAmplitude * (float)Math.Sin(
                MathHelper.ToRadians(this.angularSpeed * time + this.phaseAngle));

            Actor3D parent = actor as Actor3D;
            if(parent != null)
                parent.Transform3D.RotateBy(this.rotationAxis * rotAngle);

            //always check if its necessary to call the base method i.e. does the base method have any code?
        //    base.Update(gameTime, actor);

        }

        public object Clone()
        {
            return null;
        }
    }
}
