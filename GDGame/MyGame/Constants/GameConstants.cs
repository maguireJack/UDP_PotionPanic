using GDGame.MyGame.Enums;
using GDGame.MyGame.Objects;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GDGame.MyGame.Constants
{
    public class GameConstants
    {
        #region Common

        public static readonly int screenWidth = 1440;
        public static readonly int screenHeight = 1080;
        public static readonly Vector2 screenCentre = new Vector2(screenWidth/2, screenHeight/2);

        public static readonly Keys[][] MoveKeys = {
            new Keys[]{ Keys.W, Keys.S, Keys.A, Keys.D },
            new Keys[]{ Keys.Up, Keys.Down, Keys.Left, Keys.Right }
        };

        #endregion Common

        #region First Person Camera

        public static readonly float moveSpeed = 0.1f;
        public static readonly float strafeSpeed = 0.075f;
        public static readonly float rotateSpeed = 0.01f;

        #endregion First Person Camera

        #region Flight Camera

        public static readonly float flightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = 0.6f;
        public static readonly float flightRotateSpeed = 0.01f;

        #endregion Flight Camera

        #region Security Camera

        private static readonly float angularSpeedMultiplier = 10;
        public static readonly float lowAngularSpeed = 10;
        public static readonly float mediumAngularSpeed = lowAngularSpeed * angularSpeedMultiplier;
        public static readonly float hiAngularSpeed = mediumAngularSpeed * angularSpeedMultiplier;

        #endregion Security Camera

        #region Car

        public static readonly float carMoveSpeed = 0.08f;
        public static readonly float carRotateSpeed = 0.06f;

        #endregion Car

        #region Player

        public static readonly float playerMoveSpeed = 4f;
        public static readonly float playerRotateSpeed = 4f;

        public static readonly float playerCamOffsetY = 500;
        public static readonly float playerCamOffsetZ = 500;

        public static readonly Vector3 playerHoldPos = new Vector3(32, 40, 3);
        public static readonly Vector3 helperOffsetPos = new Vector3(0, 140, 0);
        public static readonly Vector3 potionRedPos = new Vector3(4, 18, -1.5f);

        public static readonly Keys[] playerInteractKeys = { Keys.Space, Keys.RightControl };
        public static readonly Buttons[] playerInteractButtons = { Buttons.LeftTrigger };
        public static readonly float defualtInteractionDist = 80f;

        #endregion

        #region Objects

        public static readonly Vector3 cauldronPos = new Vector3(-50, 40, 90);
        public static readonly Vector3 binPos = new Vector3(-370, 53, -100);

        #endregion

        #region Upgrades

        //Name, Upgrade Action, { TierNum, Cost, Value } - value = value to pass via event
        public static readonly Upgrade upgradeSpeed = new Upgrade("Move Speed", EventActionType.MoveSpeedUp,
        new Dictionary<int, Tuple<int, float>>(){
            { 1, new Tuple<int, float>(100, 10) },
            { 2, new Tuple<int, float>(150, 25) },
            { 3, new Tuple<int, float>(250, 50) }
        });

        public static readonly Upgrade upgradePotionValue = new Upgrade("Potion Value", EventActionType.ValueUp,
        new Dictionary<int, Tuple<int, float>>(){
            { 1, new Tuple<int, float>(100, 20) },
            { 2, new Tuple<int, float>(200, 50) },
            { 3, new Tuple<int, float>(400, 100) }
        });

        public static readonly Upgrade[] upgrades = { upgradeSpeed, upgradePotionValue };

        #endregion

        #region Ingredients

        public static readonly Dictionary<string, Objects.Ingredient> ingredients = new Dictionary<string, Objects.Ingredient>()
        {
            { "Red_Solid", new Objects.Ingredient(IngredientType.Red, IngredientState.Solid) },
            { "Red_Dust", new Objects.Ingredient(IngredientType.Red, IngredientState.Dust) },
            { "Red_Liquid", new Objects.Ingredient(IngredientType.Red, IngredientState.Liquid) },
            { "Blue_Solid", new Objects.Ingredient(IngredientType.Blue, IngredientState.Solid) },
            { "Blue_Dust", new Objects.Ingredient(IngredientType.Blue, IngredientState.Dust) },
            { "Blue_Liquid", new Objects.Ingredient(IngredientType.Blue, IngredientState.Liquid) },
            { "Green_Solid", new Objects.Ingredient(IngredientType.Green, IngredientState.Solid) },
            { "Green_Dust", new Objects.Ingredient(IngredientType.Green, IngredientState.Dust) },
            { "Green_Liquid", new Objects.Ingredient(IngredientType.Green, IngredientState.Liquid) }
        };

        #endregion

        #region Load Data

        //[0] = Hold pos, [1] = scale
        public static readonly Dictionary<string, ArrayList> pickupModelData = new Dictionary<string, ArrayList>();
        public static readonly Dictionary<Recipe, ArrayList> potions = new Dictionary<Recipe, ArrayList>();
        public static readonly float potionSpawnHeight = 100;

        private class ModelData
        {
            public string ModelName { get; set; }
            public List<float> HoldPosition { get; set; }
            public List<float> Scale { get; set; }
        }

        private class Ingredient
        {
            public string Type { get; set; }
            public string State { get; set; }
            public int Count { get; set; }
        }

        private class PotionRecipe
        {
            [JsonProperty("Name:")]
            public string Name { get; set; }
            public string ModelName { get; set; }
            public List<Ingredient> Ingredients { get; set; }
        }

        private class Root
        {
            public List<ModelData> ModelData { get; set; }
            public List<PotionRecipe> PotionRecipes { get; set; }
        }


        public static void LoadData()
        {
            try
            {
                Root root;
                using (StreamReader r = new StreamReader("Content/Assets/Data/Data.json"))
                {
                    string json = r.ReadToEnd();
                    root = JsonConvert.DeserializeObject<Root>(json);
                }

                foreach (ModelData data in root.ModelData)
                {
                    Vector3 holdPos = new Vector3(data.HoldPosition[0], data.HoldPosition[1], data.HoldPosition[2]);
                    Vector3 scale = new Vector3(data.Scale[0], data.Scale[1], data.Scale[2]);
                    pickupModelData.Add(data.ModelName, new ArrayList { holdPos, scale });
                }

                foreach (PotionRecipe pr in root.PotionRecipes)
                {
                    Recipe recipe = new Recipe();
                    foreach (Ingredient ingredient in pr.Ingredients)
                    {
                        recipe.Add(ingredients[ingredient.Type + "_" + ingredient.State], ingredient.Count);
                    }
                    potions.Add(recipe, new ArrayList { pr.Name, pr.ModelName });
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        #endregion
    }
}