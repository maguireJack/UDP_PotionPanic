using GDLibrary.Enums;
using GDLibrary.Events;
using Microsoft.Xna.Framework;

namespace GDLibrary.Parameters
{
    public class Timer
    {
        #region Fields

        private double timerLengthMS;
        private double startTime;
        private double elapsedTime;
        private bool isRunning;

        private bool isPaused;
        private double startPause;
        private double totalPauseTimeMS;

        #endregion

        #region Fields

        public double TimerLengthMS
        { 
            get { return timerLengthMS; } 
            set { timerLengthMS = value; }
        }

        public double ElapsedTime
        {
            get { return elapsedTime; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public bool IsPaused
        {
            get { return isPaused; }
        }

        #endregion

        #region Constructors

        public Timer(double timerLengthMS)
        {
            this.timerLengthMS = timerLengthMS;
            isRunning = false;
            isPaused = false;
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);
        }

        public Timer()
        {
            timerLengthMS = 0;
            isRunning = false;
            isPaused = false;
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);
        }

        #endregion

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                    Pause(eventData.Parameters[0] as GameTime);
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    Resume(eventData.Parameters[0] as GameTime);
            }
        }

        public void StartTimer(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            elapsedTime = 0;
            isRunning = true;
            isPaused = false;
            startPause = 0;
            totalPauseTimeMS = 0;
        }

        public void Pause(GameTime gameTime)
        {
            if (isPaused)
                return;
            isPaused = true;
            startPause = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public void Resume(GameTime gameTime)
        {
            if (!isPaused)
                return;
            isPaused = false;
            totalPauseTimeMS += gameTime.TotalGameTime.TotalMilliseconds - startPause;
            startPause = 0;
        }

        public void StopTimer(GameTime gameTime)
        {
            elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - startTime - totalPauseTimeMS;
            isRunning = false;
            isPaused = false;
            startPause = 0;
            totalPauseTimeMS = 0;
        }

        public void UpdateTime(GameTime gameTime)
        {
            elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - startTime - totalPauseTimeMS;
        }

        public bool IsDone(GameTime gameTime)
        {
            elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - startTime - totalPauseTimeMS;
            if(elapsedTime >= timerLengthMS)
            {
                isRunning = false;
                return true;
            }
            return false;
        }
    }
}
