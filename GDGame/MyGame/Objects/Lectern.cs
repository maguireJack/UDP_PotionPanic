using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Dictionary<string, Texture2D> textureDictionary;
        private List<UITextureObject> loadedTextures;
        private Timer timer;
        private Checklist checklist;

        public Lectern(CollidableObject modelObject, string name, float interactDistance,
            UIManager uiManager, KeyboardManager keyboardManager, GamePadManager gamePadManager,
            UITextureObject background, Dictionary<string, Texture2D> textureDictionary)
            : base(modelObject, name, interactDistance)
        {
            this.uiManager = uiManager;
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.background = background;
            this.textureDictionary = textureDictionary;
            this.loadedTextures = new List<UITextureObject>();
            this.timer = new Timer(500);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvents);
        }

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

        public void Display()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnLock, null));

            background.StatusType = StatusType.Drawn;
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

                loadedTextures.Add(uiTexture);
                uiManager.Add(uiTexture);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(keyboardManager.IsAnyKeyPressedFirstTime(GameConstants.playerInteractKeys) ||
                gamePadManager.IsAnyButtonPressed(0, GameConstants.playerInteractButtons))
            {
                foreach(UITextureObject uiTexture in loadedTextures)
                {
                    uiManager.UIObjectList.Remove(uiTexture);
                }
                loadedTextures.Clear();

                background.StatusType = StatusType.Off;
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
