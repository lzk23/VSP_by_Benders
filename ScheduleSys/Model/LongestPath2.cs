using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Model
{
    public class LongestPath2
    {
        GRBEnv env;
        public GRBModel model;
        GRBLinExpr objExpr;
        public int optimstatus;

        //全局变量
        Graph g;
        int train_num = 0;
        int M = 1000000;
        int stock_num = 0;
        int connection_time = 5;
        int[] profit;
        int[,] connect_profit;
        //决策变量

        GRBVar[,] connection_train;//两列反向列车是否接续

        GRBVar[] train_use_stock_flag;
        GRBVar[] stock_routing_first_train;
        GRBVar[] stock_routing_last_train;

        public LongestPath2()
        {
            env = new GRBEnv("GurobiSolving.log");
            //env.Set(GRB.IntParam.OutputFlag, 0);
            model = new GRBModel(env);
            objExpr = new GRBLinExpr();
        }
        public LongestPath2(Graph g)
            : this()
        {
            this.g = g;


            train_num = g.trainlist.Count();
            stock_num = g.stocklist.Count();
            connect_profit = new int[train_num, train_num];
            profit = new int[train_num];
            //决策变量的初始化

            connection_train = new GRBVar[train_num, train_num];

            stock_routing_first_train = new GRBVar[train_num];
            stock_routing_last_train = new GRBVar[train_num];

            train_use_stock_flag = new GRBVar[train_num];

            for (int train_index = 0; train_index < train_num; train_index++)
            {
                stock_routing_first_train[train_index] = model.AddVar(0, 1, 0,
                   GRB.BINARY, "b[R" + train_index + "]");
                stock_routing_last_train[train_index] = model.AddVar(0, 1, 0,
                    GRB.BINARY, "e[R" + train_index + "]");

                train_use_stock_flag[train_index] = model.AddVar(0, 1, 0,
                        GRB.BINARY, "x[T" + train_index + "]");

                profit[train_index] = train_index + 1;
            }
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 != train_index_2)
                    {
                        connection_train[train_index_1, train_index_2] = model.AddVar(0, 1, 0,
                            GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + "]");
                    }

                }
            }
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    Train train_2 = g.trainlist[train_index_2];
                    if (train_1.direction != train_2.direction &&
                        g.GetDepartureTimeByTrainStation(train_1.name, train_1.begin_station_name) -
                    g.GetArriveTimeByTrainStation(train_2.name, train_2.end_station_name) >= connection_time)
                    {
                        connect_profit[train_index_1, train_index_2] = profit[train_index_1] + profit[train_index_2];

                    }
                    else
                    {
                        connect_profit[train_index_1, train_index_2] = -1000;
                    }
                }
            }          
        }
        //给变量限制一定的值，假设所有同方向的列车没有区别，那么谁先谁后就无所谓了

        public void BuildAndSovleModel()
        {
            model.Update();
            //
            //目标函数
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 != train_index_2)
                        objExpr += connect_profit[train_index_1, train_index_2] * connection_train[train_index_1, train_index_2];
                }
            }
            //stock_routing_first_train[0].Set(GRB.);
            //model.SetObjective(objExpr, GRB.MAXIMIZE);
            model.Set(GRB.IntAttr.ModelSense, GRB.MAXIMIZE);

            GRBLinExpr expr;
            GRBLinExpr expr_1;
            //紧邻接续的列车至多只有一列
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                expr = new GRBLinExpr();
                expr_1 = new GRBLinExpr();
                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    Train train_2 = g.trainlist[train_index_2];
                    if (train_1.direction != train_2.direction)
                    {
                        expr.AddTerm(1, connection_train[train_index_1, train_index_2]);
                        expr_1.AddTerm(1, connection_train[train_index_2, train_index_1]);
                    }
                }
                expr.AddTerm(1, stock_routing_last_train[train_index_1]);
                expr_1.AddTerm(1, stock_routing_first_train[train_index_1]);
                model.AddConstr(expr, GRB.EQUAL, train_use_stock_flag[train_index_1], "most_one_stock_for_connection_1");
                model.AddConstr(expr_1, GRB.EQUAL, train_use_stock_flag[train_index_1], "most_one_stock_for_connection_2");
            }


            //动车组担当列车数量的约束

            expr = new GRBLinExpr();
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                expr.AddTerm(1, train_use_stock_flag[train_index]);
            }
            model.AddConstr(expr, GRB.LESS_EQUAL, 8, "most_train_for_one_stock");

            expr = new GRBLinExpr();
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                expr.AddTerm(1, stock_routing_first_train[train_index]);
            }
            model.AddConstr(expr, GRB.EQUAL, 1, "routing_first_train_only_one");
            //回到起始动车段的约束:第一种约束表达方式
            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
            //{
            //    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            //    {
            //        Train train_1 = g.trainlist[train_index_1];
            //        for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //        {
            //            Train train_2 = g.trainlist[train_index_2];
            //            if (train_1.direction == train_2.direction)
            //            {
            //                model.AddConstr(stock_routing_first_train[train_index_1] + stock_routing_last_train[train_index_2] <=
            //                3 - train_use_stock_flag[stock_index, train_index_1] - train_use_stock_flag[stock_index, train_index_2]
            //                     , "return_to_origin_depot");

            //            }
            //        }
            //    }
            //}
            //回到起始动车段的约束:第二种约束表达方式

            expr = new GRBLinExpr();
            expr_1 = new GRBLinExpr();
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                if (train_1.direction == 0)
                {
                    expr.AddTerm(1, train_use_stock_flag[train_index_1]);
                }
                else
                {
                    expr_1.AddTerm(1, train_use_stock_flag[train_index_1]);
                }
            }
            model.AddConstr(expr == expr_1, "reture_to_origin_depot");

        }

        //得到下一个接续的列车
        public int get_next_connection_train(Train train)
        {
            int result = -1;
            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            {
                Train train_2 = g.trainlist[train_index_2];
                if (train_2.direction != train.direction)
                {
                    if (connection_train[train.index, train_index_2].Get(GRB.DoubleAttr.X) > 0.5)
                    {
                        return train_2.index;
                    }
                }
            }
            return result;
        }
        //返回一条动车组交路
        public void SolveResult()
        {
            model.Optimize();
            optimstatus = model.Get(GRB.IntAttr.Status);
            if (optimstatus == GRB.Status.OPTIMAL)
            {
                Algorithm.Path path = new Algorithm.Path();
                //得到动车组使用信息
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    bool has_flag = false;

                    if (train_use_stock_flag[train_index].Get(GRB.DoubleAttr.X) >= 0.5)
                    {
                        Train train = g.trainlist[train_index];
                        if (train.stock_index != -1)
                        {
                            throw new Exception("One train use two stock.");
                        }
                        path.train_index_list.Add(train_index);
                        has_flag = true;
                    }

                    //if (!has_flag)
                    //{
                    //    throw new Exception("The train " + train_index + " has no stock.");
                    //}
                }
                //model.Get(GRB.DoubleAttr.Obj);
                //model.Write("out.sol");
                model.Dispose();//释放资源以便后面读取文件
                env.Dispose();
                //调整后运行图显示
                PaintClass.PaintStyle paintstyle = new PaintClass.PaintStyle(false, false, false, false, false,false, path);
                DataShow.NewTraindiagram newtriandiagram = new DataShow.NewTraindiagram();
                PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(newtriandiagram.pictureBox_newtraindiagram,
                    g, paintstyle);
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
                model.Dispose();
                env.Dispose();
            }
            else
            {
                throw new Exception("Error");
            }
        }
    }
}
