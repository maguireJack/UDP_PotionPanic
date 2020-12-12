using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDGame.MyGame.Minigames;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Cauldron : InteractableActor, IContainerInteractable
    {
        #region Fields

        private StirringMinigameController minigame;
        private Timer timer;
        private Checklist checklist;
        private int level;
        private int levelScore;

        #endregion

        #region Constructors

        public Cauldron(CollidableObject collidableObject, string name, float interactDistance,
            StirringMinigameController minigame)
            : base(collidableObject, name, interactDistance)
        {
            this.minigame = minigame;
            this.timer = new Timer();
            this.level = 1;
            this.levelScore = 0;

            EventDispatcher.Subscribe(EventCategoryType.Interactable, HandleEvent);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
            NewRecipe();
        }

        private void HandleEvent(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Interactable)
            {
                if (eventData.EventActionType == EventActionType.OnUnlock && ((string)eventData.Parameters[0]).Equals("Cauldron"))
                    Unlock();
            }
            else if (eventData.EventCategoryType == EventCategoryType.Player)
            {
                if (eventData.EventActionType == EventActionType.OnPotionDeposit)
                {
                    if(level < 4)
                    {
                        NewRecipe();
                    }
                    //else complete
                }
            }
        }

        #endregion

        private void NewRecipe()
        {
            List<Recipe> levelList = new List<Recipe>();
            foreach (Recipe key in GameConstants.potions.Keys)
            {
                if ((int)GameConstants.potions[key][2] == level)
                {
                    levelList.Add(key);
                }
            }

            Random random = new Random();
            int num = random.Next(levelList.Count);
            AssignRecipe(levelList[num], GameConstants.potions[levelList[num]][0] as string);
        }

        private void AssignRecipe(Recipe recipe, string potionName)
        {
            checklist = new Checklist(recipe, potionName);
            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnNewRecipe, new object[] { checklist }));
        }

        public override void Update(GameTime gameTime)
        {
            if((minigame.StatusType & StatusType.Update) == StatusType.Update)
            {
                if (!timer.IsRunning)
                {
                    timer.StartTimer(gameTime);
                }

                if (minigame.IsComplete())
                {
                    timer.StopTimer(gameTime);
                    StatusType = StatusType.Drawn;

                    int potionScore = 1000 - (checklist.Size * GameConstants.minigameScore);
                    potionScore = (int)(potionScore * CalculatePercentageScore(timer.ElapsedTime));

                    //Send score event
                    EventDispatcher.Publish(new EventData(EventCategoryType.UI,
                            EventActionType.OnScoreChange, new object[] { potionScore }));

                    //Send event to create potion and add to object manager
                    EventDispatcher.Publish(new EventData(EventCategoryType.Pickup,
                        EventActionType.OnCreate, new object[] { GameConstants.potions[checklist.Recipe] }));

                    levelScore = 0;
                    level++;
                }
                else minigame.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public bool Deposit(HandHeldPickup item)
        {
            switch(item.PickupType)
            {
                case PickupType.Ingredient:
                    Add(item.Ingredient);
                    CheckRecipes();
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void Add(Ingredient item)
        {
            if (checklist.CheckOffList(item))
            {
                levelScore += item.Score;
                EventDispatcher.Publish(new EventData(EventCategoryType.UI,
                        EventActionType.OnScoreChange, new object[] { item.Score }));
            }
            else
            {
                //remove score
                EventDispatcher.Publish(new EventData(EventCategoryType.UI,
                        EventActionType.OnScoreChange, new object[] 
                        { -levelScore - (checklist.CheckedCount() * GameConstants.minigameScore/2) }));
                checklist.Reset();
            }
        }

        private void CheckRecipes()
        {
            if (checklist.HasAllIngredients())
            {
                Lock();
                minigame.Start();
                StatusType = StatusType.Drawn | StatusType.Update;
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
            return 1;               //100% of score
        }
    }
}
