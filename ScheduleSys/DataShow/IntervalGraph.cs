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
    public partial class IntervalGraph : Form
    {
        Bitmap bitmap;
        Graphics graphics;
        Graph g;
        Station station=new Station();
        List<Interval> intervallist;
        List<Components> componentslist;
        Dictionary<string, float> trainnametoy;
        int min_time=1000000;
        int max_time=0;
        public IntervalGraph()
        {
            InitializeComponent();
        }

        public IntervalGraph(Graph g,Station _station):this()
        {
            this.g = g;
            this.station = _station;
            toolStripTextBox_stationname.Text = _station.name;
            intervallist = new List<Interval>();
            trainnametoy = new Dictionary<string, float>();
            PaintInterval();
        }
        public IntervalGraph(Station _station,Graph g)
        {
            this.g = g;
            this.station = _station;
            intervallist = new List<Interval>();
            componentslist = new List<Components>();
        }
        #region
        ////回调检查车站能力
        //public List<int> CheckStationCapacity()
        //{
        //    GraphToInterval(station);
        //    //按开始时间排序
        //    intervallist = intervallist.OrderBy(e => e.begin_time).ToList();

        //    IntervalToComponents();
        //    //开始找每个components的完全子图
        //    foreach (Components components in componentslist)
        //    {
        //        if (components._intervallist.Count() <= station.tracknumber)
        //        {
        //            continue;
        //        }
        //        if (components._intervallist.Count() <= 2)
        //        {
        //            return CliqueToTrainIndex(components);//最大完全子图顶点个数就是2.
        //        }
        //        //List<Components> componentslist_temp = new List<Components>();

        //        for (int interval_index_1 = 0; interval_index_1 < components._intervallist.Count() - 2; interval_index_1++)
        //        {
        //            Components clique = new Components();
        //            Interval interval_1 = intervallist[interval_index_1];
        //            clique._intervallist.Add(interval_1);
        //            clique._intervallist.Add(intervallist[interval_index_1 + 1]);
        //            for (int interval_index_2 = interval_index_1 + 2; interval_index_2 < components._intervallist.Count(); interval_index_2++)
        //            {
        //                Interval interval_2 = intervallist[interval_index_2];
        //                if (interval_1.end_time > interval_2.begin_time)
        //                {
        //                    clique._intervallist.Add(interval_2);
        //                }
        //                else
        //                {
        //                    if (clique._intervallist.Count() > station.tracknumber)
        //                    {
        //                        return CliqueToTrainIndex(clique);
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
        #endregion
        private List<int> CliqueToTrainIndex(Components com)
        {
            List<int> trainindexlist = new List<int> { };
            foreach(Interval interval in com._intervallist)
            {
                trainindexlist.Add(g.GetTrainByName(interval.train_name).index);
            }
            trainindexlist.Sort();
            return trainindexlist;
        }
        #region
        ////找到独立子图
        //public void IntervalToComponents()
        //{
        //    if (intervallist.Count() == 0)
        //    {
        //        throw new Exception("There is not Interval.");
        //        return;
        //    }
        //    int last_end_time = intervallist[0].end_time;
        //    componentslist.Add(new Components());
        //    componentslist[0]._intervallist.Add(intervallist[0]);
        //    for (int interval_index = 1; interval_index < intervallist.Count(); interval_index++)
        //    {
        //        Interval interval = intervallist[interval_index];
        //        if (interval.begin_time > last_end_time)
        //        {
        //            Components components = new Components();
        //            components._intervallist.Add(interval);
        //            componentslist.Add(components);
        //        }
        //        else
        //        {
        //            componentslist.Last()._intervallist.Add(interval);                   
        //        }
        //        //取当前已取出interval的最大的结束时间
        //        if(interval.end_time>last_end_time)
        //        {
        //            last_end_time = interval.end_time;
        //        }                
        //    }
        //}
        #endregion
        //next、last车站按钮点击的时候
        public void RePaint(Station _station)
        {
            min_time = 1000000;
            max_time = 0;
            this.station = _station;
            toolStripTextBox_stationname.Text = _station.name;
            intervallist = new List<Interval>();
            trainnametoy = new Dictionary<string, float>();
            PaintInterval();
        }
        //绘制车站的停站时间区间
        public void PaintInterval()
        {
            GraphToInterval(station);
            bitmap = new Bitmap((int)XMap(max_time)+200, 800);
            graphics = Graphics.FromImage(bitmap);
            PaintBasic();
            PaintIntervalInBasic();
            pictureBox_intervalgraph.Image = bitmap;
        }
        //绘制interval
        private void PaintIntervalInBasic()
        {
            Pen pen=new Pen(Color.Red,3);
            Brush brush=new SolidBrush(Color.Red);
            foreach(Interval interval in intervallist)
            {
                if(Math.Abs(interval.begin_time-interval.end_time)<2)
                {
                    graphics.FillEllipse(brush, XMap(interval.begin_time) - 10, trainnametoy[interval.train_name] - 10, 20, 20);
                }
                else
                {
                    graphics.DrawLine(pen, XMap(interval.begin_time), trainnametoy[interval.train_name],
                    XMap(interval.end_time), trainnametoy[interval.train_name]);
                }
            }
            //graphics.DrawLine(pen, 200, 300, 300, 400);
            pen.Dispose();
            brush.Dispose();
        }
        //绘制底图
        public void PaintBasic()
        {

            Pen pen = new Pen(Color.Green);
            Brush brush=new SolidBrush(Color.Black);
            Font font=new Font("宋体",12);
            //画横线
            List<string> trainnamelist = new List<string>();
            foreach (Train train in g.trainlist)
            {
                if (!train.past_stations_name.Contains(station.name))
                {
                    continue;
                }
                trainnamelist.Add(train.name);
            }
            int index=0;
            foreach (string trainname in trainnamelist)
            {
                trainnametoy.Add(trainname, index * 50 + 50);
                index++;
            }
            float minx = XMap(min_time);
            float maxx = XMap(max_time+120);
            foreach(string trainname in trainnamelist)
            {
                graphics.DrawLine(pen,minx,trainnametoy[trainname],maxx,trainnametoy[trainname]);
                graphics.DrawString(trainname, font, brush, minx - 60, trainnametoy[trainname]);
            }
            //画竖线
            int line_flag = 0;
            for (int time = min_time; time<=max_time+2; time=time+2)
            {             
                if (line_flag % 2 == 0)
                {
                    graphics.DrawLine(pen, XMap(time), trainnametoy[trainnamelist[0]], XMap(time), trainnametoy[trainnamelist.Last()]);
                    if(line_flag%4==0)
                    {
                        graphics.DrawString((time - min_time).ToString(), font, brush, XMap(time) - 10, trainnametoy[trainnamelist.Last()] + 20);
                        line_flag = 0;
                    }                   
                }
                line_flag++;
            }
            pen.Dispose();
        }
        //x轴
        public float XMap(int time)
        {
            float xendright = 100;
            float x_zoom = 10f;
            return (time - min_time) * x_zoom + xendright;
        }
        //将图形成interval的形式,并记录时间区段
        public void GraphToInterval(Station _station)
        {
            foreach (Train train in g.trainlist)
            {
                if (!train.past_stations_name.Contains(_station.name))
                {
                    continue;
                }
                int begin_time = g.GetArriveTimeByTrainStation(train.name, _station.name);
                int end_time = g.GetDepartureTimeByTrainStation(train.name, _station.name);
                //更新最大最小时间
                if(begin_time<min_time)
                {
                    min_time = begin_time;
                }
                if(end_time>max_time)
                {
                    max_time = end_time;
                }
                //if (begin_time != end_time)
                //{
                    intervallist.Add(new Interval(begin_time, end_time, train.name));
                //}
            }
        }
        //片段
        class Interval
        {
            public int begin_time;
            public int end_time;
            public string train_name;//所属的列车名
            public Interval(int begin_time, int end_time, string train_name)
            {
                this.begin_time = begin_time;
                this.end_time = end_time;
                this.train_name = train_name;
            }
        }
        //独立子图、完全子图
        class Components
        {
            public List<Interval> _intervallist;
            public Components()
            {
                _intervallist = new List<Interval>();
            }
        }
        //显示k-coloring的状态图，传递的参数是一个邻接矩阵比较合适
        private void kColoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[,] adjacencymatrix = new int[intervallist.Count(), intervallist.Count()];
            for (int i = 0; i < adjacencymatrix.GetLength(0); i++)
            {
                for (int j = i+1; j < adjacencymatrix.GetLength(0)-1; j++)
                {
                    Interval interval_1 = intervallist[i];
                    Interval interval_2 = intervallist[j];
                    if((interval_1.end_time>interval_2.begin_time&&interval_1.end_time<interval_2.end_time)||
                        interval_1.begin_time>interval_2.begin_time&&interval_1.begin_time<interval_2.end_time)
                    {
                        adjacencymatrix[i, j] = 1;
                    }
                }
            }
            //GraphSharpWPF.MainWindow adjacencygraphform = new GraphSharpWPF.MainWindow(adjacencymatrix);
            //GraphSharpWPF.MainWindow adjacencygraphform = new GraphSharpWPF.MainWindow();
            //adjacencygraphform.Show();
        }
        //显示上一个车站
        private void button_last_Click(object sender, EventArgs e)
        {
            if(this.station.index==0)
            {
                MessageBox.Show("The station is the first station");
                return;
            }
            Station station = g.stationlist[this.station.index-1];//使用 this.num 引用类字段。
            //为局部变量提供一个与类字段名称不同的名称,局部变量在声明之前无法使用，隐藏字段,加this
            RePaint(station);//此语为什么不能成功呢，应为新建了一个类，画的图等都是包含在新的类当中，但是原来类的变量依然存在
        }
        //显示下一个车站
        private void button_next_Click(object sender, EventArgs e)
        {
            if(this.station.index>=g.stationlist.Count()-1)
            {
                MessageBox.Show("The station is the last station.");
                return;
            }
            Station station = g.stationlist[this.station.index +1];
            RePaint(station);
        }
        //检查车站能力
        public List<int> CheckStationCapacity(Station _station)
        {
            Components maxclique = maxtrackusagefun(_station);
            if(maxclique._intervallist.Count()>_station.tracknumber)
            {
                List<int> result = CliqueToTrainIndex(maxclique);
                return result;
            }
            return null;
        }
        ////最大团,得到车站至少需要多少条到发线
        //private int MaxClique(Station _station)
        //{
        //    intervallist=new List<Interval>();
        //    componentslist = new List<Components>() ;
        //    GraphToInterval(_station);
        //    //按开始时间排序
        //    intervallist = intervallist.OrderBy(e => e.begin_time).ToList();

        //    IntervalToComponents();
        //     List<Components> componentslist_temp = new List<Components>();
        //    //开始找每个components的完全子图
        //     int maxinterval = 0;
        //    foreach (Components components in componentslist)
        //    {
        //        if (components._intervallist.Count() <= station.tracknumber)
        //        {
        //            if(components._intervallist.Count()>maxinterval)
        //            {
        //                maxinterval = components._intervallist.Count();
        //            }
        //            continue;
        //        }
        //        for (int interval_index_1 = 0; interval_index_1 < components._intervallist.Count() - 2; interval_index_1++)
        //        {
        //            Components clique = new Components();
        //            Interval interval_1 = intervallist[interval_index_1];
        //            clique._intervallist.Add(interval_1);
        //            clique._intervallist.Add(intervallist[interval_index_1 + 1]);
        //            int continue_flag = 0;//判断当前的components是否就是一个完全子图，即在下面的循环中遍历完所有的interval
        //            for (int interval_index_2 = interval_index_1 + 2; interval_index_2 < components._intervallist.Count(); interval_index_2++)
        //            {
        //                Interval interval_2 = intervallist[interval_index_2];
        //                if (interval_1.end_time > interval_2.begin_time)
        //                {
        //                    clique._intervallist.Add(interval_2);
        //                }
        //                else
        //                {
        //                    continue_flag = 1;
        //                    break;
        //                }
        //            }
        //            componentslist_temp.Add(clique);
        //            if(continue_flag==0)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    if(componentslist_temp.Count==0)
        //    {
        //        return maxinterval;
        //    }
        //    //得到最大团
        //    int maxintervalcountindex=0;//最大团所在位置索引
        //    int maxintervalcount=componentslist_temp[0]._intervallist.Count();
        //    for(int i=0;i<componentslist_temp.Count();i++)
        //    {
        //        if(componentslist_temp[i]._intervallist.Count>maxintervalcount)
        //        {
        //            maxintervalcountindex = i;
        //            maxintervalcount = componentslist_temp[i]._intervallist.Count();
        //        }
        //    }
        //    return maxintervalcount;
        //}
        //找到最大占用股道的方法,遍历每个interval的开始时间即可
        private Components maxtrackusagefun(Station _station)
        {
            intervallist = new List<Interval>();
            Components maxclique = new Components();
            GraphToInterval(_station);
            //按开始时间排序
            intervallist = intervallist.OrderBy(e => e.begin_time).ToList();
            foreach(Interval interval in intervallist)
            {
                int begin_time = interval.begin_time;
                Components maxclique_temp = new Components();
                maxclique_temp._intervallist.Add(interval);
                foreach(Interval interval_2 in intervallist)
                {
                    if(interval_2.train_name==interval.train_name)
                    {
                        continue;
                    }
                    if(interval_2.begin_time<begin_time&&interval_2.end_time>begin_time)
                    {
                        maxclique_temp._intervallist.Add(interval_2);
                    }
                    //如果当前的interval_2的开始时间比interval的结束时间还晚，则需要break
                    if(interval_2.begin_time>interval.end_time)
                    {
                        break;
                    }
                }
                if(maxclique_temp._intervallist.Count()>maxclique._intervallist.Count())
                {
                    //if(maxclique_temp._intervallist.Count()>_station.tracknumber)
                    //{
                    //    return maxclique_temp;
                    //}
                    maxclique = maxclique_temp;
                    
                }
            }
            return maxclique;
        }
        
        //到发线占用情况
        private void trackUsageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("station_name");
            dt.Columns.Add("maxusagetrack");
            dt.Columns.Add("tracknum");
            foreach(Station station in g.stationlist)
            {
                int maxtrackusage = maxtrackusagefun(station)._intervallist.Count();
                DataRow dr = dt.NewRow();
                dr["station_name"] = station.name;
                dr["maxusagetrack"] = maxtrackusage;
                dr["tracknum"] = station.tracknumber.ToString();
                dt.Rows.Add(dr);
            }
            dataGridView_capacity.DataSource = dt;
        }
    }
    
}
