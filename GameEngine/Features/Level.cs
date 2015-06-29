using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameEngine.Character;
using GameEngine.Display;
using GameEngine.Features.Null;

namespace GameEngine.Features
{
    public class Level : Feature
    {
        private Feature[,] featureGrid = null;
        public static Level Instance;
        
        public Level(int width, int height) : base(new Boundary(new Point {X=0, Y=0}, width, height))
        {
            Instance = this;
        }

        public void RefreshFeatureGrid()
        {
            featureGrid = null;
        }
        public Feature[,] FeatureGrid
        {
            get
            {
                if (featureGrid != null) return featureGrid;

                var start = DateTime.Now.Ticks;
                featureGrid = new Feature[Width, Height];
                FillFeatureGrid(featureGrid, 0, 0);
                for (var i = 0; i < featureGrid.GetLength(0); i++)
                {
                    for (var j = 0; j < featureGrid.GetLength(1); j++)
                    {
                        if (featureGrid[i, j] == null)
                        {
                            featureGrid[i, j] = NullFeature.Instance;
                        }
                    }
                }

                var end = DateTime.Now.Ticks;
                Debug.WriteLine("Took [{0}]ticks", (end - start));
                return featureGrid;
            }
        }

        public IEnumerable<Room> GetRooms()
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

        public Player GetPlayer()
        {
            var players = GetFeaturesOfType<Player>();
            return players.Count > 0 ? players.First() : NullPlayer.Instance;
        }

        public IEnumerable<Monster> GetMonsters()
        {
            return GetFeaturesOfType<Monster>();
        }


    }
}
