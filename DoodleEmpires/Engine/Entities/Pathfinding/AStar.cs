using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Entities.Pathfinding
{
    public interface IHasNeighbours<N>
    {
        IEnumerable<N> Neighbours { get; }
    }

    public class AStar
    {
        public static Path<Node> FindPath<Node>(Node start, Node destination, 
            Func<Node, Node, double> distance, Func<Node, double> estimate) where Node : IHasNeighbours<Node>
        {
            HashSet<Node> closed = new HashSet<Node>();
            PriorityQueue<double, Path<Node>> queue = new PriorityQueue<double, Path<Node>>();

            queue.Enqueue(0, new Path<Node>(start));
            while (!queue.IsEmpty)
            {
                Path<Node> path = queue.Dequeue();

                if (closed.Contains(path.LastStep))
                    continue;

                if (path.LastStep.Equals(destination))
                    return path;

                closed.Add(path.LastStep);

                foreach (Node n in path.LastStep.Neighbours)
                {
                    double d = distance(path.LastStep, n);
                    Path<Node> newPath = path.AddStep(n, d);
                    queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
                }
            }
            return null;
        }
    }
}
