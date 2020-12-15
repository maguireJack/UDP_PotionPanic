using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDGame.MyGame.Constants;
using GDLibrary.Events;
using GDLibrary.Controllers;
using GDLibrary.Parameters;
using GDLibrary;

namespace GDGame.MyGame.Controllers
{
    public class InvalidInputController : Controller
    {
        #region Fields

        private float startTime;
        private Timer timer;

        #endregion

        #region Constructors

        public InvalidInputController(string id, ControllerType controllerType)
            : base(id, controllerType)
        {
            startTime = 8000;
            timer = new Timer(startTime);
        }

        #endregion

        public override void Update(GameTime gameTime, IActor actor)
        {
            PrimitiveObject ui = actor as PrimitiveObject;
            if (!timer.IsRunning)
            {
                timer.StartTimer(gameTime);
            }

            timer.UpdateTime(gameTime);
            if (timer.ElapsedTime > 3000)
            {
                ui.EffectParameters.Alpha = (startTime - (float)timer.ElapsedTime) / startTime;
            }

            if(timer.IsDone(gameTime))
            {
                timer.StopTimer(gameTime);
                ui.StatusType = StatusType.Off;
            }

            base.Update(gameTime, actor);
        }

        public new object Clone()
        {
            throw new NotImplementedException();
        }

        public new ControllerType GetControllerType()
        {
            throw new NotImplementedException();
        }
    }
}
