using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleSys.DataShow;
using ScheduleSys.SystemSetting;
using Gurobi;

namespace ScheduleSys
{
    public partial class MainForm : Form
    {
        Graph g;//初始
        Graph new_g;//调整之后
        Status status;//面板显示转态的类
        PaintClass.PaintStyle paintsyle;
        List<Algorithm.ConflictEventPair> conflictpairs;//冲突对
        public Figure.GraphSharpControl GraphControl { get; set; }
        Stopwatch sw;
        public MainForm()
        {
            InitializeComponent();
            g = new Graph();
            status = new Status();
            InitialValue();
            
        }
        void InitialValue()
        {
            //调整的类型，晚点调整还是区间故障
            toolStripComboBox_schedulereason.Items.Add("Delay");
            toolStripComboBox_schedulereason.Items.Add("Broken");
            toolStripComboBox_schedulereason.Items.Add("Optimal");
            toolStripComboBox_schedulereason.SelectedIndex = 1;
            //导入数据的类型，秒格式还是分钟
            toolStripComboBox_timeformat.Items.Add("Second");
            toolStripComboBox_timeformat.Items.Add("Minute");
            toolStripComboBox_timeformat.SelectedIndex = int.Parse(DataReadWrite.ReadIniData("TimeFormat", "MinuteORNot", "", Application.StartupPath + "\\Paremeter.ini"));

            //设置运行图显示模式，颜色，列车名称
            bool color = bool.Parse(DataReadWrite.ReadIniData("Display", "currentcolorstatus", "", Application.StartupPath + "\\Paremeter.ini"));
            bool trainname = bool.Parse(DataReadWrite.ReadIniData("Display", "currenttrainnamestatus", "", Application.StartupPath + "\\Paremeter.ini"));
            bool trainindex = bool.Parse(DataReadWrite.ReadIniData("Display", "currenttrainindexstatus", "", Application.StartupPath + "\\Paremeter.ini"));
            bool arrivetime = bool.Parse(DataReadWrite.ReadIniData("Display", "currentarrivetime", "", Application.StartupPath + "\\Paremeter.ini"));
            bool departuretime = bool.Parse(DataReadWrite.ReadIniData("Display", "currentdeparturetime", "", Application.StartupPath + "\\Paremeter.ini"));

            paintsyle = new PaintClass.PaintStyle(color,false,true,arrivetime,departuretime,false,null);
            //默认考虑车头间距,考虑车站能力约束
            headwayToolStripMenuItem.Checked = true;
            stationcapacityToolStripMenuItem.Checked = false;
            acToolStripMenuItem.Checked = true;
            //importToolStripMenuItem.PerformClick();//无效
            importinitialtimetable();
        }
        private void improtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importinitialtimetable();
        }
        private void importinitialtimetable()
        {
            //DataReadWrite.OpenFile();//静态方法直接由类调用即可
            string file_path = "Timetable.csv";//文件路径
            //drw.OpenDialog(file_path);
            //if (file_path != "")
            //{
            //    drw.OpenCSV(file_path);
            //}
            DataReadWrite.ImportStation("Station_runtime.csv", g);
            DataReadWrite.ImportTrainTimetable(file_path, g);//导入初始的列车时刻表
            DataReadWrite.ImportStock("Stock.csv",g);
            DataReadWrite.ImportDepot(g);
            //DataReadWrite.ImportMaintenance("maintenance.csv",g);
            //画出初始运行图
            PaintClass.PaintStyle paintstyle = new PaintClass.PaintStyle();
            PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(initial_train_diagram_picturebox, g, paintsyle);

            status.importtimetime_flag = true;//已经导入的时刻表数据
            //显示列车和车站的数量
            toolStripStatusLabel_trainnumber.Text = g.trainlist.Count().ToString();
            toolStripStatusLabel_stationnumber.Text = g.stationlist.Count().ToString();
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (status.importtimetime_flag == false)
            {
                MessageBox.Show("Please import train timetable", "Message");
                return;
            }
            DataShow.InitialTimetable initialtimetable = new DataShow.InitialTimetable();
            DataReadWrite.ShowTimetable(g, initialtimetable.dataGridView_initialtimetable);
            initialtimetable.Show();
        }
        //参数设置
        private void paremeterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemSetting.ParemeterSetting paremeter_setting = new SystemSetting.ParemeterSetting();
            paremeter_setting.Show();
        }
        //清空数据
        private void clearAndRestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //把动车组的信息清空
            foreach (Train train in g.trainlist)
            {
                train.stock_index = -1;
            }
            ClearData();
        }
        public void ClearData()
        {
            g = new Graph();
            initial_train_diagram_picturebox.Image = null;//清空画布
        }
        //运行按钮
        private void toolstripbutton_run_Click(object sender, EventArgs e)
        {
            DataReadWrite.SetParemeter(g);//设置车头间距
            if (status.importtimetime_flag == false)
            {
                MessageBox.Show("Please import train timetable", "Message");
                return;
            }
            else
            {
                SelectSolveMethod selectsolvemethod = new SelectSolveMethod();
                DialogResult resultform= selectsolvemethod.ShowDialog();
                //if (resultform == DialogResult.Cancel)
                //{
                //    return;
                //}
                DataReadWrite.ClearText("GurobiSolving.log");//清空求解信息的文本文件
                DataReadWrite.ClearText("callback.log");//清空求解信息的文本文件
                DataReadWrite.ClearText("bendersmater.log");//清空求解信息的文本文件
                DataReadWrite.ClearText("benderssubproblem.log");//清空求解信息的文本文件
                DataReadWrite.ClearText("benders.txt");
                if (selectsolvemethod.method == solving_method.Model_solve)
                {
                    //Model.ModelTimetableandstock3 model = new Model.ModelTimetableandstock3(g);
                    //model.BuildAndSovleModel();
                    //model.SolveResult(new_g);

                    //Model.Model4 model = new Model.Model4(g);
                    //model.BuildAndSovleModel();
                    //model.SolveResult(new_g);

                    Model.Model5 model = new Model.Model5(g);
                    model.BuildAndSovleModel();
                    model.SolveResult(new_g);

                    //Model.Model4_callback model = new Model.Model4_callback(g);
                    //model.BuildAndSovleModel();
                    //model.SolveResult(new_g);

                    //Model.RolllingStock model = new Model.RolllingStock(g);
                    //model.BuildAndSovleModel();
                    //new_g = g;
                    //model.SolveResult(new_g);
                }
                if (selectsolvemethod.method == solving_method.Branch_price_cut_solve)
                {
                    //Algorithm.Benders4 benders = new Algorithm.Benders4(g);
                    //benders.bendersmainprocess();
                    Algorithm.Benders2 benders = new Algorithm.Benders2(g);
                    benders.bendersmainprocess();
                    //Algorithm.Benders3 benders = new Algorithm.Benders3(g);
                    //benders.bendersmainprocess();
                }
                //Model.LongestPath2 model = new Model.LongestPath2(g);
                //model.BuildAndSovleModel();
                //model.SolveResult();
            }
        }
        
        //导入时间格式的选择
        private void toolstripcombobox_timeformat_value_change(object sender, EventArgs e)
        {
            DataReadWrite.WriteIniData("TimeFormat", "MinuteORNot", toolStripComboBox_timeformat.SelectedIndex.ToString(), Application.StartupPath + "\\Paremeter.ini");
        }
        //晚点数据、区间故障加载
        private void loadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (status.importtimetime_flag == false)
            {
                MessageBox.Show("Please import train timetable", "Message");
                return;
            }
            else
            {
                SystemSetting.Delayloading delayloadingform = new SystemSetting.Delayloading(g,status.schedulefor);
                delayloadingform.Show();

            }
        }
        //晚点信息显示在图中
        private void showInDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //检查冲突
        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.ChangeToEvent keepordertoevent = new Algorithm.ChangeToEvent(g);
            //keepordertoevent.CheckConflictModel(0);
            this.conflictpairs = keepordertoevent.CheckConflictModel(0);
            status.conflictcheck = true;
            Algorithm.ChangeToEvent.ReportConflict(conflictpairs);
        }
        /// <summary>
        /// 枚举类型作为参数，根据按钮点击参过来的枚举
        /// </summary>
        /// <param name="?"></param>
        private void TrainDiagramDispalyModel(bool onestyle,string name)
        {
            PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(initial_train_diagram_picturebox, g, paintsyle);
            
            if (onestyle == false)
            {
                DataReadWrite.WriteIniData("Display", name, "true", Application.StartupPath + "\\Paremeter.ini");              
            }
            else
            {
                DataReadWrite.WriteIniData("Display", name, "false", Application.StartupPath + "\\Paremeter.ini");
            }
        }
        #region
        
        //是否标示颜色
        private void toolStripButton_color_Click(object sender, EventArgs e)
        {
            paintsyle.color = !paintsyle.color;
            TrainDiagramDispalyModel(paintsyle.color, "currentcolorstatus");
        }
        //是否显示列车名
        private void toolStripButton_trainname_Click(object sender, EventArgs e)
        {
            paintsyle.trainname = !paintsyle.trainname;
            TrainDiagramDispalyModel(paintsyle.trainname, "currenttrainnamestatus");
        }
        //是否显示到达时刻
        private void toolStripButton_arrivetime_Click(object sender, EventArgs e)
        {
            TrainSelect trainselect = new TrainSelect(g);
            trainselect.ShowDialog();
            TrainDiagramDispalyModel(paintsyle.arrivetime, "currentarrivetime");
        }
        //是否显示出发时刻
        private void toolStripButton_departuretime_Click(object sender, EventArgs e)
        {
            TrainSelect trainselect = new TrainSelect(g);
            trainselect.ShowDialog();
            TrainDiagramDispalyModel(paintsyle.departuretime, "currentdeparturetime");
         
        }
        //是否显示列车index
        private void toolStripButton_trainindex_Click(object sender, EventArgs e)
        {
            paintsyle.trainindex = !paintsyle.trainindex;
            TrainDiagramDispalyModel(paintsyle.trainindex, "currenttrainindexstatus");
        }

        #endregion
      
       
        private void sovleSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolvingSizeTrainStation solvingsize = new SolvingSizeTrainStation(g,this);
            solvingsize.Show();
        }
        //设置列车线条的颜色
        private void trainColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(status.importtimetime_flag==false)
            {
                MessageBox.Show("Please import train timetable", "Message");
                return;
            }
            TrainColor traincolor = new TrainColor();
            traincolor.Show();
        }
        //设置是否考虑车头间距约束
        private void headwayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Checked)
            {
                headwayToolStripMenuItem.Checked = false;
            }
            else
            {
                ((ToolStripMenuItem)sender).Checked = true;
            }
            //headwayToolStripMenuItem.GetCurrentParent();
        }
       
        //不考虑车头间距等的冲突展示
        private void toolStripButton_delayconflict_Click(object sender, EventArgs e)
        {
            if (status.importtimetime_flag == false)
            {
                MessageBox.Show("Please import train timetable", "Message");
                return;
            }
            else
            {
               
            }
        }
        //减速约束采用确切的模型
        private void speedReduceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Checked)
            {
                speedReduceToolStripMenuItem.Checked = false;
            }
            else
            {
                ((ToolStripMenuItem)sender).Checked = true;
            }
        }
        //减速约束，采用非确切的模型，启发式的
        private void speedReduceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Checked)
            {
                speedReduceToolStripMenuItem1.Checked = false;
            }
            else
            {
                ((ToolStripMenuItem)sender).Checked = true;
            }
        }   
        private void stationCaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Checked)
            {
                ((ToolStripMenuItem)sender).Checked = false;
            }
            else
            {
                ((ToolStripMenuItem)sender).Checked = true;
            }
        }
        //晚点还是区间故障调整的combox选择改变触发的事件
        private void schedulereason_selectindex_changed(object sender, EventArgs e)
        {
            if(toolStripComboBox_schedulereason.SelectedIndex==0)
            {
                status.schedulefor = 0;
            }
            else if(toolStripComboBox_schedulereason.SelectedIndex==1)
            {
                status.schedulefor = 1;
            }
            else
            {
                status.schedulefor = 2;
            }
        }
        //车站能力检查
        private void checkToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IntervalGraph intervalgraph = new IntervalGraph(g, g.stationlist[0]);
            intervalgraph.Show();
        }
        //比较时间的按钮
        private void solvingTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowResultByGraph compartime = new ShowResultByGraph();
            compartime.Show();
        }
        //保存图片
        private void saveAsDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataReadWrite.SavePicture(initial_train_diagram_picturebox);
        }

        private void hearisticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void showBBNodeTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphControl = new Figure.GraphSharpControl("Node_Tree.txt");
            Figure.GraphSharpForm graphsharpform = new Figure.GraphSharpForm(GraphControl);
            graphsharpform.elementHost1.Child = GraphControl;
            //graphsharpform.Layout.
            graphsharpform.Show();
        }
    }
}
