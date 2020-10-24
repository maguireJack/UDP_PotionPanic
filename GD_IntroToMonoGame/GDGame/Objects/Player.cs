using GDLibrary;

namespace GDGame
{
    public class Player : MoveableObject
    {
        #region Fields
        private HandHeldPickup handItem;
        //inventory
        #endregion

        #region Properties
        public HandHeldPickup HandItem
        {
            get { return handItem; }
        }
        #endregion

        #region Constructors
        public Player(ModelObject modelObject, IController controller) :
            base(modelObject, controller)
        {
            this.handItem = null;
        }
        #endregion

    }
}
