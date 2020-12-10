using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Player : CharacterObject
    {
        #region Fields

        private ObjectManager objectManager;
        private KeyboardManager keyBoardManager;
        private GamePadManager gamePadManager;

        private PrimitiveObject helper;
        private Dictionary<string, Texture2D> helperTextures;
        private HandHeldPickup handItem;
        private Checklist checklist;
        private int score;

        private List<DrawnActor3D> interactableList;
        private int lastListSize;

        #endregion

        #region Properties

        public HandHeldPickup HandItem
        {
            get { return handItem; }
        }

        public Vector3 HandPos
        {
            get { return Transform3D.Translation + GameConstants.playerHoldPos; }
        }

        public int Score
        {
            get { return score; }
        }

        #endregion

        #region Constructors

        public Player(ModelObject modelObject,
            float radius, float height, float accelerationRate, float decelerationRate,
            ObjectManager objectManager, KeyboardManager keyboardManager, GamePadManager gamePadManager,
            IController controller, PrimitiveObject helper, Dictionary<string, Texture2D> helperTextures)
            : base(modelObject, radius, height, accelerationRate, decelerationRate)
        {
            this.objectManager = objectManager;
            this.keyBoardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.helper = helper;
            this.helperTextures = helperTextures;
            this.handItem = null;
            this.lastListSize = 0;
            this.score = 0;
            ControllerList.Add(controller);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);

            //TODO This is TEMPORARY
            foreach (Recipe recipe in GameConstants.potions.Keys)
            {
                if (checklist == null)
                {
                    AssignRecipe(recipe, GameConstants.potions[recipe][0] as string);
                    break;
                }
            }
        }

        #endregion

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnMinigameStir)
            {
                if (!checklist.IsDone &&
                    ((string)eventData.Parameters[1]).Equals(checklist.PotionName))
                {
                    double time = (double)eventData.Parameters[0];
                    int potionScore = 1000 - (checklist.Size * 100);
                    potionScore = (int)(potionScore * CalculatePercentageScore(time));

                    score += potionScore;
                    SendScoreEvent();
                    checklist.IsDone = true;
                }
            }
            else if (eventData.EventActionType == EventActionType.OnMinigameGrind)
            {
                double time = (double)eventData.Parameters[0];
                int minigameScore = 100;
                minigameScore = (int)(minigameScore * CalculatePercentageScore(time));

                if (checklist.CheckOffList((Ingredient)eventData.Parameters[1], minigameScore))
                {
                    score += minigameScore;
                    SendScoreEvent();
                }
            }
        }

        private double CalculatePercentageScore(double time)
        {
            if (time > 12000)       //20% of score
                return 20f / 100f;
            else if (time > 8000)   //50% of score
                return 50f / 100f;
            else if (time > 6000)   //70% of score
                return 70f / 100f;
            return 1;
        }

        private void SendScoreEvent()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.UI,
                        EventActionType.OnScoreChange, new object[] { score }));
        }

        public override void Update(GameTime gameTime)
        {
            if(GamePad.GetCapabilities(PlayerIndex.One).IsConnected)
            {
                helper.EffectParameters.Texture = helperTextures["helper_A"];
            }
            else helper.EffectParameters.Texture = helperTextures["helper_space"];
            FindInteractables();
            UpdateHandItemPos();

            base.Update(gameTime);
            
        }

        private void AssignRecipe(Recipe recipe, string potionName)
        {
            checklist = new Checklist(recipe, potionName);
        }

        private void UpdateInteractableList()
        {
            //Check if there size of the object manager has changed, if it has, get and update the interactable list of objects here
            if (lastListSize != objectManager.NewID())
            {
                //This is a temporary way to get a list of interactable objects, I have an idea of how to have this optimised
                //but going to wait to see what Niall does before I update this.
                lastListSize = objectManager.ListSize();
                interactableList = objectManager.GetActorList(ActorType.Interactable);

                if (handItem != null)
                    interactableList.Remove(handItem);
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
            {
                helper.Transform3D.Translation = closestActor.Transform3D.Translation;
                if (DisplayHelper(closestActor))
                {
                    helper.Transform3D.Translation = new Vector3(
                        closestActor.Transform3D.Translation.X + GameConstants.helperOffsetPos.X,
                        GameConstants.helperOffsetPos.Y,
                        closestActor.Transform3D.Translation.Z + GameConstants.helperOffsetPos.Z);
                    helper.StatusType = StatusType.Drawn;
                }
                else helper.StatusType = StatusType.Off;

                InteractWith(closestActor);
            }
            else helper.StatusType = StatusType.Off;
        }

        public bool DisplayHelper(InteractableActor iActor)
        {
            if (handItem == null)
            {
                if (iActor is HandHeldPickup || iActor is IngredientGiver)
                {
                    return true;
                }
            }
            else
            {
                if (iActor.Name.Equals("Bin"))
                {
                    return true;
                }
                else if (handItem.PickupType == PickupType.Ingredient)
                {
                    if (iActor is Cauldron)
                    {
                        return true;
                    }
                    else if(iActor is IngredientProccessor)
                    {
                        IngredientProccessor proccessor = iActor as IngredientProccessor;
                        if(proccessor.InputState == HandItem.Ingredient.IngredientState)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void InteractWith(InteractableActor iActor)
        {
            //check if the interact key is pressed
            if (keyBoardManager.IsAnyKeyPressedFirstTime(GameConstants.playerInteractKeys) ||
                gamePadManager.IsAnyButtonPressed(PlayerIndex.One, GameConstants.playerInteractButtons))
            {
                //if player hand is empty
                if (handItem == null)
                {
                    //and actor is a pickup, place it in hand
                    if (iActor is HandHeldPickup)
                    {
                        handItem = iActor as HandHeldPickup;

                        if (handItem.PickupType == PickupType.Potion)
                        {
                            //if potion, unlock the cauldron
                            EventDispatcher.Publish(new EventData(EventCategoryType.Interactable,
                                EventActionType.OnUnlock, new object[] { "Cauldron" }));
                        }
                        else if(handItem.PickupType == PickupType.Ingredient)
                        {
                            if(handItem.Ingredient.IngredientState != IngredientState.Solid)
                            {
                                //unlock ingredient processors
                                EventDispatcher.Publish(new EventData(EventCategoryType.Interactable,
                                    EventActionType.OnUnlock, new object[] { handItem.Ingredient.IngredientState.ToString() }));
                            }
                        }
                    }
                    //else actor is an ingredient giver, place item in hand
                    else if (iActor is IngredientGiver)
                    {
                        handItem = ((IngredientGiver)iActor).TakeItem();
                    }

                }
                //If our hand is not empty and actor is interactable container, transfer item
                else if (iActor is IContainerInteractable)
                {
                    IContainerInteractable container = iActor as IContainerInteractable;
                    //If deposit was successful, remove item
                    if (container.Deposit(handItem))
                    {
                        if (checklist.CheckOffList(handItem.Ingredient, 100))
                        {
                            score += 100;
                            SendScoreEvent();
                        }

                        interactableList.Remove(handItem);
                        EventDispatcher.Publish(new EventData(EventCategoryType.Object,
                            EventActionType.OnRemoveActor, new object[] { handItem }));
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
                //Translate the potions hold position to players hand pos
                Vector3 newHandPos = HandPos - handItem.HeldCoords;

                //Get players rotation
                float rotation = Transform3D.RotationInDegrees.Y;

                //Rotate hand item around the player's position
                newHandPos = Vector3.Transform(
                    newHandPos - Transform3D.Translation,
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotation)))
                    + Transform3D.Translation;

                //Set handitems translation
                handItem.Transform3D.Translation = newHandPos;

                //Rotate item to the players look direction
                handItem.Transform3D.RotateAroundUpBy(GDLibrary.MathUtility.CalculateRotationToVector(
                    handItem.Transform3D.Look, Transform3D.Look) * 10);
            }
        }

    }
}
