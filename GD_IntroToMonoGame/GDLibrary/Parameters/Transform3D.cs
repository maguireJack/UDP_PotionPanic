using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Holds data related to actor (e.g. player, pickup, decorator, architecture, camera) position
    /// </summary>
    public class Transform3D : ICloneable
    {
        private Vector3 translation, rotationInDegrees, scale;
        private Vector3 look, up; //right = look x up
        private Vector3 originalLook;
        private Vector3 originalUp;

        //add a clean/dirty flag later
        public Matrix World
        {
            get
            {
                return Matrix.Identity
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(this.rotationInDegrees.X))
                      * Matrix.CreateRotationY(MathHelper.ToRadians(this.rotationInDegrees.Y))
                        * Matrix.CreateRotationZ(MathHelper.ToRadians(this.rotationInDegrees.Z))
                        * Matrix.CreateTranslation(this.translation);
            }
        }



        public Vector3 Look
        {
            get
            {
                look.Normalize(); //less-cpu intensive than Vector3.Normalize()
                return look;
            }
            set
            {
                this.look = value;
            }
        }
        public Vector3 Up
        {
            get
            {
                up.Normalize(); //less-cpu intensive than Vector3.Normalize()
                return up;
            }
            set
            {
                this.up = value;
            }
        }
        public Vector3 Right
        {
            get
            {
              //  Vector3 right = Vector3.Cross(this.look, this.up);
              //  right.Normalize();
              //  return right;

                return Vector3.Normalize(Vector3.Cross(this.look, this.up));
            }
        }

        public Vector3 Translation {
            get
            {
                return this.translation;
            }
            set
            {
                this.translation = value;
            } 
        }

        private Vector3 originalRotationInDegrees;

        public Vector3 RotationInDegrees
        {
            get
            {
                return this.rotationInDegrees;
            }
            set
            {
                this.rotationInDegrees = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }

        //constructor suitable for Camera3D (i.e. no rotation or scale)
        public Transform3D(Vector3 translation, Vector3 look, Vector3 up) : this(translation, Vector3.Zero, Vector3.One,
               look, up)
        {

        }
        //constructor suitable for drawn actors
        public Transform3D(Vector3 translation, Vector3 rotationInDegrees, 
            Vector3 scale, Vector3 look, Vector3 up)
        {
            this.Translation = translation;
            this.originalRotationInDegrees = this.RotationInDegrees = rotationInDegrees;
            this.Scale = scale;
            this.originalLook = this.Look = look;
            this.originalUp = this.Up = up;
        }

        public void TranslateBy(Vector3 delta)
        {
            this.translation += delta;
        }

        public void RotateAroundUpBy(float magnitude)
        {
            //to do...
        }

        public void RotateBy(Vector3 axisAndMagnitude)
        {
            //add this statement to allow us to add/subtract from whatever the current rotation is
            Vector3 rotation = this.originalRotationInDegrees + axisAndMagnitude;

            //explain: yaw, pitch, roll
            //create a new "XYZ" axis to rotate around using the (x,y,0) values from mouse and any current rotation
            Matrix rotMatrix = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(rotation.X), //Pitch
                MathHelper.ToRadians(rotation.Y), //Yaw
                MathHelper.ToRadians(rotation.Z)); //Roll

            //update the look and up vector (i.e. rotate them both around this new "XYZ" axis)
            this.look = Vector3.Transform(this.originalLook, rotMatrix);
            this.up = Vector3.Transform(this.originalUp, rotMatrix);
        }

        public object Clone()
        {
            return new Transform3D(this.translation, this.rotationInDegrees, this.scale,
                this.look, this.up);
        }
        
    }
}
