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
using QuickGraph;
using GraphSharp.Controls;

namespace GraphSharpWPF
{
   
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //int[,] adjacencymatrix;
        //BidirectionalGraph<object, IEdge<object>> graph;
        ////AdjacencyGraph g = new AdjacencyGraph(new VertexAndEdgeListGraph(), false);
        //private IBidirectionalGraph<object, IEdge<object>> _graphToVisalize;//字段
        ////属性，只读
        //public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        //{
        //    get { return _graphToVisalize; }
        //}
        //public MainWindow()
        //{
        //    InitializeComponent();
        //}
        //public MainWindow(int[,] adjacencymatrix)
        //    : this()
        //{

        //    graph = new BidirectionalGraph<object, IEdge<object>>();
        //    this.adjacencymatrix = adjacencymatrix;
        //    CreateGraphToVisualize();

        //}
        //private void CreateGraphToVisualize()
        //{
        //    //add vertex
        //    int vertexcount = adjacencymatrix.GetLength(0);
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        graph.AddVertex(i.ToString());
        //    }
        //    //add edge
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        for (int j = i + 1; j < vertexcount - 1; j++)
        //        {
        //            if (adjacencymatrix[i, j] != 0)
        //            {
        //                graph.AddEdge(new Edge<Object>(i.ToString(), j.ToString()));
        //            }
        //        }
        //    }
        //    _graphToVisalize = graph;
        //}
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisalize;//字段
        //属性，只读
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get { return _graphToVisalize; }
        }
        public MainWindow()
        {
            CreateGraphToVisualize();
            InitializeComponent();
        }
        private void CreateGraphToVisualize()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();
            string[] vertices = new string[5];
            for (int i = 0; i < 5; i++)
            {
                vertices[i] = i.ToString();
                g.AddVertex(vertices[i]);
            }
            g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[4]));
            g.AddEdge(new Edge<object>(vertices[3], vertices[4]));

            _graphToVisalize = g;
        }
    }
}
