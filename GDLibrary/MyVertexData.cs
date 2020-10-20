using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GD_IntroToMonoGame
{
    /// <summary>
    /// Holds vertex array, primitive type and primitive count for drawn primitive
    /// </summary>
    public class MyVertexData : ICloneable
    {
        #region Fields
        private VertexPositionColor[] vertices;
        private PrimitiveType primitiveType;
        private int primitiveCount;
        #endregion

        #region Properties

        #endregion

        public MyVertexData(VertexPositionColor[] vertices,
            PrimitiveType primitiveType, int primitiveCount)
        {
            this.vertices = vertices;
            this.primitiveType = primitiveType;
            this.primitiveCount = primitiveCount;
        }

        public object Clone()
        {
            return this;
            //    return new VertexData(this.vertices.Clone() as VertexPositionColor[], 
            //       this.primitiveType, this.primitiveCount);
        }

        public void Draw(BasicEffect effect,
            Matrix world, Matrix view, Matrix projection,
            GraphicsDevice graphicsDevice)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                this.primitiveType,
                this.vertices, 0, this.primitiveCount);
        }


    }
}




