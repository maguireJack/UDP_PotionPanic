using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDGame.MyGame.Controllers
{
    public class GameStateManager : PausableGameComponent
    {
        private UIManager uiManager;
        private UITextureObject timeBackground;
        private UITextureObject timeBar;
        private UITextObject timeTextObject;
        private UITextObject scoreTextObject;
        private int score;
        private float startTime;
        private Timer timer;

        private UITextureObject scoreBackground;
        private Texture2D starEmpty;
        private Texture2D starPerfect;
        private Texture2D star;


        public GameStateManager(Game game, StatusType statusType, UIManager uiManager,
            UITextureObject timeBackground, UITextureObject timeBar,
            UITextObject timeTextObject, UITextObject scoreTextObject,
            UITextureObject scoreBackground, Texture2D starEmpty,
            Texture2D starPerfect, Texture2D star)
            : base(game, statusType)
        {
            this.uiManager = uiManager;
            this.timeBackground = timeBackground;
            this.timeBar = timeBar;
            this.timeTextObject = timeTextObject;
            this.scoreTextObject = scoreTextObject;
            this.score = 0;
            this.startTime = 120000;
            this.timer = new Timer(startTime);

            this.scoreBackground = scoreBackground;
            this.starEmpty = starEmpty;
            this.starPerfect = starPerfect;
            this.star = star;

            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvent);
        }
        /// <summary>
        /// Updates the players score
        /// </summary>
        /// <param name="eventData">contains eventActionType that enables a score change</param>
        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnScoreChange)
            {
                score += (int)eventData.Parameters[0];
                scoreTextObject.Text = "Score: " + score;
            }

            base.HandleEvent(eventData);
        }
        /// <summary>
        /// Updates the game timer and its its UI
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        public override void Update(GameTime gameTime)
        {
            if (!timer.IsRunning && StatusType == StatusType.Update)
            {
                timer.StartTimer(gameTime);
            }

            timer.UpdateTime(gameTime);
            double time = Math.Round((startTime - timer.ElapsedTime) / 60000d, 2);
            timeTextObject.Text = Math.Floor(time) + ":" + (Math.Round(Math.Abs(time - Math.Truncate(time)) * 60));

            timeBar.Transform2D.Scale = new Vector2(
                timeBar.Transform2D.Scale.X,
                ((float)(startTime - timer.ElapsedTime) / startTime) * 244);

            timeBar.Transform2D.Translation = new Vector2(
                timeBar.Transform2D.Translation.X,
                (28 + timeBackground.Transform2D.Bounds.Height / 2) + (244 - timeBar.Transform2D.Scale.Y) / 2);

            if(timer.ElapsedTime >= startTime)
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                    EventActionType.OnLose, null));
                StatusType = StatusType.Off;
                ScoreScreen();
            }

            base.Update(gameTime);
        }

        private void ScoreScreen()
        {
            scoreBackground.StatusType = StatusType.Drawn;
            if(score >= 3000)
            {

            }
            for(int i = 1; i <= 3; i++)
            {
                if(score >= 1000 * i)
                {

                }
            }
        }
    }
}
