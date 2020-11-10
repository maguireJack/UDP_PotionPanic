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
            FindInteractables();
            UpdateHandItemPos();
        }

        private void UpdateInteractableList()
        {
            //Check if there size of the object manager has changed, if it has, get and update the interactable list of objects here
            if (lastListSize != objectManager.TotalListChanges())
            {
                //This is a temporary way to get a list of interactable objects, I have an idea of how to have this optimised
                //but going to wait to see what Niall does before I update this.
                lastListSize = objectManager.ListSize();
                interactableList = objectManager.GetActorList(ActorType.Interactable);
            }
        }

        private void FindInteractables()
        {
            UpdateInteractableList();

            float closestDistance = float.MaxValue;
            InteractableActor closestActor = null;

            //Find the closest interactable actor
            foreach (DrawnActor3D actor in interactableList)
            {
                InteractableActor iActor = actor as InteractableActor;

                //If the actor is locked, ignore it
                if (iActor.Locked)
                    continue;

                float distance = iActor.GetDistance(this);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestActor = iActor;
                }
            }

            //If the player is in range of the interactable objects
            if (closestActor != null && closestDistance <= closestActor.InteractDistance)
                InteractWith(closestActor);
        }

        private void InteractWith(InteractableActor iActor)
        {
            //check if the interact key is pressed
            if (keyBoardManager.IsKeyDown(GameConstants.playerInteractKey))
            {
                //if player hand is empty
                if (handItem == null)
                {
                    //and actor is a pickup, place it in hand
                    if (iActor is HandHeldPickup)
                    {
                        handItem = iActor as HandHeldPickup;
                        if(handItem.PickupType == Enums.PickupType.Potion)
                            EventDispatcher.Publish(EventType.PotionPicked, new ArrayList());
                    }
                    //else actor is an ingredient giver, place item in hand
                    else if (iActor is IngredientGiver)
                        handItem = ((IngredientGiver)iActor).TakeItem();
                }
                //If our hand is not empty and actor is interactable container, transfer item
                else if (iActor is IContainerInteractable)
                {
                    IContainerInteractable container = iActor as IContainerInteractable;
                    //If deposit was successful, remove item
                    if (container.Deposit(handItem))
                    {
                        EventDispatcher.Publish(EventType.Remove, new ArrayList() { handItem.ID });
                        handItem = null;
                        return;
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
