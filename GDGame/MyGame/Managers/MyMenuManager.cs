using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GDGame.MyGame.Managers
{
    public class MyMenuManager : MenuManager
    {
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private List<DrawnActor2D> loadedTempTexture;
        private bool batchRemove;

        public MyMenuManager(Game game, StatusType statusType, SpriteBatch spriteBatch,
            MouseManager mouseManager, KeyboardManager keyboardManager)
            : base(game, statusType, spriteBatch)
        {
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
            this.loadedTempTexture = new List<DrawnActor2D>();
            this.batchRemove = false;
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
        }
        /// <summary>
        /// Handles Menu Events
        /// </summary>
        /// <param name="eventData">Menu Events</param>
        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                {
                    StatusType = StatusType.Update | StatusType.Drawn;
                    SetScene("pause");
                    
                }
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    StatusType = StatusType.Off;
            }
            else if(eventData.EventCategoryType == EventCategoryType.Player)
            {
                if (eventData.EventActionType == EventActionType.OnGameOver)
                {
                    StatusType = StatusType.Update | StatusType.Drawn;
                    loadedTempTexture = (List<DrawnActor2D>)eventData.Parameters[0];
                    foreach (DrawnActor2D texture in loadedTempTexture)
                    {
                        Add("score", texture);
                    }
                    SetScene("score");
                }
            }
        }
        /// <summary>
        /// Handles Mouse and Keyboard input for menus
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        protected override void HandleInput(GameTime gameTime)
        {
            if(batchRemove)
            {
                foreach (DrawnActor2D actor in loadedTempTexture)
                {
                    Remove("score", a => a.ID == actor.ID);
                }
                batchRemove = false;
            }
            HandleMouse(gameTime);
            HandleKeyboard(gameTime);

            //base.HandleInput(gameTime);
        }
        /// <summary>
        /// handles mouse interaction with the menu
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        protected override void HandleMouse(GameTime gameTime)
        {
            foreach (DrawnActor2D actor in this.ActiveList)
            {
                if (actor is UIButtonObject)
                {
                    if (actor.Transform2D.Bounds.Contains(this.mouseManager.Bounds))
                    {
                        if (this.mouseManager.IsLeftButtonClickedOnce())
                        {
                            HandleClickedButton(gameTime, actor as UIButtonObject);
                        }
                    }
                }
            }
            base.HandleMouse(gameTime);
        }
        /// <summary>
        /// Handles what happens when a button is clicked
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        /// <param name="uIButtonObject">Contains what the button does(start game, show control sceme etc)</param>
        private void HandleClickedButton(GameTime gameTime, UIButtonObject uIButtonObject)
        {
            switch (uIButtonObject.ID)
            {
                case "play_btn":
                    SetScene("game");
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, new object[] { gameTime }));
                    EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, new object[] { "main_menu" }));
                    break;

                case "controls_btn":
                    SetScene("controls");
                    break;

                case "menu_btn":
                    SetScene("main");
                    break;

                case "exit_btn":
                    Game.Exit();
                    break;

                case "back_btn":
                    SetScene("main");
                    break;

                case "score_menu_btn":
                    batchRemove = true;
                    SetScene("main");
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// Handles keyboard interaction with menu
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        protected override void HandleKeyboard(GameTime gameTime)
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Escape))
            {
                if (StatusType == StatusType.Off)
                {
                    //show menu
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu,
                        EventActionType.OnPause, new object[] { gameTime }));
                    EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPause, new object[] { "main_menu" }));

                }
                else
                {
                    //show game
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu,
                        EventActionType.OnPlay, new object[] { gameTime }));
                    EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, new object[] { "main_menu" }));
                }
            }

            base.HandleKeyboard(gameTime);
        }
    }
}