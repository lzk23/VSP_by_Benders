using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickGraph;

namespace ScheduleSys.ListColoring
{
    public partial class AdjacencyGraphForm : Form
    {
        //int[,] adjacencymatrix;
        //BidirectionalGraph<object, IEdge<object>> graph;
        //private IBidirectionalGraph<object, IEdge<object>> _graphToVisalize;//字段
        //属性，只读
        //public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        //{
        //    get { return _graphToVisalize; }
        //}
        //public AdjacencyGraphForm(int[,] adjacencymatrix)
        //{
        //    InitializeComponent();
        //    graph = new BidirectionalGraph<object, IEdge<object>>();
        //    this.adjacencymatrix = adjacencymatrix;
        //    CreateGraphToVisualize();
            
        //}
        //private void CreateGraphToVisualize()
        //{
        //    add vertex
        //    int vertexcount = adjacencymatrix.GetLength(0);
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        graph.AddVertex(i.ToString());
        //    }
        //    add edge
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        for (int j = i + 1; j < vertexcount - 1; j++)
        //        {
        //            if (adjacencymatrix[i, j] != 0)
        //            {
        //                graph.AddEdge(new Edge<Object>(i.ToString(),j.ToString()));
        //            }
        //        }
        //    }
        //    _graphToVisalize = graph;
        //}
        //private IUndirectedGraph<object, IEdge<object>> _graphundirected;
        //public IUndirectedGraph<object, IEdge<object>> GraphToVisualize
        //{
        //    get { return _graphundirected; }
        //}
        //public AdjacencyGraphForm()
        //{
        //    InitializeComponent();
        //}
        //AdjacencyGraph<int, Edge<int>> graph;
        //int[,] adjacencymatrix;
        //public AdjacencyGraphForm(int[,] adjacencymatrix)
        //{
        //    graph = new AdjacencyGraph<int, Edge<int>>();
        //    this.adjacencymatrix = adjacencymatrix;
        //    AddVertexFun();
        //    AddEdgeFun();
        //    _graphundirected = graph;
        //}
        ////添加顶点
        //private void AddVertexFun()
        //{
        //    int vertexcount = adjacencymatrix.GetLength(0);
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        graph.AddVertex(i);
        //    }
        //}
        ////添加边
        //private void AddEdgeFun()
        //{
        //    int vertexcount = adjacencymatrix.GetLength(0);
        //    for (int i = 0; i < vertexcount; i++)
        //    {
        //        for (int j = i + 1; j < vertexcount - 1; j++)
        //        {
        //            if (adjacencymatrix[i, j] != 0)
        //            {
        //                Edge<int> edge = new Edge<int>(i, j);
        //                graph.AddEdge(edge);
        //            }
        //        }
        //    }
        //}
    }
}
