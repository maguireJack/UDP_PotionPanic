using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Checklist
    {
        #region Fields

        private string potionName;
        private bool isDone;
        //                 Ingredient, isDone, score
        private List<Tuple<Ingredient, bool, int>> list;

        #endregion

        public string PotionName
        {
            get { return potionName; }
        }

        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }

        public int Size 
        { 
            get { return list.Count; }
        }

        #region Constructor

        public Checklist(Recipe recipe, string potionName)
        {
            this.potionName = potionName;
            list = new List<Tuple<Ingredient, bool, int>>();

            foreach (Ingredient key in recipe.Ingredients.Keys)
            {
                int count = recipe.Ingredients[key];
                for (int i = 0; i < count; i++)
                {
                    list.Add(new Tuple<Ingredient, bool, int>(key, false, 0));
                }
            }
        }

        #endregion

        public bool CheckOffList(Ingredient item, int score)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].Item2 == false && list[i].Item1.Equals(item))
                {
                    list[i] = new Tuple<Ingredient, bool, int>(item, true, score);
                    return true;
                }
            }
            return false;
        }
    }
}
