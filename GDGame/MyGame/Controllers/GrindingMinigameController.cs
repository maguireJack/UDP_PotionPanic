﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using GDGame.MyGame.Constants;
using Microsoft.Xna.Framework.Input;
namespace GDGame.MyGame.Controllers
{
    public class GrindingMinigameController : Controller
    {
        private KeyboardManager keyboardManager;
        private int ACount, BCount;
        private Keys[][] moveKeys;
        private bool mashingComplete;
        private bool AButtonReleased;
        private bool BButtonReleased;
        public GrindingMinigameController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager, Keys[][] moveKeys)
            : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.moveKeys = moveKeys;
            this.ACount = 0;
            this.BCount = 0;
            this.mashingComplete = false;
            this.AButtonReleased = true;
            this.BButtonReleased = true;
        }


        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (mashingComplete)
            {
                drawnActor.StatusType = StatusType.Off;
            }

            else if (actor != null)
            {
                if (capabilities.IsConnected)
                {
                    HandleControlerMashing(gameTime, actor, capabilities);
                }
                else
                {
                    HandleMashing(gameTime, actor);
                }
            }

            base.Update(gameTime, actor);
        }

        private void HandleControlerMashing(GameTime gameTime, IActor actor, GamePadCapabilities capabilities)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (!mashingComplete)
            {
                if (capabilities.HasAButton)
                {
                    if (state.IsButtonDown(Buttons.A) && AButtonReleased == true) 
                    {
                        ACount += (int)ButtonState.Pressed;
                        Debug.WriteLine("A pressed");
                        Debug.WriteLine("A = " + ACount);
                        AButtonReleased = false;
                    }
                    if (state.IsButtonUp(Buttons.A)) { AButtonReleased = true; }
                }
                if (capabilities.HasXButton)
                {
                    if (state.IsButtonDown(Buttons.B) && BButtonReleased == true)
                    {
                        BCount++;
                        Debug.WriteLine("B pressed");
                        Debug.WriteLine("B = " + BCount);
                        BButtonReleased = false;
                    }
                    if (state.IsButtonUp(Buttons.B)) { BButtonReleased = true; }
                }
                if (ACount >= 10 && BCount >= 10)
                {
                    mashingComplete = true;
                }

            }
        } 
        private void HandleMashing(GameTime gameTime, IActor actor)
        {
            if (!mashingComplete) 
            {
                if (keyboardManager.IsFirstKeyPress(Keys.D))
                {
                    ACount++;
                    //Debug.WriteLine("A pressed");
                    //Debug.WriteLine("A = " + ACount);
                }


                else if (keyboardManager.IsFirstKeyPress(Keys.A))
                {
                    BCount++;
                    //Debug.WriteLine("B pressed = " +BCount);
                }
                
            }
            if (ACount >= 10 && BCount >= 10) 
            {
                
                mashingComplete = true;
            }
        }
    }
}