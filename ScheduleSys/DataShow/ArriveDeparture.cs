using ScheduleSys.PaintClass;
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
    public partial class ArriveDeparture : Form
    {
        Graph g;
        Bitmap BmpDiagram;
        Graphics DiagramGraphic;
        public ArriveDeparture()
        {
            InitializeComponent();
        }
        Station station;
        int[] arrivetime;
        int[] departuretime;
        int timezoom;
        int[] beginandendhour;
        int[] leftandrightx;
        int high;
        public ArriveDeparture(Graph g,Station station, int[] arrivetime, int[] departuretime):this()
        {
            this.g = g;
            this.station = station;
            this.arrivetime = arrivetime;
            this.departuretime = departuretime;

            BmpDiagram = new Bitmap(2400, 2000);
            DiagramGraphic = Graphics.FromImage(BmpDiagram);
            paint();
            pictureBox_arrivedeparture.Image = BmpDiagram;
            pictureBox_arrivedeparture.BackColor = Color.White;
        }
        void paint()
        {
            getspan();
            Brush brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(brush,1);
            Pen trainpath = new Pen(brush,2);
            Font font=new Font("宋体",12);
            //写倾斜的文字
            GraphicsText graphicsText = new GraphicsText(DiagramGraphic);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center; 
            //画框架
            for (int i = beginandendhour[0]; i <= beginandendhour[1];i++ )
            {
                float x=MapTimeToX(i * 60);
                DiagramGraphic.DrawLine(pen, new PointF(x,high-30), new PointF(x,high+30));
                DiagramGraphic.DrawString(i.ToString(), font, brush, new PointF(x, high + 30));
            }
            //车站名
            DiagramGraphic.DrawString(station.name, font, brush, new PointF(30, 30));
            //画一条车站的水平线
            DiagramGraphic.DrawLine(pen,new Point(leftandrightx[0],high),new Point(leftandrightx[1],high));
            //
            foreach(Train train in g.trainlist)
            {
                if(!train.past_stations_name.Contains(station.name))
                {
                    continue;
                }
                trainpath.Color = trainpath.Color = ColorTranslator.FromHtml
                    (DataReadWrite.ReadIniData("TrainColor", train.name, "", Application.StartupPath + "\\Paremeter.ini")); ;//标示颜色
                brush=new SolidBrush(trainpath.Color);
                if(arrivetime[train.index]!=0)
                {
                    float arrivetimemapx = MapTimeToX((int)arrivetime[train.index]);
                    //到达线
                    DiagramGraphic.DrawLine(trainpath, new PointF(arrivetimemapx - 20, high - 20), new PointF(arrivetimemapx, high));
                    //DiagramGraphic.DrawString(train.name, font, brush, new PointF(arrivetimemapx - 20, high - 20));
                    graphicsText.DrawString(arrivetime[train.index].ToString(), font, brush, new PointF(arrivetimemapx - 20, high - 40)
                            , format, 45f);
                }
                if(departuretime[train.index]!=0)
                {
                    //离开线
                    float departuretimemapx = MapTimeToX((int)departuretime[train.index]);
                    DiagramGraphic.DrawLine(trainpath, new PointF(departuretimemapx, high), new PointF(departuretimemapx + 20, high + 20));
                    graphicsText.DrawString(departuretime[train.index].ToString(), font,brush,new PointF(departuretimemapx + 20, high + 40), format, 45f);
                    if(arrivetime[train.index]==0)
                    {
                        //DiagramGraphic.DrawString(train.name, myfont, brush, new PointF(departuretimemapx + 20, high + 20));
                        graphicsText.DrawString(train.index.ToString(), font, brush, new PointF(departuretimemapx , high )
                            , format, 45f);
                    }
                }                          
            }
            brush.Dispose();
            pen.Dispose();
            trainpath.Dispose();
        }
        void getspan()
        {
            high = 200;
            beginandendhour = new int[2];
            leftandrightx = new int[2];
            timezoom = 150;//时间轴放大/缩小倍数
            int[] timespan = gettimespan();
            int begin_timehour = (int)(timespan[0]/ 60);//将值舍入到最接近的整数或指定的小数位数
            int end_timehour = (int)Math.Ceiling(Convert.ToDouble(timespan[1]) / 60);
            beginandendhour[0] = begin_timehour;
            beginandendhour[1] = end_timehour;
            leftandrightx[0] = 30;
            leftandrightx[1] = leftandrightx[0] + timezoom * (end_timehour - begin_timehour);
        }
        int[] gettimespan()
        {
            int[] result = new int[2];
            int minvar=1000000;
            int maxvar=0;
            
            foreach(double var in arrivetime)
            {
                if(var==0)
                {
                    continue;
                }
                if(var<minvar)
                {
                    minvar = (int)var;
                }
            }
            if (minvar == 1000000)
            {
                foreach (double var in departuretime)
                {
                    if (var == 0)
                    {
                        continue;
                    }
                    if (var < minvar)
                    {
                        minvar = (int)var;
                    }
                }
            }
            foreach(double var in departuretime)
            {
                if (var == 0)
                {
                    continue;
                }
                if(var>maxvar)
                {
                    maxvar =(int) var;
                }
            }
            if (maxvar == 0)
            {
                foreach (double var in arrivetime)
                {
                    if (var == 0)
                    {
                        continue;
                    }
                    if (var > maxvar)
                    {
                        maxvar = (int)var;
                    }
                }
            }
            result[0] = minvar;
            result[1] = maxvar;
            return result;
        }
        float MapTimeToX(int time)
        {
            int begin_timehour = beginandendhour[0];
            float mapx = (time - begin_timehour * 60) * timezoom / 60 + leftandrightx[0];
            return mapx;
        }
    }
}
