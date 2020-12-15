using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GDGame.MyGame.Actors;
using System.Collections.Generic;
using GDLibrary.Events;

namespace GDGame.MyGame.Minigames
{
    public class GrindingMinigameController : Minigame
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private Dictionary<string, UITextureObject> uiPanels;
        private int count;
        private bool keyA;

        /// <summary>
        /// Grinding minigame cintroler hanldes the ui, user input and logic of the grinding minigame.
        /// Worked on by Aidan and Vilandas
        /// </summary>
        public GrindingMinigameController(string id, ActorType actorType, StatusType statusType,
            KeyboardManager keyboardManager, GamePadManager gamePadManager,
            Dictionary<string, UITextureObject> uiPanels)
            : base(id, actorType, statusType)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.count = 0;
            this.keyA = true;
            this.uiPanels = uiPanels;
        }

        /// <summary>
        /// Start() draws the UI for the minigame and changes the status type to update
        /// </summary>
        public override void Start()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                EventActionType.OnPause, new object[] { "walking" }));

            EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                EventActionType.OnRestart,new object[] { "pestol" }));
            SendLockEvent();
            uiPanels["grinding_A"].StatusType = StatusType.Drawn;
            StatusType = StatusType.Update;
        }

        /// <summary>
        /// Is complete checks if the minigame is complete by seeing if the count of button presses is 20,
        /// </summary>
        /// <returns>true if minigame is complete</returns>
        public override bool IsComplete()
        {
            if (count >= 20)
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnPause, new object[] { "pestol" }));
                count = 0;
                keyA = true;
                StatusType = StatusType.Off;
                foreach (UITextureObject uiPanel in uiPanels.Values)
                    uiPanel.StatusType = StatusType.Off;

                SendUnlockEvent();
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnPlay, new object[] { "ping" }));
                
                return true;
            }
            return false;
        }
        /// <summary>
        /// Handles the control schemes for the minigames and updates the minigame
        /// </summary>
        /// <param name="gameTime">Passes time related information, Is required to update Actors</param>
        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetCapabilities(PlayerIndex.One).IsConnected)
                HandleController();
            else HandleKeyboard();

            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the Xbox button prompt UI and counts the A & B presses,
        /// It will only count the button presses if done alternatingly.
        /// </summary>
        private void HandleController()
        {
            if (uiPanels["grinding_A"].StatusType == StatusType.Drawn)
            {
                uiPanels["grinding_xbox_A"].StatusType = StatusType.Drawn;
                uiPanels["grinding_A"].StatusType = StatusType.Off;
            }
            else if (uiPanels["grinding_D"].StatusType == StatusType.Drawn)
            {
                uiPanels["grinding_xbox_B"].StatusType = StatusType.Drawn;
                uiPanels["grinding_D"].StatusType = StatusType.Off;
            }

            if (keyA && gamePadManager.IsFirstButtonPress(0, Buttons.A))
            {
                count++;
                keyA = false;
                uiPanels["grinding_xbox_A"].StatusType = StatusType.Off;
                uiPanels["grinding_xbox_B"].StatusType = StatusType.Drawn;
            }
            else if (gamePadManager.IsFirstButtonPress(0, Buttons.B))
            {
                count++;
                keyA = true;
                uiPanels["grinding_xbox_A"].StatusType = StatusType.Drawn;
                uiPanels["grinding_xbox_B"].StatusType = StatusType.Off;
            }
        }
        /// <summary>
        /// Draws the Keyboard button prompt UI and counts the A & D presses,
        /// It will only count the button presses if done alternatingly.
        /// </summary>
        private void HandleKeyboard()
        {
            if (uiPanels["grinding_xbox_A"].StatusType == StatusType.Drawn)
            {
                uiPanels["grinding_xbox_A"].StatusType = StatusType.Off;
                uiPanels["grinding_A"].StatusType = StatusType.Drawn;
            }
            else if (uiPanels["grinding_xbox_B"].StatusType == StatusType.Drawn)
            {
                uiPanels["grinding_xbox_B"].StatusType = StatusType.Off;
                uiPanels["grinding_D"].StatusType = StatusType.Drawn;
            }

            if (keyA && keyboardManager.IsFirstKeyPress(Keys.A))
            {
                count++;
                keyA = false;
                uiPanels["grinding_A"].StatusType = StatusType.Off;
                uiPanels["grinding_D"].StatusType = StatusType.Drawn;
            }
            else if (!keyA && keyboardManager.IsFirstKeyPress(Keys.D))
            {
                count++;
                keyA = true;
                uiPanels["grinding_A"].StatusType = StatusType.Drawn;
                uiPanels["grinding_D"].StatusType = StatusType.Off;
            }
        }
    }
}