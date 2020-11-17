using GDGame.MyGame.Constants;
using System;
using System.Diagnostics;
using System.IO;

namespace GDGame.MyGame.Objects
{
    public class PersistantData
    {
        #region Fields

        private int gold;
        private int highscore;

        #endregion

        #region Properties

        public int Gold 
        { 
            get { return gold; } 
        }

        public int Highscore
        {
            get { return highscore; }
        }

        #endregion

        #region Constructor & Core

        public PersistantData()
        {
            gold = 0;
            highscore = 0;

            LoadData();
        }

        public bool Purchase(int cost)
        {
            if(gold >= cost)
            {
                gold -= cost;
                return true;
            }
            return false;
        }

        public void LoadData()
        {
            try
            {
                string[] lines = File.ReadAllLines("persistantData.txt");
                gold = Convert.ToInt32(lines[0]);
                highscore = Convert.ToInt32(lines[1]);

                string[] upgradeLines = lines[2].Split('.');
                for (int i = 0; i < upgradeLines.Length; i++)
                {
                    int tier = Convert.ToInt32(lines[i]);
                    if(tier != 0)
                        GameConstants.upgrades[i].AquireUpgrade(tier);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void SaveData()
        {
            string[] upgradeLines = new string[GameConstants.upgrades.Length];
            for(int i = 0; i < GameConstants.upgrades.Length; i++)
            {
                upgradeLines[i] = GameConstants.upgrades[i].CurrentTier.ToString();
            }
            string[] lines = { gold.ToString(), highscore.ToString(), string.Join(".", upgradeLines)};

            try
            {
                File.WriteAllLines("persistantData.txt", lines);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion
    }
}
