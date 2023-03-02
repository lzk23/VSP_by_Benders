using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Algorithm
{
    public class Benders2
    {
        GRBEnv env;
        GRBModel bendersmaster;
        GRBModel benderssubmodel;

        public int optimalstatus;

        //全局变量
        Graph g;
        Graph new_g;//benders主问题所产生的新的图
        int train_num = 0;
        int M = 1000000;
        int stock_num = 0;
        int connection_time_min = 10;
        int connection_time_max = 60;
        int max_train_for_one_stock = 10;
        int column_generation_iteration_time = 3000;//benders子问题中列生成的迭代次数
        int cancel_cost = 200;//取消列车的费用
        int min_use_stock_cost = 0;//使用一列动车组的费用
        StreamWriter each_column_generation_upperbound;//输出每次列生成的求解信息,其实同时生成了两条路径
        DataShow.Message log_visual;
        int main_iter;
        int main_iteration_time = 1000;//主程序迭代次数
        int time_window = 5;//松弛折返时间
        int stock_type_count = 0;
        int stock_type_min_undertake;
        int stock_type_max_undertake;

        Stopwatch sw;

        double timetable_total_deviate;
        double benders_master_objvalue;
        double benders_subproblem_objvalue;
        double bender_upper_bound;
        double bender_lower_bound;

        Model.LongestPath shortest_path_model;

        //决策变量

        GRBVar[] arrivetime_train;//到达车站的时间
        GRBVar[] departuretime_train;//离开车站的时间
        GRBVar[] deviate_train;//偏离
        GRBVar z0;//返回benders割的中间变量

        GRBVar[,] connection_train;//两列反向列车是否接续，此处接续表示的是能否接续，而不是紧邻接续的


        List<Path> stock_path_list;
        int[,] add_turnaround_constrain_flag;
        double[] dual_max_used_stock;
        double[] dual_each_train_one_stock;
        double[,] dual_turnaround_time;

        public Benders2()
        {
            env = new GRBEnv("bendersmater.log");
            env.MIPGap = 0.02;
            //env.Set(GRB.IntParam.OutputFlag, 0);
            bendersmaster = new GRBModel(env);
            env = new GRBEnv("benderssubproblem.log");
            env.Set(GRB.IntParam.Method, GRB.METHOD_DUAL);
            env.Set(GRB.IntParam.Presolve, 0);
            benderssubmodel = new GRBModel(env);
            sw = new Stopwatch();
        }
        public Benders2(Graph g)
            : this()
        {
            this.g = g;
            new_g = new Graph();
            stock_path_list = new List<Path>();

            train_num = g.trainlist.Count();
            stock_num = g.stocklist.Count();
            stock_type_count = g.stock_type_list.Count();
            stock_type_min_undertake = g.stock_type_list.Last()[0];
            stock_type_max_undertake = g.stock_type_list.First()[0];
            if(stock_type_max_undertake<stock_type_min_undertake)
            {
                throw new Exception("Error.");
            }

            add_turnaround_constrain_flag = new int[train_num, train_num];
            min_use_stock_cost = g.stock_cost_dict.Last().Value;
            
            dual_max_used_stock=new double[stock_type_count];
            dual_each_train_one_stock = new double[train_num];
            dual_turnaround_time = new double[train_num, train_num];
            //logfile = new StreamWriter("benders.txt");
            each_column_generation_upperbound = new StreamWriter("benders.txt");
            log_visual = new DataShow.Message();
            //决策变量的初始化
            arrivetime_train = new GRBVar[train_num];
            departuretime_train = new GRBVar[train_num];
            deviate_train = new GRBVar[train_num];

            connection_train = new GRBVar[train_num, train_num];

            z0 = new GRBVar();

            z0 = bendersmaster.AddVar((train_num/max_train_for_one_stock)*100, double.MaxValue, 0, GRB.CONTINUOUS, "z0");
            //z0 = bendersmaster.AddVar(2200, double.MaxValue, 0, GRB.CONTINUOUS, "z0");

            

            for (int train_index = 0; train_index < train_num; train_index++)
            {
                arrivetime_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "a[" + ",T" + train_index + "]");
                departuretime_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "d[" + ",T" + train_index + "]");
                deviate_train[train_index] = bendersmaster.AddVar(0, 1440, 0, GRB.INTEGER, "deviate[T" + train_index + "]");
            }
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 != train_index_2)
                    {
                        connection_train[train_index_1, train_index_2] = bendersmaster.AddVar(0, 1, 0,
                            GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + "]");
                    }

                }
            }
            //初始化求解最长路模型
            shortest_path_model = new Model.LongestPath(g);
            shortest_path_model.BuildModel();
        }
        //主流程
        public void bendersmainprocess()
        {
            //建立主问题
            buildbendersmaster();
            benders_master_objvalue = 0;
            //建立子问题
            buildbenderssubmodel();

            //添加初始列
            //get_initial_solution_by_heuristic(0);//可行解
            //get_initial_solution_by_heuristic(-slack_turn_time);//松弛
            StreamWriter upandlow_bound = new StreamWriter("benders_upandlow_bound.txt");
            log_visual.Show();//求解日志显示
            sw.Start();
            for (main_iter = 1; main_iter <= main_iteration_time; main_iter++)
            {
                outputform("-------------------------------------main" + main_iter + "---------------------------------------------");
                //if (main_iter == 1)
                //{
                //    timetable_total_deviate = benders_master_objvalue;
                //}
                //else
                //{
                //    benders_master_objvalue = bendersmaster.Get(GRB.DoubleAttr.Obj);
                //    timetable_total_deviate = benders_master_objvalue - bendersmaster.GetVarByName("z0").Get(GRB.DoubleAttr.X);//把目标函数扣除zo部分
                //}
                //if (main_iter == 2)
                //{
                //    time_window = 0;
                //    //z0.Set(GRB.DoubleAttr.LB, -double.MaxValue);
                //}
                //主问题求解
                outputform("Solving Master.");

                bendersmaster.Optimize();

                if (bendersmaster.Get(GRB.IntAttr.Status) != GRB.Status.OPTIMAL)
                {
                    throw new Exception("Error.");
                }
                bendersmaster.Write("master.lp");
                generationnewtimetable();
                //子问题求解
                outputform("Solving Subproblem.");
                //检查冲突，行生成
                for (int path_index = 0; path_index < stock_path_list.Count(); path_index++)
                {
                    Path path = stock_path_list[path_index];
                    checkconflictandrowgeneration(path);
                }
                bool add_grobal_path = benderssubmodelsolve(true);
        
                if (add_grobal_path == false)
                {
                    outputform("There is no path adding in benderssubproblem.stop.");
                    //break;                
                }
                //输出dual值
                outputform("Output dual cost.");
                for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                {
                    int[] stock_type = g.stock_type_list[stock_type_index];
                    outputform(get_constrains_mas_used_stcok(stock_type[0]).Get(GRB.StringAttr.ConstrName) + ":" +
                        dual_max_used_stock[stock_type_index]);
                }
                
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    outputform(get_constrains_one_train_one_stock(train_index).Get(GRB.StringAttr.ConstrName) + ":" + dual_each_train_one_stock[train_index]);
                }
                for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
                {
                    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                    {
                        if (add_turnaround_constrain_flag[train_index_1, train_index_2] == 1)
                        {
                            outputform(get_constrains_turnaround_time
                                (train_index_1, train_index_2).Get(GRB.StringAttr.ConstrName) + ":" + dual_turnaround_time[train_index_1, train_index_2]);
                        }
                    }
                }
                //message.ShowDialog();
                //更新上下界值，上界值为目前为止得到的可行解的目标值
                benders_master_objvalue = bendersmaster.Get(GRB.DoubleAttr.ObjVal);
                timetable_total_deviate = benders_master_objvalue - bendersmaster.GetVarByName("z0").Get(GRB.DoubleAttr.X);//把目标函数扣除zo部分
                benders_subproblem_objvalue = benderssubmodel.Get(GRB.DoubleAttr.ObjVal) + timetable_total_deviate;
                bender_upper_bound = benders_subproblem_objvalue;//注意：train_num*cancel_cost项
                bender_lower_bound = benders_master_objvalue;
                outputform("上界值：" + bender_upper_bound);
                outputform("下界值：" + bender_lower_bound);
                upandlow_bound.WriteLine(bender_upper_bound + "," + bender_lower_bound + "," + timetable_total_deviate);

                outputform("时刻表偏移：" + timetable_total_deviate);

                if (bender_upper_bound - bender_lower_bound <= 1)
                {
                    upandlow_bound.Close();
                    break;
                }
                //返回benders cut
                GRBLinExpr expr = new GRBLinExpr();
                for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                {
                    int[] element = g.stock_type_list[stock_type_index];
                    double right_hand_side = get_constrains_mas_used_stcok(element[0]).RHS;
                    expr.AddConstant(dual_max_used_stock[stock_type_index] * right_hand_side);
                }
                
                for (int train_index = 0; train_index < train_num; train_index++)
                {
                    expr.AddConstant(dual_each_train_one_stock[train_index]);
                }
                for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
                {
                    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                    {
                        if (add_turnaround_constrain_flag[train_index_1, train_index_2] == 1)
                        {
                            if (Math.Abs(dual_turnaround_time[train_index_1, train_index_2]) > 0.5)
                            {
                                expr += dual_turnaround_time[train_index_1, train_index_2] * connection_train[train_index_1, train_index_2];
                            }     
                        }
                    }
                }
                GRBConstr constr = bendersmaster.AddConstr(z0 >= expr + cancel_cost * train_num, "cut_" + main_iter);//
                outputform("Add Cut.");
                //break;
                //outputform("After each subproblem solving,we try to use branch and bound to get integer solution.");
                //GRBModel benderssubmodel_temp=new GRBModel(benderssubmodel);
                //List<Path> stock_path_list_temp=new List<Path>(stock_path_list);

                //branchandbound();
                //benderssubmodel=benderssubmodel_temp;
                //stock_path_list=stock_path_list_temp;
            }
            outputform("Benders iteration solving completely, then using branch and bound algorithm to get integer solution.");
            //solving by branch and bound
            //set_integer_and_resolve();
            submodelresult();
            //visual_stock_route(null, false, null);
            //branchandbound();
            each_column_generation_upperbound.Close();
            sw.Stop();
            outputform("Solving Completely.");
            outputform("Total consume time:" + sw.Elapsed);
            //清理
            bendersmaster.Dispose();
            benderssubmodel.Dispose();
            Clear();
        }

        //建立主问题
        public void buildbendersmaster()
        {
            GRBLinExpr objExpr = new GRBLinExpr();
            //目标函数
            foreach (Train train in g.trainlist)
            {
                objExpr += deviate_train[train.index];
            }
            objExpr += z0;
            bendersmaster.SetObjective(objExpr, GRB.MINIMIZE);
            foreach (Train train in g.trainlist)
            {
                bendersmaster.AddConstr(deviate_train[train.index] >= (departuretime_train[train.index] -
                    g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name)), "deviate_1");
                bendersmaster.AddConstr(deviate_train[train.index] >= (g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name) -
                    departuretime_train[train.index]), "deviate_2");
                bendersmaster.AddConstr(deviate_train[train.index] <= 5, "max_deviate");
            }
            //bendersmaster.AddConstr(objExpr - z0 <= 100, "max_deviation");
            //运行时间约束
            foreach (Train train in g.trainlist)
            {
                bendersmaster.AddConstr(arrivetime_train[train.index] - departuretime_train[train.index] == g.GetArriveTimeByTrainStation
                    (train.name, train.end_station_name) - g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name), "runtime" + train.name);
            }
            //同向列车追踪间隔
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {

                Train train_1 = g.trainlist[train_index_1];

                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    if (train_index_1 >= train_index_2)
                    {
                        continue;
                    }
                    Train train_2 = g.trainlist[train_index_2];

                    if (train_1.direction == train_2.direction)
                    {
                        bendersmaster.AddConstr(departuretime_train[train_index_2] - departuretime_train[train_index_1] >= g.departuredeparturehead,
                            "departure_order_1" + train_index_1 + "," + train_index_2);

                        bendersmaster.AddConstr(arrivetime_train[train_index_2] - arrivetime_train[train_index_1] >= g.arrivearrivehead,
                            "arrive_order_2" + train_index_1 + "," + train_index_2);
                    }
                }
            }
            //GRBVar[,] departure_order = new GRBVar[train_num, train_num];
            //GRBVar[,] arrival_order = new GRBVar[train_num, train_num];

            //for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            //{
            //    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //    {
            //        departure_order[train_index_1,train_index_2]= bendersmaster.AddVar(0,1,0,GRB.BINARY,"departure_order_"+train_index_1+"_"+train_index_2);
            //        arrival_order[train_index_1, train_index_2] = bendersmaster.AddVar(0, 1, 0, GRB.BINARY, "arrival_order_" + train_index_1 + "_" + train_index_2);
            //    }
            //}
            //bendersmaster.Update();
            ////同向列车追踪间隔
            //for (int train_index_1 = 0; train_index_1 < train_num-1; train_index_1++)
            //{

            //    Train train_1 = g.trainlist[train_index_1];

            //    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //    {
            //        //if (train_index_1 >= train_index_2)
            //        //{
            //        //    continue;
            //        //}
            //        if (train_index_1 == train_index_2)
            //        {
            //            continue;
            //        }
            //        Train train_2 = g.trainlist[train_index_2];

            //        if (train_1.direction == train_2.direction)
            //        {
            //            bendersmaster.AddConstr(departuretime_train[train_index_2] - departuretime_train[train_index_1] >= g.departuredeparturehead
            //                -(1-departure_order[train_index_1,train_index_2])*M , "departure_order_1" + train_index_1 + "," + train_index_2);
            //            bendersmaster.AddConstr(departuretime_train[train_index_1] - departuretime_train[train_index_2] >= g.departuredeparturehead
            //                - departure_order[train_index_1, train_index_2] * M, "departure_order_1" + train_index_1 + "," + train_index_2);


            //            bendersmaster.AddConstr(arrivetime_train[train_index_2] - arrivetime_train[train_index_1] >= g.arrivearrivehead
            //               -(1-arrival_order[train_index_1,train_index_2])*M ,"arrive_order_2" + train_index_1 + "," + train_index_2);
            //            bendersmaster.AddConstr(arrivetime_train[train_index_1] - arrivetime_train[train_index_2] >= g.arrivearrivehead
            //               - arrival_order[train_index_1, train_index_2] * M, "arrive_order_2" + train_index_1 + "," + train_index_2);

            //            bendersmaster.AddConstr(arrival_order[train_index_1,train_index_2]==departure_order[train_index_1,train_index_2],"keep_order");
            //        }
            //    }
            //}
            //最小接续时间约束和最大接续时间
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    Train train_2 = g.trainlist[train_index_2];
                    if (train_1.direction != train_2.direction)
                    {
                        bendersmaster.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] >=
                            connection_time_min - (1 - connection_train[train_index_1, train_index_2]) * M,
                            "connection_train_T" + train_index_1 + "_T" + train_index_2);

                        bendersmaster.AddConstr(departuretime_train[train_index_2] - arrivetime_train[train_index_1] <=
                          connection_time_max + (1 - connection_train[train_index_1, train_index_2]) * M,
                          "connection_train_MaxT" + train_index_1 + "_T" + train_index_2);
                    }
                }
            }
            //bendersmaster.SetCallback(new BendersCallback());
        }

        //建立子问题
        public void buildbenderssubmodel()
        {
            //目标函数在addcolumn中添加变量时,已添加，此处不需要设置目标函数
            GRBLinExpr expr = new GRBLinExpr();
            expr.AddConstant(cancel_cost * train_num);
            benderssubmodel.SetObjective(expr, GRB.MINIMIZE);
            expr = new GRBLinExpr();
            int total_value = 0;
            for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
            {
                int[] element=g.stock_type_list[stock_type_index];
                total_value += element[1];
                benderssubmodel.AddConstr(expr, GRB.LESS_EQUAL, total_value , "max_used_stock"+element[0]);
            }         
            expr = new GRBLinExpr();
            for (int train_index = 0; train_index < train_num; train_index++)
            {
                benderssubmodel.AddConstr(expr, GRB.LESS_EQUAL, 1, "one_train_one_stock_" + train_index);
            }
            benderssubmodel.Update();
        }
        //benders子问题求解,参数是否有列生成
        public bool benderssubmodelsolve(bool column_generation_flag)
        {
            bool add_grobal_path = false;
            for (int iter = 0; iter < column_generation_iteration_time; iter++)
            {
                outputform("----------sub" + iter + "-------------");
                //求解模型
                benderssubmodel.Optimize();
                optimalstatus = benderssubmodel.Get(GRB.IntAttr.Status);
                if (optimalstatus == GRB.Status.OPTIMAL)
                {
                    outputform("Objvalue=" + benderssubmodel.Get(GRB.DoubleAttr.ObjVal));
                    each_column_generation_upperbound.WriteLine(benderssubmodel.Get(GRB.DoubleAttr.ObjVal));
                    benderssubmodel.Write("out.sol");
                    benderssubmodel.Write("sub.lp");
                    //一旦iter==column_generation_iteration_time-1,虽然后面生成的path.reduce<0,此处不再添加
                    if (iter == column_generation_iteration_time - 1||!column_generation_flag)
                    {
                        break;
                    }
                    //得到对偶变量
                    for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                    {
                        int[] stock_type = g.stock_type_list[stock_type_index];
                        dual_max_used_stock[stock_type_index] = get_constrains_mas_used_stcok(stock_type[0]).Get(GRB.DoubleAttr.Pi);
                    }

                    for (int train_index = 0; train_index < train_num; train_index++)
                    {
                        dual_each_train_one_stock[train_index] = get_constrains_one_train_one_stock(train_index).Get(GRB.DoubleAttr.Pi);
                    }
                    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
                    {
                        for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                        {
                            if (add_turnaround_constrain_flag[train_index_1, train_index_2] == 1)
                            {
                                dual_turnaround_time[train_index_1, train_index_2] = get_constrains_turnaround_time
                                    (train_index_1, train_index_2).Get(GRB.DoubleAttr.Pi);
                            }
                            else
                            {
                                dual_turnaround_time[train_index_1, train_index_2] = 0;
                            }
                        }
                    }
                    //最短路模型
                    //Path path = column_generation_base_on_shortestpath();
                    //outputform("path ruduce cost:" + path.reduce_cost + ",path number:" + stock_path_list.Count());
                    ////检查reduce cost
                    //if (path.reduce_cost >= -1)
                    //{
                    //    break;
                    //}
                    //path.index = stock_path_list.Count();
                    //addColumn(path);
                    ////visual_stock_route(path, true);
                    //stock_path_list.Add(path);
                    //checkconflictandrowgeneration(path);//由于松弛了接续时间，新生成的路径可能无法接续，所以要检查冲突
                    //最短路算法

                    List<Path> pathlist = shortest_path_generation_base_on_algorithm();//参数表示k短路
                    int add_path_flag = 0;
                    foreach (Path path in pathlist)
                    {
                        outputform("path ruduce cost:" + path.reduce_cost + ",path number:" + stock_path_list.Count());
                        //检查reduce cost
                        if (path.reduce_cost >= -1)
                        {
                            continue;
                        }
                        path.index = stock_path_list.Count();
                        addColumn(path);
                        add_path_flag = 1;                    
                        //visual_stock_route(path, true,null);
                        stock_path_list.Add(path);
                        checkconflictandrowgeneration(path);//由于松弛了接续时间，新生成的路径可能无法接续，所以要检查冲突
                        add_grobal_path = true;
                    }
                    if (add_path_flag == 0)
                    {
                        break;
                    }
                }
                else if (optimalstatus == GRB.Status.INFEASIBLE)
                {
                    throw new Exception("infeasible.");

                }
                else
                {
                    throw new Exception("unknow error.");
                }

            }
            return add_grobal_path;
        }

        //分支定界算法
        public void branchandbound()
        {
            //time_window = 0;//再次生成的列不允许存在冲突
            BBTree bbtree = new BBTree();
            BBNode root_node = new BBNode(benderssubmodel, stock_path_list, 1);
            bbtree.add_bbnode(root_node);
            StreamWriter writer = DataReadWrite.CleanAndAppendText("Node_Tree.txt");
            int iter_branch = 0;
            while (bbtree.has_next_bbnode())
            {
                iter_branch++;
                BBNode bbnode = null;
                bbnode = bbtree.get_next_bbnode_base_DF();
                outputform("Solve node " + bbnode.id);
                //outputform("Before solve,there has total column " + bbnode.get_total_column_number());
                //根节点不需要执行以下操作
                if (iter_branch != 1)
                {
                    bbnode.set_fix_value();
                    benderssubmodel = bbnode.get_lp_model();//把节点的模型复制到当前的benders中,以便调用benders类的 benderssubmodelsolve();
                    //如果固定值等于0，则不采用列生成求解
                    bool solve_subproblem_with_column_generation = true;
                    if(bbnode.fix_col.fix_path_value==0)
                    {
                        solve_subproblem_with_column_generation = false;
                    }
                    stock_path_list = bbnode.stock_path_list;
                    benderssubmodelsolve(solve_subproblem_with_column_generation);
                }

                //设置节点解的状态
                bbnode.set_solution_and_path_value_state();

                /*update the pseudocost*/
                //bbnode.update_single_node_pseudocost_after_solve();
                //int solve_by_cg_time = bbtree.pseudocost_with_strong_branch_initial(bbnode);
                //outputform("The strong branch method evaluate " + solve_by_cg_time + " variable(including up and down cut)");

                //bbtree.update_all_pseudocost(bbnode);

                //if (iter_branch == 1)
                //{
                //    bbtree.update_best_int_obj_val(bbnode.get_int_sol_obj_value());
                //    outputform("Initial integer solution obj value is:" + bbnode.get_int_sol_obj_value());
                //}
                //outputform("The iteration " + iter_branch + " column generation time is " + sw.Elapsed);
                //不是整数解
                if (bbnode.get_lp_status() == LP_Status.opt_notint)
                {
                    double gap = (bbtree.get_best_int_obj_val() - bbnode.get_opt_obj_val()) / bbtree.get_best_int_obj_val();

                    if (bbnode.get_opt_obj_val() >= bbtree.get_best_int_obj_val() - 1)
                    {
                        outputform("LP worse than integer incumbent " + bbnode.get_opt_obj_val() +
                            ">=" + bbtree.get_best_int_obj_val() + ". Cut bbnode. ");
                        writer.WriteLine(bbnode.id + " worse bound cut");
                    }
                    else if (bbtree.get_best_int_obj_val() - bbnode.get_opt_obj_val()<min_use_stock_cost-1)
                    {
                        outputform("The node obj value is " + bbnode.get_opt_obj_val() + ",it is not lowwer enough than the best int solution " + bbtree.get_best_int_obj_val() + ",Cut Node");
                        writer.WriteLine(bbnode.id + " gap bound cut");
                    }
                    else
                    {
                        outputform("Current node obj func value = " + bbnode.get_opt_obj_val() + "," + "integer incumbent=" + bbtree.get_best_int_obj_val()
                            + ". Need to branch.");
                        int branch_num = bbtree.branch(bbnode);
                        writer.WriteLine(bbnode.id + " branch " + bbnode.id * 2 + " " + (bbnode.id * 2 + 1));
                        //在需要分支的条件下，求解整数解
                        //outputform("Solving with integer constraint.");
                        //set_integer_and_resolve();
                        //if (benderssubmodel.Status == GRB.Status.OPTIMAL)
                        //{
                        //    //benderssubmodel.Write("out.sol");
                        //    bbnode.opt_obj_val = benderssubmodel.ObjVal;
                        //    bool update_flag=bbtree.update_best_int_obj_val(bbnode);//根据解的状态是否更新上界解
                        //    if(update_flag)
                        //    {
                        //        outputform("Updata upper bound:" + bbtree.get_best_int_obj_val());
                        //    }
                        //}
                        //if(branch_num==1)
                        //{
                        //    writer.WriteLine(bbnode.id + " branch " +(bbnode.id * 2 + 1));
                        //}
                        //else
                        //{
                        //    writer.WriteLine(bbnode.id + " branch " + bbnode.id * 2 + " " + (bbnode.id * 2 + 1));
                        //}

                    }
                }
                //为整数解
                else if (bbnode.get_lp_status() == LP_Status.opt_int)
                {
                    outputform("The node solution is integer, obj func value is " + bbnode.get_opt_obj_val() +
                        ",best int func value is " + bbtree.get_best_int_obj_val());
                    bool updata_upbownd_flag = bbtree.update_best_int_obj_val(bbnode);
                    writer.WriteLine(bbnode.id + " Integer");
                    if (updata_upbownd_flag)
                    {
                        outputform("update upbound.");
                    }
                    else
                    {
                        outputform("don't updata upbound.");
                    }
                }

                else
                {
                    throw new Exception("error.");
                }
                bbtree.remove_last_bbnode();
            }
            //sw_total_solve_time.Stop();
            //outputform("The total solve time is:" + sw_total_solve_time.Elapsed.ToString());
            outputform("Branch and bound solving completely.");
            last_result(bbtree.get_best_path_list());
            writer.Close();
        }
        //在所有求解结束之后，将子问题中的设置为整数变量在求解
        public void set_integer_and_resolve()
        {
            for (int path_index = 0; path_index < stock_path_list.Count(); path_index++)
            {
                get_variable_by_path_index(path_index).Set(GRB.CharAttr.VType, GRB.BINARY);
            }
            benderssubmodel.Optimize();
            benderssubmodel.Write("out.sol");
            //optimalstatus = benderssubmodel.Get(GRB.IntAttr.Status);
            //if (optimalstatus == GRB.Status.OPTIMAL)
            //{

            //    //submodelresult();
            //}
            //else if (optimalstatus == GRB.Status.INFEASIBLE)
            //{
            //    //throw new Exception("infeasible.");

            //}
            //else
            //{
            //    //throw new Exception("unknow error.");
            //}
        }
        //在分支定界算法求解之后，输出结果
        public void last_result(BBNode best_node)
        {
            Path total_path = new Path();
            int stock_index = 0;

            for (int path_index = 0; path_index < best_node.stock_path_value_statu.Length; path_index++)
            {
                if (best_node.stock_path_value_statu[path_index] == PathValueStatus.Interger_one)
                {
                    Path path = best_node.stock_path_list[path_index];
                    foreach (int train_index in path.train_index_list)
                    {
                        Train train = g.trainlist[train_index];
                        train.stock_index = stock_index;
                    }
                    stock_index++;
                }
            }
            visual_stock_route(null, false, null);
            //把动车组的信息清空
            foreach (Train train in g.trainlist)
            {
                train.stock_index = -1;
            }
        }

        //由于benders分解算法可能最终得到的不是一个整数可行解，可能出现时刻表偏移过度的现象，此时在最终求解得到动车组交路的前提下，
        //固定动车组交路，再平移列车运行线
        public void retrim(List<Path> best_path_list)
        {
            outputform("Retrim timetable.\r\n");
            //即固定可以接续的对
            foreach (Path path in best_path_list)
            {
                for (int list_index = 0; list_index < path.train_index_list.Count() - 1; list_index++)
                {
                    int train_index_1 = path.train_index_list[list_index];
                    int train_index_2 = path.train_index_list[list_index + 1];
                    bendersmaster.GetVarByName("connection_train[T" + train_index_1 + ",T" + train_index_2 + "]").Set(GRB.DoubleAttr.UB,1);
                    bendersmaster.GetVarByName("connection_train[T" + train_index_1 + ",T" + train_index_2 + "]").Set(GRB.DoubleAttr.LB, 1);
                }
            }
            bendersmaster.Optimize();
            if (bendersmaster.Get(GRB.IntAttr.Status) == GRB.Status.OPTIMAL)
            {
                double total_save_deviate_time = 0;
                total_save_deviate_time = timetable_total_deviate -
                    (bendersmaster.Get(GRB.DoubleAttr.ObjVal) - bendersmaster.GetVarByName("z0").Get(GRB.DoubleAttr.X));//把目标函数扣除zo部分
                outputform("Total save deviate time:" + total_save_deviate_time);
                generationnewtimetable();
            }
            else
            {
                throw new Exception("Error.");
            }
        }
        public void last_result(List<Path> best_path_list)
        {
            int stock_index = 0;
            foreach (Path path in best_path_list)
            {
                foreach (int train_index in path.train_index_list)
                {
                    Train train = g.trainlist[train_index];
                    train.stock_index = stock_index;
                }
                stock_index++;
            }
            save_result_to_file();
            retrim(best_path_list);
            visual_stock_route(null, false, best_path_list);
            
        }
        //将接续关系等保存到file中
        public void save_result_to_file()
        {
            

        }
        //在子问题求解完成后，检查解是否为整数，若为整数输出结果
        public void submodelresult()
        {
            //Path total_path = new Path();
            //for (int path_index = 0; path_index < stock_path_list.Count(); path_index++)
            //{           
            //    if(get_variable_by_path_index(path_index).Get(GRB.DoubleAttr.X)>0.9)
            //    {
            //        //listA.AddRange(listB);
            //        //List Result = listA.Union(listB).toList();//删除重复项
            //        //List Result = listA.Concat(listB).toList();//保留重复项
            //        Path path = stock_path_list[path_index];
            //        //total_path.train_index_list.Union(path.train_index_list);
            //        total_path.train_index_list.AddRange(path.train_index_list);
            //    }
            //}
            //visual_stock_route(total_path);

            Path total_path = new Path();
            int stock_index = 0;
            for (int path_index = 0; path_index < stock_path_list.Count(); path_index++)
            {
                if (get_variable_by_path_index(path_index).Get(GRB.DoubleAttr.X) > 0.9)
                {
                    Path path = stock_path_list[path_index];
                    foreach (int train_index in path.train_index_list)
                    {
                        Train train = g.trainlist[train_index];
                        train.stock_index = stock_index;
                    }
                    stock_index++;
                }
            }
            visual_stock_route(null, false, null);

        }
        //最短路算法
        public List<Path> shortest_path_generation_base_on_algorithm()
        {
            double[,] connect_profit = new double[train_num, train_num];
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                Train train_1 = g.trainlist[train_index_1];
                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
                {
                    Train train_2 = g.trainlist[train_index_2];
                    //double departure_time = departuretime_train[train_2.index].Get(GRB.DoubleAttr.X);
                    //double arrival_time = arrivetime_train[train_1.index].Get(GRB.DoubleAttr.X);
                    double departure_time = g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name);
                    double arrival_time = g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name);
                    if (train_1.direction != train_2.direction &&
                        departure_time - arrival_time >= connection_time_min - 2*time_window
                        && departure_time - arrival_time <= connection_time_max)
                    {
                        if (add_turnaround_constrain_flag[train_index_1, train_index_2] == 1)
                        {
                            connect_profit[train_index_1, train_index_2] = -dual_turnaround_time[train_index_1, train_index_2];
                        }
                        else
                        {
                            connect_profit[train_index_1, train_index_2] = 0;
                        }
                    }
                    else
                    {
                        connect_profit[train_index_1, train_index_2] = 10000;
                    }
                }
            }
            List<Path> pathlist = new List<Path>();
            //pathlist = stock_shortest_path_2(connect_profit, dual_each_train_one_stock, 1);
            int flag_1 = stock_type_max_undertake + 1;
            int flag_2 = stock_type_max_undertake + 1;

            for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
            {
                int max_used_vertices = g.stock_type_list[stock_type_index][0];
                if (flag_1 > max_used_vertices)
                {
                    Path path_1 = stock_shortest_path_2(connect_profit, dual_each_train_one_stock, 1, max_used_vertices);
                    flag_1 = path_1.get_train_count();
                    pathlist.Add(path_1);
                }
                if (flag_2 > max_used_vertices)
                {
                    Path path_2 = stock_shortest_path_2(connect_profit, dual_each_train_one_stock, 0, max_used_vertices);
                    flag_2 = path_2.get_train_count();
                    pathlist.Add(path_2);
                }
            }       
            return pathlist;
        }
        //最短路模型
        //public Path column_generation_base_on_shortestpath()
        //{
        //    double[,] connect_profit = new double[train_num, train_num];
        //    GRBLinExpr objexpr = new GRBLinExpr();
        //    //把权重更新到目标函数上,由于最短路算法的目标函数与对偶值相关，故在此处设置
        //    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
        //    {
        //        Train train_1 = g.trainlist[train_index_1];
        //        for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
        //        {
        //            Train train_2 = g.trainlist[train_index_2];
        //            double departure_time = departuretime_train[train_2.index].Get(GRB.DoubleAttr.X);
        //            double arrival_time = arrivetime_train[train_1.index].Get(GRB.DoubleAttr.X);
        //            if (train_1.direction != train_2.direction &&
        //                departure_time - arrival_time >= connection_time_min - 2*time_window
        //                && departure_time - arrival_time <= connection_time_max+2*time_window)
        //            {
        //                if (add_turnaround_constrain_flag[train_index_1, train_index_2] == 1)
        //                {
        //                    connect_profit[train_index_1, train_index_2] = dual_turnaround_time[train_index_1, train_index_2];
        //                }
        //                else
        //                {
        //                    connect_profit[train_index_1, train_index_2] = 0;
        //                }
        //                //shortest_path_model.model.SetObjective();
        //            }
        //            else
        //            {
        //                connect_profit[train_index_1, train_index_2] = -10000;
        //            }
        //        }
        //    }
        //    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
        //    {
        //        for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
        //        {
        //            if (train_index_1 != train_index_2)
        //                objexpr += connect_profit[train_index_1, train_index_2] * shortest_path_model.Get_var_by_name("connection_train[T" + train_index_1 +
        //                    ",T" + train_index_2 + "]");
        //        }
        //    }

        //    for (int train_index = 0; train_index < train_num; train_index++)
        //    {
        //        objexpr += (cancel_cost + dual_each_train_one_stock[train_index]) * shortest_path_model.Get_var_by_name("x[T" + train_index + "]");
        //    }
        //    shortest_path_model.model.SetObjective(objexpr, GRB.MAXIMIZE);//重新设置目标函数
        //    Path path = shortest_path_model.SolveResult();
        //    if (path == null)
        //    {
        //        throw new Exception("Path is null.");
        //    }
        //    path.index = stock_path_list.Count();

        //    path.profit = use_stock_cost - path.get_train_count() * cancel_cost;
        //    double objvalue = shortest_path_model.get_obj_value();
        //    path.reduce_cost = use_stock_cost - dual_max_used_stock - objvalue;

        //    return path;
        //}
        //在新产生的时刻表的前提下检查冲突，添加冲突约束
        public void checkconflictandrowgeneration(Path path)
        {
            for (int list_index = 0; list_index < path.train_index_list.Count() - 1; list_index++)
            {
                Train train_1 = g.trainlist[path.train_index_list[list_index]];
                Train train_2 = g.trainlist[path.train_index_list[list_index + 1]];
                //int departure_time = new_g.GetDepartureTimeByTrainStation(train_1.name, train_1.begin_station_name);
                //int arrival_time = new_g.GetArriveTimeByTrainStation(train_2.name, train_2.end_station_name);
                double departure_time = departuretime_train[train_2.index].Get(GRB.DoubleAttr.X);
                double arrival_time = arrivetime_train[train_1.index].Get(GRB.DoubleAttr.X);
                //如果不可行
                if (departure_time - arrival_time < connection_time_min || departure_time - arrival_time > connection_time_max)
                {
                    path.feasible = false;
                    //如果未添加
                    if (add_turnaround_constrain_flag[train_1.index, train_2.index] == 0)
                    {
                        benderssubmodel.AddConstr(get_variable_by_path_index(path.index) <= 0, "turnaroundtime_" + train_1.index + "_" + train_2.index);
                        add_turnaround_constrain_flag[train_1.index, train_2.index] = 1;
                        //注意：
                        benderssubmodel.Update();
                    }
                    //否则直接在相应中添加相应变量
                    else
                    {
                        benderssubmodel.ChgCoeff(get_constrains_turnaround_time(train_1.index, train_2.index),
                            get_variable_by_path_index(path.index), 1);
                    }
                }
                //如果可行
                else
                {
                    path.feasible = true;
                    //如果已添加，需要移除
                    if (add_turnaround_constrain_flag[train_1.index, train_2.index] == 1)
                    {
                        //benderssubmodel.Remove(benderssubmodel.GetConstrByName("turnaroundtime_" + train_1.index + "_" + train_2.index));
                        benderssubmodel.Remove(get_constrains_turnaround_time(train_1.index, train_2.index));
                        add_turnaround_constrain_flag[train_1.index, train_2.index] = 0;
                    }
                    else
                    {
                        //pass
                    }
                }
            }
            //for (int path_index = 0; path_index < stock_path_list.Count(); path_index++)
            //{
            //    Path path = stock_path_list[path_index];
            //    for (int list_index = 0; list_index < path.train_index_list.Count() - 1; list_index++)
            //    {
            //        Train train_1 = g.trainlist[path.train_index_list[list_index]];
            //        Train train_2 = g.trainlist[path.train_index_list[list_index + 1]];
            //        int departure_time = new_g.GetDepartureTimeByTrainStation(train_1.name,train_1.begin_station_name);
            //        int arrival_time = new_g.GetArriveTimeByTrainStation(train_2.name,train_2.end_station_name);
            //        if(departure_time-arrival_time<connection_time)
            //        {
            //            if(add_turnaround_constrain_flag[train_1.index,train_2.index]==0)
            //            {
            //                benderssubmodel.AddConstr(get_variable_by_path_index(path_index)<=0,"turnaroundtime_"+train_1.index+"_"+train_2.index);
            //                add_turnaround_constrain_flag[train_1.index, train_2.index] = 1;
            //            }
            //            //否则直接在相应中添加相应变量
            //            else
            //            {
            //                benderssubmodel.ChgCoeff(get_constrains_turnaround_time(train_1.index,train_2.index),
            //                    get_variable_by_path_index(path_index),1);
            //            }
            //        }
            //    }
            //}
        }

        public Path stock_shortest_path_2(double[,] adjacent_matrix, double[] vertice_cost, int direction, int max_used_vertice)
        {
            List<int> shourtest_path = new List<int>();
            //int max_used_vertice = DataReadWrite.max_train_for_one_stock;
            int vertice_num = adjacent_matrix.GetLength(0);//
            double[,] current_distance = new double[max_used_vertice,vertice_num];//从源点到每个顶点的最短距离
            int[,] pre_vertice = new int[max_used_vertice, vertice_num];

            for (int used_vertice_num = 0; used_vertice_num < max_used_vertice;used_vertice_num++ )
            {
                for (int vertice = 0; vertice < vertice_num; vertice++)
                {
                    if (g.trainlist[vertice].direction == direction)
                    {
                        current_distance[used_vertice_num, vertice] = -(cancel_cost + vertice_cost[vertice]);
                        pre_vertice[used_vertice_num, vertice] = -1;
                    }
                    else
                    {
                        current_distance[used_vertice_num, vertice] = M;
                        pre_vertice[used_vertice_num, vertice] = -2;
                    }                 
                }
            }
            
            for(int used_vertice=1;used_vertice<max_used_vertice;used_vertice++)
            {
                for (int vertice_1 = 0; vertice_1 < vertice_num; vertice_1++)
                {
                    for (int vertice_2 = 0; vertice_2 < vertice_num; vertice_2++)
                    {
                        double current_cost = adjacent_matrix[vertice_2, vertice_1] + current_distance[used_vertice-1,vertice_2] - 
                            (cancel_cost + vertice_cost[vertice_1]);                
                        if (current_cost < current_distance[used_vertice, vertice_1])
                        {
                            current_distance[used_vertice, vertice_1] = current_cost;
                            pre_vertice[used_vertice, vertice_1] = vertice_2;
                        }
                    }
                }
            }

            double min_value = double.MaxValue;
            int min_vertice = -1;
            for (int vertice = 0; vertice < vertice_num; vertice++)
            {
                if (current_distance[max_used_vertice-1, vertice] < min_value && g.trainlist[vertice].direction != direction)
                {
                    min_value = current_distance[max_used_vertice-1, vertice];
                    min_vertice = vertice;
                }
            }
            shourtest_path.Add(min_vertice);
            for (int vertice_used = max_used_vertice - 1; vertice_used >= 0; vertice_used--)
            {
                if (pre_vertice[vertice_used, min_vertice] == -1)
                {
                    break;
                }
                shourtest_path.Add(pre_vertice[vertice_used, min_vertice]);
                min_vertice = pre_vertice[vertice_used, min_vertice];
            }
            shourtest_path.Reverse();

            Path path = new Path();
            path.train_index_list = shourtest_path;
            //path.index = stock_path_list.Count();
            path.profit = get_path_cost(path.get_train_count()) - path.get_train_count() * cancel_cost;
            double objvalue = shortest_path_model.get_obj_value();
            double total_dual_max_used_stock = 0;
            if (path.get_train_count() >= stock_type_min_undertake)
            {
                for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                {
                    int[] element = g.stock_type_list[stock_type_index];
                    if(path.get_train_count()>=element[0])
                    {
                        total_dual_max_used_stock += dual_max_used_stock[stock_type_index];
                    }
                }
            }
            else
            {
                total_dual_max_used_stock = dual_max_used_stock[stock_type_count-1];
            }
            path.reduce_cost = get_path_cost(path.get_train_count()) - total_dual_max_used_stock + min_value;
            return path;

        }
        public List<Path> stock_shortest_path_3(double[,] adjacent_matrix, double[] vertice_cost, int direction)
        {
            

            //int max_used_vertice = DataReadWrite.max_train_for_one_stock;
            int vertice_num = adjacent_matrix.GetLength(0);//
            double[,] current_distance = new double[stock_type_max_undertake, vertice_num];//从源点到每个顶点的最短距离
            int[,] pre_vertice = new int[stock_type_max_undertake, vertice_num];

            for (int used_vertice_num = 0; used_vertice_num < stock_type_max_undertake; used_vertice_num++)
            {
                for (int vertice = 0; vertice < vertice_num; vertice++)
                {
                    if (g.trainlist[vertice].direction == direction)
                    {
                        current_distance[used_vertice_num, vertice] = -(cancel_cost + vertice_cost[vertice]);
                        pre_vertice[used_vertice_num, vertice] = -1;
                    }
                    else
                    {
                        current_distance[used_vertice_num, vertice] = M;
                        pre_vertice[used_vertice_num, vertice] = -2;
                    }
                }
            }

            for (int used_vertice = 1; used_vertice < stock_type_max_undertake; used_vertice++)
            {
                for (int vertice_1 = 0; vertice_1 < vertice_num; vertice_1++)
                {
                    for (int vertice_2 = 0; vertice_2 < vertice_num; vertice_2++)
                    {
                        double current_cost = adjacent_matrix[vertice_2, vertice_1] + current_distance[used_vertice - 1, vertice_2] -
                            (cancel_cost + vertice_cost[vertice_1]);
                        if (current_cost < current_distance[used_vertice, vertice_1])
                        {
                            current_distance[used_vertice, vertice_1] = current_cost;
                            pre_vertice[used_vertice, vertice_1] = vertice_2;
                        }
                    }
                }
            }

            List<Path> path_list = new List<Path>();
            for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
            {
                int[] element = g.stock_type_list[stock_type_index];
                List<int> shourtest_path = new List<int>();

                double min_value = double.MaxValue;
                int min_vertice = -1;
                for (int vertice = 0; vertice < vertice_num; vertice++)
                {
                    if (current_distance[element[0] - 1, vertice] < min_value && g.trainlist[vertice].direction != direction)
                    {
                        min_value = current_distance[element[0] - 1, vertice];
                        min_vertice = vertice;
                    }
                }
                shourtest_path.Add(min_vertice);
                for (int vertice_used = element[0] - 1; vertice_used >= 0; vertice_used--)
                {
                    if (pre_vertice[vertice_used, min_vertice] == -1)
                    {
                        break;
                    }
                    shourtest_path.Add(pre_vertice[vertice_used, min_vertice]);
                    min_vertice = pre_vertice[vertice_used, min_vertice];
                }
                shourtest_path.Reverse();

                Path path = new Path();
                path.train_index_list = shourtest_path;
                //path.index = stock_path_list.Count();
                path.profit = get_path_cost(path.get_train_count()) - path.get_train_count() * cancel_cost;
                double objvalue = shortest_path_model.get_obj_value();
                double total_dual_max_used_stock = 0;
                if (path.get_train_count() >= stock_type_min_undertake)
                {
                    for (int stock_type_index_2 = 0; stock_type_index_2 < stock_type_count; stock_type_index_2++)
                    {
                        int[] element_2 = g.stock_type_list[stock_type_index];
                        if (path.get_train_count() >= element_2[0])
                        {
                            total_dual_max_used_stock += dual_max_used_stock[stock_type_index_2];
                        }
                    }
                }
                else
                {
                    total_dual_max_used_stock = dual_max_used_stock[stock_type_count - 1];
                }
                path.reduce_cost = get_path_cost(path.get_train_count()) - total_dual_max_used_stock + min_value;
                path_list.Add(path);
            }
            
            return path_list;

        }

        //生成stock shortest path 的算法
        //虚拟起点和终点，与每一列车都有连线,这里不把虚拟节点加入vertice_num,只是为了便于理解
        //参数direction=1表示下行车场出发，回到下行车场，而=0时则表示上行车场出发回到上行车场
        //或者同时查找上行和下行
        public Path stock_shortest_path(double[,] adjacent_matrix, double[] vertice_cost, int direction)
        {
            List<int> shourtest_path = new List<int>();
            int max_used_vertice = DataReadWrite.max_train_for_one_stock;
            int vertice_num = adjacent_matrix.GetLength(0);//
            double[] current_distance = new double[vertice_num];//从源点到每个顶点的最短距离
            double[] pre_distance = new double[vertice_num];//前一阶段（指vertice_used-1）的最短距离
            int[,] pre_vertice = new int[max_used_vertice, vertice_num];
            //初始化
            //double[,] cost = new double[vertice_num, vertice_num];

            for (int vertice = 0; vertice < vertice_num; vertice++)
            {
                if (g.trainlist[vertice].direction == direction)
                {
                    current_distance[vertice] = -(cancel_cost + vertice_cost[vertice]);
                    //current_distance[vertice] = -vertice_cost[vertice];
                    pre_distance[vertice] = current_distance[vertice];
                    pre_vertice[0, vertice] = -1;
                }
                else
                {
                    current_distance[vertice] = M;
                    pre_distance[vertice] = current_distance[vertice];
                    pre_vertice[0, vertice] = -2;
                }
            }
            //限制最多可经过的顶点数量
            for (int vertice_used = 1; vertice_used < max_used_vertice; vertice_used++)
            {
                for (int vertice_1 = 0; vertice_1 < vertice_num; vertice_1++)
                {
                    for (int vertice_2 = 0; vertice_2 < vertice_num; vertice_2++)
                    {
                        double current_cost = adjacent_matrix[vertice_2, vertice_1] + pre_distance[vertice_2] - (cancel_cost + vertice_cost[vertice_1]);
                        //double current_cost = adjacent_matrix[vertice_2, vertice_1] + pre_distance[vertice_2]-vertice_cost[vertice_1];
                        if (current_cost < current_distance[vertice_1])
                        {
                            current_distance[vertice_1] = current_cost;
                            pre_vertice[vertice_used, vertice_1] = vertice_2;
                        }
                    }
                    if (pre_distance[vertice_1] == current_distance[vertice_1])
                    {
                        pre_vertice[vertice_used, vertice_1] = pre_vertice[vertice_used - 1, vertice_1];
                    }
                }
                for (int vertice = 0; vertice < vertice_num; vertice++)
                {
                    pre_distance[vertice] = current_distance[vertice];
                }
            }
            double min_value = double.MaxValue;
            int min_vertice = -1;
            for (int vertice = 0; vertice < vertice_num; vertice++)
            {
                if (current_distance[vertice] < min_value && g.trainlist[vertice].direction != direction)
                {
                    min_value = current_distance[vertice];
                    min_vertice = vertice;
                }
            }
            shourtest_path.Add(min_vertice);
            for (int vertice_used = max_used_vertice - 1; vertice_used >= 0; vertice_used--)
            {
                if (pre_vertice[vertice_used, min_vertice] == -1)
                {
                    break;
                }
                shourtest_path.Add(pre_vertice[vertice_used, min_vertice]);
                min_vertice = pre_vertice[vertice_used, min_vertice];
            }
            shourtest_path.Reverse();

            Path path = new Path();
            path.train_index_list = shourtest_path;
            //path.index = stock_path_list.Count();
            path.profit = get_path_cost(path.get_train_count()) - path.get_train_count() * cancel_cost;
            double objvalue = shortest_path_model.get_obj_value();
            double total_dual_max_used_stock = 0;
            if (path.get_train_count() >= stock_type_min_undertake)
            {
                for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                {
                    int[] element = g.stock_type_list[stock_type_index];
                    if (path.get_train_count() >= element[0])
                    {
                        total_dual_max_used_stock += dual_max_used_stock[stock_type_index];
                    }
                }
            }
            else
            {
                total_dual_max_used_stock = dual_max_used_stock[stock_type_count - 1];
            }
            path.reduce_cost = get_path_cost(path.get_train_count()) - total_dual_max_used_stock + min_value;
            return path;
        }
        //添加初始列,用贪婪算法
        public void get_initial_solution_by_heuristic(int slack_turn_around_time)
        {
            int[] train_has_assignment_stock = new int[train_num];//记录是否已经被分配了
            int stock_index = 0;
            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            {
                if (train_has_assignment_stock[train_index_1] == 1)
                {
                    continue;
                }
                Path path = new Path();
                path.index = stock_path_list.Count();
                path.train_index_list.Add(train_index_1);
                train_has_assignment_stock[train_index_1] = 1;//train_index_1记录的是每一条交路的第一列车的信息

                for (int train_index_2 = train_index_1; train_index_2 < train_num; )
                {
                    if (path.train_index_list.Count() >= max_train_for_one_stock)
                    {
                        break;
                    }
                    Train train_2 = g.trainlist[train_index_2];
                    train_index_2 = get_next_connection_train(train_2, train_has_assignment_stock, slack_turn_around_time);
                    if (train_index_2 == -1)
                    {
                        break;
                    }
                    path.train_index_list.Add(train_index_2);
                }
                path.profit = get_path_cost(path.get_train_count()) - path.get_train_count() * cancel_cost;
                addColumn(path);
                stock_path_list.Add(path);
                stock_index++;
            }
            //for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
            //{
            //    if (train_has_assignment_stock[train_index_1] == 1)
            //    {
            //        continue;
            //    }
            //    Path path = new Path();
            //    path.index = stock_path_list.Count();
            //    //path.index = 0;
            //    train_has_assignment_stock[train_index_1] = 1;
            //    path.train_index_list.Add(train_index_1);
            //    Train train_1 = g.trainlist[train_index_1];
            //    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            //    {
            //        if (train_has_assignment_stock[train_index_2] == 1)
            //        {
            //            continue;
            //        }
            //        if (path.train_index_list.Count() >= max_train_for_one_stock)
            //        {
            //            break;
            //        }
            //        Train train_2 = g.trainlist[train_index_2];
            //        if (train_2.direction != train_1.direction)
            //        {
            //            if (g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name) -
            //            g.GetArriveTimeByTrainStation(train_1.name, train_1.end_station_name) >= connection_time + slack_turn_around_time)
            //            {
            //                path.train_index_list.Add(train_index_2);
            //                train_has_assignment_stock[train_index_2] = 1;
            //            }
            //        }
            //    }
            //    path.profit = use_stock_cost - path.get_train_count() * cancel_cost;
            //    addColumn(path);
            //    stock_path_list.Add(path);
            //    stock_index++;
            //}
        }
        //得到下一个接续的列车
        public int get_next_connection_train(Train train, int[] train_has_assignment_stock, int slack_turn_around_time)
        {
            int result = -1;
            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
            {
                if (train_has_assignment_stock[train_index_2] == 1)
                {
                    continue;
                }
                Train train_2 = g.trainlist[train_index_2];
                if (train_2.direction != train.direction)
                {
                    if (g.GetDepartureTimeByTrainStation(train_2.name, train_2.begin_station_name) -
                        g.GetArriveTimeByTrainStation(train.name, train.end_station_name) >= connection_time_min + slack_turn_around_time)
                    {
                        train_has_assignment_stock[train_index_2] = 1;
                        return train_index_2;
                    }
                }
            }
            return result;
        }
        //添加列
        public void addColumn(Path path)
        {
            GRBColumn column = new GRBColumn();
            int path_train_num = path.get_train_count();
            
            if(path_train_num>=stock_type_min_undertake)
            {
                for (int stock_type_index = 0; stock_type_index < stock_type_count; stock_type_index++)
                {
                    int[] element = g.stock_type_list[stock_type_index];
                    if (path.get_train_count() >= element[0])
                    {
                        column.AddTerm(1, get_constrains_mas_used_stcok(element[0]));
                    }
                }
            }
            else
            {
                column.AddTerm(1, get_constrains_mas_used_stcok(stock_type_min_undertake));
            }

            for (int index = 0; index < path.get_train_count(); index++)
            {
                column.AddTerm(1, get_constrains_one_train_one_stock(
                    path.get_train_index_by_list_index(index)));
            }
            benderssubmodel.AddVar(0, 1, path.profit, GRB.CONTINUOUS, column, "f_" + path.index);

            benderssubmodel.Update();
        }
        //输出求解信息
        public void outputform(string str)
        {
            log_visual.textBox_message.AppendText(str + "\r\n");

        }
        public void outputtext(string str)
        {
            //logfile.WriteLine(str);
        }
        //显示每次添加的动车组交路
        public void visual_stock_route(Path path, bool showrightnow, List<Path> best_path_list)
        {
            if (main_iter != 0)
            {
                PaintClass.PaintStyle paintstyle = new PaintClass.PaintStyle(true, false, false, false, false, false, path);
                paintstyle.best_path_list = best_path_list;
                DataShow.NewTraindiagram newtriandiagram = new DataShow.NewTraindiagram(g, new_g, paintstyle);
                PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(newtriandiagram.pictureBox_newtraindiagram, g,
                    new_g, paintstyle);
                if (showrightnow)
                    newtriandiagram.ShowDialog();
                else
                    newtriandiagram.Show();
            }

        }
        //根据索引得到约束条件
        public GRBConstr get_constrains_mas_used_stcok(int max_uesd_vertices)
        {
            return benderssubmodel.GetConstrByName("max_used_stock"+max_uesd_vertices);
        }
        //产生新的时刻表
        public void generationnewtimetable()
        {
            if (main_iter != 0)
            {
                foreach (Train train in g.trainlist)
                {
                    KeyValuePair<string, string> key_1 = new KeyValuePair<string, string>(train.name, train.begin_station_name);
                    KeyValuePair<string, string> key_2 = new KeyValuePair<string, string>(train.name, train.end_station_name);
                    new_g.trainstationtoarrivetime[key_2] = Convert.ToInt32(arrivetime_train[train.index].Get(GRB.DoubleAttr.X));
                    new_g.trainstationtodeparturetime[key_1] = Convert.ToInt32(departuretime_train[train.index].Get(GRB.DoubleAttr.X));
                }
            }
            else
            {
                foreach (Train train in g.trainlist)
                {
                    KeyValuePair<string, string> key_1 = new KeyValuePair<string, string>(train.name, train.begin_station_name);
                    KeyValuePair<string, string> key_2 = new KeyValuePair<string, string>(train.name, train.end_station_name);
                    new_g.trainstationtoarrivetime.Add(key_2, g.GetArriveTimeByTrainStation(train.name, train.end_station_name));
                    new_g.trainstationtodeparturetime.Add(key_1, g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name));
                }
            }
        }
        public GRBConstr get_constrains_one_train_one_stock(int train_index)
        {
            return benderssubmodel.GetConstrByName("one_train_one_stock_" + train_index);
        }

        public GRBConstr get_constrains_turnaround_time(int train_index_1, int train_index_2)
        {
            return benderssubmodel.GetConstrByName("turnaroundtime_" + train_index_1 + "_" + train_index_2);
        }

        public GRBVar get_variable_by_path_index(int path_index)
        {
            return benderssubmodel.GetVarByName("f_" + path_index);
        }

        public void Clear()
        {

        }

        public int get_path_cost(int path_train_num)
        {
            int result = 0;
            if(path_train_num<stock_type_min_undertake)
            {
                result = g.stock_cost_dict[stock_type_min_undertake];
            }
            else
            {
                for(int temp=path_train_num;temp<=stock_type_max_undertake;temp=temp+2)
                {
                    for (int index = 0; index < stock_type_count; index++)
                    {
                        int[] element = g.stock_type_list[index];
                        if(element[0]==temp)
                        {
                            result = g.stock_cost_dict[temp];
                            temp = stock_type_max_undertake + 2;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
    public class Path
    {
        public int profit;
        public int index;
        public double reduce_cost;
        public bool feasible;//指示在新的时刻表条件下，该条路径是否存在冲突
        //public int stock_index;
        public List<int> train_index_list;
        public Path()
        {
            train_index_list = new List<int>();
            //profit = 100;
        }
        public int get_train_count()
        {
            return train_index_list.Count();
        }
        public int get_train_index_by_list_index(int index)
        {
            return train_index_list[index];
        }
        public string get_train_list_in_string()
        {
            string str = "";
            foreach (int value in train_index_list)
            {
                str += value + "|";
            }
            return str;
        }
    }
}
