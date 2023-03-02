using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.DataShow
{
    class Status
    {
        public bool importtimetime_flag;//是否导入的时刻表
        public bool conflictcheck;//是否检查了冲突
        public bool conflictshow;//当前状态是否显示了冲突
        public int schedulefor;//是晚点还是broken,若是晚点则取值为true
        public Dictionary<int, bool> DisplayModelDic;//以上显示状态的索引

        public Status()
        {
            DisplayModelDic = new Dictionary<int, bool>();
            importtimetime_flag = false;
            conflictcheck = false;
            conflictshow = false;
            schedulefor = 2;
        }
        static System.Timers.Timer timer_solving;
        static System.Windows.Forms.ToolStripTextBox textbox;
        static Stopwatch sw;
        public static void RunTimer(System.Windows.Forms.ToolStripTextBox textbox_)
        {
            textbox = textbox_;
            timer_solving = new System.Timers.Timer(1000);
            timer_solving.Elapsed += new System.Timers.ElapsedEventHandler(timer_solving_tick);
            timer_solving.AutoReset = true;

            sw = new Stopwatch();
            sw.Start();
            timer_solving.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }
        private static void timer_solving_tick(object sender, EventArgs e)
        {
            textbox.Text = sw.ElapsedMilliseconds.ToString();
        }
        public static void StopTimer()
        {
            sw.Stop();
            timer_solving.Enabled = false;
        }
    }
}
