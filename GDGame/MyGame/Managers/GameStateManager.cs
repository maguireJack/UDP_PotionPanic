using GDGame.MyGame.Constants;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Controllers
{
    public class GameStateManager : PausableGameComponent
    {
        private UITextureObject timeBackground;
        private UITextureObject timeBar;
        private UITextObject timeTextObject;
        private UITextObject scoreTextObject;
        private GameTime gameTime;
        private int score;
        private float startTime;
        private Timer timer;

        private Texture2D starEmpty;
        private Texture2D starPerfect;
        private Texture2D star;


        public GameStateManager(Game game, StatusType statusType,
            UITextureObject timeBackground, UITextureObject timeBar,
            UITextObject timeTextObject, UITextObject scoreTextObject,
            Texture2D starEmpty, Texture2D starPerfect, Texture2D star)
            : base(game, statusType)
        {
            this.timeBackground = timeBackground;
            this.timeBar = timeBar;
            this.timeTextObject = timeTextObject;
            this.scoreTextObject = scoreTextObject;
            this.score = 0;
            this.startTime = 120000;
            this.timer = new Timer(startTime);

            this.starEmpty = starEmpty;
            this.starPerfect = starPerfect;
            this.star = star;

            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvent);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
        }
        /// <summary>
        /// Updates the players score
        /// </summary>
        /// <param name="eventData">contains eventActionType that enables a score change</param>
        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.UI)
            {
                if (eventData.EventActionType == EventActionType.OnScoreChange)
                {
                    score += (int)eventData.Parameters[0];
                    scoreTextObject.Text = "Score: " + score;
                }
            }
            else if (eventData.EventCategoryType == EventCategoryType.Player)
            {
                if (eventData.EventActionType == EventActionType.OnWin)
                {
                    EndGame(gameTime);
                }
            }
            base.HandleEvent(eventData);
        }
        /// <summary>
        /// Updates the game timer and its its UI
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            if (StatusType == StatusType.Update)
            {
                if (!timer.IsRunning)
                {
                    timer.StartTimer(gameTime);
                }

                timer.UpdateTime(gameTime);
                double time = Math.Round((startTime - timer.ElapsedTime) / 60000d, 2);
                timeTextObject.Text = Math.Floor(time) + ":" + Math.Round(Math.Abs(time - Math.Truncate(time)) * 60);
                if (timeTextObject.Text.Length == 3)
                {
                    string temp = timeTextObject.Text.Substring(0, 2);
                    timeTextObject.Text = temp + "0" + timeTextObject.Text[2];
                }

                timeBar.Transform2D.Scale = new Vector2(
                    timeBar.Transform2D.Scale.X,
                    ((float)(startTime - timer.ElapsedTime) / startTime) * 244);

                timeBar.Transform2D.Translation = new Vector2(
                    timeBar.Transform2D.Translation.X,
                    (28 + timeBackground.Transform2D.Bounds.Height / 2) + (244 - timeBar.Transform2D.Scale.Y) / 2);

                if (timer.ElapsedTime >= startTime)
                {
                    EndGame(gameTime);
                }

                base.Update(gameTime);
            }
        }

        private void EndGame(GameTime gameTime)
        {
            timer.StopTimer(gameTime);
            timeTextObject.Text = "0:00";
            StatusType = StatusType.Off;
            ScoreScreen();
        }

        private void ScoreScreen()
        {
            Texture2D texture;
            Vector2 translation;
            Transform2D transform2D;
            UITextureObject uiTexture;
            List<DrawnActor2D> loadedTextures = new List<DrawnActor2D>();

            for (int i = 1; i <= 3; i++)
            {
                translation = new Vector2(
                            GameConstants.screenCentre.X - 600 + (300 * i),
                            GameConstants.screenCentre.Y - 150);

                if (score >= 3000)
                    texture = starPerfect;
                else if (score >= 800 * i)
                    texture = star;
                else texture = starEmpty;

                transform2D = new Transform2D(translation, 0,
                    Vector2.One,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    new Integer2(texture.Width, texture.Height));

                uiTexture = new UITextureObject("score_" + texture.Name + i, ActorType.UITextureObject,
                StatusType.Drawn, transform2D, Color.White, 4, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

                loadedTextures.Add(uiTexture);
            }

            UITextObject clone = scoreTextObject.Clone() as UITextObject;
            clone.Transform2D.Translation = GameConstants.screenCentre + new Vector2(0, 30);
            loadedTextures.Add(clone);

            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnGameOver, new object[] { loadedTextures }));

            score = 0;
            scoreTextObject.Text = "Score: " + score;
        }
    }
}
