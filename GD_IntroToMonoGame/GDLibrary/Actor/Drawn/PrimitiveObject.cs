using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class PrimitiveObject : DrawnActor3D
    {
        #region Fields
        private IVertexData vertexData;
        #endregion

        #region Properties
        public IVertexData IVertexData
        {
            get
            {
                return this.vertexData;
            }
        }
        #endregion

        #region Constructors
        public PrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D, 
            EffectParameters effectParameters, IVertexData vertexData) 
                        : base(id, actorType, statusType, transform3D, effectParameters)
        {
            this.vertexData = vertexData;
        }
        #endregion

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            this.EffectParameters.Draw(this.Transform3D.World, camera);
            this.IVertexData.Draw(gameTime, this.EffectParameters.Effect, graphicsDevice);
        }

        public new object Clone()
        {
            return new PrimitiveObject(this.ID, this.ActorType, this.StatusType, this.Transform3D.Clone() as Transform3D,
                this.EffectParameters.Clone() as EffectParameters, this.vertexData.Clone()
                as IVertexData);
        }

        public override bool Equals(object obj)
        {
            return obj is PrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<IVertexData>.Default.Equals(vertexData, @object.vertexData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), vertexData);
        }
    }
}
