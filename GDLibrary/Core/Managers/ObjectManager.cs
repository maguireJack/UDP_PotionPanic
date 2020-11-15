using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class ObjectManager : PausableDrawableGameComponent
    {
        #region Fields

        private CameraManager<Camera3D> cameraManager;
        private List<DrawnActor3D> opaqueList, transparentList;
        private int count;

        #endregion Fields

        #region Constructors & Core

        public ObjectManager(Game game, StatusType statusType,
          int initialOpaqueDrawSize, int initialTransparentDrawSize,
          CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);
            this.count = 0;

            //        EventDispatcherV2.Subscribe(EventCategoryType.Menu, HandleMenuChanged);

            SubscribeToEvents();
        }

        protected override void SubscribeToEvents()
        {
            //menu
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);


            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add

            //transparency
        }

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                    this.StatusType = StatusType.Off;
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    this.StatusType = StatusType.Drawn | StatusType.Update;
            }
            else if (eventData.EventCategoryType == EventCategoryType.Object)
            {
                if (eventData.EventActionType == EventActionType.OnRemoveActor)
                {
                    DrawnActor3D removeObject = eventData.Parameters[0] as DrawnActor3D;

                    this.opaqueList.Remove(removeObject);

                }
                else if (eventData.EventActionType == EventActionType.OnAddActor)
                {
                    Add(eventData.Parameters[0] as DrawnActor3D);
                }
            }
        }

        public void Add(DrawnActor3D actor)
        {
            count++;
            if (actor.EffectParameters.Alpha < 1)
                transparentList.Add(actor);
            else
                opaqueList.Add(actor);
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

        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }
        }

        #endregion Constructors & Core

        #region Custom (Not Nialls)

        public bool RemoveByID(string id)
        {
            count++;
            for (int i = 0; i < opaqueList.Count; i++)
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