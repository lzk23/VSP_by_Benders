using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.ShortestPath
{
    public class Bellman_Ford:IAlgorithm
    {
        public Network network { get; set; }

        public Node source { get; set; }

        public Bellman_Ford()
        {
            network = null;
            source = null;
        }

        public Bellman_Ford(Network network, Node source)
        {
            this.network = network;
            this.source = source;
        }

        public void runAlgorithm()
        {
            if (network == null || source == null)
            {
                Console.WriteLine("Error: Invalid graph detected!");

                // Wait for the user to press a key.
                Console.ReadKey();
                Environment.Exit(-1);
            }

            for (int i = 0; i < network.nodes.Count; i++)
            {
                network.nodes[i].dist = int.MaxValue;
                network.nodes[i].lastNode = null;
            }

            // This is the node we start with.
            source.dist = 0;

            for (int i = 0; i < (network.nodes.Count - 1); i++)
            {
                for (int j = 0; j < network.edges.Count; j++)
                {
                    if ((network.edges[j].node1.dist + network.edges[j].weight) < network.edges[j].node2.dist)
                    {
                        network.edges[j].node2.dist = (network.edges[j].node1.dist + network.edges[j].weight);
                        network.edges[j].node2.lastNode = network.edges[j].node1;
                    }
                }
            }

            for (int i = 0; i < network.edges.Count; i++)
            {
                if ((network.edges[i].node1.dist + network.edges[i].weight) < network.edges[i].node2.dist)
                {
                    Console.WriteLine("Error: Negative-weight cycle detected!");

                    // Wait for the user to press a key.
                    Console.ReadKey();
                    Environment.Exit(-1);
                }
            }
        }
    }
}
