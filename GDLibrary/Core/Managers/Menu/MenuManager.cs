using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Allows us to create a pausable drawable component that stores multiple menu scenes (e.g. main, audio, controls) in a dictionary
    /// and updates/draws the appropriate scene to the scene. 
    /// </summary>
    public class MenuManager : PausableDrawableGameComponent
    {
        private Dictionary<string, MenuScene> dictionary;
        private MenuScene activeMenuScene;
        private SpriteBatch spriteBatch;

        #region Constructors & Core
        public MenuManager(Game game, StatusType statusType, SpriteBatch spriteBatch)
           : base(game, statusType)
        {
            this.dictionary = new Dictionary<string, MenuScene>();
            this.activeMenuScene = null;
            this.spriteBatch = spriteBatch;
        }

        public void Add(string sceneID, DrawnActor2D actor)
        {
            if (this.dictionary.ContainsKey(sceneID))
            {
                MenuScene menuScene = this.dictionary[sceneID];
                menuScene.Add(actor);
            }
            else
            {
                MenuScene newScene = new MenuScene(sceneID);
                newScene.Add(actor);
                this.dictionary.Add(sceneID, newScene);
            }
        }

        public bool SetScene(string sceneID)
        {
            if (!this.dictionary.ContainsKey(sceneID))
                throw new System.Exception("Invalid scene ID - " + sceneID);

            //if valid then access the menu scene and set as active
            activeMenuScene = this.dictionary["sceneID"];
            return true;
        }


        public bool Remove(/*params*/)
        {
            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //just in case user didn't call SetScene
            if (activeMenuScene != null)
            {
                for (int i = 0; i < activeMenuScene.Count; i++)
                {
                    DrawnActor2D actor = activeMenuScene[i];
                    actor.Update(gameTime);
                }
                //HandleInput();
            }
        }
        protected override void ApplyDraw(GameTime gameTime)
        {
            //just in case user didn't call SetScene
            if (activeMenuScene != null)
            {
                for (int i = 0; i < activeMenuScene.Count; i++)
                    activeMenuScene[i].Draw(gameTime, spriteBatch);
            }
        }
        #endregion Constructors & Core
    }
}