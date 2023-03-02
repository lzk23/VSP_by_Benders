using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Algorithm
{
    //class Benders
    //{
    //    GRBEnv env;
    //    GRBModel bendersmaster;
    //    GRBModel benderssubmodel;
    //    GRBLinExpr objExpr;
    //    public int optimstatus;


    //    //全局变量
    //    Graph g;
    //    Graph new_g;//benders主问题所产生的新的图
    //    int train_num = 0;
    //    int M = 1000000;
    //    int time_num = 1440;
    //    int stock_num = 0;
    //    int connection_time = 5;
    //    int max_train_for_one_stock = 4;



    //    //决策变量

    //    GRBVar[] arrivetime_train;//到达车站的时间
    //    GRBVar[] departuretime_train;//离开车站的时间
    //    GRBVar[] deviate_train;//偏离

    //    GRBVar[,] connection_train;//两列反向列车是否接续

    //    GRBVar[,] train_use_stock_flag;
    //    GRBVar[] stock_routing_first_train;
    //    GRBVar[] stock_routing_last_train;

    //    List<Path>[] stock_path_list;
    //    public Benders()
    //    {
    //        env = new GRBEnv("bendersmater.log");
    //        //env.Set(GRB.IntParam.OutputFlag, 0);
    //        bendersmaster = new GRBModel(env);
    //        env = new GRBEnv("benderssubproblem.log");
    //        benderssubmodel = new GRBModel(env);
    //        objExpr = new GRBLinExpr();
    //    }
    //    public Benders(Graph g):this()
    //    {
    //        this.g = g;
    //        stock_path_list = new List<Path>[stock_num];
    //        train_num = g.trainlist.Count();
    //        stock_num = g.stocklist.Count();
    //        //决策变量的初始化
    //        arrivetime_train = new GRBVar[train_num];
    //        departuretime_train = new GRBVar[train_num];
    //        deviate_train = new GRBVar[train_num];

    //        connection_train = new GRBVar[train_num, train_num];

    //        stock_routing_first_train = new GRBVar[train_num];
    //        stock_routing_last_train = new GRBVar[train_num];

    //        train_use_stock_flag = new GRBVar[stock_num, train_num];

    //        for (int stock_index = 0; stock_index < stock_num; stock_index++)
    //        {
    //            stock_path_list[stock_index] = new List<Path>();
    //        }

    //        for (int train_index = 0; train_index < train_num; train_index++)
    //        {
    //            arrivetime_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "a[" + ",T" + train_index + "]");
    //            departuretime_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "d[" + ",T" + train_index + "]");
    //            deviate_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "deviate[T" + train_index + "]");

    //            stock_routing_first_train[train_index] = bendersmaster.AddVar(0, 1, 0,
    //               GRB.BINARY, "b[R" + train_index + "]");
    //            stock_routing_last_train[train_index] = bendersmaster.AddVar(0, 1, 0,
    //                GRB.BINARY, "e[R" + train_index + "]");
    //        }
    //        for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
    //        {
    //            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
    //            {
    //                if (train_index_1 != train_index_2)
    //                {
    //                    connection_train[train_index_1, train_index_2] = bendersmaster.AddVar(0, 1, 0,
    //                        GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + "]");
    //                }

    //            }
    //        }

    //        for (int stock_index = 0; stock_index < stock_num; stock_index++)
    //        {
    //            for (int train_index = 0; train_index < train_num; train_index++)
    //            {
    //                train_use_stock_flag[stock_index, train_index] = bendersmaster.AddVar(0, 1, 0,
    //                    GRB.BINARY, "x[R" + stock_index + ",T" + train_index + "]");
    //            }

    //        }
    //    }
    //    //建立主问题
    //    public void buildbendersmaster()
    //    {
    //        GRBLinExpr expr;
    //        //运行时间约束
    //        foreach (Train train in g.trainlist)
    //        {
    //            bendersmaster.AddConstr(arrivetime_train[train.index] - departuretime_train[train.index] == g.GetArriveTimeByTrainStation
    //                (train.name, train.end_station_name) - g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name), "runtime" + train.name);
    //        }
    //        //同向列车追踪间隔
    //        for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
    //        {

    //            Train train_1 = g.trainlist[train_index_1];

    //            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
    //            {
    //                if (train_index_1 >= train_index_2)
    //                {
    //                    continue;
    //                }
    //                Train train_2 = g.trainlist[train_index_2];

    //                if (train_1.direction == train_2.direction)
    //                {
    //                    bendersmaster.AddConstr(departuretime_train[train_index_2] - departuretime_train[train_index_1] >= g.departuredeparturehead,
    //                        "departure_order_1" + train_index_1 + "," + train_index_2);

    //                    bendersmaster.AddConstr(arrivetime_train[train_index_2] - arrivetime_train[train_index_1] >= g.arrivearrivehead,
    //                        "arrive_order_2" + train_index_1 + "," + train_index_2);
    //                }
    //            }
    //        }
    //        //最小接续时间约束
    //        for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
    //        {
    //            Train train_1 = g.trainlist[train_index_1];
    //            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
    //            {
    //                Train train_2 = g.trainlist[train_index_2];
    //                if (train_1.direction != train_2.direction)
    //                {
    //                    bendersmaster.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] >=
    //                        connection_time - (1 - connection_train[train_index_1, train_index_2]) * M,
    //                        "connection_train_T" + train_index_1 + "_T" + train_index_2);
    //                }
    //            }
    //        }
    //        bendersmaster.SetCallback(new BendersCallback());
    //    }

    //    //建立子问题
    //    public void buildbenderssubmodel()
    //    {
    //        GRBLinExpr expr = new GRBLinExpr();
    //        for (int stock_index = 0; stock_index < stock_num; stock_index++)
    //        {
    //            benderssubmodel.AddConstr(expr,GRB.LESS_EQUAL,1,"each_stock_less_one_routing_"+stock_index);
    //        }
    //        expr=new GRBLinExpr();
    //        for (int  train_index= 0;  train_index< train_num; train_index++)
    //        {
    //            benderssubmodel.AddConstr(expr,GRB.EQUAL,1,"one_train_one_stock_"+train_index);
    //        }
    //        benderssubmodel.Update();
    //    }
    //    //benders子问题求解
    //    public void benderssubmodelsolve()
    //    {
    //        //添加初始列
    //        get_initial_solution_by_heuristic(0);//可行解
    //        get_initial_solution_by_heuristic(-10);//松弛10分钟
    //        int iteration_time = 30;

    //        for (int iter = 0; iter < iteration_time; iter++)
    //        {
    //            //检查冲突，行生成


    //            //求解模型
    //            benderssubmodel.Optimize();
                
    //            //得到对偶变量

    //            //最短路算法
    //        }           
    //    }
    //    //在新产生的时刻表的前提下检查冲突，添加冲突约束
    //    public void checkconflictandrowgeneration()
    //    {
    //        for (int stock_index = 0; stock_index < stock_num; stock_index++)
    //        {
    //            for (int path_index = 0; path_index < stock_path_list[stock_index].Count(); path_index++)
    //            {
    //                Path path=stock_path_list[stock_index][path_index];
    //                for (int list_index = 0; list_index < path.train_index_list.Count()-1; list_index++)
    //                {
    //                    Train train_1=g.trainlist[path.train_index_list[list_index]];
    //                    Train train_2=g.trainlist[path.train_index_list[list_index+1]];
    //                }
    //            }           
    //        }
    //    }
    //    public void columngeneration()
    //    {
    //        //
    //    }
    //    //添加初始列,用贪婪算法
    //    public void get_initial_solution_by_heuristic(int slack_turn_around_time)
    //    {
    //        int[] train_has_assignment_stock = new int[train_num];
    //        int stock_index = 0;
    //        for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
    //        {
    //            if(train_has_assignment_stock[train_index_1]==1)
    //            {
    //                continue;
    //            }
    //            Path path = new Path();
    //            path.index = stock_path_list[stock_index].Count();
    //            //path.index = 0;
    //            train_has_assignment_stock[train_index_1] = 1;
    //            path.train_index_list.Add(train_index_1);
    //            Train train_1 = g.trainlist[train_index_1];
    //            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
    //            {
    //                if(train_has_assignment_stock[train_index_2]==1)
    //                {
    //                    continue;
    //                }
    //                if (path.train_index_list.Count() >= max_train_for_one_stock)
    //                {
    //                    break;
    //                }
    //                Train train_2 = g.trainlist[train_index_2];
    //                if(train_2.direction!=train_1.direction)
    //                {
    //                    if (g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name) -
    //                    g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name) >= connection_time+slack_turn_around_time)
    //                    {
    //                        path.train_index_list.Add(train_index_2);
    //                        train_has_assignment_stock[train_index_2] = 1;
    //                    }
    //                }
    //            }
    //            addColumn(path,stock_index);
    //            stock_path_list[stock_index].Add(path);
    //            stock_index++;
    //        }
    //    }

    //    //添加列
    //    public void addColumn(Path path,int stock_index)
    //    {
    //        GRBColumn column = new GRBColumn();
    //        column.AddTerm(1,get_constrains_stcok_less_one_route(stock_index));

    //        for (int index = 0; index < path.get_train_count(); index++)
    //        {
    //            column.AddTerm(1,get_constrains_one_train_one_stock(
    //                path.get_train_index_by_list_index(index)));
    //        }
    //        benderssubmodel.AddVar(0,1,path.cost,GRB.CONTINUOUS,column,"f_"+path.index+"_"+stock_index);

    //        benderssubmodel.Update();
    //    }
    //    //根据索引得到约束条件
    //    public GRBConstr get_constrains_stcok_less_one_route(int stock_index)
    //    {
    //        return benderssubmodel.GetConstrByName("each_stock_less_one_routing_" + stock_index);
    //    }
    //    public GRBConstr get_constrains_one_train_one_stock(int train_index)
    //    {
    //        return benderssubmodel.GetConstrByName("one_train_one_stock_" + train_index);
    //    }
    //}
    //public class Path
    //{
    //    public int cost;
    //    public int index;
    //    //public int stock_index;
    //    public List<int> train_index_list;
    //    public Path()
    //    {
    //        train_index_list = new List<int>();
    //        cost = 100;
    //    }
    //    public int get_train_count()
    //    {
    //        return train_index_list.Count();
    //    }
    //    public int get_train_index_by_list_index(int index)
    //    {
    //        return train_index_list[index];
    //    }
    //}
}
