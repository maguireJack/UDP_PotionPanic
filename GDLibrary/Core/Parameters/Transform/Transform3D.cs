using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the data related to actor (e.g. player, pickup, decorator, architecture, camera) transformation (e.g. translation, scale) and orientation (look, up).
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject"/>
    /// <seealso cref="GDLibrary.Actors.PrimitiveObject"/>
    public class Transform3D : ICloneable
    {
        #region Fields

        private Vector3 translation, rotationInDegrees, scale;
        private Vector3 look, up; //right = Vector3.Cross(look, up)
        private Vector3 originalLook, originalUp, originalRotationInDegrees;

        #endregion Fields

        #region Properties

        public Matrix World
        {
            get
            {
                return Matrix.Identity
                    * Matrix.CreateScale(scale)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(rotationInDegrees.X))
                      * Matrix.CreateRotationY(MathHelper.ToRadians(rotationInDegrees.Y))
                        * Matrix.CreateRotationZ(MathHelper.ToRadians(rotationInDegrees.Z))
                        * Matrix.CreateTranslation(translation);
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
                look = value;
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
                up = value;
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(look, up));
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
            }
        }

        public Vector3 RotationInDegrees
        {
            get
            {
                return rotationInDegrees;
            }
            set
            {
                rotationInDegrees = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        //constructor suitable for Camera3D (i.e. no rotation or scale)
        public Transform3D(Vector3 translation, Vector3 look, Vector3 up) : this(translation, Vector3.Zero, Vector3.One,
               look, up)
        {
        }

        //constructor suitable for drawn actors
        public Transform3D(Vector3 translation, Vector3 rotationInDegrees,
            Vector3 scale, Vector3 look, Vector3 up)
        {
            Translation = translation;
            originalRotationInDegrees = RotationInDegrees = rotationInDegrees;
            Scale = scale;
            originalLook = Look = look;
            originalUp = Up = up;
        }

        #region Movement

        public void TranslateBy(Vector3 delta)
        {
            translation += delta;
        }

        public void RotateAroundUpBy(float magnitude)
        {
            //add to existing rotation
            rotationInDegrees.Y += magnitude;

            //transform the original look using this rotationInDegrees around UnitY and normalize
            look = Vector3.Normalize(Vector3.Transform(originalLook, Matrix.CreateRotationY(MathHelper.ToRadians(rotationInDegrees.Y))));
        }

        public void RotateBy(Vector3 axisAndMagnitude)
        {
            //add this statement to allow us to add/subtract from whatever the current rotation is
            rotationInDegrees = originalRotationInDegrees + axisAndMagnitude;

            //explain: yaw, pitch, roll
            //create a new "XYZ" axis to rotate around using the (x,y,0) values from mouse and any current rotation
            Matrix rotMatrix = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(rotationInDegrees.X), //Pitch
                MathHelper.ToRadians(rotationInDegrees.Y), //Yaw
                MathHelper.ToRadians(rotationInDegrees.Z)); //Roll

            //update the look and up vector (i.e. rotate them both around this new "XYZ" axis)
            look = Vector3.Transform(originalLook, rotMatrix);
            up = Vector3.Transform(originalUp, rotMatrix);
        }

        #endregion Movement

        public object Clone()
        {
            return new Transform3D(translation, rotationInDegrees, scale,
                look, up);
        }

        #endregion Constructors & Core
    }
}