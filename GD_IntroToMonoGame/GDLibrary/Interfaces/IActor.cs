using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public interface IActor : ICloneable
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);

     //   string GetID();
     //   float GetAlpha();
    }
}
