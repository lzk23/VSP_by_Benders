using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using System.IO;

namespace ScheduleSys.Model
{
    public class Model4
    {
        int node_num = 0;//总节点的数目
        GRBEnv env;
        GRBModel model;
        GRBLinExpr objExpr;
        public int optimstatus;

        //全局变量
        Graph g;
        int train_num = 0;
        int M = 1000000;
        int stock_num = 0;
        int connection_time_min = 10;
        int connection_time_max = 60;
        int time_window = 5;

        int depot1start = 0;
        int depot1end = 0;
        int depot2start = 0;
        int depot2end = 0;

        StreamWriter logfile_callback;
        //决策变量
        GRBVar[] arrivetime_train;//到达车站的时间
        GRBVar[] departuretime_train;//离开车站的时间
        GRBVar[] deviate_train;//偏离
        GRBVar[] cancel_train;//取消列车的约束

        GRBVar[,,] connection_node;//两列反向列车是否接续
        GRBVar[,] train_use_stock_flag;

        Dictionary<int, List<int>> depot1outnodedict;
        Dictionary<int, List<int>> depot1innodedict;

        Dictionary<int, List<int>> depot2outnodedict;
        Dictionary<int, List<int>> depot2innodedict;

        public Model4()
        {
            env = new GRBEnv("GurobiSolving.log");
            //env.Set(GRB.IntParam.LazyConstraints,1);
            //env.Set(GRB.IntParam.OutputFlag, 0);
            model = new GRBModel(env);
            //model.Parameters.Heuristics=0;
            //model.Set(GRB.IntParam.LazyConstraints,1);
            objExpr = new GRBLinExpr();
            logfile_callback = new StreamWriter("callback.log");
        }
        public Model4(Graph g)
            : this()
        {
            this.g = g;
            train_num = g.trainlist.Count();
            stock_num = g.stocklist.Count();
            buildnetwork();
            //决策变量的初始化
            arrivetime_train = new GRBVar[train_num];
            departuretime_train = new GRBVar[train_num];
            cancel_train = new GRBVar[train_num];
            deviate_train = new GRBVar[train_num];

            connection_node = new GRBVar[node_num, node_num,2];  
            train_use_stock_flag = new GRBVar[stock_num, train_num];

            for (int train_index = 0; train_index < train_num; train_index++)
            {
                arrivetime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "a[" + ",T" + train_index + "]");
                departuretime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "d[" + ",T" + train_index + "]");
                deviate_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "deviate[T" + train_index + "]");

