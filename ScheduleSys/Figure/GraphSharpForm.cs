using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleSys.Figure
{
    public partial class GraphSharpForm : Form
    {
        public GraphSharpControl GraphControl { get; set; }
        public GraphSharpForm(GraphSharpControl gsc)
        {
            InitializeComponent();
            this.GraphControl = gsc;
        }
        //out put node coordinate
        private void outPutNodeCoodinateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.StreamWriter wr = DataReadWrite.GetStreamWriter("output_node_coordinate.csv");
                wr.WriteLine("nodel_label,x,y");

                for (int i = 0; i < GraphControl.layout.Graph.Vertices.Count(); i++)
                {
                    string label = GraphControl.layout.Graph.Vertices.ElementAt(i).ToString();
                    double x = GraphCanvas.GetX(GraphControl.layout.GetVertexControl(GraphControl.graph.Vertices.ElementAt(i)));
                    double y = GraphCanvas.GetY(GraphControl.layout.GetVertexControl(GraphControl.graph.Vertices.ElementAt(i)));
                    wr.WriteLine(label + "," + (int)x + "," + (int)y);
                }
                wr.Close();
                MessageBox.Show("OutPut completely!!");
            }
            catch (System.IO.IOException er)
            {
                throw new Exception(er.Message);
            }

        }
        //set or reread the coordinate
        private void readCoodinateFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.StreamReader sr = DataReadWrite.GetStreamReader("output_node_coordinate.csv");
                string line = sr.ReadLine();
                for (int i = 1; i < GraphControl.layout.Graph.Vertices.Count() + 1; i++)
                {
                    line = sr.ReadLine();
                    if (line == null)
                    {
                        throw new Exception("data error!!");
                    }
                    string[] str_list = line.Trim().Split(',');
                    VertexControl vs = GraphControl.layout.GetVertexControl(GraphControl.graph.Vertices.ElementAt(i - 1));
                    GraphCanvas.SetX(vs, int.Parse(str_list[1]));
                    GraphCanvas.SetY(vs, int.Parse(str_list[2]));
                }
                sr.Close();
                GraphControl.layout.UpdateLayout();

            }
            catch (System.IO.IOException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //output graph tree structure
        private void outGraphTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //VertexControl vs = GraphControl.layout.GetVertexControl(GraphControl.graph.Vertices.ElementAt(0));
            // GraphControl.layout.Graph.OutEdges(vs);
        }
    }
}
