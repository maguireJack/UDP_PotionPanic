using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;

namespace GDGame.MyGame.Controllers
{
    public class ScoreController : Controller
    {
        private UITextObject score;

        public ScoreController(string id, ControllerType controllerType, UITextObject score) : base(id, controllerType)
        {
            this.score = score;
            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvents);
        }

        private void HandleEvents(EventData eventData)
        {
            if(eventData.EventActionType == EventActionType.OnScoreChange)
            {
                score.Text = "Score: " + ((int)eventData.Parameters[0]);
            }
        }
    }
}
