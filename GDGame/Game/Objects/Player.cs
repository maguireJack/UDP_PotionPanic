﻿using GDGame.Game.Actors;
using GDGame.Game.Constants;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Core.Events;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GDGame.Game.Objects
{
    public class Player : MoveableObject
    {
        #region Fields

        private ObjectManager objectManager;
        private KeyboardManager keyBoardManager;
        private HandHeldPickup handItem;
        private List<DrawnActor3D> interactableList;
        private int lastListSize;
        //inventory

        #endregion

        #region Properties

        public HandHeldPickup HandItem
        {
            get { return handItem; }
        }

        public Vector3 HandPos
        {
            get
            {
                return Transform3D.Translation + GameConstants.playerHoldPos;
            }
        }

        #endregion

        #region Constructors

        public Player(ObjectManager objectManager, KeyboardManager keyboardManager, ModelObject modelObject, IController controller) :
            base(modelObject, controller)
        {
            this.objectManager = objectManager;
            this.keyBoardManager = keyboardManager;
            this.handItem = null;
            this.lastListSize = 0;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateHandItemPos();
            AttemptInteraction();
        }

        private void AttemptInteraction()
        {
            //This is a temporary way to get a list of interactable objects, I have an idea of how to have this optimised
            //but going to wait to see what Niall does before I update this.

            //Check if there size of the object manager has changed, if it has, get and update the interactable list of objects here
            if (lastListSize != objectManager.TotalListChanges())
            {
                lastListSize = objectManager.ListSize();
                interactableList = objectManager.GetActorList(ActorType.Interactable);
            }
            foreach (DrawnActor3D actor in interactableList)
            {
                InteractableObject iObject = actor as InteractableObject;
                //If we are in range of the interactable objects
                if (iObject.GetDistance(this) <= iObject.InteractDistance)
                {
                    //check if our hand is empty
                    //check if it is an item we can pickup (rather than a interactable workstation, etc
                    //then check if the Pickup key is pressed
                    if (keyBoardManager.IsKeyDown(GameConstants.playerInteractKey))
                    {
                        if (handItem == null && iObject is HandHeldPickup)
                            handItem = iObject as HandHeldPickup;
                        else if (handItem != null && iObject is IContainerInteractable)
                        {
                            IContainerInteractable container = iObject as IContainerInteractable;
                            if (container.Deposit(handItem))
                            {
                                EventDispatcher.Publish(EventType.Remove, new ArrayList() { handItem.ID }); ;
                                handItem = null;
                                return;
                            }
                        }

                    }
                }
            }
        }

        private void UpdateHandItemPos()
        {
            //If we have an item, update it's position constantly to our hand pos
            if (handItem != null)
            {
                handItem.Transform3D.Translation = HandPos - handItem.HeldCoords;
            }
        }

    }
}
