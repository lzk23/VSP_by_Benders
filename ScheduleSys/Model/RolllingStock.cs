//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Gurobi;
//using System.IO;

//namespace ScheduleSys.Model
//{
//    public class RolllingStock:GRBCallback
//    {
//        GRBEnv env;
//        GRBModel model;
//        GRBLinExpr objExpr;
//        public int optimstatus;

//        StreamWriter logfile_callback;

//        //全局变量
//        Graph g;
//        int train_num = 0;
//        int M = 1000000;
//        int stock_num = 0;
//        int connection_time_min = 10;
//        int connection_time_max = 60;


//        //决策变量

//        GRBVar[] cancel_train;//取消列车的约束

//        GRBVar[,] connection_train;//两列反向列车是否接续

//        GRBVar[,] train_use_stock_flag;
//        GRBVar[] stock_routing_first_train;
//        GRBVar[] stock_routing_last_train;

//        public RolllingStock()
//        {
//            env = new GRBEnv("GurobiSolving.log");
//            env.Set(GRB.IntParam.LazyConstraints,1);
//            //env.Set(GRB.IntParam.OutputFlag, 0);
//            model = new GRBModel(env);
//            //model.Set(GRB.IntParam.LazyConstraints,1);
//            objExpr = new GRBLinExpr();
//            logfile_callback = new StreamWriter("callback.log");
//        }
//        public RolllingStock(StreamWriter sw)
//        {
//            this.logfile_callback = sw;
//        }
//        protected override void Callback()
//        {
//            try
//            {
//                if (where == GRB.Callback.POLLING)
//                {
//                    // Ignore polling callback
//                }
//                else if (where == GRB.Callback.PRESOLVE)
//                {
//                    // Presolve callback
//                    //int cdels = GetIntInfo(GRB.Callback.PRE_COLDEL);
//                    //int rdels = GetIntInfo(GRB.Callback.PRE_ROWDEL);
//                    //if (cdels != 0 || rdels != 0)
//                    //{
//                    //    Console.WriteLine(cdels + " columns and " + rdels
//                    //        + " rows are removed");
//                    //}
//                }
//                else if (where == GRB.Callback.SIMPLEX)
//                {
//                    // Simplex callback
//                    //double itcnt = GetDoubleInfo(GRB.Callback.SPX_ITRCNT);
//                    //if (itcnt - lastiter >= 100)
//                    //{
//                    //    lastiter = itcnt;
//                    //    double obj = GetDoubleInfo(GRB.Callback.SPX_OBJVAL);
//                    //    int ispert = GetIntInfo(GRB.Callback.SPX_ISPERT);
//                    //    double pinf = GetDoubleInfo(GRB.Callback.SPX_PRIMINF);
//                    //    double dinf = GetDoubleInfo(GRB.Callback.SPX_DUALINF);
//                    //    char ch;
//                    //    if (ispert == 0) ch = ' ';
//                    //    else if (ispert == 1) ch = 'S';
//                    //    else ch = 'P';
//                    //    Console.WriteLine(itcnt + " " + obj + ch + " "
//                    //        + pinf + " " + dinf);
//                    //}
//                }
//                else if (where == GRB.Callback.MIP)
//                {
//                    // General MIP callback
//                    double runtime = GetDoubleInfo(GRB.Callback.RUNTIME);
//                    double nodecnt = GetDoubleInfo(GRB.Callback.MIP_NODCNT);
//                    double objbst = GetDoubleInfo(GRB.Callback.MIP_OBJBST);
//                    double objbnd = GetDoubleInfo(GRB.Callback.MIP_OBJBND);
//                    int solcnt = GetIntInfo(GRB.Callback.MIP_SOLCNT);
//                    //if (nodecnt - lastnode >= 100)
//                    //{
//                    //    lastnode = nodecnt;
//                    //    int actnodes = (int)GetDoubleInfo(GRB.Callback.MIP_NODLFT);
//                    //    int itcnt = (int)GetDoubleInfo(GRB.Callback.MIP_ITRCNT);
//                    //    int cutcnt = GetIntInfo(GRB.Callback.MIP_CUTCNT);
//                    //    Console.WriteLine(nodecnt + " " + actnodes + " "
//                    //        + itcnt + " " + objbst + " " + objbnd + " "
//                    //        + solcnt + " " + cutcnt);
//                    //}
//                    //if (Math.Abs(objbst - objbnd) < 0.05 * (1.0 + Math.Abs(objbst)))
//                    //{
//                    //    logfile_callback.WriteLine("Stop early - 1% gap achieved");
//                    //    Abort();
//                    //}
//                    //if (Math.Abs(objbst - objbnd) < 0.005 * (1.0 + Math.Abs(objbst)) && runtime >= 100)
//                    //{
//                    //    logfile.WriteLine("Stop early - 0.5% gap achieved");
//                    //    Abort();
//                    //}
//                    if (runtime >= 7200)
//                    {
//                        logfile_callback.WriteLine("Stop early - 7200s time achieved");
//                        Abort();
//                    }
//                    //if (nodecnt >= 10000 && solcnt > 0)
//                    //{
//                    //    logfile.WriteLine("Stop early - 10000 nodes explored");
//                    //    Abort();
//                    //}
//                }
//                else if (where == GRB.Callback.MIPSOL)
//                {
//                    // MIP solution callback
//                    //int nodecnt = (int)GetDoubleInfo(GRB.Callback.MIPSOL_NODCNT);
//                    //double obj = GetDoubleInfo(GRB.Callback.MIPSOL_OBJ);
//                    //int solcnt = GetIntInfo(GRB.Callback.MIPSOL_SOLCNT);
//                    //double[] x = GetSolution(vars);
//                    //logfile.WriteLine("**** New solution at node " + nodecnt
//                    //    + ", obj " + obj + ", sol " + solcnt
//                    //    + ", x[0] = " + x[0] + " ****");
//                }
//                else if (where == GRB.Callback.MIPNODE)
//                {
//                    // MIP node callback
//                    //logfile.WriteLine("**** New node ****");
//                    //if (GetIntInfo(GRB.Callback.MIPNODE_STATUS) == GRB.Status.OPTIMAL)
//                    //{
//                    //    double[] x = GetNodeRel(vars);
//                    //    SetSolution(vars, x);
//                    //}
//                }
//                else if (where == GRB.Callback.BARRIER)
//                {
//                    // Barrier callback
//                    //int itcnt = GetIntInfo(GRB.Callback.BARRIER_ITRCNT);
//                    //double primobj = GetDoubleInfo(GRB.Callback.BARRIER_PRIMOBJ);
//                    //double dualobj = GetDoubleInfo(GRB.Callback.BARRIER_DUALOBJ);
//                    //double priminf = GetDoubleInfo(GRB.Callback.BARRIER_PRIMINF);
//                    //double dualinf = GetDoubleInfo(GRB.Callback.BARRIER_DUALINF);
//                    //double cmpl = GetDoubleInfo(GRB.Callback.BARRIER_COMPL);
//                    //Console.WriteLine(itcnt + " " + primobj + " " + dualobj + " "
//                    //    + priminf + " " + dualinf + " " + cmpl);
//                }
//                else if (where == GRB.Callback.MESSAGE)
//                {
//                    // Message callback
//                    //string msg = GetStringInfo(GRB.Callback.MSG_STRING);
//                    //if (msg != null) logfile.Write(msg);
//                }
//            }
//            catch (GRBException e)
//            {
//                logfile_callback.WriteLine("Error code: " + e.ErrorCode);
//                logfile_callback.WriteLine(e.Message);
//                logfile_callback.WriteLine(e.StackTrace);
//            }
//            catch (Exception e)
//            {
//                logfile_callback.WriteLine("Error during callback");
//                logfile_callback.WriteLine(e.StackTrace);
//            }
//        }
//        public RolllingStock(Graph g)
//            : this()
//        {
//            this.g = g;

