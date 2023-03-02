using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.ShortestPath
{
    public class Network
    {
        public List<Edge> edges { get; set; }
        public List<Node> nodes { get; set; }

        public Dictionary<string, Node> node_name_to_node;//节点名称索引节点



        public Network()
        {
            edges = null;
            nodes = null;
        }

        public Network(List<Edge> edges, List<Node> nodes)
        {
            this.edges = edges;
            this.nodes = nodes;
        }
    }
    public class Node
    {
        public List<Edge> edges { get; set; }
        public Node lastNode { get; set; }
        public string name { get; set; }
        public int dist { get; set; }

        public Node()
        {
            edges = null;
            lastNode = null;
            name = null;
            dist = int.MaxValue;
        }

        public Node(string name)
        {
            this.name = name;
            edges = null;
            lastNode = null;
            dist = int.MaxValue;
        }

        public Node(string name, List<Edge> edges)
        {
            this.name = name;
            this.edges = edges;
            lastNode = null;
            dist = int.MaxValue;
        }
    }

    public class Edge
    {
        public Node node1 { get; set; }
        public Node node2 { get; set; }
        public int weight { get; set; }

        public Edge()
        {
            node1 = null;
            node2 = null;
            weight = int.MinValue;
        }

        public Edge(int weight)
        {
            this.weight = weight;
            node1 = null;
            node2 = null;
        }
        public Edge(Node node1, Node node2, int weight)
        {
            this.weight = weight;
            this.node1 = node1;
            this.node2 = node2;
        }
        
    }
}
