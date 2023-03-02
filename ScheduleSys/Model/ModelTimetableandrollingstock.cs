﻿using Gurobi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Model
{
    class ModelTimetableandrollingstock
    {
        GRBEnv env;
        GRBModel model;
        GRBLinExpr objExpr;
        public int optimstatus;

        StreamWriter logfile_callback;

        //全局变量
        Graph g;
        int train_num = 0;
        int M = 1000000;
        int stock_num = 0;
        int connection_time = 5;
      
        //决策变量
        GRBVar[] arrivetime_train;//到达车站的时间
        GRBVar[] departuretime_train;//离开车站的时间
        GRBVar[] deviate_train;//偏离

        GRBVar[,] departure_order;//列车从车站离开的顺序
        GRBVar[,] arrive_order;//列车到达车站的顺序
        GRBVar[,] departure_arrive_order;

        GRBVar[,] train_use_stock_flag;
        GRBVar[] stock_use_flag;//车底是否被使用

        public ModelTimetableandrollingstock()
        {
            env = new GRBEnv("GurobiSolving.log");
            //env.Set(GRB.IntParam.OutputFlag, 0);
            model = new GRBModel(env);
            objExpr = new GRBLinExpr();
        }
        public ModelTimetableandrollingstock(Graph g):this()
        {
            this.g = g;

            train_num = g.trainlist.Count();
            stock_num = g.stocklist.Count();
            
            //决策变量的初始化
            arrivetime_train = new GRBVar[train_num];
            departuretime_train = new GRBVar[train_num];
            deviate_train = new GRBVar[train_num];
            departure_order = new GRBVar[train_num, train_num];
            arrive_order = new GRBVar[train_num, train_num];
            departure_arrive_order=new GRBVar[train_num,train_num];

            stock_use_flag = new GRBVar[stock_num];
            train_use_stock_flag = new GRBVar[stock_num, train_num];
          
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                arrivetime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "a[" + ",T" + train_index + "]");
                departuretime_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "d[" + ",T" + train_index + "]");
                deviate_train[train_index] = model.AddVar(0, 1440, 0, GRB.INTEGER, "deviate[T" + train_index + "]");
            }
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 != train_index_2)
                    {
                        departure_order[train_index_1, train_index_2] = model.AddVar(0, 1, 0,
                            GRB.BINARY, "v[" + ",T" + train_index_1 + ",T" + train_index_2 + "]");
                        arrive_order[train_index_1, train_index_2] = model.AddVar(0, 1, 0, GRB.BINARY,
                            "0[" + ",T" + train_index_1 + ",T" + train_index_2 + "]");

                        departure_arrive_order[train_index_1, train_index_2] = model.AddVar(0, 1, 0,
                            GRB.BINARY, "da[T" + train_index_1 + "," + train_index_2);
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

                stock_use_flag[stock_index] = model.AddVar(0,1,0,
                    GRB.BINARY,"u[R"+stock_index+"]");
            }

            initial_decision_value();
        }
        //给变量限制一定的值，假设所有同方向的列车没有区别，那么谁先谁后就无所谓了
        public void initial_decision_value()
        {
            for (int train_index_1 = 0; train_index_1 < train_num - 1; train_index_1++)
            {
                for (int train_index_2 = train_index_1 + 1; train_index_2 < train_num; train_index_2++)
                {
                    //设置起始值
                    //departure_order[train_index_1, train_index_2].Start = 1;
                    departure_order[train_index_1, train_index_2].Set(GRB.DoubleAttr.UB, 0);
                    arrive_order[train_index_1, train_index_2].Set(GRB.DoubleAttr.UB, 0);
                }
            }
        }
        public void BuildAndSovleModel()
        {
            model.Update();
            //目标函数
            foreach (Train train in g.trainlist)
            {
                objExpr += deviate_train[train.index];
            }
            for (int stock_index = 0; stock_index < stock_num; stock_index++)
            {
                objExpr += stock_use_flag[stock_index]*g.stocklist[stock_index].cost;
            }
            model.SetObjective(objExpr, 1);

            foreach(Train train in g.trainlist)
            {
                model.AddConstr(deviate_train[train.index] >= (departuretime_train[train.index] - 
                    g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name)),"deviate_1");
                model.AddConstr(deviate_train[train.index] >= (g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name)-
                    departuretime_train[train.index]), "deviate_2");
            }

            //运行时间约束

            foreach (Train train in g.trainlist)
            {
                model.AddConstr(arrivetime_train[train.index] - departuretime_train[train.index] == g.GetArriveTimeByTrainStation
                    (train.name, train.end_station_name) - g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name), "runtime" + train.name);

            }
            

            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {

                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 == train_index_2)
                    {
                        continue;
                    }
                    Train train_2 = g.trainlist[train_index_2];
                    //同向列车追踪间隔
                    if(train_1.direction==train_2.direction)
                    {
                        model.AddConstr(departuretime_train[train_index_1] -
                       departuretime_train[train_index_2] >= (departure_order[train_index_1, train_index_2] - 1) * M + g.departuredeparturehead,
                       "departure_order_1" + train_index_1 + "," + train_index_2);

                        model.AddConstr(departuretime_train[train_index_2] -
                            departuretime_train[train_index_1] >= -departure_order[train_index_1, train_index_2] * M + g.departuredeparturehead,
                            "departure_order_1" + train_index_1 + "," + train_index_2);


                        model.AddConstr(arrivetime_train[train_index_1] -
                            arrivetime_train[train_index_2] >= (arrive_order[train_index_1, train_index_2] - 1) * M + g.arrivearrivehead,
                            "arrive_order_1" + train_index_1 + "," + train_index_2);

                        model.AddConstr(arrivetime_train[train_index_2] -
                            arrivetime_train[train_index_1] >= -arrive_order[train_index_1, train_index_2] * M + g.arrivearrivehead,
                            "arrive_order_2" + train_index_1 + "," + train_index_2);

                        model.AddConstr(departure_order[train_index_1, train_index_2] == arrive_order[train_index_1, train_index_2], "order_keep_" + train_index_1
                            + "," + train_index_2);
                    }
                    //反向接续
                    else
                    {
                        for (int stock_index = 0; stock_index < stock_num; stock_index++)
                        {
                            model.AddConstr(departuretime_train[train_index_1] - arrivetime_train[train_index_2] >=
                            connection_time - (1 - departure_arrive_order[train_index_1, train_index_2]) * M -
                            (2 - train_use_stock_flag[stock_index, train_index_1] - train_use_stock_flag[stock_index, train_index_2]) * M,
                            "connection_1_R" + stock_index + "_T" + train_index_1 + "_T" + train_index_2);

                            model.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] >=
                            connection_time - departure_arrive_order[train_index_1, train_index_2] * M -
                            (2 - train_use_stock_flag[stock_index, train_index_1] - train_use_stock_flag[stock_index, train_index_2]) * M,
                            "connection_2_R" + stock_index + "_T" + train_index_1 + "_T" + train_index_2);
                        }
                        
                    }
                }
            }
            //
            GRBLinExpr expr;
            //车底是否被使用的标识
            for (int stock_index = 0; stock_index < stock_num; stock_index++)
            {
                expr= new GRBLinExpr();
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    expr.AddTerm(1,train_use_stock_flag[stock_index, train_index]);
                }
                model.AddConstr(expr,GRB.LESS_EQUAL, stock_use_flag[stock_index] * M, "use_R" + stock_index);
            }
            //每一列车使用一个车底
            
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                expr = new GRBLinExpr();
                for (int stock_index = 0; stock_index < stock_num; stock_index++)
                {
                    expr.AddTerm(1,train_use_stock_flag[stock_index, train_index]);
                }
                model.AddConstr(expr,GRB.EQUAL,1, "each_train_each_stock_T" + train_index);
            }
            model.Optimize();
            optimstatus = model.Get(GRB.IntAttr.Status);
        }

        public void SolveResult(Graph new_g)
        {
            if (optimstatus == GRB.Status.OPTIMAL)
            {
                new_g = new Graph(g);
                DataReadWrite.SetParemeter(new_g);//设置车头间距，检查冲突的时候有用
                foreach (Train train in g.trainlist)
                {
                    KeyValuePair<string, string> key_1 = new KeyValuePair<string, string>(train.name, train.begin_station_name);
                    KeyValuePair<string, string> key_2 = new KeyValuePair<string, string>(train.name,train.end_station_name);
                    //if (new_g.trainstationtoarrivetime.ContainsKey(key))
                    //{
                    //    new_g.trainstationtoarrivetime[key] = Convert.ToInt32(arrivetime_train[g.stationnametoindex[station_name],
                    //        train.index].Get(GRB.DoubleAttr.X));
                    //    new_g.trainstationtodeparturetime[key] = Convert.ToInt32(departuretime_train[g.stationnametoindex[station_name],
                    //        train.index].Get(GRB.DoubleAttr.X));
                    //}
                    //else
                    //{
                        new_g.trainstationtoarrivetime.Add(key_2, Convert.ToInt32(arrivetime_train[train.index].Get(GRB.DoubleAttr.X)));
                        new_g.trainstationtodeparturetime.Add(key_1, Convert.ToInt32(departuretime_train[train.index].Get(GRB.DoubleAttr.X)));
                    //}  
                }
                //得到动车组使用信息
                
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    bool has_flag = false;
                    for (int stock_index = 0; stock_index < stock_num; stock_index++)
                    {
                        if (train_use_stock_flag[stock_index, train_index].Get(GRB.DoubleAttr.X) >= 0.5)
                        {            
                            Train train = new_g.trainlist[train_index];
                            if(train.stock_index!=-1)
                            {
                                throw new Exception("One train use two stock.");
                            }
                            train.stock_index = stock_index;
                            has_flag = true;
                        }
                    }
                    if(!has_flag)
                    {
                        throw new Exception("The train " + train_index + " has no stock.");
                    }
                }
                       
                model.Dispose();//释放资源以便后面读取文件
                env.Dispose();
                //调整后运行图显示
                //DataShow.NewTraindiagram newtriandiagram = new DataShow.NewTraindiagram(g, new_g);
                //PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(newtriandiagram.pictureBox_newtraindiagram,
                //    new_g, true, false, true, false, false, false);
                //newtriandiagram.Show();

        
                //调整前后时刻表
                //DataShow.NewTimetable newtimetable = new DataShow.NewTimetable();
                //DataReadWrite.ShowTimetable(new_g, g, newtimetable.dataGridView_timetable);
                //newtimetable.textBox_solvingprocess.Text = (System.IO.File.ReadAllText("GurobiSolving.log", Encoding.Default));
                //newtimetable.Show();
              
            }
            else if (optimstatus == GRB.Status.INFEASIBLE)
            {
                DataShow.SolvingResult resultform = new DataShow.SolvingResult();
                resultform.textBox_result.AppendText("The model is infeasible.Please check follow constrains and variables:");
                model.ComputeIIS();
                model.Write("model.ilp");
                resultform.textBox_result.AppendText(System.IO.File.ReadAllText("model.ilp", Encoding.Default));//读取文本信息到文本控件中
                resultform.Show();
                logfile_callback.Close();
                model.Dispose();
                env.Dispose();
            }
            else
            {
                throw new Exception("Error");
            }
            //if (optimstatus == GRB.Status.INF_OR_UNBD)
            //{
            //    gurobidoubletrackmodel.GetEnv().Set(GRB.IntParam.Presolve, 0);
            //    gurobidoubletrackmodel.Optimize();
            //    optimstatus = gurobidoubletrackmodel.Get(GRB.IntAttr.Status);
            //}                       
        }
    }

}