//            train_num = g.trainlist.Count();
//            stock_num = g.stocklist.Count();
//            //决策变量的初始化

//            cancel_train = new GRBVar[train_num];

//            connection_train = new GRBVar[train_num, train_num];

//            stock_routing_first_train = new GRBVar[train_num];
//            stock_routing_last_train = new GRBVar[train_num];

//            train_use_stock_flag = new GRBVar[stock_num, train_num];

//            for (int train_index = 0; train_index < train_num; train_index++)
//            {
//                stock_routing_first_train[train_index] = model.AddVar(0, 1, 0,
//                   GRB.BINARY, "b[R" + train_index + "]");
//                stock_routing_last_train[train_index] = model.AddVar(0, 1, 0,
//                    GRB.BINARY, "e[R" + train_index + "]");

//                cancel_train[train_index] = model.AddVar(0, 1, 0, GRB.BINARY, "m[T" + train_index + "]");
//            }
//            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//            {
//                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//                {
//                    if (train_index_1 != train_index_2)
//                    {
//                        connection_train[train_index_1, train_index_2] = model.AddVar(0, 1, 0,
//                            GRB.BINARY, "connection_train[T" + train_index_1 + ",T" + train_index_2 + "]");
//                    }

//                }
//            }

//            for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            {
//                for (int train_index = 0; train_index < train_num; train_index++)
//                {
//                    train_use_stock_flag[stock_index, train_index] = model.AddVar(0, 1, 0,
//                        GRB.BINARY, "x[R" + stock_index + ",T" + train_index + "]");
//                }
               
