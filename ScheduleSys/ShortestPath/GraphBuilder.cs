using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.ShortestPath
{
    class GraphBuilder
    {
        Graph g;
        double[] dual_train;
        double[,] dual_turnaround;
        public Network graph { get; set; }

        public GraphBuilder(Graph g,double[] dual_train,double[,] dual_turnaround)
        {
            this.g = g;
            graph = null;
            this.dual_train = dual_train;
            this.dual_turnaround = dual_turnaround;
        }

        //public GraphBuilder(String type)
        //{
        //    switch (type)
        //    {
        //        case "small":
        //            graph = initSmall();
        //            break;
        //        case "large":
        //            graph = null;
        //            break;
        //        case "negative":
        //            graph = null;
        //            break;
        //        default:
        //            graph = null;
        //            break;
        //    }
        //}

        private Network initSmall()
        {
            Node source_0 = new Node("source_0");
            for (int train_index = 0; train_index < g.trainlist.Count(); train_index++)
            {
                Train train = g.trainlist[train_index];
                if(train.direction==0)
                {
                    Node train_node = new Node("train_index");


                }
            }
            Node nodeA = new Node("A");
            Node nodeB = new Node("B");
            Node nodeC = new Node("C");
            Node nodeD = new Node("D");
            Node nodeE = new Node("E");
            Node nodeF = new Node("F");

            Edge edge0 = new Edge(nodeA, nodeB, 3);
            Edge edge1 = new Edge(nodeA, nodeC, 2);
            Edge edge2 = new Edge(nodeA, nodeD, 6);
            Edge edge3 = new Edge(nodeB, nodeE, 1);
            Edge edge4 = new Edge(nodeB, nodeC, 3);
            Edge edge5 = new Edge(nodeC, nodeE, 5);
            Edge edge6 = new Edge(nodeC, nodeF, 8);
            Edge edge7 = new Edge(nodeC, nodeD, 4);
            Edge edge8 = new Edge(nodeD, nodeF, 5);
            Edge edge9 = new Edge(nodeE, nodeF, 2);

            nodeA.edges = new List<Edge>(new Edge[] { edge0, edge1, edge2 });
            nodeB.edges = new List<Edge>(new Edge[] { edge0, edge3, edge4 });
            nodeC.edges = new List<Edge>(new Edge[] { edge1, edge4, edge5, edge6, edge7 });
            nodeD.edges = new List<Edge>(new Edge[] { edge2, edge7, edge8 });
            nodeE.edges = new List<Edge>(new Edge[] { edge3, edge5, edge9 });
            nodeF.edges = new List<Edge>(new Edge[] { edge6, edge8, edge9 });

            List<Edge> edges = new List<Edge>(new Edge[] { edge0, edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8, edge9 });
            List<Node> nodes = new List<Node>(new Node[] { nodeA, nodeB, nodeC, nodeD, nodeE, nodeF });

            return new Network(edges, nodes);
        }
    }
}
