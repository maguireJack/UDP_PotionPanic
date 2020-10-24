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

        public static readonly float PlayerCamOffsetY = 500;
        public static readonly float PlayerCamOffsetZ = 500;

        //keys
        public static readonly Keys[] PlayerMoveKeysOne = { Keys.W, Keys.S, Keys.A, Keys.D };
        #endregion
    }
}