//            }
//        }
//        //给变量限制一定的值，假设所有同方向的列车没有区别，那么谁先谁后就无所谓了

//        public void BuildAndSovleModel()
//        {
//            model.Update();
//            //目标函数
//            foreach (Train train in g.trainlist)
//            {
//                objExpr += stock_routing_first_train[train.index]*100;
//                objExpr += cancel_train[train.index] * 200;
//            }
            
//            model.SetObjective(objExpr, 1);

               
//            //若取消列车则把始发时间平移到
//            //紧邻接续的辆列车采用相同的动车组
//            for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            {
//                for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//                {
//                    Train train_1 = g.trainlist[train_index_1];
//                    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//                    {
//                        Train train_2 = g.trainlist[train_index_2];
//                        if (train_1.direction != train_2.direction)
//                        {
//                            model.AddConstr(train_use_stock_flag[stock_index, train_index_1] >= train_use_stock_flag[stock_index, train_index_2] -
//                                (1 - connection_train[train_index_1, train_index_2]) * M, "connection_train_use_same_stock_1");
//                            model.AddConstr(train_use_stock_flag[stock_index, train_index_1] <= train_use_stock_flag[stock_index, train_index_2] +
//                                (1 - connection_train[train_index_1, train_index_2]) * M, "connection_train_use_same_stock_2");
//                        }
//                    }
//                }
//            }
//            //动车组交路的第一列车采用的动车组不相同
//            for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            {
//                for (int train_index_1 = 0; train_index_1 < train_num-1; train_index_1++)
//                {
  
//                    for (int train_index_2 = train_index_1+1; train_index_2 < train_num; train_index_2++)
//                    {
                 
//                        model.AddConstr(train_use_stock_flag[stock_index, train_index_1] +train_use_stock_flag[stock_index, train_index_2]<=
//                            3-stock_routing_first_train[train_index_1]-
//                        stock_routing_first_train[train_index_2], "not_connection_train_use_diff_stock_1");                       
                    
//                    }
//                }
//            }
//            //最小和最大接续时间约束
//            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//            {
//                Train train_1 = g.trainlist[train_index_1];
//                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//                {
//                    Train train_2 = g.trainlist[train_index_2];
//                    if (train_1.direction != train_2.direction)
//                    {
//                        int departure_time=g.GetDepartureTimeByTrainStation(train_2.name,train_2.begin_station_name);
//                        int arrive_time=g.GetArriveTimeByTrainStation(train_1.name,train_1.end_station_name);
//                        if(departure_time-arrive_time>connection_time_max||departure_time-arrive_time<connection_time_min)
//                        {
//                            connection_train[train_index_1, train_index_2].Set(GRB.DoubleAttr.LB,0);
//                            connection_train[train_index_1, train_index_2].Set(GRB.DoubleAttr.UB,0);
//                        }
//                    }
//                }
//            }

//            GRBLinExpr expr;
//            GRBLinExpr expr_1;
//            //紧邻接续的列车至多只有一列
//            for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//            {
//                expr = new GRBLinExpr();
//                expr_1 = new GRBLinExpr();
//                Train train_1 = g.trainlist[train_index_1];
//                for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//                {
//                    Train train_2 = g.trainlist[train_index_2];
//                    if (train_1.direction != train_2.direction)
//                    {
//                        expr.AddTerm(1, connection_train[train_index_1, train_index_2]);
//                        expr_1.AddTerm(1, connection_train[train_index_2, train_index_1]);
//                    }
//                }
//                expr.AddTerm(1,stock_routing_last_train[train_index_1]);
//                expr_1.AddTerm(1, stock_routing_first_train[train_index_1]);
//                //添加取消列车的变量

