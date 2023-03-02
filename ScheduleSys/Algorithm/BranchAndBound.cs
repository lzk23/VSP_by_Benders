using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;

namespace ScheduleSys.Algorithm
{
    public class BBTree
    {
        int temp_variable_to_branch = 3;

        List<BBNode> bbnode_list;
        //BBNode best_int_sol_bbnode;
        List<Path> best_path_list;
        double best_int_obj_val = double.MaxValue;
        double best_obj_val = double.MaxValue;//the lowest obj func value
        ///*pseudocost branching*/
        //List<double> pseudo_cost_up;
        //List<double> pseudo_cost_down;
        //List<double> sigmaUp;
        //List<int> numUp;
        //List<int> numDown;
        //List<double> sigmaDown;

        //List<double>[] pseudo_cost_up;
        //List<double>[] pseudo_cost_down;
        //List<double>[] sigmaUp;
        //List<int>[] numUp;
        //List<int>[] numDown;
        //List<double>[] sigmaDown;
        public BBTree()
        {
            bbnode_list = new List<BBNode>();
            //sigmaUp = new List<double>[DataReadWrite.trainnum];
            //numUp = new List<int>[DataReadWrite.trainnum];
            //numDown = new List<int>[DataReadWrite.trainnum];
            //sigmaDown = new List<double>[DataReadWrite.trainnum];

            //pseudo_cost_up = new List<double>[DataReadWrite.trainnum];
            //pseudo_cost_down = new List<double>[DataReadWrite.trainnum];

            //for (int i = 0; i < DataReadWrite.trainnum; i++)
            //{
            //    sigmaDown[i] = new List<double>();
            //    sigmaUp[i] = new List<double>();
            //    numDown[i] = new List<int>();
            //    numUp[i] = new List<int>();
            //    pseudo_cost_up[i] = new List<double>();
            //    pseudo_cost_down[i] = new List<double>();
            //}
        }
        public double get_best_int_obj_val()
        {
            return best_int_obj_val;
        }
        public bool update_best_int_obj_val(BBNode _bbnode)
        {
            bool update_upbound_flag = false;
            if(best_int_obj_val>_bbnode.get_opt_obj_val())
            {
                best_int_obj_val = _bbnode.get_opt_obj_val();
                //best_int_sol_bbnode = _bbnode;
                //此处不选择保留节点，而是选择把整数解直接保留下来
                best_path_list=_bbnode.get_integer_solution();
                update_upbound_flag = true;
            }
            return update_upbound_flag;
        }
        public void update_best_int_obj_val(int int_obj_value)
        {
            best_int_obj_val = int_obj_value;
        }
        public double get_best_obj_val()
        {
            return best_obj_val;
        }
        public void update_best_obj_val(double value)
        {
            if(value<best_obj_val)
            {
                best_obj_val = value;
            }
        }
        public void add_bbnode(BBNode bbnode)
        {
            bbnode_list.Add(bbnode);
        }
        public void remove_last_bbnode()
        {
            if(bbnode_list.Count()==0)
            {
                throw new Exception("Error.");
            }
            else
            {
                for (int i = 0; i < bbnode_list.Count(); i++)
                {
                    if(bbnode_list[i].get_lp_status()!=LP_Status.unsolve)
                    {
                        bbnode_list.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public bool has_next_bbnode()
        {
            return bbnode_list.Count() != 0 ? true : false;
        }
        //deep first
        public BBNode get_next_bbnode_base_DF()
        {
            return bbnode_list.Last();
        }
        public BBNode get_next_bbnode_base_BF()
        {
            return bbnode_list.First();
        }
        //public BBNode get_next_bbnode()
        //{

        //}
        //public int branch(BBNode bbnode)
        //{
        //    BBNode left_child = null;
        //    BBNode right_child = null;
        //    BBNode_Fix_Col bbnode_fix_col = get_path_fractional(bbnode);
        //    if (bbnode_fix_col == null)
        //    {
        //        throw new Exception("error.");
        //    }

        //    left_child = new BBNode(2 * bbnode.id, bbnode);
        //    left_child.add_var_fix(bbnode_fix_col, 0);
        //    bbnode_list.Add(left_child);

        //    BBNode_Fix_Col bbnode_fix_col_2 = new BBNode_Fix_Col(bbnode_fix_col);
        //    right_child = new BBNode(2 * bbnode.id + 1, bbnode);
        //    right_child.add_var_fix(bbnode_fix_col_2, 1);
        //    bbnode_list.Add(right_child);
        //    return 1;

        //}
        /*13.49 seconds*/
        public int branch(BBNode bbnode)
        {
            BBNode left_child = null;
            BBNode right_child = null;
            //BBNode_Fix_Col bbnode_fix_col = get_path_fractional_base_on_pseudocost(bbnode);
            //BBNode_Fix_Col bbnode_fix_col = full_strong_branch(bbnode);
            BBNode_Fix_Col bbnode_fix_col = get_path_fractional_random(bbnode);
            if (bbnode_fix_col == null)
            {
                throw new Exception("error.");
            }
            left_child = new BBNode(2 * bbnode.id, bbnode);
            bbnode_fix_col.fix_path_value = 0;
            left_child.fix_col = bbnode_fix_col;
            bbnode_list.Add(left_child);

            bbnode_fix_col = new BBNode_Fix_Col(bbnode_fix_col);
            bbnode_fix_col.fix_path_value = 1;
            right_child = new BBNode(2 * bbnode.id + 1, bbnode);
            right_child.fix_col = bbnode_fix_col;
            bbnode_list.Add(right_child);

           
            return 1;
            //if (bbnode_fix_col.fix_path_value == 1)
            //{
            //    left_child = new BBNode(2 * bbnode.id, bbnode);
            //    bbnode_fix_col.fix_path_value = 0;
            //    left_child.fix_col = bbnode_fix_col;
            //    bbnode_list.Add(left_child);

            //    bbnode_fix_col.fix_path_value = 1;
            //    right_child = new BBNode(2 * bbnode.id + 1, bbnode);
            //    right_child.fix_col=bbnode_fix_col;
            //    bbnode_list.Add(right_child);
            //    return 1;
            //}
            //else
            //{
            //    bbnode_fix_col.fix_path_value = 1;
            //    right_child = new BBNode(2 * bbnode.id + 1, bbnode);
            //    right_child.fix_col = bbnode_fix_col;
            //    bbnode_list.Add(right_child);

            //    left_child = new BBNode(2 * bbnode.id, bbnode);
            //    bbnode_fix_col.fix_path_value = 0;
            //    left_child.fix_col = bbnode_fix_col;
            //    bbnode_list.Add(left_child);
            //    return 2;
            //}

        }

        public BBNode_Fix_Col get_path_fractional_random(BBNode bbnode)
        {
            BBNode_Fix_Col fix_col=new BBNode_Fix_Col();
            if (bbnode.max_value_flag_index == -1)
            {
                throw new Exception("Error.");
            }
            string var_name = "f_" + bbnode.max_value_flag_index;
            fix_col = new BBNode_Fix_Col(var_name);
            

            //优先查找接近于的0.5变量
            //for (int path_index = 0; path_index < bbnode.stock_path_value_statu.Length; path_index++)
            //{
            //    if (bbnode.stock_path_value_statu[path_index] == PathValueStatus.Fractional_middle)
            //    {
            //        string var_name = "f_" + path_index;
            //        fix_col = new BBNode_Fix_Col(var_name);
            //    }
            //}

            //if(string.IsNullOrEmpty(fix_col.var_name))
            //{
            //    for (int path_index = 0; path_index < bbnode.stock_path_value_statu.Length; path_index++)
            //    {
            //        if (bbnode.stock_path_value_statu[path_index] == PathValueStatus.Fractional_less||
            //            bbnode.stock_path_value_statu[path_index]==PathValueStatus.Fractional_more)
            //        {
            //            string var_name = "f_" + path_index;
            //            fix_col = new BBNode_Fix_Col(var_name);
            //        }
            //    }
            //}
            //if (string.IsNullOrEmpty(fix_col.var_name))
            //{
            //    throw new Exception("Error.");
            //}
            return fix_col;
        }
        #region
        //public BBNode_Fix_Col get_path_fractional(BBNode bbnode)
        //{

        //    List<Path>[] path_list = bbnode.get_node_path_list();
        //    BBNode_Fix_Col bbnode_fix_col = new BBNode_Fix_Col();
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            if (path_list[i][j] == null)
        //            {
        //                continue;
        //            }
        //            if (path_list[i][j].value_status == PathValueStatus.Fractional_more)
        //            {
        //                bbnode_fix_col.train_id = i;
        //                bbnode_fix_col.path_index = j;
        //                bbnode_fix_col.fix_path_value = 1;
        //                return bbnode_fix_col;
        //            }
        //        }
        //    }
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            if (path_list[i][j] == null)
        //            {
        //                continue;
        //            }
        //            if (path_list[i][j].value_status == PathValueStatus.Fractional_less)
        //            {
        //                bbnode_fix_col.train_id = i;
        //                bbnode_fix_col.path_index = j;
        //                bbnode_fix_col.fix_path_value = 0;
        //                return bbnode_fix_col;
        //            }
        //        }
        //    }

        //    if (true)
        //    {
        //        throw new Exception("There has no fractional more than 0.5.");
        //    }
        //    //return null;
        //}
        //public BBNode_Fix_Col get_path_fractional_base_on_pseudocost(BBNode bbnode)
        //{
        //    List<Path>[] path_list = bbnode.get_node_path_list();
        //    List<double>[] score_list = new List<double>[DataReadWrite.trainnum];

        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        score_list[i] = new List<double>();
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            score_list[i].Add(0);
        //        }
        //    }
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            Path path = path_list[i][j];
        //            if (path == null||path.value_status==PathValueStatus.Integer)
        //            {
        //                continue;
        //            }
        //            score_list[i][j] = calculate_score(path.value* path.pseudo_cost_down,(1-path.value)*path.pseudo_cost_up);
        //        }
        //    }
        //    return get_the_best_score(score_list,path_list);
        //}
        //public BBNode_Fix_Col get_the_best_score(List<double>[] score_list, List<Path>[] path_list)
        //{
        //    /*get the best score*/
        //    double max_score = 0;
        //    int max_train_id = 0;
        //    int max_path_index = 0;
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < score_list[i].Count(); j++)
        //        {
        //            if (score_list[i][j] > max_score)
        //            {
        //                max_score = score_list[i][j];
        //                max_train_id = i;
        //                max_path_index = j;
        //            }
        //        }
        //    }
        //    if (max_score == 0)
        //    {
        //        throw new Exception("Error.");
        //    }
        //    BBNode_Fix_Col fix_col = new BBNode_Fix_Col(max_train_id, max_path_index, -1,path_list[max_train_id][max_path_index].value);
        //    return fix_col;
        //}
        //public double calculate_score(double num_1,double num_2)
        //{
        //    double score=0;
        //    double mu=0.1667;
        //    if(num_1<num_2)
        //    {
        //        score = (1 - mu) * num_1 + mu * num_2;
        //    }
        //    else
        //    {
        //        score = (1 - mu) * num_2 + mu * num_1;
        //    }
        //    return score;
        //}
        //public double calculate_score_only_down(double num_1, double num_2)
        //{
        //    double score = 0;
        //    double mu = 0.1667;
        //    if (num_1 < num_2)
        //    {
        //        score = (1 - mu) * num_1 + mu * num_2;
        //    }
        //    else
        //    {
        //        score = (1 - mu) * num_2 + mu * num_1;
        //    }
        //    return score;
        //}
        #endregion

        public List<Path> get_best_path_list()
        {
            return best_path_list;
        }
        #region
        //public int pseudocost_with_strong_branch_initial(BBNode bbnode)
        //{
        //    int solve_time_lp_relation = 0;
        //    List<Path>[] path_list = bbnode.get_node_path_list();
        //    double obj_val_origin=bbnode.get_opt_obj_val();
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            Path path = path_list[i][j];
        //            if (path == null)
        //            {
        //                continue;
        //            }
        //            if (path.value_status == PathValueStatus.Fractional_more
        //                ||path.value_status==PathValueStatus.Fractional_less)
        //            {
        //                if (path.numUp < 0.5)
        //                {
        //                    double temp_obj_value=bbnode.add_cut_to_model_solve(i, j, 1);
        //                    //BBNode_Fix_Col bbnode_fix_col_up = new BBNode_Fix_Col(i,j,1);
        //                    //double temp_obj_value=bbnode.temp_solve_with_cg(bbnode_fix_col_up);
        //                    if (temp_obj_value != -1)
        //                    {
        //                        solve_time_lp_relation++;
        //                        if(temp_obj_value - obj_val_origin>0)
        //                        {
        //                            double obj_gain_temp = (temp_obj_value - obj_val_origin) / (1 - path.value);
        //                            path.sigmaUp += obj_gain_temp;
        //                            path.numUp++;
        //                        }
        //                        else
        //                        {
        //                            path.sigmaUp += 0;
        //                            path.numUp++;
        //                        }
        //                    }
        //                    bbnode.remove_special_cut_model(i,j);
        //                }

        //                if(path.numDown<0.5)
        //                {
        //                    double temp_obj_value = bbnode.add_cut_to_model_solve(i, j, 0);
        //                    //BBNode_Fix_Col bbnode_fix_col_down = new BBNode_Fix_Col(i, j, 0);
        //                    //double temp_obj_value = bbnode.temp_solve_with_cg(bbnode_fix_col_down);
        //                    if (temp_obj_value != -1)
        //                    {
        //                        solve_time_lp_relation++;                           
        //                        if (temp_obj_value - obj_val_origin>0)
        //                        {
        //                            double obj_gain_temp = (temp_obj_value - obj_val_origin) / (path.value);
        //                            path.sigmaDown += obj_gain_temp;
        //                            path.numDown++;
        //                        }
        //                        else
        //                        {
        //                            path.sigmaDown += 0;
        //                            path.numDown++;
        //                        }
        //                    }
        //                    bbnode.remove_special_cut_model(i, j);
        //                }
        //            }
        //        }
        //    }

        //    return solve_time_lp_relation;
        //}
        
        //public void update_all_pseudocost(BBNode bbnode)
        //{
        //    DataReadWrite.CleanFile("pseudocost.txt");
        //    StreamWriter sw = DataReadWrite.GetStreamWriter("pseudocost.txt");
        //    sw.WriteLine("bbnode_"+bbnode.id);
        //    List<Path>[] path_list = bbnode.get_node_path_list();
        //    double obj_val_origin = bbnode.get_opt_obj_val();
        //    for (int i = 0; i < DataReadWrite.trainnum; i++)
        //    {
        //        for (int j = 0; j < path_list[i].Count(); j++)
        //        {
        //            Path path=path_list[i][j];
        //            if (path == null)
        //            {
        //                continue;
        //            }
        //            if (path.value_status == PathValueStatus.Fractional_more
        //                || path.value_status == PathValueStatus.Fractional_less)
        //            {
        //                if(path.numUp>0.5)
        //                {
        //                    path.pseudo_cost_up = path.sigmaUp / path.numUp;
        //                    sw.WriteLine("train_" + i + ",path_" + j + ",number up " + path.numUp + ",sigma up " + path.sigmaUp);
        //                }
        //                else
        //                {
        //                    throw new Exception("Error!!");
        //                }
        //                if(path.numDown>0.5)
        //                {
        //                    path.pseudo_cost_down = path.sigmaDown / path.numDown;
        //                    sw.WriteLine("train_" + i + ",path_" + j + ",number down " + path.numDown + ",sigma down " + path.sigmaDown);
        //                }
        //                else
        //                {
        //                    throw new Exception("Error!!");
        //                }
        //            }
        //        }
        //    }
        //    sw.Close();
        //}

    //    public BBNode_Fix_Col full_strong_branch(BBNode bbnode)
    //    {
    //        List<Path>[] path_list = bbnode.get_node_path_list();
    //        List<double>[] score_list = new List<double>[DataReadWrite.trainnum];
    //        for (int i = 0; i < DataReadWrite.trainnum; i++)
    //        {
    //            score_list[i] = new List<double>();
    //            for (int j = 0; j < path_list[i].Count(); j++)
    //            {
    //                score_list[i].Add(0);
    //            }
    //        }
    //        for (int i = 0; i < DataReadWrite.trainnum; i++)
    //        {
    //            for (int j = 0; j < path_list[i].Count(); j++)
    //            {
    //                Path path = path_list[i][j];
    //                if (path == null)
    //                {
    //                    continue;
    //                }
    //                if (path.value_status == PathValueStatus.Fractional_more
    //                    || path.value_status == PathValueStatus.Fractional_less)
    //                {
    //                    double up_cut_obj_value = bbnode.add_cut_to_model_solve(i, j, 1);
    //                    bbnode.remove_special_cut_model(i, j);
    //                    double down_cut_objv_value = bbnode.add_cut_to_model_solve(i, j, 0);
    //                    bbnode.remove_special_cut_model(i, j);
    //                    score_list[i][j] = 0.16*(up_cut_obj_value-bbnode.get_opt_obj_val())/(1-path.value)+
    //                        0.84*(down_cut_objv_value-bbnode.get_opt_obj_val())/path.value;
    //                }
    //            }
    //        }

    //        return get_the_best_score(score_list,path_list);
    //    }
    //    public int get_best_fix_column_in_number(List<double> numbers)
    //    {
    //        int count = numbers.Count();
    //        if(count==0)
    //        {
    //            throw new Exception("Error!!");
    //        }
    //        int max_value_index = 0;
    //        double max_value = numbers[0];
    //        for (int i = 1; i < count; i++)
    //        {
    //            if (numbers[i] > max_value)
    //            {
    //                max_value = numbers[i];
    //                max_value_index = i;
    //            }
    //        }
    //        return max_value_index;
        //    }
        #endregion
    }
    public class BBNode
    {
        public long id;
        public double fractional_value;
        public LP_Status lp_status;
        public double opt_obj_val;
        double parent_node_opt_obj_val;
        public BBNode_Fix_Col fix_col;
        GRBModel lp_model;
        public List<Path> stock_path_list;
        public PathValueStatus[] stock_path_value_statu;
        public int max_value_flag_index=-1;
        public BBNode(GRBModel lp_model,List<Path> stock_path_list,long id)
        {
            this.id = id;
            this.stock_path_list = new List<Path>(stock_path_list);//path是同一个对象，但stock_path_list不是
            this.lp_model = lp_model;
        }
        public BBNode(long id,BBNode parent)
        {
            this.id = id;
            this.parent_node_opt_obj_val = parent.get_opt_obj_val();
            this.stock_path_list = new List<Path>(parent.stock_path_list);
            this.lp_model = new GRBModel(parent.lp_model);       
            lp_status = LP_Status.unsolve;
        }
        //设置固定值
        public void set_fix_value()
        {        
            if(fix_col!=null)
            {
                lp_model.GetVarByName(fix_col.var_name).Set(GRB.DoubleAttr.LB, fix_col.fix_path_value);
                lp_model.GetVarByName(fix_col.var_name).Set(GRB.DoubleAttr.UB, fix_col.fix_path_value);
            }
            else
            {
                throw new Exception("Error.");
            }
        }
        //在已知是整数解的情况下，获取值为1的列
        public List<Path> get_integer_solution()
        {
            opt_obj_val = lp_model.Get(GRB.DoubleAttr.ObjVal);
            List<Path> best_path_list = new List<Path>();
            foreach (Path path in stock_path_list)
            {
                if(get_variable_by_path_index(path.index).Get(GRB.DoubleAttr.X)>=0.5)
                {
                    best_path_list.Add(path);
                }
            }
            if(best_path_list.Count()==0)
            {
                throw new Exception("Error.");
            }
            return best_path_list;
        }
        //设置解的状态
        public void set_solution_and_path_value_state()
        {
            bool integet_sol = true;
            stock_path_value_statu =new PathValueStatus[stock_path_list.Count()];
            ////给目标值赋值
            opt_obj_val = lp_model.Get(GRB.DoubleAttr.ObjVal);
            int max_value = 0;
            List<double> path_value_list = new List<double>();
            foreach(Path path in stock_path_list)
            {
                //存在冲突的路径不能被选择、
                if(path.feasible==false)
                {
                    continue;
                }
                double path_value = get_variable_by_path_index(path.index).Get(GRB.DoubleAttr.X);
                

                if (1 - path_value <= 0.0001)
                {
                    stock_path_value_statu[path.index] = PathValueStatus.Interger_one;
                }
                else if (path_value <= 0.0001)
                {
                    stock_path_value_statu[path.index] = PathValueStatus.Integer_zero;
                }
                else
                {
                    integet_sol = false;
                }
                if (path_value > max_value && path_value != 1)
                {
                    max_value_flag_index = path.index;
                }
                if(path_value!=0)
                {
                    path_value_list.Add(path_value);
                }
                //else if (Math.Abs(path_value - 0.5) <= 0.1)
                //{
                //    stock_path_value_statu[path.index] = PathValueStatus.Fractional_middle;
                //    integet_sol = false;
                //}
                //else if (path_value < 0.4)
                //{
                //    stock_path_value_statu[path.index] = PathValueStatus.Fractional_less;
                //    integet_sol = false;
                //}
                //else if (path_value > 0.6)
                //{
                //    stock_path_value_statu[path.index] = PathValueStatus.Fractional_more;
                //    integet_sol = false;
                //}
            }
            if(integet_sol)
            {
                lp_status = LP_Status.opt_int;
            }
            else
            {
                lp_status = LP_Status.opt_notint;
            }
        }
        //得到变量
        public GRBVar get_variable_by_path_index(int path_index)
        {
            return lp_model.GetVarByName("f_" + path_index);
        }
        #region
        /*strong branching initialization solve*/
        //public void update_single_node_pseudocost_after_solve()
        //{
        //    if(fix_col_list==null)
        //    {
        //        return;
        //    }
        //    List<Path>[] path_list = get_node_path_list();
        //    BBNode_Fix_Col last_bbnode_fix_col = fix_col_list.Last();
        //    Path fix_path = path_list[last_bbnode_fix_col.train_id][last_bbnode_fix_col.path_index];
        //    /*up cut*/
        //    if(last_bbnode_fix_col.fix_path_value>0.5)
        //    {
        //        if(opt_obj_val>parent_node_opt_obj_val)
        //        {
        //            if (Math.Abs(last_bbnode_fix_col.parent_path_value-DataReadWrite.round_to_integer)<=0)
        //            {
        //                throw new Exception("Error.");
        //            }
        //            fix_path.sigmaUp += (opt_obj_val - parent_node_opt_obj_val) / (1 - last_bbnode_fix_col.parent_path_value);
        //            fix_path.numUp++;
        //        }               
        //    }
        //    else
        //    {
        //        if (opt_obj_val > parent_node_opt_obj_val)
        //        {
        //            fix_path.sigmaDown += (opt_obj_val - parent_node_opt_obj_val) / last_bbnode_fix_col.parent_path_value;
        //            fix_path.numDown++;
        //        } 
        //    }
        //}
    
        //public bool check_fix_path_conflict()
        //{
        //    bool result_conflict = false;
        //    if(fix_col_list==null)
        //    {
        //        return result_conflict;
        //    }
        //    for (int i = 0; i < fix_col_list.Count()-1; i++)
        //    {
        //        for (int j = i+1; j < fix_col_list.Count(); j++)
        //        {
        //            BBNode_Fix_Col fix_1 = fix_col_list[i];
        //            BBNode_Fix_Col fix_2 = fix_col_list[j];
        //            if(fix_1.Train_Value_Equal_to(fix_2))
        //            {
        //                result_conflict = true;
        //                break;
        //            }
        //            if(fix_1.Train_Path_Equal_to(fix_2))
        //            {
        //                throw new Exception("Error,Please check.");                      
        //            }
        //            if(fix_1.fix_path_value==1&&fix_2.fix_path_value==1)
        //            {
        //                result_conflict=cg.check_conflict(fix_1,fix_2);
        //                break;
        //            }
        //        }
        //        if(result_conflict==true)
        //        {
        //            break;
        //        }
        //    }
        //    return result_conflict;
        //}
        //public void add_var_fix(BBNode_Fix_Col bbnode_fix_col, int value)
        //{
        //    if (fix_col_list != null)
        //    {
        //        foreach (BBNode_Fix_Col _bbnode_fix_col in fix_col_list)
        //        {
        //            if (bbnode_fix_col.Train_Path_Equal_to(_bbnode_fix_col))
        //            {
        //                throw new Exception("Error,Has been fixed.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        fix_col_list = new List<BBNode_Fix_Col>();
        //    }
        //    bbnode_fix_col.fix_path_value = value;
        //    fix_col_list.Add(bbnode_fix_col);
        //}
        #endregion
        public LP_Status get_lp_status()
        {
            return lp_status;
        }
        public double get_opt_obj_val()
        {
            return opt_obj_val;
        }
        public GRBModel get_lp_model()
        {
            return lp_model;
        }
    }

    public enum LP_Status
    {
        unsolve,
        solve_no_optimal,
        opt_notint,
        opt_int,
        break_in_exceed_max_iteration_time,
        break_in_lpmodel_infeasibel,
        break_in_unknown_status
    }
    public class BBNode_Fix_Col
    {
        public string var_name;
        public int path_index;
        public int fix_path_value;
        public double parent_path_value;
        public BBNode_Fix_Col()
        {

        }

        public BBNode_Fix_Col(string var_name)
        {
            this.var_name = var_name;
        }
        public BBNode_Fix_Col(int path_index,int value,double parent_path_value)
        {
            this.path_index = path_index;
            this.fix_path_value = value;
            this.parent_path_value = parent_path_value;
        }
        public BBNode_Fix_Col(BBNode_Fix_Col _bbnode_fix_col)
        {
            this.path_index = _bbnode_fix_col.path_index;
            this.var_name = _bbnode_fix_col.var_name;
            //this.fix_path_value = _bbnode_fix_col.fix_path_value;
        }
       
    }
    public enum PathValueStatus { Integer_zero,Interger_one, Fractional_middle, Fractional_less,Fractional_more };
}
