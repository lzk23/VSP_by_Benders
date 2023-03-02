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
    public partial class SelectSolveMethod : Form
    {
        public solving_method method;
        
        public SelectSolveMethod()
        {
            InitializeComponent();

            checkedListBox_solvingmethod.Items.Add("Model_solve", false);
            checkedListBox_solvingmethod.Items.Add("Branch_price_cut_solve", false);
            //checkBox_none_benders.Checked = false;
            //checkBox_station_capacity.Checked = true;            
            //checkBox_slack.Checked = false;
            //checkBox_both.Checked = false;
            //benderskind = BendersKind.none_benders;
            
            //textBox_span.Text = DataReadWrite.ReadIniData("Optimal", "Span","",DataReadWrite.paremeterfilepath);//获得
            ////groupBox_checkbox.Controls.Add(checkBox_both);
            ////groupBox_checkbox.Controls.Add(checkBox_headway_kind_separete);
            ////groupBox_checkbox.Controls.Add(checkBox_station_capacity);
        }
        //第一种模型求解
        //private void button_gurobi_Click(object sender, EventArgs e)
        //{
        //    gurobimodelonesolve = true;
        //    this.Close();
        //}
        ////第二种模型求解
        //private void button_gurobi_model_two_Click(object sender, EventArgs e)
        //{
        //    gurobimodeltwosolve = true;
        //    this.Close();
        //}
        //private void button_gurobi_benders_two_Click(object sender, EventArgs e)
        //{
        //    ChechSelectDetail();
        //    gurobiandbenderssolvetwo = true;
        //    this.Close();
        //}
        //private void button_gurobi_benders_Click(object sender, EventArgs e)
        //{
        //    ChechSelectDetail();
        //    gurobiandbenderssolve = true;
            
        //    this.Close();
        //}
        private void ChechSelectDetail()
        {
            int selectcount = 0;
            for (int item_index = 0; item_index < checkedListBox_solvingmethod.Items.Count; item_index++)
            {
                if (checkedListBox_solvingmethod.GetItemChecked(item_index) == true)
                {
                    selectcount++;
                }
            }

            //foreach (Control con in checkedListBox_solvingmethod.Controls)
            //{
            //    if (((CheckBox)con).Checked == true && ((CheckBox)con).Name != "checkBox_slack")
            //    {
            //        selectcount++;
            //    }
            //}
            if (selectcount == 0)
            {
                MessageBox.Show("Please select a mode.");
                return;
            }
            if (selectcount > 1)
            {
                MessageBox.Show("Only one mode is allowed.");
                return;
            }
            for (int item_index = 0; item_index < checkedListBox_solvingmethod.Items.Count; item_index++)
            {
                if (checkedListBox_solvingmethod.GetItemChecked(item_index) == true)
                {
                    switch ((string)checkedListBox_solvingmethod.Items[item_index])
                    {
                        case "Model_solve":
                            method = solving_method.Model_solve;
                            break;
                        case "Branch_price_cut_solve":
                            method = solving_method.Branch_price_cut_solve;
                            break;

                    }
                }
            }
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            ChechSelectDetail();
            this.Close();
        }

    }

    public enum solving_method:int
    {
        close=0,//关闭窗体
        Model_solve=1,
        Branch_price_cut_solve,
    }
}
