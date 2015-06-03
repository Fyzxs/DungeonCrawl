using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Features;

namespace GrowingTree.Pathing
{
    class PathFinding
    {
        /*
         * Build list of node->ParentNode from start and goal
         * When goal-> and start-> meet; trace parents back
         */

        public void CustomPathFinding(Feature[,] map, Feature start, Feature goal)
        {
            var startClosedFeatures = new Dictionary<Feature, Feature>();
            var startOpenFeatures = new List<Feature>();
            //var goalClosedFeatures = new Dictionary<Feature, Feature>();
            //var goalOpenFeatures = new Dictionary<Feature, Feature>();

            startOpenFeatures.Add(start);
            //goalOpenFeatures.Add(NullFeature.Instance, goal);

            while (startOpenFeatures.Any())//Something start == something goal
            {
                for (var index = startOpenFeatures.Count - 1; index >= 0; index--)
                {
                    startClosedFeatures.Add(startOpenFeatures[index]);
                }
            }
        }

        public static IEnumerable<Feature> FindPath(Feature start, Feature goal)
        {
            var closedSet = new HashSet<Feature>();
            var openSet = new HashSet<Feature>();
            var cameFrom = new HashSet<Feature>();

            IDictionary<Feature, int> gScore = new Dictionary<Feature, int>();
            IDictionary<Feature, int> fScore = new Dictionary<Feature, int>();

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = gScore[start] + HeuristicCostEstimate(start, goal);

            while (openSet.Any())
            {
                var current = GetLowestScore(fScore);
                if (current == goal)
                {
                    return ReconstructPath(cameFrom, goal);
                }
            }

            return null;
        }

        private static Feature GetLowestScore(IDictionary<Feature, int> fScore)
        {
            var lowValue = 0;
            Feature lowFeature = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var key in fScore.Keys)
            {
                if (fScore[key] >= lowValue) continue;

                lowValue = fScore[key];
                lowFeature = key;
            }
            return lowFeature;
        }

        private static int HeuristicCostEstimate(Feature start, Feature goal)
        {
            return Math.Abs(start.Left - goal.Left) + Math.Abs(start.Top - goal.Top);
        }

        private static HashSet<Feature> ReconstructPath(HashSet<Feature> cameFrom, Feature goal)
        {
            return null;
        }
        /*
        function A*(start,goal)
            closedset := the empty set    // The set of nodes already evaluated.
            openset := {start}    // The set of tentative nodes to be evaluated, initially containing the start node
            came_from := the empty map    // The map of navigated nodes.
 
            g_score[start] := 0    // Cost from start along best known path.
            // Estimated total cost from start to goal through y.
            f_score[start] := g_score[start] + heuristic_cost_estimate(start, goal)
 
            while openset is not empty
                current := the node in openset having the lowest f_score[] value
                if current = goal
                    return reconstruct_path(came_from, goal)
 
                remove current from openset
                add current to closedset
                for each neighbor in neighbor_nodes(current)
                    if neighbor in closedset
                        continue
                    tentative_g_score := g_score[current] + dist_between(current,neighbor)
 
                    if neighbor not in openset or tentative_g_score < g_score[neighbor] 
                        came_from[neighbor] := current
                        g_score[neighbor] := tentative_g_score
                        f_score[neighbor] := g_score[neighbor] + heuristic_cost_estimate(neighbor, goal)
                        if neighbor not in openset
                            add neighbor to openset
 
            return failure
 
        function reconstruct_path(came_from,current)
            total_path := [current]
            while current in came_from:
                current := came_from[current]
                total_path.append(current)
            return total_path
        */
    }
}
