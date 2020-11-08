using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class ObjectManager : DrawableGameComponent
    {
        #region Fields

        private CameraManager<Camera3D> cameraManager;
        private List<DrawnActor3D> opaqueList, transparentList;
        private int count;

        #endregion Fields

        #region Constructors & Core

        public ObjectManager(Game game,
          int initialOpaqueDrawSize, int initialTransparentDrawSize,
          CameraManager<Camera3D> cameraManager) : base(game)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);
            count = 0;
        }

        public void Add(DrawnActor3D actor)
        {
            count++;
            if (actor.EffectParameters.Alpha < 1)
            {
                transparentList.Add(actor);
            }
            else
            {
                opaqueList.Add(actor);
            }
        }

        public bool RemoveFirstIf(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            int position = opaqueList.FindIndex(predicate);

            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                return true;
            }

            return false;
        }

        public int RemoveAll(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            return opaqueList.RemoveAll(predicate);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                {
                    actor.Draw(gameTime,
                       cameraManager.ActiveCamera,
                        GraphicsDevice);
                }
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                {
                    actor.Draw(gameTime,
                       cameraManager.ActiveCamera,
                        GraphicsDevice);
                }
            }
        }

        #endregion Constructors & Core

        #region Custom (Not Nialls)

        public bool RemoveByID(string id)
        {
            count++;
            for(int i = 0; i < opaqueList.Count; i++)
            {
                if (opaqueList[i].ID.Equals(id))
                {
                    opaqueList.RemoveAt(i);
                    return true;
                }    
            }

            for (int i = 0; i < transparentList.Count; i++)
            {
                if (opaqueList[i].ID.Equals(id))
                {
                    transparentList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public List<DrawnActor3D> GetActorList(ActorType actorType)
        {
            List<DrawnActor3D> list = new List<DrawnActor3D>();
            foreach (DrawnActor3D actor in opaqueList)
            {
                if (actor.ActorType == actorType)
                {
                    list.Add(actor);
                }
            }
            return list;
        }

        public int ListSize()
        {
            return opaqueList.Count + transparentList.Count;
        }

        public int TotalListChanges()
        {
            return count;
        }

        #endregion
    }
}