using GDLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace GDLibrary.Containers
{
    /// <summary>
    /// Used to store event handlers to be applied to an actor
    /// </summary>
    /// <see cref="GDLibrary.Actors.Actor"/>
    public class EventHandlerList : List<IEventHandler>
    {
        #region Constructors & Core

        public virtual bool Remove(Predicate<IEventHandler> predicate)
        {
            int position = this.FindIndex(predicate);
            if (position != -1)
            {
                this.RemoveAt(position);
                return true;
            }
            return false;
        }

        public virtual int Transform(Predicate<IEventHandler> filter,
                                            Action<IEventHandler> transform)
        {
            int count = 0;
            foreach (IEventHandler obj in this)
            {
                if (filter(obj))
                {
                    transform(obj);
                    count++;
                }
            }
            return count;
        }

        //public virtual void Update(GameTime gameTime, IActor parent)
        //{
        //    //calls update on all controllers
        //    foreach (IEventHandler controller in this)
        //    {
        //        controller.Update(gameTime, parent);
        //    }
        //}


        #endregion Constructors & Core
    }
}