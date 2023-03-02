using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleSys.DataShow
{
    public partial class NewTraindiagram : Form
    {
        Graph g;
        Graph new_g;
        public Dictionary<KeyValuePair<string, string>, int> trainstationtoarrivetime;
        public Dictionary<KeyValuePair<string, string>, int> trainstationtodeparturetime;
        PaintClass.PaintStyle paintstyle;
        public NewTraindiagram()
        {
            InitializeComponent();
        }
        public NewTraindiagram(Graph g,Graph new_g,PaintClass.PaintStyle paintstyle):this()
        {
            this.g = g;
            this.new_g = new_g;
            this.paintstyle = new PaintClass.PaintStyle(paintstyle);
            colorToolStripMenuItem.Checked = paintstyle.color;
            trainNameToolStripMenuItem.Checked = paintstyle.trainname;
            trainIndexToolStripMenuItem.Checked = paintstyle.trainindex;
            arriveTimeToolStripMenuItem.Checked = paintstyle.arrivetime;
            departureTimeToolStripMenuItem.Checked = paintstyle.departuretime;
            CheckObjValue();
        }
        
        #region 显示样式
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorToolStripMenuItem.Checked =! colorToolStripMenuItem.Checked;
            paintstyle.color = !paintstyle.color;
            RePaint();
        }

        private void trainNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trainNameToolStripMenuItem.Checked = !trainNameToolStripMenuItem.Checked;
            paintstyle.trainname = !paintstyle.trainname;
            RePaint();
       
        }

        private void trainIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            trainIndexToolStripMenuItem.Checked = !trainIndexToolStripMenuItem.Checked;
            paintstyle.trainindex=!paintstyle.trainindex;
            RePaint();
         
        }

        private void arriveTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arriveTimeToolStripMenuItem.Checked == false)
            {
                arriveTimeToolStripMenuItem.Checked = true;
                RePaint();
            }
            else
            {
                arriveTimeToolStripMenuItem.Checked = false;
                RePaint();
            }
        }

        private void departureTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (departureTimeToolStripMenuItem.Checked == false)
            {
                departureTimeToolStripMenuItem.Checked = true;
                RePaint();
            }
            else
            {
                departureTimeToolStripMenuItem.Checked = false;
                RePaint();
            }
        }
        //显示动车组
        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stockToolStripMenuItem.Checked = !stockToolStripMenuItem.Checked;
            paintstyle.stock = !paintstyle.stock;
            RePaint();

        }
        //点击显示broken
        private void brokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(brokenToolStripMenuItem.Checked==false)
            {
                brokenToolStripMenuItem.Checked = true;
                RePaint();
            }
            else
            {
                brokenToolStripMenuItem.Checked = true;
                RePaint();
            }
        }
        //重绘，颜色，列车名，..
        private void RePaint()
        {           
            PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(pictureBox_newtraindiagram,
                    g,new_g, paintstyle);
        }