//                model.AddConstr(expr, GRB.EQUAL, 1-cancel_train[train_index_1], "most_one_stock_for_connection_1");
//                model.AddConstr(expr_1, GRB.EQUAL, 1-cancel_train[train_index_1], "most_one_stock_for_connection_2");
//            }
//            //车底是否被使用的标识
//            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            //{
//            //    expr = new GRBLinExpr();
//            //    for (int train_index = 0; train_index < train_num; train_index++)
//            //    {
//            //        expr.AddTerm(1, train_use_stock_flag[stock_index, train_index]);
//            //    }
//            //    model.AddConstr(expr, GRB.LESS_EQUAL, stock_use_flag[stock_index] * M, "use_R" + stock_index);
//            //}
//            //每一列车使用一个车底
//            for (int train_index = 0; train_index < train_num; train_index++)
//            {
//                expr = new GRBLinExpr();
//                for (int stock_index = 0; stock_index < stock_num; stock_index++)
//                {
//                    expr.AddTerm(1, train_use_stock_flag[stock_index, train_index]);
//                }
//                model.AddConstr(expr, GRB.EQUAL, 1-cancel_train[train_index], "each_train_each_stock_T" + train_index);
//            }
//            //动车组担当列车数量的约束
//            for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            {
//                expr = new GRBLinExpr();
//                for (int train_index = 0; train_index < train_num; train_index++)
//                {
//                    expr.AddTerm(1,train_use_stock_flag[stock_index,train_index]);
//                }
//                model.AddConstr(expr, GRB.LESS_EQUAL, DataReadWrite.max_train_for_one_stock, "most_train_for_one_stock");
//            }
//            //回到起始动车段的约束:第一种约束表达方式
//            for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            {
//                for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//                {
//                    Train train_1 = g.trainlist[train_index_1];
//                    for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//                    {
//                        Train train_2 = g.trainlist[train_index_2];
//                        if (train_1.direction == train_2.direction)
//                        {
//                            model.AddConstr(stock_routing_first_train[train_index_1] + stock_routing_last_train[train_index_2] <=
//                            3 - train_use_stock_flag[stock_index, train_index_1] - train_use_stock_flag[stock_index, train_index_2]
//                                 , "return_to_origin_depot");

//                        }
//                    }
//                }
//            }
//            //回到起始动车段的约束:第二种约束表达方式
//            //for (int stock_index = 0; stock_index < stock_num; stock_index++)
//            //{
//            //    expr = new GRBLinExpr();
//            //    expr_1 = new GRBLinExpr();
//            //    for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//            //    {
//            //        Train train_1 = g.trainlist[train_index_1];
//            //        if(train_1.direction==0)
//            //        {
//            //            expr.AddTerm(1, train_use_stock_flag[stock_index, train_index_1]);
//            //        }
//            //        else
//            //        {
//            //            expr_1.AddTerm(1, train_use_stock_flag[stock_index, train_index_1]);
//            //        }
//            //    }
//            //    model.AddConstr(expr==expr_1,"reture_to_origin_depot");
//            //}
//            model.SetCallback(new RolllingStock(logfile_callback));
//            model.Optimize();
//            optimstatus = model.Get(GRB.IntAttr.Status);
//        }
//        //得到下一个接续的列车
//        public int get_next_connection_train(Train train)
//        {
//            int result = -1;
//            for (int train_index_2 = 0; train_index_2 < train_num; train_index_2++)
//            {
//                Train train_2 = g.trainlist[train_index_2];
//                if(train_2.direction!=train.direction)
//                {
//                    if (connection_train[train.index, train_index_2].Get(GRB.DoubleAttr.X) > 0.5)
//                    {
//                        return train_2.index;
//                    }
//                }
//            }
//            return result;
//        }

//        public void SolveResult(Graph new_g)
//        {
//            if (model.SolCount!=0)
//            {
//                ////new_g = new Graph(g);
//                //DataReadWrite.SetParemeter(new_g);//设置车头间距，检查冲突的时候有用
//                //foreach (Train train in g.trainlist)
//                //{
//                //    KeyValuePair<string, string> key_1 = new KeyValuePair<string, string>(train.name, train.begin_station_name);
//                //    KeyValuePair<string, string> key_2 = new KeyValuePair<string, string>(train.name, train.end_station_name);
//                //    //if (new_g.trainstationtoarrivetime.ContainsKey(key))
//                //    //{
//                //    //    new_g.trainstationtoarrivetime[key] = Convert.ToInt32(arrivetime_train[g.stationnametoindex[station_name],
//                //    //        train.index].Get(GRB.DoubleAttr.X));
//                //    //    new_g.trainstationtodeparturetime[key] = Convert.ToInt32(departuretime_train[g.stationnametoindex[station_name],
//                //    //        train.index].Get(GRB.DoubleAttr.X));
//                //    //}
//                //    //else
//                //    //{
//                //    new_g.trainstationtoarrivetime.Add(key_2, Convert.ToInt32(arrivetime_train[train.index].Get(GRB.DoubleAttr.X)));
//                //    new_g.trainstationtodeparturetime.Add(key_1, Convert.ToInt32(departuretime_train[train.index].Get(GRB.DoubleAttr.X)));
//                //    //}  
//                //}
//                //得到动车组使用信息

