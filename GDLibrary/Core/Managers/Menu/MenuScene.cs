using GDLibrary.Actors;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class MenuScene
    {
        private string id; //"main"
        private List<DrawnActor2D> list;

        public DrawnActor2D this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public MenuScene(string id)
        {
            this.id = id;
            this.list = new List<DrawnActor2D>();
        }

        public void Add(DrawnActor2D actor)
        {
            if (!this.list.Contains(actor))
                this.list.Add(actor);
        }
    }
}
