using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public interface IController : ICloneable
    {
        void Update(GameTime gameTime, IActor actor); //update the actor controller by this controller
        
        //add more methods here over time...
    }
}
