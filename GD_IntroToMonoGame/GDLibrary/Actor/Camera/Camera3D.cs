using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class Camera3D : Actor3D
    {
        #region Fields
        //name, active, Transform3D::Reset??
        private ProjectionParameters projectionParameters;
        #endregion

        #region Properties
        public Matrix Projection
        {
            get
            {
                return this.projectionParameters.Projection;
            }
        }
        //add a clean/dirty flag later
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(this.Transform3D.Translation,
                    //to do...add Transform3D::Target
                    this.Transform3D.Translation + this.Transform3D.Look,
                    this.Transform3D.Up);
            }
        }
        #endregion

        #region Constructors
        public Camera3D(string id, ActorType actorType, StatusType statusType, 
            Transform3D transform3D, 
                    ProjectionParameters projectionParameters)
            : base(id, actorType, statusType, transform3D)
        {
            this.projectionParameters = projectionParameters;
        }
        #endregion


        public override void Update(GameTime gameTime)
        {
            //check for keyboard input?
            //if input, then modify transform
          //  this.controller.Update(gameTime, this);

            base.Update(gameTime);
        }

        public new object Clone()
        {
            return new Camera3D(this.ID, this.ActorType, this.StatusType, this.Transform3D.Clone() as Transform3D, 
                this.projectionParameters.Clone() as ProjectionParameters);
        }

        public override bool Equals(object obj)
        {
            return obj is Camera3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<ProjectionParameters>.Default.Equals(projectionParameters, d.projectionParameters);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(projectionParameters);
            return hash.ToHashCode();
        }
    }
}
