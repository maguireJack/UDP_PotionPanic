//using GDLibrary.Actors;
//using Microsoft.Xna.Framework;
//using System.Collections;
//using System.Collections.Generic;

//namespace GDLibrary.Core.Events
//{
//    public enum EventType
//    {
//        Camera,
//        Player,
//        Recipe,
//        PotionPicked,
//        Add,
//        Remove,
//        WinLose,
//        Menu,
//        Sound
//    }
//    public class EventData
//    {
//        private EventType eventType;
//        private ArrayList data;

//        public EventType EventType { get => eventType; set => eventType = value; }
//        public ArrayList Data { get => data; set => data = value; }

//        public EventData(EventType eventType, ArrayList data)
//        {
//            this.eventType = eventType;
//            this.data = data;
//        }
//    }

//    public class EventDispatcher : GameComponent
//    {
//        private static Queue<EventData> queue;

//        public EventDispatcher(Game game) : base(game)
//        {
//            queue = new Queue<EventData>();
//        }

//        #region Events

//        public delegate void RecipeEventHandler(ArrayList data);
//        public event RecipeEventHandler RecipeEvent;

//        public delegate void PotionPickedEventHandler();
//        public event PotionPickedEventHandler PotionPickedEvent;

//        public delegate void AddEventHandler(DrawnActor3D actor);
//        public event AddEventHandler AddEvent;

//        public delegate void RemoveEventHandler(string id);
//        public event RemoveEventHandler RemoveEvent;

//        #endregion

//        public static void Publish(EventType eventType, ArrayList data)
//        {
//            queue.Enqueue(new EventData(eventType, data));
//        }

//        public override void Update(GameTime gameTime)
//        {
//            Process();
//            base.Update(gameTime);
//        }

//        private void Process()
//        {
//            for(int i = 0; i < queue.Count; i++)
//            {
//                EventData evt = queue.Dequeue();
//                switch(evt.EventType)
//                {
//                    case EventType.PotionPicked:
//                        PotionPickedEvent();
//                        break;
//                    case EventType.Recipe:
//                        RecipeEvent(evt.Data);
//                        break;
//                    case EventType.Add:
//                        AddEvent((DrawnActor3D)evt.Data[0]);
//                        break;
//                    case EventType.Remove:
//                        RemoveEvent((string)evt.Data[0]);
//                        break;
//                    default:
//                        break;
//                }
//            }
//        }
//    }
//}
