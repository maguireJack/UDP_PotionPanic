using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Holds vertex array, primitive type and primitive count for drawn primitive
    /// </summary>
    public class VertexData<T> : IVertexData, ICloneable where T : struct, IVertexType
    {
        #region Fields
        private T[] vertices;
        private PrimitiveType primitiveType;
        private int primitiveCount;
        #endregion

        #region Properties

        #endregion

        public VertexData(T[] vertices, 
            PrimitiveType primitiveType, int primitiveCount)
        {
            this.vertices = vertices;
            this.primitiveType = primitiveType;
            this.primitiveCount = primitiveCount;
        }

        public object Clone()
        {
            //shallow (no use of the "new" keyword)
            return this;
        }


        public void Draw(GameTime gameTime, BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            //serialising the vertices from RAM to VRAM
            //constrained by the bandwidth of the bus and 
            //by the lower of the two clock speeds (cpu, gpu)
            graphicsDevice.DrawUserPrimitives<T>(this.primitiveType,
                this.vertices, 0, this.primitiveCount);
        }

        public int GetPrimitiveCount()
        {
            return this.primitiveCount;
        }

        public PrimitiveType GetPrimitiveType()
        {
            return this.primitiveType;
        }
        /*
public void Draw(BasicEffect effect, 
   Matrix world, Camera3D camera3D,
   GraphicsDevice graphicsDevice)
{
   effect.World = world;
   effect.View = camera3D.View;
   effect.Projection = camera3D.Projection;
   effect.CurrentTechnique.Passes[0].Apply();
   graphicsDevice.DrawUserPrimitives<T>(
       this.primitiveType,
       this.vertices, 0, this.primitiveCount);
}
*/

    }
}




