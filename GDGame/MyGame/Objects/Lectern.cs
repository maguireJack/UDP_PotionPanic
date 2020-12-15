using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Lectern : InteractableActor
    {
        private UIManager uiManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private UITextureObject background;
        private UITextureObject backgroundController;
        private Dictionary<string, Texture2D> textureDictionary;
        private List<DrawnActor2D> loadedTextures;
        private Timer timer;
        private Checklist checklist;
        private SpriteFont spriteFont;

        public Lectern(CollidableObject modelObject, string name, float interactDistance,
            UIManager uiManager, KeyboardManager keyboardManager, GamePadManager gamePadManager,
            UITextureObject background, UITextureObject backgroundController,
            Dictionary<string, Texture2D> textureDictionary, SpriteFont spriteFont)
            : base(modelObject, name, interactDistance)
        {
            this.uiManager = uiManager;
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.background = background;
            this.backgroundController = backgroundController;
            this.textureDictionary = textureDictionary;
            this.spriteFont = spriteFont;
            this.loadedTextures = new List<DrawnActor2D>();
            this.timer = new Timer(500);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvents);
        }
        /// <summary>
        /// Handles events fot Lectern, displays potion page
        /// </summary>
        /// <param name="eventData"></param>
        private void HandleEvents(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Player)
            {
                if (eventData.EventActionType == EventActionType.OnNewRecipe)
                {
                    checklist = eventData.Parameters[0] as Checklist;
                    Display();
                }
            }
        }
        /// <summary>
        /// Displays potion recipe page
        /// </summary>
        public void Display()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnLock, null));

            EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                EventActionType.OnRestart, new object[] { "page_turn" }));

            if (GamePad.GetCapabilities(PlayerIndex.One).IsConnected)
                backgroundController.StatusType = StatusType.Drawn;
            else background.StatusType = StatusType.Drawn;
            StatusType = StatusType.Drawn | StatusType.Update;

            Texture2D texture;
            Vector2 translation;
            Transform2D transform2D;
            UITextureObject uiTexture;

            List<Tuple<Ingredient, bool>> list = checklist.List;
            for (int i = 0; i < list.Count; i++)
            {
                string key = list[i].Item1.Name;

                translation = new Vector2(
                        background.Transform2D.Translation.X - 400 + (400 * i),
                        background.Transform2D.Translation.Y - 241);

                texture = textureDictionary[key];
                transform2D = new Transform2D(translation, 0,
                    Vector2.One,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    new Integer2(texture.Width, texture.Height));

                uiTexture = new UITextureObject("Lectern_UI_" + key, ActorType.UITextureObject,
                StatusType.Drawn, transform2D, Color.White, 4, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

                string text = key.Replace('_', ' ');
                Vector2 originalDimensions = spriteFont.MeasureString(text);

                transform2D = new Transform2D(
                    new Vector2(translation.X, translation.Y + 140), 0,
                    Vector2.One,
                    new Vector2(originalDimensions.X / 2, originalDimensions.Y / 2),
                    new Integer2(originalDimensions));

                UITextObject textObject = new UITextObject("score", ActorType.UIText,
                    StatusType.Drawn, transform2D,
                    Color.Black, 4, SpriteEffects.None,
                    text, spriteFont);

                loadedTextures.Add(uiTexture);
                loadedTextures.Add(textObject);
                uiManager.Add(uiTexture);
                uiManager.Add(textObject);

                bool contains = false;
                if (key.Contains("Dust"))
                {
                    contains = true;
                    texture = textureDictionary["grinder"];
                }
                else if (key.Contains("Liquid"))
                {
                    contains = true;
                    texture = textureDictionary["liquid"];
                }
                if (contains)
                {
                    transform2D = new Transform2D(translation + new Vector2(0, 300), 0,
                        Vector2.One,
                        new Vector2(texture.Width / 2, texture.Height / 2),
                        new Integer2(texture.Width, texture.Height));

                    uiTexture = new UITextureObject("Lectern_UI_" + key, ActorType.UITextureObject,
                    StatusType.Drawn, transform2D, Color.White, 4, SpriteEffects.None, texture,
                    new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
                    loadedTextures.Add(uiTexture);
                    uiManager.Add(uiTexture);
                }
            }
        }
        /// <summary>
        /// Removes the recipe UI when the player presses a key and removes movement lock
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        public override void Update(GameTime gameTime)
        {
            if(keyboardManager.IsAnyKeyPressedFirstTime(GameConstants.playerInteractKeys) ||
                gamePadManager.IsAnyButtonPressed(0, GameConstants.playerInteractButtons))
            {
                foreach(DrawnActor2D uiTexture in loadedTextures)
                {
                    uiManager.UIObjectList.Remove(uiTexture);
                }
                loadedTextures.Clear();

                background.StatusType = StatusType.Off;
                backgroundController.StatusType = StatusType.Off;
                Lock();
                EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnUnlock, null));

                timer.StartTimer(gameTime);
            }
            else if(timer.IsRunning)
            {
                if(timer.IsDone(gameTime))
                {
                    Unlock();
                    StatusType = StatusType.Drawn;
                }
            }
            base.Update(gameTime);
        }
    }
}
