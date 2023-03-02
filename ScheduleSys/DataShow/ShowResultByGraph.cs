using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleSys.DataShow
{
    public partial class ShowResultByGraph : Form
    {
        List<double> line_data_1;
        List<double> line_data_2;
        List<double> line_data_3;

        public ShowResultByGraph()
        {
            InitializeComponent();
            ReadTimeToTextBox();
        }

        private void ReadTimeToTextBox()
        {
            //string[] timelist = DataReadWrite.INIGetAllItems(DataReadWrite.paremeterfilepath, "SolvingTime");
            //foreach(string str in timelist)
            //{
            //    textBox_solvingtime.AppendText(str+"\r\n");
            //}
            line_data_1 = new List<double>();
            line_data_2 = new List<double>();
            line_data_3 = new List<double>();
            System.IO.StreamReader sr = new System.IO.StreamReader("benders_upandlow_bound.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                textBox.AppendText(line + "\r\n");
                string[] str_list = line.Split(',');
                for (int str_index = 0; str_index < str_list.Length; str_index++)
                {
                    double value = double.Parse(str_list[str_index]);
                    switch (str_index)
                    {
                        case 0:
                            line_data_1.Add(value);
                            break;
                        case 1:
                            line_data_2.Add(value);
                            break;
                        case 2:
                            line_data_3.Add(value);
                            break;
                    }
                }

            }
            
        }
        //显示柱状图
        private void button_showbar_Click(object sender, EventArgs e)
        {
            string[] stringtimelist = DataReadWrite.INIGetAllItems(DataReadWrite.paremeterfilepath, "SolvingTime");
            List<double> doubletimelist=new List<double>{ };
            
            for (int i = 0; i < stringtimelist.Count(); i++)
			{
			    string str_item=stringtimelist[i];
                double time = double.Parse(str_item.Split('=')[1]);
                doubletimelist.Add(time);
			}
            if(doubletimelist.Count()%2!=0)
            {
                throw new Exception("The data error.");
                
            }
            int tempcount=doubletimelist.Count()/3;
            double[] timelist1 = new double[tempcount];
            double[] timelist2 = new double[tempcount];
            double[] timelist3 = new double[tempcount];
            for (int i = 0; i < doubletimelist.Count(); i++)
            {
                if(i<tempcount)
                {
                    timelist1[i] = doubletimelist[i];
                }
                else if(i<tempcount*2&&i>=tempcount)
                {
                    timelist2[i - tempcount] = doubletimelist[i];
                }
                else
                {
                    timelist3[i - tempcount * 2] = doubletimelist[i];
                }
            }
            ZedGraphPro.ShowZedgraph comparetime = new ZedGraphPro.ShowZedgraph(timelist1, timelist2,timelist3);
            comparetime.Show();
        }
        //显示曲线图
        private void line_graph_btn_Click(object sender, EventArgs e)
        {
            if(line_data_1==null)
            {
                return;
            }
            ZedGraphPro.ShowZedgraph showzed = new ZedGraphPro.ShowZedgraph(line_data_1.ToArray(), line_data_2.ToArray(),line_data_3.ToArray());
            showzed.Show();
        }

        private void Import_data_btn_Click(object sender, EventArgs e)
        {

        }
    }
}
