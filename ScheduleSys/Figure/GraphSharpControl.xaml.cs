using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphSharp.Sample;
using QuickGraph;
using GraphSharp;
using GraphSharp.Controls;
using QuickGraph.Serialization;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace ScheduleSys.Figure
{
    /// <summary>
    /// GraphSharpControl.xaml 的交互逻辑
    /// </summary>
    public partial class GraphSharpControl : UserControl
    {
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisalize;//字段
        UndirectedBidirectionalGraph<string, Edge<string>> undirectedGraph;
        BidirectionalGraph<string, Edge<string>> directedGraph;
        public BidirectionalGraph<object, IEdge<object>> graph;
        //属性，只读
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get { return _graphToVisalize; }
        }
        public GraphSharpControl(Graph g)
            : this()
        {
            //directedGraph = new BidirectionalGraph<string, Edge<string>>();
            //foreach (ModelNode node in g.nodelist)
            //{
            //    directedGraph.AddVertex(node.label.ToString());
            //}
            //foreach (ModelEdge edge in g.edgelist)
            //{
            //    directedGraph.AddEdge(new Edge<string>(edge.start_node_label.ToString(),
            //        edge.end_node_label.ToString()));
            //}
            //undirectedGraph = new UndirectedBidirectionalGraph<string, Edge<string>>(directedGraph);
            //graph = new BidirectionalGraph<object, IEdge<object>>();
            //foreach (ModelNode node in g.nodelist)
            //{
            //    graph.AddVertex(node.label.ToString());
            //}
            //foreach (ModelEdge edge in g.edgelist)
            //{
            //    graph.AddEdge(new Edge<object>(edge.start_node_label.ToString(),
            //        edge.end_node_label.ToString()));
            //}
            //layout.LayoutMode = LayoutMode.Automatic;
            //layout.LayoutAlgorithmType = "CompoundFDP";
            //layout.OverlapRemovalConstraint = AlgorithmConstraints.Automatic;
            //layout.OverlapRemovalAlgorithmType = "FSA";
            //layout.HighlightAlgorithmType = "Simple";
            //layout.Graph = graph;

        }
        public GraphSharpControl()
        {
            InitializeComponent();
            //var g = new CompoundGraph<object, IEdge<object>>();
            //var vertices = new string[30];
            //for (int i = 0; i < 30; i++)
            //{
            //    vertices[i] = i.ToString();
            //    g.AddVertex(vertices[i]);
            //}

            //for (int i = 6; i < 15; i++)
            //{
            //    g.AddChildVertex(vertices[i % 5], vertices[i]);
            //}
            //layout.LayoutMode = LayoutMode.Automatic;
            //layout.LayoutAlgorithmType = "CompoundFDP";
            //layout.OverlapRemovalConstraint = AlgorithmConstraints.Automatic;
            //layout.OverlapRemovalAlgorithmType = "FSA";
            //layout.HighlightAlgorithmType = "Simple";
            //layout.Graph = g;
        }

        public GraphSharpControl(string filename)
            : this()
        {
            graph = new BidirectionalGraph<object, IEdge<object>>();
            StreamReader reader = DataReadWrite.GetStreamReader(filename);
            string line = "";
            bool first_line = true;
            while ((line = reader.ReadLine()) != null)
            {
                string[] str_list = line.Split(' ');
                if (first_line)
                {
                    if (str_list[1] == "branch")
                    {

                        graph.AddVertex(str_list[0]);
                        graph.AddVertex(str_list[2]);


                        graph.AddEdge(new Edge<object>(str_list[0], str_list[2]));

                        if (str_list.Count() == 4)
                        {
                            graph.AddVertex(str_list[3]);
                            graph.AddEdge(new Edge<object>(str_list[0], str_list[3]));
                        }

                    }
                    else
                    {
                        throw new Exception("Data Error.");
                    }
                    first_line = false;
                    continue;
                }

                if (str_list[1] == "branch")
                {
                    if (!graph.ContainsVertex(str_list[0]))
                    {
                        throw new Exception("Data Error.");
                    }
                    graph.AddVertex(str_list[2]);

                    graph.AddEdge(new Edge<object>(str_list[0], str_list[2]));

                    if (str_list.Count() == 4)
                    {
                        graph.AddVertex(str_list[3]);
                        graph.AddEdge(new Edge<object>(str_list[0], str_list[3]));
                    }
                }
            }
            layout.Graph = graph;
        }

        //public GraphSharpControl(string filename)
        //    : this()
        //{
        //    var directed_g = new CompoundGraph<object, IEdge<object>>();
        //    StreamReader reader = Data.GetStreamReader(filename);
        //    string line="";
        //    bool first_line = true;
        //    while((line=reader.ReadLine())!=null)
        //    {
        //        string[] str_list = line.Split(' ');
        //        if(first_line)
        //        {
        //            if(str_list[1]=="branch")
        //            {
        //                directed_g.AddVertex(str_list[0]);
        //                directed_g.AddVertex(str_list[2]);
        //                directed_g.AddVertex(str_list[3]);

        //                directed_g.AddEdge(new Edge<object>(str_list[0], str_list[2]));
        //                directed_g.AddEdge(new Edge<object>(str_list[0], str_list[3]));
        //            }
        //            else
        //            {
        //                throw new Exception("Data Error.");
        //            }
        //            first_line = false;
        //            continue;
        //        }

        //        if(str_list[1]=="branch")
        //        {
        //            if (!directed_g.ContainsVertex(str_list[0]))
        //            {
        //                throw new Exception("Data Error.");
        //            }
        //            directed_g.AddVertex(str_list[2]);
        //            directed_g.AddVertex(str_list[3]);

        //            directed_g.AddEdge(new Edge<object>(str_list[0], str_list[2]));
        //            directed_g.AddEdge(new Edge<object>(str_list[0], str_list[3]));
        //        }
        //    }
        //    layout.Graph = directed_g;
        //}

        public void OpenFile(string fileName)
        {
            //graph where the vertices and edges should be put in
            //var graph = new CompoundGraph<object, IEdge<object>>();

            //try
            //{
            //    //open the file of the graph
            //    var reader = XmlReader.Create(fileName);

            //    //create the serializer
            //    var serializer = new GraphMLDeserializer<object, IEdge<object>, CompoundGraph<object, IEdge<object>>>();


            //    //deserialize the graph
            //    serializer.Deserialize(reader, graph,
            //                           id => id, (source, target, id) => new Edge<object>(source, target)
            //        );

            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex);
            //}
            //layout.Graph = graph;
            //layout.UpdateLayout();

        }
        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            layout.Relayout();
        }
     
    }
}
