using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.MyGame.Managers
{
    public class MyMenuManager : MenuManager
    {
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;

        public MyMenuManager(Game game, StatusType statusType, SpriteBatch spriteBatch,
            MouseManager mouseManager, KeyboardManager keyboardManager)
            : base(game, statusType, spriteBatch)
        {
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
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
                    StatusType = StatusType.Drawn | StatusType.Update;
                    SetScene("pause");
                }
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    StatusType = StatusType.Off;
            }
        }
        /// <summary>
        /// Handles Mouse and Keyboard input for menus
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        protected override void HandleInput(GameTime gameTime)
        {
            HandleMouse(gameTime);
            HandleKeyboard(gameTime);

            //base.HandleInput(gameTime); //nothing happening in the base method
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
                }
                else
                {
                    //show game
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu,
                        EventActionType.OnPlay, new object[] { gameTime }));
                }
            }

            base.HandleKeyboard(gameTime);
        }
    }
}