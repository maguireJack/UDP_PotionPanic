using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    /// <summary>
    /// All vertex data objects will implement this interface
    /// </summary>
    public interface IVertexData : ICloneable
    {
        void Draw(GameTime gameTime, BasicEffect effect, GraphicsDevice graphicsDevice);

        //getters
        PrimitiveType GetPrimitiveType();
        int GetPrimitiveCount();

        //setters

        //public int PrimitiveCount
        //{
        //    get
        //    {
        //        return this.primitiveCount;
        //    }
        //}
    }
}
