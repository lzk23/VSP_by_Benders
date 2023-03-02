using Gurobi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Model
{
    class Model5
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


        StreamWriter logfile_callback;
        //决策变量
        GRBVar[] arrivetime_train;//到达车站的时间
        GRBVar[] departuretime_train;//离开车站的时间
        GRBVar[] deviate_train;//偏离
        GRBVar[] cancel_train;//取消列车的约束

        GRBVar[,,] connection_node;//两列反向列车是否接续
        GRBVar[,] train_order;

        Dictionary<KeyValuePair<int,int>, List<int>> depotoutnodedict;//keyvaluepair<depot_id,train_index>
        Dictionary<KeyValuePair<int,int>, List<int>> depotinnodedict;

        public Model5()
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
        public Model5(Graph g)
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
            train_order = new GRBVar[train_num, train_num];

            connection_node = new GRBVar[node_num, node_num,stock_num];  

            for (int train_index = 0; train_index < train_num; train_index++)
            {
                arrivetime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "a[" + ",T" + train_index + "]");
                departuretime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "d[" + ",T" + train_index + "]");
                deviate_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "deviate[T" + train_index + "]");

                cancel_train[train_index] = model.AddVar(0, 1, 0, GRB.BINARY, "m[T" + train_index + "]");
            }
            for (int stock_index = 0; stock_index < stock_num; stock_index++)
			{
			    for (int node_index_1 = 0; node_index_1 < node_num; node_index_1++)
                {
                    for (int node_index_2 = 0; node_index_2 < node_num; node_index_2++)
                    {
                        if (node_index_1 != node_index_2)
                        {
                            connection_node[node_index_1, node_index_2, stock_index] = model.AddVar(0, 1, 0,
                                GRB.BINARY, "connection_train[T" + node_index_1 + ",T" + node_index_2 + ",S"+stock_index);
                        }
                    }
                 }
			}
            
            foreach(Train train_1 in g.trainlist)
            {
                foreach(Train train_2 in g.trainlist)
                {
                    if(train_1.direction!=train_2.direction)
                    {
                        train_order[train_1.index, train_2.index] = model.AddVar(0,1,0,GRB.BINARY,"order[T"+train_1.index
                            +","+train_2.index+"]");
                    }
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
                objExpr += cancel_train[train.index] * 200;
            }
            foreach(Depot depot in g.depotlist)
            {
                foreach(Stock stock in g.stocklist)
                {
                    foreach(Train train in g.trainlist)
                    {                      
                        objExpr+=stock.cost*connection_node[depot.start_node,train.index,stock.index];
                    }
                }
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
            //不同时到发间隔约束
            for (int train_index_1 = 0; train_index_1 < train_num-1; train_index_1++)
            {
                for(int train_index_2=train_index_1+1;train_index_2<train_num;train_index_2++)
                {
                    Train train_1 = g.trainlist[train_index_1];
                    Train train_2 = g.trainlist[train_index_2];
                    if(train_1.direction!=train_2.direction)
                    {
                        model.AddConstr(departuretime_train[train_1.index] - arrivetime_train[train_2.index] >= g.arrivedeparture - (1 - train_order
                            [train_1.index, train_2.index]) * M, "departure_arrival_order_1");
                        model.AddConstr(departuretime_train[train_2.index] - arrivetime_train[train_1.index] >= g.arrivedeparture - (1 - train_order
                            [train_2.index, train_1.index]) * M, "departure_arrival_order_2");
                    }
                }
            }
            //回动车段约束,depot1
            foreach(Depot depot in g.depotlist)
            {
                foreach(Stock stock in g.stocklist)
                {
                    if(stock.depot==depot.id)
                    {
                        for (int train_index = 0; train_index < train_num; train_index++)
                        {
                             GRBLinExpr expr = new GRBLinExpr();
                             GRBLinExpr expr_1 = new GRBLinExpr();
                             KeyValuePair<int, int> key_1 = new KeyValuePair<int, int>(depot.id,train_index);
                             foreach(int value in depotinnodedict[key_1])
                             {
                                 expr.AddTerm(1, connection_node[value, train_index, stock.index]);
                             }
                             foreach(int value in depotoutnodedict[key_1])
                             {
                                 expr_1.AddTerm(1,connection_node[train_index,value,stock.index]);
                             }
                             model.AddConstr(expr - expr_1 == 0, "depot1_balance_constraints");
                        }
                    }
                }
            }
        
            //如果列车未取消，则有且由一车底担当

            for (int train_index = 0; train_index < train_num; train_index++)
            {
                GRBLinExpr expr = new GRBLinExpr();
                foreach(Depot depot in g.depotlist)
                {
                    KeyValuePair<int, int> key = new KeyValuePair<int, int>(depot.id,train_index);
                    List<int> outnodelist = depotoutnodedict[key];
                    foreach(Stock stock in g.stocklist)
                    {
                        if(stock.depot==depot.id)
                        {
                            foreach(int value in outnodelist)
                            {
                                expr.AddTerm(1,connection_node[train_index,value,stock.index]);
                            }
                        }
                    }
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
                        GRBLinExpr expr = new GRBLinExpr();
                        for (int stock_index = 0; stock_index < stock_num; stock_index++)
                        {
                            expr.AddTerm(1,connection_node[train_index_1,train_index_2,stock_index]);
                        }
                        model.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] >=
                            connection_time_min - (1 - expr) * M, "connection_train_MinT" + train_index_1 + "_T" + train_index_2);

                        model.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] <=
                           connection_time_max + (1 - expr) * M, "connection_train_MaxT" + train_index_1 + "_T" + train_index_2);
                    }
                }
            }

            //动车组交路的第一列车采用的动车组不相同
            //foreach(Depot depot in g.depotlist)
            //{
            //    foreach(Stock stock in g.stocklist)
            //    {
            //        if(stock.depot==depot.id)
            //        {
            //            for (int train_index_1 = 0; train_index_1 < train_num-1; train_index_1++)
            //            {
            //                Train train_1=g.trainlist[train_index_1];
            //                for (int train_index_2 = train_index_1+1; train_index_2 < train_num; train_index_2++)
            //                {
            //                    Train train_2=g.trainlist[train_index_2];
            //                    if(train_1.direction==train_2.direction)
            //                    {
            //                        model.AddConstr(connection_node[depot.start_node,train_1.index,stock.index]+
            //                            connection_node[depot.start_node,train_2.index,stock.index]<=1,"the_first_trip_in_route_uses_different_stock");
            //                    }
            //                }
            //            }
                        
            //        }
            //    }
            //}
            foreach (Depot depot in g.depotlist)
            {
                foreach (Stock stock in g.stocklist)
                {
                    if (stock.depot == depot.id)
                    {
                        GRBLinExpr expr = new GRBLinExpr();
                        foreach(Train train in g.trainlist)
                        {
                            if(train.direction==depot.id)
                            {
                                expr.AddTerm(1,connection_node[depot.start_node,train.index,stock.index]);
                            }
                        }
                        model.AddConstr(expr<=1,"");
                    }
                }
            }
            //担当数量约束
            foreach(Depot depot in g.depotlist)
            {
                foreach(Stock stock in g.stocklist)
                {
                    if(stock.depot==depot.id)
                    {
                        GRBLinExpr expr = new GRBLinExpr();
                        foreach(Train train_1 in g.trainlist)
                        {
                            foreach(Train train_2 in g.trainlist)
                            {
                                if(train_1.direction!=train_2.direction)
                                {
                                    expr.AddTerm(1, connection_node[train_1.index, train_2.index, stock.index]);
                                }
                            }
                        }
                        model.AddConstr(expr<=stock.max_undertake_train_num-1,"max_trips_for_one_stock");
                    }
                }
            }
          
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

                foreach(Depot depot in g.depotlist)
                {
                    foreach (Stock stock in g.stocklist)
                    {
                        if(stock.depot==depot.id)
                        {
                            foreach (Train train_1 in g.trainlist)
                            {
                                if(train_1.direction==depot.id)
                                {
                                    if (connection_node[depot.start_node, train_1.index, stock.index].X > 0.9)
                                    {
                                        train_1.stock_index = stock.index;
                                        Train current_train = train_1;
                                        
                                        while(true)
                                        {
                                            if (connection_node[current_train.index, depot.end_node, stock.index].X > 0.9)
                                            {
                                                break;
                                            }

                                            foreach (Train train_2 in g.trainlist)
                                            {
                                                if (train_2.direction != current_train.direction)
                                                {
                                                    if(connection_node[current_train.index,train_2.index,stock.index].X>0.9)
                                                    {
                                                        train_2.stock_index = stock.index;
                                                        current_train = train_2;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        
                                    }  
                                } 
                           
                            }
                        }                
                    }
                }
                
             

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
            foreach(Depot depot in g.depotlist)
            {
                depot.start_node=train_num+2*depot.id;
                depot.end_node=train_num+2*depot.id+1;
            }

            depotinnodedict = new Dictionary<KeyValuePair<int,int>, List<int>>();
            depotoutnodedict = new Dictionary<KeyValuePair<int,int>, List<int>>();

            foreach(Depot depot in g.depotlist)
            {
                //KeyValuePair<int, int> key_1 = new KeyValuePair<int, int>(depot.id,depot.start_node);
                //KeyValuePair<int, int> key_2 = new KeyValuePair<int, int>(depot.id, depot.end_node);

                foreach (Train train_1 in g.trainlist)
                {
                    List<int> depotinnodelist = new List<int>();
                    List<int> depotoutnodelist = new List<int>();
                    KeyValuePair<int, int> key = new KeyValuePair<int, int>(depot.id,train_1.index);
                    if (train_1.direction == depot.id)
                    {
                        depotinnodelist.Add(depot.start_node);
                        depotinnodedict.Add(key, depotinnodelist);
                        depotoutnodedict.Add(key,depotoutnodelist);
                    }
                    else
                    {
                        depotoutnodelist.Add(depot.end_node);
                        depotoutnodedict.Add(key, depotoutnodelist);
                        depotinnodedict.Add(key,depotinnodelist);
                    }
                    foreach (Train train_2 in g.trainlist)
                    {
                        if (train_1.direction != train_2.direction)
                        {
                            int departure_time = g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name);
                            int arrive_time = g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name);
                            if (departure_time - arrive_time <= connection_time_max && departure_time - arrive_time >= connection_time_min - time_window * 2)
                            {
                                depotoutnodedict[key].Add(train_2.index);
                            }
                            arrive_time = g.GetArriveTimeByTrainStation(train_2.name, train_2.end_station_name);
                            departure_time = g.GetDepartureTimeByTrainStation(train_1.name, train_1.begin_station_name);
                            if (departure_time - arrive_time <= connection_time_max && departure_time - arrive_time >= connection_time_min - time_window * 2)
                            {
                                depotinnodedict[key].Add(train_2.index);
                            }
                        }
                    }
                }
            }
           
        }
    }
}