//                for (int train_index = 0; train_index < train_num; train_index++)
//                {
//                    bool has_flag = false;
//                    for (int stock_index = 0; stock_index < stock_num; stock_index++)
//                    {
//                        if (train_use_stock_flag[stock_index, train_index].Get(GRB.DoubleAttr.X) >= 0.5)
//                        {
//                            Train train = new_g.trainlist[train_index];
//                            if (train.stock_index != -1)
//                            {
//                                throw new Exception("One train use two stock.");
//                            }
//                            train.stock_index = stock_index;
//                            has_flag = true;
//                        }
//                    }
//                    //if (!has_flag)
//                    //{
//                    //    throw new Exception("The train " + train_index + " has no stock.");
//                    //}
//                }
//                //得到交路信息,并给交路赋予动车组
//                //int stock_flag = 0;
//                //for (int train_index_1 = 0; train_index_1 < train_num; train_index_1++)
//                //{                
//                //    if(stock_routing_first_train[train_index_1].Get(GRB.DoubleAttr.X)>0.5)
//                //    {
//                //        Train train_1 = g.trainlist[train_index_1];
//                //        if (train_1.stock_index != -1)
//                //        {
//                //            throw new Exception("Error.");
//                //        }
//                //        train_1.stock_index = stock_flag;
//                //        for (int train_index_2 = train_index_1; train_index_2 < train_num;)
//                //        {
//                //            Train train_2 = g.trainlist[train_index_2];
//                //            if (stock_routing_last_train[train_index_2].Get(GRB.DoubleAttr.X) > 0.5)
//                //            {                        
//                //                train_2.stock_index = stock_flag;
//                //                stock_flag++;
//                //                break;
//                //            }
//                //            else
//                //            {
//                //                train_index_2 = get_next_connection_train(train_2);
//                //                Train train_2_temp = g.trainlist[train_index_2];
//                //                if(train_2_temp.stock_index!=-1)
//                //                {
//                //                    throw new Exception("Error.");
//                //                }
//                //                train_2_temp.stock_index=stock_flag;
//                //            }
//                //        }     
//                //    }
//                //}
//                //model.Get(GRB.DoubleAttr.Obj);
//                model.Write("out.sol");
//                model.Dispose();//释放资源以便后面读取文件
//                env.Dispose();
//                //调整后运行图显示
//                PaintClass.PaintStyle paintstyle = new PaintClass.PaintStyle(true, false, false, false, false,false, null);
//                DataShow.NewTraindiagram newtriandiagram = new DataShow.NewTraindiagram(g, new_g,paintstyle);
//                PaintClass.TrainDiagramPaint initialtraindigram = new PaintClass.TrainDiagramPaint(newtriandiagram.pictureBox_newtraindiagram,
//                    g, paintstyle);
//                newtriandiagram.Show();


//                //调整前后时刻表
//                //DataShow.NewTimetable newtimetable = new DataShow.NewTimetable();
//                //DataReadWrite.ShowTimetable(new_g, g, newtimetable.dataGridView_timetable);
//                //newtimetable.textBox_solvingprocess.Text = (System.IO.File.ReadAllText("GurobiSolving.log", Encoding.Default));
//                //newtimetable.Show();

//            }
//            //else if (optimstatus == GRB.Status.INFEASIBLE)
//            //{
//            //    DataShow.SolvingResult resultform = new DataShow.SolvingResult();
//            //    resultform.textBox_result.AppendText("The model is infeasible.Please check follow constrains and variables:");
//            //    model.ComputeIIS();
//            //    model.Write("model.ilp");
//            //    resultform.textBox_result.AppendText(System.IO.File.ReadAllText("model.ilp", Encoding.Default));//读取文本信息到文本控件中
//            //    resultform.Show();
//            //    //logfile_callback.Close();
//            //    model.Dispose();
//            //    env.Dispose();
//            //}
//            else
//            {
//                throw new Exception("Error");
//            }
//            //if (optimstatus == GRB.Status.INF_OR_UNBD)
//            //{
//            //    gurobidoubletrackmodel.GetEnv().Set(GRB.IntParam.Presolve, 0);
//            //    gurobidoubletrackmodel.Optimize();
//            //    optimstatus = gurobidoubletrackmodel.Get(GRB.IntAttr.Status);
//            //}                       
//        }
//    }

//}
