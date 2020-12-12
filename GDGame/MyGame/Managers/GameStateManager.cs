using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace GDGame.MyGame.Controllers
{
    public class GameStateManager : PausableGameComponent
    {
        private UITextureObject timeBackground;
        private UITextureObject timeBar;
        private UITextObject timeTextObject;
        private UITextObject scoreTextObject;
        private int score;
        private float startTime;
        private Timer timer;

        public GameStateManager(Game game, StatusType statusType,
            UITextureObject timeBackground, UITextureObject timeBar,
            UITextObject timeTextObject, UITextObject scoreTextObject)
            : base(game, statusType)
        {
            this.timeBackground = timeBackground;
            this.timeBar = timeBar;
            this.timeTextObject = timeTextObject;
            this.scoreTextObject = scoreTextObject;
            this.score = 0;
            this.startTime = 120000;
            this.timer = new Timer(startTime);

            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvent);
        }

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnScoreChange)
            {
                score = (int)eventData.Parameters[0];
                scoreTextObject.Text = "Score: " + score;
            }

            base.HandleEvent(eventData);
        }

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
            }

            base.Update(gameTime);
        }
    }
}
