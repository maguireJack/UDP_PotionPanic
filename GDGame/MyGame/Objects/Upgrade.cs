using GDLibrary.Enums;
using GDLibrary.Events;
using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Upgrade
    {
        #region Fields

        //Tier should start at 1 and go in sequential order
        private Dictionary<int, Tuple<int, float>> tier_cost_value;
        private EventActionType eventActionType;
        private string name;
        private int currentTier;

        #endregion

        #region Properties

        public string Name 
        { 
            get
            {
                return "T" + currentTier + " " + name;
            }
        }

        public int CurrentTier
        {
            get
            {
                return currentTier;
            }
        }

        #endregion

        #region Constructor & Core

        public Upgrade(string name, EventActionType eventActionType,
            Dictionary<int, Tuple<int, float>> tier_cost_value)
        {
            this.name = name;
            this.eventActionType = eventActionType;
            this.tier_cost_value = tier_cost_value;
        }

        public void AquireUpgrade()
        {
            if(currentTier < tier_cost_value.Count)
            {
                currentTier++;
                EventDispatcher.Publish(new EventData(EventCategoryType.Upgrade,
                    eventActionType, new object[] { tier_cost_value[currentTier].Item2 }));
            }
        }

        public void AquireUpgrade(int tier)
        {
            if (tier < tier_cost_value.Count)
            {
                currentTier = tier;
                EventDispatcher.Publish(new EventData(EventCategoryType.Upgrade,
                    eventActionType, new object[] { tier_cost_value[currentTier].Item2 }));
            }
        }

        public int GetCost()
        {
            if(currentTier < tier_cost_value.Count)
            {
                return tier_cost_value[currentTier + 1].Item1;
            }
            return 0;
        }

        #endregion

    }
}
