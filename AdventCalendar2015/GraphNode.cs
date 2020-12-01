using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace AdventCalendar2015
{
    public class GraphNode
    {
        public GraphNode(string id)
        {
            this.id = id;
        }

        protected bool Equals(GraphNode other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((GraphNode) obj);
        }

        public override int GetHashCode()
        {
            return (id != null ? id.GetHashCode() : 0);
        }

        public bool AddEdge(GraphNode graphNode, int i)
        {
            distance.Add(graphNode, i);
            return true;
        }

        public int FindShortest(HashSet<GraphNode> visitedNodes, int dest)
        {

            var minDist = int.MaxValue;
            if (!visitedNodes.Contains(this))
            {
                // already visited
                return minDist;
            }
            if (visitedNodes.Count == 1)
            {
                return dest;
            }
            visitedNodes.Remove(this);
            
            minDist = distance.Select(pair => pair.Key.FindShortest(visitedNodes, dest + pair.Value)).Concat(new[] {minDist}).Min();

            visitedNodes.Add(this);
            return minDist;
        }        
        
        public int FindLongest(HashSet<GraphNode> visitedNodes, int dest)
        {

            var maxDist = 0;
            if (!visitedNodes.Contains(this))
            {
                // already visited
                return maxDist;
            }
            if (visitedNodes.Count == 1)
            {
                return dest;
            }
            visitedNodes.Remove(this);
            
            maxDist = distance.Select(pair => pair.Key.FindLongest(visitedNodes, dest + pair.Value)).Max();

            visitedNodes.Add(this);
            return maxDist;
        }
        
        private readonly string id;
        private readonly Dictionary<GraphNode, int> distance = new Dictionary<GraphNode, int>();
    }

    public class Graph
    {

        public GraphNode GetNode(string nodeId)
        {
            if (graphNodes.ContainsKey(nodeId))
            {
                return graphNodes[nodeId];
            }
            var graphNode = new GraphNode(nodeId);
            graphNodes[nodeId] = graphNode;
            return graphNode;
        }
        

        public (GraphNode from, GraphNode to) AddEdge(string idFrom, string idTo, int distance, bool biDirectional)
        {
            GetNode(idFrom).AddEdge(GetNode(idTo), distance);
            if (biDirectional)
            {
                GetNode(idTo).AddEdge(GetNode(idFrom), distance);
            }
            return (graphNodes[idFrom], graphNodes[idTo]);
        }
        
        public int TravelingSalesman()
        {
            var nodes = new HashSet<GraphNode>(graphNodes.Values);

            return graphNodes.Values.Select(value =>
            {
                var res = value.FindShortest(nodes, 0);
                return res;
            }).Concat(new[] {int.MaxValue}).Min();
        }        
        public int LongestPath()
        {
            var nodes = new HashSet<GraphNode>(graphNodes.Values);

            return graphNodes.Values.Select(value =>
            {
                var res = value.FindLongest(nodes, 0);
                return res;
            }).Max();
        }
        
        private readonly Dictionary<string, GraphNode> graphNodes = new Dictionary<string, GraphNode>();
    }
}