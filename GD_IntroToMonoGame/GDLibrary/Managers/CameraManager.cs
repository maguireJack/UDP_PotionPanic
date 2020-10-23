using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class CameraManager : GameComponent
    {
        private List<Camera3D> list;
        private int activeCameraIndex = 0;

        /// <summary>
        /// Indexer for the camera manager
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// See <see href="www."/> for more info
        public Camera3D this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }

        public Camera3D ActiveCamera
        {
            get
            {
                return this.list[this.activeCameraIndex];
            }
        }
        public int ActiveCameraIndex
        {
            get
            {
                return this.activeCameraIndex;
            }
            set
            {
                //in a 3 camera world [0,1,2,3,4,5,...] become [0,1,2,0,1,2,...]
                value = value % this.list.Count;
                this.activeCameraIndex = value; //bug!!! [0, list.size()-1]
            }
        }


        public CameraManager(Game game) : base(game)
        {
           this.list = new List<Camera3D>();
        }

        public void Add(Camera3D camera)
        {
            this.list.Add(camera);
        }

        public bool RemoveIf(Predicate<Camera3D> predicate)
        {
            int position = this.list.FindIndex(predicate);

            if (position != -1)
            {
                this.list.RemoveAt(position);
                return true;
            }

            return false;
        }

        public void CycleActiveCamera()
        {
            this.activeCameraIndex++;
            this.activeCameraIndex %= this.list.Count;
        }


        public override void Update(GameTime gameTime)
        {
            foreach(Camera3D camera in this.list)
            {
                if ((camera.StatusType & StatusType.Update) == StatusType.Update)
                    camera.Update(gameTime);
            }

            base.Update(gameTime);
        }
    }
}
