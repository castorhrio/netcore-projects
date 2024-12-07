using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra
{
    public class Node
    {
        public Point Position { get; set; }
        public double Distance { get; set; }
        public Node? Parent { get; set; }
        public NodeState State { get; set; }

        public Node(Point position)
        {
            Position = position;
            Distance = double.MaxValue;
            State = NodeState.Unvisited;
        }
    }

    public enum NodeState
    {
        Unvisited,
        Visited,
        Path,
        Wall,
        Start,
        End
    }
}
