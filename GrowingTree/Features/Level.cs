using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Level : Feature
    {
        public static Level Instance;
        
        public Level(int width, int height) : base(new Boundary(new Point {X=0, Y=0}, width, height))
        {
            Instance = this;
        }

        internal IEnumerable<Room> GetRooms()
        {
            return GetFeaturesOfType<Room>();
        }

        internal IEnumerable<Hallway> GetHallways()
        {
            return GetFeaturesOfType<Hallway>();
        }

        internal IEnumerable<Door> GetDoors()
        {
            return GetFeaturesOfType<Door>();
        }

        internal Player GetPlayer()
        {
            return GetFeaturesOfType<Player>().First();
        } 

    }
}
