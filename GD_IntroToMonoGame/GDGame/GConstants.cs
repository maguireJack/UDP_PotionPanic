using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame
{
    public class GConstants
    {
        #region Player
        private static readonly float StrafeSpeedMultiplier = 0.75f;
        public static readonly float PlayerMoveSpeed = 0.1f;
        public static readonly float PlayerStrafeSpeed = StrafeSpeedMultiplier * PlayerMoveSpeed;
        public static readonly float PlayerRotateSpeed = 0.01f;

        public static readonly float PlayerCamOffsetY = 200;
        public static readonly float PlayerCamOffsetZ = 200;

        public static readonly Vector3 PlayerHoldPos = new Vector3(32, 40, 3);
        public static readonly Vector3 PotionRedPos = new Vector3(4, 18, -1.5f);

        //keys
        public static readonly Keys[] PlayerMoveKeysOne = { Keys.W, Keys.S, Keys.A, Keys.D };
        #endregion
    }
}