                cancel_train[train_index] = model.AddVar(0, 1, 0, GRB.BINARY, "m[T" + train_index + "]");
            }
            //for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            //{
            //    Train train = g.trainlist[train_index_1];
            //    if(train.direction==0)
            //    {
            //        connection_node[depot1start, train_index_1, 0] = model.AddVar(0,1,0,
            //            GRB.BINARY, "connection_train[T" + depot1start + ",T" + train_index_1 + ",0]");
            //        connection_node[train_index_1, depot2end, 1] = model.AddVar(0, 1, 0,
            //            GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + depot2end + ",1]");
            //    }
            //    else
            //    {
            //        connection_node[depot2start, train_index_1, 0] = model.AddVar(0, 1, 0,
            //            GRB.BINARY, "connection_train[T" + depot2start + ",T" + train_index_1 + ",1]");
            //        connection_node[train_index_1, depot1end, 1] = model.AddVar(0, 1, 0,
            //            GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + depot1end + ",0]");
            //    }
            //    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //    {
            //        if (train_index_1 != train_index_2)
            //        {
            //            connection_node[train_index_1, train_index_2,0] = model.AddVar(0, 1, 0,
            //                GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + ",0]");
            //            connection_node[train_index_1, train_index_2, 1] = model.AddVar(0, 1, 0,
            //                GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + ",1]");
            //        }
            //    }
            //}
            for (int node_index_1 = 0; node_index_1 < node_num; node_index_1++)
            {
                for (int node_index_2 = 0; node_index_2 < node_num; node_index_2++)
                {
                    if (node_index_1 != node_index_2)
                    {
                        connection_node[node_index_1, node_index_2, 0] = model.AddVar(0, 1, 0,
                            GRB.BINARY, "connection_train[T" + node_index_1 + ",T" + node_index_2 + ",0]");
                        connection_node[node_index_1, node_index_2, 1] = model.AddVar(0, 1, 0,
                            GRB.BINARY, "connection_train[T" + node_index_1 + ",T" + node_index_2 + ",1]");
                    }
                }
            }

            for (int stock_index = 0; stock_index < stock_num; stock_index++)
            {
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    train_use_stock_flag[stock_index, train_index] = model.AddVar(0, 1, 0,
                        GRB.BINARY, "x[R" + stock_index + ",T" + train_index + "]");
                }            
            }
            
        }
        //构建模型
        public void BuildAndSovleModel()
        {
            model.Update();
            //目标函数
            foreach (Train train in g.trainlist)
            {
                objExpr += deviate_train[train.index];
                if(train.direction==0)
                {
                    objExpr += connection_node[depot1start, train.index, 0]*100;
                }
                else
                {
                    objExpr += connection_node[depot2start, train.index, 1]*100;
                }
                objExpr += cancel_train[train.index] * 200;
            }
            model.SetObjective(objExpr, 1);
            //model.AddConstr(objExpr<=1800,"UPPER_BOUND_FOR_OBJ");
            foreach (Train train in g.trainlist)
            {
                model.AddConstr(deviate_train[train.index] >= (departuretime_train[train.index] -
                   g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name)), "deviate_1");
                model.AddConstr(deviate_train[train.index] >= (g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name) -
                    departuretime_train[train.index]), "deviate_2");

                model.AddConstr(deviate_train[train.index] <= time_window, "max_deviate");
            }

            //运行时间约束
            foreach (Train train in g.trainlist)
            {
                model.AddConstr(arrivetime_train[train.index] - departuretime_train[train.index] == g.GetArriveTimeByTrainStation
                    (train.name, train.end_station_name) - g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name), "runtime" + train.name);
            }

            //同向列车追踪间隔
            for (int train_index_1 = 0; train_index_1 < train_num-1; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                Train train_2 = g.trainlist[train_index_1+1];
                if (train_1.direction == train_2.direction)
                {
                    model.AddConstr(departuretime_train[train_index_1+1] - departuretime_train[train_index_1] >= g.departuredeparturehead,
                        "departure_order_1" + train_index_1 + "," + train_index_1+1);
                    model.AddConstr(arrivetime_train[train_index_1+1] - arrivetime_train[train_index_1] >= g.arrivearrivehead,
                        "arrive_order_2" + train_index_1 + "," + train_index_1+1);
                }
            }

            //回动车段约束,depot1
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                GRBLinExpr expr = new GRBLinExpr();
                GRBLinExpr expr_1 = new GRBLinExpr();
                foreach(int value in depot1innodedict[train_index])
                {
                    expr.AddTerm(1,connection_node[value,train_index,0]);
                }
                foreach(int value in depot1outnodedict[train_index])
                {
                    expr_1.AddTerm(1, connection_node[train_index, value, 0]);
                }
                model.AddConstr(expr-expr_1==0,"depot1_balance_constraints");
            }
            //回动车段约束，depot2
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                GRBLinExpr expr = new GRBLinExpr();
                GRBLinExpr expr_1 = new GRBLinExpr();
                foreach (int value in depot2innodedict[train_index])
                {
                    expr.AddTerm(1, connection_node[value, train_index, 1]);
                }
                foreach (int value in depot2outnodedict[train_index])
                {
                    expr_1.AddTerm(1, connection_node[train_index, value, 1]);
                }
                model.AddConstr(expr - expr_1 == 0, "depot2_balance_constraints");
            }

            //如果列车未取消，则有且由一车底担当
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                GRBLinExpr expr = new GRBLinExpr();
                foreach(int value in depot1outnodedict[train_index])
                {
                    expr.AddTerm(1, connection_node[train_index, value, 0]);
                }
                foreach(int value in depot2outnodedict[train_index])
                {
                    expr.AddTerm(1, connection_node[train_index, value, 1]);
                }
                model.AddConstr(expr == 1 - cancel_train[train_index], "vehicle_cover_each_train");
            }
            //最小接续时间和最大接续时间约束
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    Train train_2 = g.trainlist[train_index_2];
                    if (train_1.direction != train_2.direction)
                    {
                        model.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] >=
                            connection_time_min - (1 - connection_node[train_index_1, train_index_2,0]-
                            connection_node[train_index_1,train_index_2,1]) * M,
                            "connection_train_MinT" + train_index_1 + "_T" + train_index_2);

                        model.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] <=
                           connection_time_max + (1 - connection_node[train_index_1, train_index_2,0]-
                           connection_node[train_index_1,train_index_2,1]) * M,
                           "connection_train_MaxT" + train_index_1 + "_T" + train_index_2);
                    }
                }
            }

            //紧邻接续的辆列车采用相同的动车组
            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
            //{
            //    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            //    {
            //        Train train_1 = g.trainlist[train_index_1];
            //        for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //        {
            //            Train train_2 = g.trainlist[train_index_2];
            //            if (train_1.direction != train_2.direction)
            //            {
            //                model.AddConstr(train_use_stock_flag[stock_index, train_index_1] >= train_use_stock_flag[stock_index, train_index_2] -
            //                (1 - connection_node[train_index_1, train_index_2, 0]) * M, "connection_train_use_same_stock_1");
            //                model.AddConstr(train_use_stock_flag[stock_index, train_index_1] <= train_use_stock_flag[stock_index, train_index_2] +
            //                    (1 - connection_node[train_index_1, train_index_2, 0]) * M, "connection_train_use_same_stock_2");

            //                model.AddConstr(train_use_stock_flag[stock_index, train_index_1] >= train_use_stock_flag[stock_index, train_index_2] -
            //                (1 - connection_node[train_index_1, train_index_2, 1]) * M, "connection_train_use_same_stock_1");
            //                model.AddConstr(train_use_stock_flag[stock_index, train_index_1] <= train_use_stock_flag[stock_index, train_index_2] +
            //                    (1 - connection_node[train_index_1, train_index_2, 1]) * M, "connection_train_use_same_stock_2");
            //            }
            //        }
            //    }
            //}
            ////每一列车使用一个车底
            //for (int train_index = 0; train_index < train_num; train_index++)
            //{
            //    GRBLinExpr expr = new GRBLinExpr();
            //    for (int stock_index = 0; stock_index < stock_num; stock_index++)
            //    {
            //        expr.AddTerm(1, train_use_stock_flag[stock_index, train_index]);
            //    }
            //    model.AddConstr(expr, GRB.EQUAL, 1 - cancel_train[train_index], "each_train_each_stock_T" + train_index);
            //}
            ////动车组担当列车数量的约束
            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
            //{
            //    GRBLinExpr expr = new GRBLinExpr();
            //    for (int train_index = 0; train_index < train_num; train_index++)
            //    {
            //        expr.AddTerm(1, train_use_stock_flag[stock_index, train_index]);
            //    }
            //    model.AddConstr(expr, GRB.LESS_EQUAL, DataReadWrite.max_train_for_one_stock, "most_train_for_one_stock");
            //}
            ////动车组交路的第一列车采用的动车组不相同
            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
            //{
            //    for (int train_index_1 = 0; train_index_1 < train_num - 1; train_index_1++)
            //    {
            //        for (int train_index_2 = train_index_1 + 1; train_index_2 < train_num; train_index_2++)
            //        {
            //            GRBLinExpr expr_1 = new GRBLinExpr();
            //            GRBLinExpr expr_2 = new GRBLinExpr();
            //            //Train train_1 = g.trainlist[train_index_1];
            //            //Train train_2 = g.trainlist[train_index_2];
            //            //int startnodefortrain_1 = train_1.direction == 0 ? depot1start : depot2start;
            //            //int startnodefortrain_2 = train_2.direction==0?depot1start:depot2start;
            //            expr_1.AddTerm(1, connection_node[depot1start, train_index_1, 0]);
            //            expr_1.AddTerm(1, connection_node[depot2start, train_index_1, 1]);

            //            expr_2.AddTerm(1, connection_node[depot1start, train_index_2, 0]);
            //            expr_2.AddTerm(1, connection_node[depot2start, train_index_2, 1]);

            //            model.AddConstr(train_use_stock_flag[stock_index, train_index_1] + train_use_stock_flag[stock_index, train_index_2] <=
            //                3 - expr_1 - expr_2, "not_connection_train_use_diff_stock_1");

            //        }
            //    }
            //}
            model.Optimize();
            optimstatus = model.Get(GRB.IntAttr.Status);
        }
        public void SolveResult(Graph new_g)
        {
            if (optimstatus == GRB.Status.OPTIMAL)
            {
                model.Write("model.lp");
                new_g = new Graph(g);
                DataReadWrite.SetParemeter(new_g);//设置车头间距，检查冲突的时候有用
                foreach (Train train in g.trainlist)
                {
                    KeyValuePair<string, string> key_1 = new KeyValuePair<string, string>(train.name, train.begin_station_name);
                    KeyValuePair<string, string> key_2 = new KeyValuePair<string, string>(train.name, train.end_station_name);
                    new_g.trainstationtoarrivetime.Add(key_2, Convert.ToInt32(arrivetime_train[train.index].Get(GRB.DoubleAttr.X)));
                    new_g.trainstationtodeparturetime.Add(key_1, Convert.ToInt32(departuretime_train[train.index].Get(GRB.DoubleAttr.X)));
                }
                //得到动车组使用信息
                int stock_index = 0;
                foreach (Train train in g.trainlist)
                {
                    int startnodefortrain = train.direction == 0 ? depot1start : depot2start;
                    int depot = train.direction == 0 ? 0 : 1;
                    if (Convert.ToInt32(connection_node[startnodefortrain, train.index, depot].X) == 0)
                    {
                        //train.stock_index = stock_index;
                        continue;
                    }
                    int current_index = train.index;
                    train.stock_index = stock_index;
                    while (true)
                    {
                        int temp_value_1 = 0;
                        int temp_value_2 = 0;

                        foreach (int value in depot1outnodedict[current_index])
                        {
                            if (Convert.ToInt32(connection_node[current_index, value, 0].X) == 1)
                            {
                                if (value != depot1end && value != depot2end)
                                {
                                    g.trainlist[value].stock_index = stock_index;
                                }
                                current_index = value;
                                temp_value_1 = 1;
                                break;
                            }
                        }
                        if (temp_value_1 == 0)
                        {
                            foreach (int value in depot2outnodedict[current_index])
                            {
                                if (Convert.ToInt32(connection_node[current_index, value, 1].X) == 1)
                                {
                                    if (value != depot1end && value != depot2end)
                                    {
                                        g.trainlist[value].stock_index = stock_index;
                                    }
                                    current_index = value;
                                    temp_value_2 = 1;
                                    break;
                                }
                            }
                        }

                        if ((temp_value_1 == 1 && temp_value_2 == 1) ||
                            (temp_value_1 == 0 && temp_value_2 == 0 && Convert.ToInt32(cancel_train[current_index].X) == 0))
                        {
                            throw new Exception("Error.");
                        }

                        if (current_index == depot1end || current_index == depot2end || Convert.ToInt32(cancel_train[current_index].X) == 1)
                        {
                            stock_index++;
                            break;
                        }
                    }

                }

                //for (int train_index = 0; train_index < train_num; train_index++)
                //{
                //    bool has_flag = false;
                //    for (int stock_index = 0; stock_index < stock_num; stock_index++)
                //    {
                //        if (train_use_stock_flag[stock_index, train_index].Get(GRB.DoubleAttr.X) >= 0.5)
                //        {
                //            Train train = new_g.trainlist[train_index];
                //            if (train.stock_index != -1)
                //            {
                //                throw new Exception("One train use two stock.");
                //            }
                //            train.stock_index = stock_index;
                //            has_flag = true;
                //        }
                //    }
                //    //if (!has_flag)
                //    //{
                //    //    throw new Exception("The train " + train_index + " has no stock.");
                //    //}
                //}
                model.Write("out.sol");
                model.Dispose();//释放资源以便后面读取文件
                env.Dispose();
                //调整后运行图显示
                PaintClass.PaintStyle paintstyle = new PaintClass.PaintStyle(true, false, false, false, false, false, null);
                DataShow.NewTraindiagram newtriandiagram = new DataShow.NewTraindiagram(g, new_g, paintstyle);
                PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(newtriandiagram.pictureBox_newtraindiagram,
                   g, new_g, paintstyle);
                newtriandiagram.Show();

            }
            else if (optimstatus == GRB.Status.INFEASIBLE)
            {
                DataShow.SolvingResult resultform = new DataShow.SolvingResult();
                resultform.textBox_result.AppendText("The model is infeasible.Please check follow constrains and variables:");
                model.ComputeIIS();
                model.Write("model.ilp");
                resultform.textBox_result.AppendText(System.IO.File.ReadAllText("model.ilp", Encoding.Default));//读取文本信息到文本控件中
                resultform.Show();
                //logfile_callback.Close();
                model.Dispose();
                env.Dispose();
            }
            else
            {
                throw new Exception("Error");
            }                      
        }
        //构建网络
        public void buildnetwork()
        {
            node_num = train_num + 4;//train index 0,...,x-1,(x,x+1,x+2,x+3),x,x+1 对应方向为0，x为起点，x+1为终点
            depot1start = train_num;
            depot1end = train_num + 1;
            depot2start = train_num + 2;
            depot2end = train_num + 3;

            depot1innodedict = new Dictionary<int, List<int>>();
            depot1outnodedict = new Dictionary<int, List<int>>();
            depot2innodedict = new Dictionary<int, List<int>>();
            depot2outnodedict = new Dictionary<int, List<int>>();
            foreach(Train train_1 in g.trainlist)
            {
                List<int> depot1innodelist = new List<int>();
                List<int> depot1outnodelist = new List<int>();
                List<int> depot2innodelist = new List<int>();
                List<int> depot2outnodelist = new List<int>();

                if(train_1.direction==0)
                {
                    depot1innodelist.Add(depot1start);
                    depot2outnodelist.Add(depot2end);
                    depot1innodedict.Add(train_1.index,depot1innodelist);
                    depot1outnodedict.Add(train_1.index, depot1outnodelist);

                    depot2innodedict.Add(train_1.index, depot2innodelist);
                    depot2outnodedict.Add(train_1.index, depot2outnodelist);
                }
                if(train_1.direction==1)
                {
                    depot2innodelist.Add(depot2start);
                    depot1outnodelist.Add(depot1end);
                    depot1innodedict.Add(train_1.index,depot1innodelist);
                    depot1outnodedict.Add(train_1.index, depot1outnodelist);

                    depot2innodedict.Add(train_1.index, depot2innodelist);
                    depot2outnodedict.Add(train_1.index, depot2outnodelist);
                }
                foreach(Train train_2 in g.trainlist)
                {
                    if (train_1.direction != train_2.direction)
                    {
                        int departure_time = g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name);
                        int arrive_time = g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name);
                        if (departure_time - arrive_time <= connection_time_max && departure_time - arrive_time >= connection_time_min - time_window*2)
                        {
                            depot1outnodedict[train_1.index].Add(train_2.index);
                            depot2outnodedict[train_1.index].Add(train_2.index);
                        }
                        arrive_time = g.GetArriveTimeByTrainStation(train_2.name,train_2.end_station_name);
                        departure_time = g.GetDepartureTimeByTrainStation(train_1.name, train_1.begin_station_name);
                        if (departure_time - arrive_time <= connection_time_max && departure_time - arrive_time >= connection_time_min - time_window * 2)
                        {
                            depot1innodedict[train_1.index].Add(train_2.index);
                            depot2innodedict[train_1.index].Add(train_2.index);
                        }
                    }
                }
            }
            //for (int train_index = 0; train_index < train_num; train_index++)
            //{
            //    cancel_train[train_index].Set(GRB.DoubleAttr.LB, 0);
            //    cancel_train[train_index].Set(GRB.DoubleAttr.UB, 0);
            //}
        }
    }
}
