﻿using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Handlers
{
    public class PlayerSpawnEventHandler : EventHandler
    {
        public PlayerSpawnEventHandler(EventCategoryType eventCategoryType, IActor parent)
            : base(eventCategoryType, parent)
        {

        }
        /// <summary>
        /// Spawns the player
        /// </summary>
        /// <param name="eventData">Event that tells player to spawn</param>
        public override void HandleEvent(EventData eventData)
        {
            if(eventData.EventActionType == EventActionType.OnSpawn)
            {
                object[] parameters = eventData.Parameters;
                Vector3 pos = (Vector3)parameters[2];

                Actor3D p = Parent as Actor3D;

                p.StatusType = StatusType.Off;
            }
        }
    }
}
