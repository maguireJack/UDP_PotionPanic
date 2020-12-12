using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Checklist
    {
        #region Fields

        private string potionName;
        private bool isDone;
        //                 Ingredient, isDone
        private List<Tuple<Ingredient, bool>> list;
        private Recipe recipe;

        #endregion

        public string PotionName
        {
            get { return potionName; }
        }

        public List<Tuple<Ingredient, bool>> List
        {
            get { return list; }
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

        public Recipe Recipe
        {
            get { return recipe; }
        }

        #region Constructor

        public Checklist(Recipe recipe, string potionName)
        {
            this.potionName = potionName;
            this.recipe = recipe;
            list = new List<Tuple<Ingredient, bool>>();

            foreach (Ingredient key in recipe.Ingredients.Keys)
            {
                int count = recipe.Ingredients[key];
                for (int i = 0; i < count; i++)
                {
                    list.Add(new Tuple<Ingredient, bool>(key, false));
                }
            }
        }

        #endregion

        /// <summary>
        /// Check if the input item is in the list and not
        /// already accounted for.
        /// </summary>
        /// <param name="item">Ingredient to check the list for</param>
        /// <returns>True if the ingredient is on the list and not accounted for yet</returns>
        public bool CheckOffList(Ingredient item)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].Item2 == false && list[i].Item1.Equals(item))
                {
                    list[i] = new Tuple<Ingredient, bool>(item, true);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reset the done state to false for all items
        /// </summary>
        public void Reset()
        {
            for(int i = 0; i < list.Count; i++)
            {
                list[i] = new Tuple<Ingredient, bool>(list[i].Item1, false);
            }
        }

        /// <summary>
        /// Check if all the ingredients in the list are checked true
        /// </summary>
        /// <returns>True if all the ingredients are checked</returns>
        public bool HasAllIngredients()
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Item2 == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Return the number of items that have been marked as done
        /// </summary>
        /// <returns>Number of checked items</returns>
        public int CheckedCount()
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item2 == true)
                    count++;
            }
            return count;
        }
    }
}
