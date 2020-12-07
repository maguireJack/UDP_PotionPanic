using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        protected override void HandleInput(GameTime gameTime)
        {
            HandleMouse(gameTime);

            //base.HandleInput(gameTime); //nothing happening in the base method
        }

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

        private void HandleClickedButton(GameTime gameTime, UIButtonObject uIButtonObject)
        {
            throw new NotImplementedException();
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            base.HandleKeyboard(gameTime);
        }
    }
}