#endregion
        //检查车头间距、最小运行时间、最小停站时间等约束条件
        private void headwayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.ChangeToEvent keepordertoevent = new Algorithm.ChangeToEvent(new_g);
            List<Algorithm.ConflictEventPair> conpairlist=keepordertoevent.CheckConflictModel(3);
            Algorithm.ChangeToEvent.ReportConflict(conpairlist);

        }
        //检查车站能力冲突
        private void capacityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IntervalGraph intervalgraph = new IntervalGraph(new_g,new_g.stationlist[0]);
            intervalgraph.Show();
        }
        //检验目标函数值
        private void CheckObjValue()
        {
            int objvalue = 0;
            int max_deviate_time = 0;
            //foreach (Station station in g.stationlist)
            //{
            //    foreach (Train train in station.arrive_train_list)
            //    {
            //        objvalue += new_g.GetArriveTimeByTrainStation(train.name,station.name)  -
            //            g.GetArriveTimeByTrainStation(train.name, station.name);
            //    }
            //    foreach (Train train in station.departure_train_list)
            //    {
            //        objvalue += new_g.GetDepartureTimeByTrainStation(train.name, station.name) -
            //       g.GetDepartureTimeByTrainStation(train.name, station.name);
            //    }
            //}
            foreach (Train train in g.trainlist)
            {
                int deviate_time = Math.Abs(new_g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name) -
                    g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name));
                objvalue += deviate_time;

                if (deviate_time > max_deviate_time)
                {
                    max_deviate_time = deviate_time;
                }
            }
            toolStripTextBox_objvalue.Text = "Obj:"+objvalue.ToString()+",Max_deviate:"+max_deviate_time;
        }
        
        //保持发车顺序依次顺延
        private void postpone_click(object sender, EventArgs e)
        {
            if(toolStripMenuItem_postpone.Checked==true)
            {
                if(trainstationtoarrivetime==null)
                {
                    throw new Exception("Error.");
                }
                new_g.trainstationtoarrivetime = trainstationtoarrivetime;
                new_g.trainstationtodeparturetime = trainstationtodeparturetime;
                toolStripMenuItem_postpone.Checked = false;
            }
            else
            {
                if(trainstationtoarrivetime==null)
                {
                    trainstationtoarrivetime = new_g.trainstationtoarrivetime;
                    trainstationtodeparturetime = new_g.trainstationtodeparturetime;
                }             
                Algorithm.ChangeToEvent keepordertoevent = new Algorithm.ChangeToEvent(new_g);
                keepordertoevent.KeepOrderFreeConflict();
                toolStripMenuItem_postpone.Checked = true;
            }
            RePaint();
            CheckObjValue();
            MessageBox.Show("Succeed.");
        }
        // 保存图片
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            //try
            //{
            //    string file_path = DataReadWrite.SaveFileDialog();
            //    if(file_path=="")
            //    {
            //        return;
            //    }
            //    pictureBox_newtraindiagram.Image.Save(file_path);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            DataReadWrite.SavePicture(pictureBox_newtraindiagram);
        }

        private void showStockPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataShow.Message message = new DataShow.Message();
            if(paintstyle.path!=null)
            {            
                foreach(int value in paintstyle.path.train_index_list)
                {
                    message.textBox_message.AppendText(value+"\r\n");
                }
                
            }
            else
            {
                message.textBox_message.AppendText("No path.");
            }
            message.ShowDialog();
        }
        //显示每一列车所用的动车组，以及所有路径所包含的列车
        private void showSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolvingResult sr = new SolvingResult();
            int total_cancel = 0;
            List<int> use_stock = new List<int>();
            foreach(Train train in g.trainlist)
            {
                sr.textBox_result.AppendText("Train_"+train.index+" stock index: "+train.stock_index+"\r\n");
                if(train.stock_index==-1)
                {
                    total_cancel++;
                }
                if(!use_stock.Contains(train.stock_index)&&train.stock_index!=-1)
                {
                    use_stock.Add(train.stock_index);
                }
            }
            //统计取消的列车数量
            sr.textBox_result.AppendText("------------------------\r\n");
            sr.textBox_result.AppendText("Total cancel train:"+total_cancel+"\r\n");
            sr.textBox_result.AppendText("Total used stock:" + use_stock.Count() + "\r\n");
            if(paintstyle.best_path_list!=null)
            {             
                foreach(Algorithm.Path path in paintstyle.best_path_list)
                {
                    sr.textBox_result.AppendText("Path_" + path.index + ":" + path.get_train_list_in_string()+
                        ","+path.feasible+"\r\n");
                }
            }
            else
            {
                foreach(int value in use_stock)
                {
                    sr.textBox_result.AppendText("stock_"+value);
                    int count = 0;
                    foreach (Train train in g.trainlist)
                    {
                        if(train.stock_index==value)
                        {
                            count++;
                        }
                    }
                    sr.textBox_result.AppendText(":"+count+"\r\n");
                }
                
            }
            if (paintstyle.best_path_list != null)
            {
                sr.textBox_result.AppendText("------------Time imformation--------------\r\n");
                foreach (Algorithm.Path path in paintstyle.best_path_list)
                {
                    string time_list = "";
                    foreach(int train_index in path.train_index_list)
                    {
                        Train train = g.trainlist[train_index];
                        time_list += "("+train_index+","+new_g.GetDepartureTimeByTrainStation(train.name,train.begin_station_name)+","
                            +new_g.GetArriveTimeByTrainStation(train.name, train.end_station_name)+"),";
                    }
                    sr.textBox_result.AppendText("Path_" + path.index + ":" + time_list +
                        "," + path.feasible + "\r\n");
                }
            }
            sr.textBox_result.AppendText("-------Those train move:------------\r\n");
            foreach(Train train in g.trainlist)
            {
                int origin_departure_time = g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name);
                int new_departure_time = new_g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name);

                if(new_departure_time-origin_departure_time<0)
                    sr.textBox_result.AppendText("Train "+train.index+" move left "+(new_departure_time-origin_departure_time)+"\r\n");
                else if(new_departure_time-origin_departure_time>0)
                    sr.textBox_result.AppendText("Train " + train.index + " move right " + (new_departure_time - origin_departure_time)+"\r\n");
            }

            sr.Show();
        }
        //检查所有路径是否满足接续约束
        private void checkStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolvingResult sr = new SolvingResult();
            bool has_conflict = false;
            if (paintstyle.best_path_list != null)
            {
                foreach (Algorithm.Path path in paintstyle.best_path_list)
                {
                    //sr.textBox_result.AppendText("Path_" + path.index + ":" + path.get_train_list_in_string() + "\r\n");
                    for (int list_index = 0; list_index < path.train_index_list.Count()-1; list_index++)
                    {
                        Train train_1 = g.trainlist[path.train_index_list[list_index]];
                        Train train_2 = g.trainlist[path.train_index_list[list_index + 1]];
                        int departure_time = new_g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name);
                        int arrive_time = new_g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name);
                        if(departure_time-arrive_time>60||departure_time-arrive_time<10)
                        {
                            has_conflict = true;
                            sr.textBox_result.AppendText("Path_" + path.index + ":" + path.get_train_list_in_string() +
                                " is conflicted"+"\r\n");
                        }
                    }
                }
            }
            if(has_conflict==false)
            {
                sr.textBox_result.AppendText("There has no conflicts for all paths.\r\n");
            }
            sr.Show();
        }

        private void saveTimetableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataReadWrite.SaveTimetable(g,new_g,"Timetable_new.csv");
            MessageBox.Show("Succeed!!");
        }

        private void saveVariableToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataReadWrite.Savevariable(g,new_g);
            MessageBox.Show("Succeed!!");
        }      
    }

}
