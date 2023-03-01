using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace ZedGraphPro
{
    public partial class ShowZedgraph : Form
    {
        double[] array1;
        double[] array2;
        double[] array3;
        public ShowZedgraph()
        {
            InitializeComponent();
        }
        public ShowZedgraph(double[] array1,double[] array2):this()
        {
            this.array1 = array1;
            this.array2 = array2;
            CreateChart(zedGraphControl1);//初始化面板
            SetSize();
        }
        public ShowZedgraph(List<double> list1,List<double> list2)
            : this()
        {
            this.array1 = list1.ToArray();
            this.array2 = list2.ToArray();
            CreateChart(zedGraphControl1);//初始化面板
            SetSize();
        }
        public ShowZedgraph(double[] array1,double[] array2,double[] array3):this()
        {
            this.array1 = array1;
            this.array2 = array2;
            this.array3 = array3;
            CreateChart(zedGraphControl1);//初始化面板
            SetSize();
        }

        private void CreateChart(ZedGraphControl zgc)
        {
            //zgc.GraphPane.CurveList.Clear();
            //zgc.GraphPane.GraphObjList.Clear();
            //初始化图表页面信息  
            GraphPane myPane = zedGraphControl1.GraphPane;
           
            //根据选择显示不同类型的图表  
            //背景设置透明
            myPane.Fill.Color = Color.Transparent;
            myPane.Fill.IsVisible = false;
            myPane.Chart.Fill.IsVisible = false;
            //边框
            myPane.Chart.Border.IsVisible = false;
            //外边框
            myPane.Border.IsVisible = false;
            this.zedGraphControl1.BorderStyle = BorderStyle.None;
            this.zedGraphControl1.GraphPane.Border.IsVisible = false;
            this.zedGraphControl1.MasterPane.Border.IsVisible = false;
            myPane.Chart.Border.IsVisible = false;
            
            //myPane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XScaleFormatEvent);
            //myPane.YAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(YScaleFormatEvent);
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);        
            //图例
            myPane.Legend.Border.IsVisible = false;
            myPane.Legend.FontSpec.Size = 24;
            myPane.Legend.Position = ZedGraph.LegendPos.InsideTopLeft;
            myPane.Legend.Fill.IsVisible = false;
            myPane.Legend.IsReverse = true;
            //myPane.Legend.IsShowLegendSymbols = false;
            myPane.Legend.IsHStack = true;
            zgc.AxisChange();
            zgc.Refresh();
            LineChart(myPane);//绘制曲线
            //BarChart(myPane);//绘制柱状图
            //添加标注

        }
        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);

            //保留一个小的页面空白在控件的周围
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 20,
                                                          ClientRectangle.Height - 20);
        }
        private void BarChart(GraphPane myPane)
        {
            BarItem myCurve;
            //定义笔刷
            //Rectangle rect1 = new Rectangle(20, 80, 250, 100);
            //HatchBrush myhbrush1 = new HatchBrush(HatchStyle.DiagonalCross,
            //     Color.DarkGray, Color.White);
            //HatchBrush myhbrush2 = new HatchBrush(HatchStyle.DarkVertical,
            //     Color.DarkGray, Color.White);//
            //LinearGradientBrush mylbrush3 = new LinearGradientBrush(rect1,
            //     Color.DarkGray, Color.White,
            //     LinearGradientMode.BackwardDiagonal);
            //HatchBrush myhbrush4 = new HatchBrush(HatchStyle.LargeGrid,
            //     Color.DarkGray, Color.White);//
            //柱状
            myCurve = myPane.AddBar("Gurobi", null, array1, Color.Brown);
            //myCurve.Bar.Fill = new Fill(myhbrush1, false);
            //设置柱状的legend不可见
            //myCurve.Label.IsVisible = false;
            myCurve = myPane.AddBar("BDS", null, array2, Color.YellowGreen);
            if(array3!=null)
            {
                myCurve = myPane.AddBar("BDSH",null,array3,Color.Blue);
            }
            //myCurve.Bar.Fill = new Fill(myhbrush2, false);
            //for (int i = 0; i < array1.Length; i++)
            //{
            //    // Create a text label from the Y data value
            //    TextObj text1 = new TextObj(array1[i].ToString(), i + 1 - 0.37, array1[i] + 2, CoordType.AxisXYScale, AlignH.Left, AlignV.Center);
            //    TextObj text2 = new TextObj(array2[i].ToString(), i + 1, array2[i] - 3, CoordType.AxisXYScale, AlignH.Left, AlignV.Center);
            //    //if (pt1 == myCurve1.Points[0])
            //    //{
            //    //    text1 = new TextObj(pt1.Y.ToString(), pt1.X, pt1.Y + 8, CoordType.AxisXYScale, AlignH.Left, AlignV.Center);
            //    //}
            //    text1.ZOrder = ZOrder.A_InFront;
            //    // Hide the border and the fill
            //    //text.FontSpec.Border.IsVisible = false;
            //    //text.FontSpec.Fill.IsVisible = false;
            //    //text.FontSpec.Fill = new Fill( Color.FromArgb( 100, Color.White ) );
            //    // Rotate the text to 90 degrees
            //    //text.FontSpec.Angle = 60;  //字体倾斜度
            //    text1.FontSpec.Border.IsVisible = false;
            //    text1.FontSpec.Size = 20;
            //    text1.FontSpec.Fill.IsVisible = false;
            //    myPane.GraphObjList.Add(text1);
            //    text2.FontSpec.Border.IsVisible = false;
            //    text2.FontSpec.Size = 20;
            //    text2.FontSpec.Fill.IsVisible = false;
            //    myPane.GraphObjList.Add(text2);
            //}
        }

        private void LineChart(GraphPane myPane)
        {

            if(array1==null||array2==null)
            {
                return;
            }
            //x轴
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "";
            //myPane.XAxis.Type = AxisType.Text;
            myPane.XAxis.Scale.Max = array1.Length + 1;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.Format = "";
            myPane.YAxis.Title.Text = "";
            myPane.Y2Axis.IsVisible = true;
            myPane.Y2Axis.Title.Text = "";

            myPane.XAxis.MajorGrid.IsVisible = false;
            myPane.XAxis.MinorGrid.IsVisible = false;
            myPane.X2Axis.IsVisible = false;
            myPane.XAxis.Scale.FontSpec.Size = 24;
            myPane.XAxis.Title.Text = "";
            myPane.XAxis.Title.FontSpec.Size = 24;
            //string[] xLables = new string[array1.Length];
            //for (int array_index = 0; array_index < array1.Length; array_index++)
            //{
            //    xLables[array_index]=array_index.ToString();
            //}
            
            //myPane.XAxis.Scale.TextLabels = xLables;
            myPane.XAxis.Scale.FontSpec.Size = 24;
            //y轴
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.Title.Text = "";
            myPane.YAxis.Title.FontSpec.Size = 24;
            myPane.YAxis.Scale.MajorStep = 100;
            myPane.YAxis.Scale.MinorStep = 100;
            myPane.YAxis.Scale.Max = 4000;
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.FontSpec.Size = 24;
            //y2轴
            myPane.Y2Axis.Title.Text = "";
            myPane.Y2Axis.MajorGrid.IsZeroLine = false;
            myPane.Y2Axis.Scale.Align = AlignP.Inside;
            myPane.Y2Axis.Scale.Max = 184;
            myPane.Y2Axis.Scale.MajorStep = 20;
            myPane.Y2Axis.Scale.MinorStep = 20;
            myPane.Y2Axis.Scale.FontSpec.Size = 24;
            myPane.Y2Axis.Title.FontSpec.Size = 24;
            myPane.Y2Axis.IsVisible = false;
            //对面的坐标轴是否显示刻度
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MinorTic.IsOpposite = false;

            PointPairList pointlist1=listvaluetopointlist(array1);
            LineItem myCurve_1 = myPane.AddCurve("",
                            pointlist1, Color.Green, SymbolType.None);

            
            PointPairList pointlist2=listvaluetopointlist(array2);           
            LineItem myCurve_2 = myPane.AddCurve("",
                            pointlist2, Color.Green, SymbolType.None);

            PointPairList pointlist3=listvaluetopointlist(array3);
            LineItem myCurve_3=myPane.AddCurve("",pointlist3,Color.Red,SymbolType.None);
        }
        private PointPairList listvaluetopointlist(double[] array)
        {
            PointPairList pointlist = new PointPairList();
            for(int array_index=0;array_index< array.Length;array_index++)
            {
                pointlist.Add(array_index,array[array_index]);
            }
            return pointlist;
        }
    }
}
