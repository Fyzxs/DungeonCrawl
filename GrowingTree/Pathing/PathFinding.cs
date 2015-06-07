using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Features;

namespace GrowingTree.Pathing
{
    class Node
    {
        private KeyValuePair<Feature, Feature> kvp;

        public Node(Feature child, Feature parent)
        {
            kvp = new KeyValuePair<Feature, Feature>(child, parent);
        }

        public Feature Child
        {
            get { return kvp.Key; }
        }

        public Feature Parent
        {
            get { return kvp.Value; }
        }

    }
    class Nodes : IEnumerable<Node>
    {
        private IList<Node> features = new List<Node>();
        
        public void Add(Feature child, Feature parent)
        {
            Add(new Node(child, parent));

        }

        public void Add(Node node)
        {
            features.Add(node);
        }

        public Node RemoveAt(int index)
        {
            var node = features[index];
            features.RemoveAt(index);
            return node;
        }

        public Node GetByChild(Feature child)
        {
            return features.FirstOrDefault(x => x.Child == child);
        }

        public int Count
        {
            get { return features.Count; }
        }

        public Node this[int index]
        {
            get { return features[index]; }
        }

        public void Sort(Feature pos2)
        {
            features = features.OrderBy(
                x =>
                    Math.Sqrt((x.Child.Left - pos2.Left)*(x.Child.Left - pos2.Left) +
                              (x.Child.Top - pos2.Top)*(x.Child.Top - pos2.Top))).ToList();
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return features.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    class PathFinding
    {
        private static void Draw(Node node, int colors)
        {
            return;
            //MMMMM.... Slow Drawing
            var left = node.Child.Parent != null ? node.Child.Parent.Left + node.Child.Left : node.Child.Left;
            var top = node.Child.Parent != null ? node.Child.Parent.Top + node.Child.Top : node.Child.Top;
            Console.SetCursorPosition(left, top);
            switch (colors)
            {
                case 0:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case 1:
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.Write("@");
        }
        public static List<Feature> CustomPathFinding(Feature[,] map, Feature start, Feature goal)
        {
            if (start == goal) { return new List<Feature>();}

            var bc = Console.BackgroundColor;
            var fc = Console.ForegroundColor;

            var closedSet = new Nodes();
            var openSet = new Nodes {{start, NullFeature.Instance}};

            var reachedGoal = false;

            while (openSet.Any() && !reachedGoal)
            {
                if (Program.SystemState.DebugFlags.SearchHeuristics)
                {
                    CustomPathFindingSortingHeuristic(openSet, goal);
                }
                for (var index = openSet.Count - 1; index >= 0 && !reachedGoal; index--)
                {
                    var curNode = openSet.RemoveAt(index);
                    closedSet.Add(curNode);

                    Draw(curNode, 0);

                    if (curNode.Child == goal)//This should be an OK object reference check since all
                    //nodes should be getting pulled from the same map
                    {
                        reachedGoal = true;
                        continue;
                    }

                    var validSteps = GetValidSteps(map, curNode.Child, openSet, closedSet);
                    if (!validSteps.Any()) continue;

                    foreach (var validStep in validSteps)
                    {
                        Draw(validStep, 1);
                        openSet.Add(validStep);
                    }

                }
            }

            return reachedGoal ? GetPath(closedSet, goal) : null;
        }

        private static List<Feature> GetPath(Nodes nodes, Feature goal)
        {
            var path = new List<Feature>();
            var node = nodes.GetByChild(goal);
            if (node == null) return path;
            do
            {
                Draw(node, 2);
                path.Add(node.Child);
            } while (!NullFeature.IsNullFeature((node = nodes.GetByChild(node.Parent)).Parent));

            Draw(node, 2);
            path.Add(node.Child);//Start Node

            return path;

        }

        private static void CustomPathFindingSortingHeuristic(Nodes set, Feature goal)
        {
            set.Sort(goal);
        }

        private static Nodes GetValidSteps(Feature[,] map, Feature feature, Nodes openSet, Nodes closedSet)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var features = new Nodes();
            var lMod = feature.Parent != null ? feature.Parent.Left : 0;
            var tMod = feature.Parent != null ? feature.Parent.Top : 0;

            var xMod = -1;
            var yMod = 0;
            AddValidStep(features, map, feature, xMod+lMod, yMod+tMod, width, height, openSet, closedSet);

            xMod = +1;
            yMod = 0;
            AddValidStep(features, map, feature, xMod + lMod, yMod + tMod, width, height, openSet, closedSet);
            
            xMod = 0;
            yMod = -1;
            AddValidStep(features, map, feature, xMod + lMod, yMod + tMod, width, height, openSet, closedSet);

            xMod = 0;
            yMod = +1;
            AddValidStep(features, map, feature, xMod + lMod, yMod + tMod, width, height, openSet, closedSet);

            return features;
        }

        private static void AddValidStep(Nodes features, Feature[,] map, Feature parent, int xMod, int yMod, int width, int height, Nodes openSet, Nodes closedSet)
        {
            Feature cell = null;
            if (!OutOfBounds(parent.Left, parent.Top, xMod, yMod, width, height) && 
                !NullFeature.IsNullFeature(cell = map[parent.Left + xMod, parent.Top + yMod]) &&
                openSet.All(x => x.Child != cell) && closedSet.All(x => x.Child != cell))
            {
                features.Add(cell, parent);
            }
        }

        private static bool OutOfBounds(int left, int top, int xMod, int yMod, int width, int height)
        {
            return
                left + xMod < 0 || left + xMod >= width ||
                top + yMod < 0  || top + yMod >= height;
        }
    }
}
