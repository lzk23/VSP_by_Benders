using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleSys
{
    public static class DataReadWrite
    {
        public static int max_train_for_one_stock = 10;
        //配置文件的路径
        public static string paremeterfilepath = Application.StartupPath+"\\Paremeter.ini";
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        /// <summary>  
        /// 读取INI文件中指定的Key的值  
        /// </summary>  
        /// <param name="lpAppName">节点名称。如果为null,则读取INI中所有节点名称,每个节点名称之间用\0分隔</param>  
        /// <param name="lpKeyName">Key名称。如果为null,则读取INI中指定节点中的所有KEY,每个KEY之间用\0分隔</param>  
        /// <param name="lpDefault">读取失败时的默认值</param>  
        /// <param name="lpReturnedString">读取的内容缓冲区，读取之后，多余的地方使用\0填充</param>  
        /// <param name="nSize">内容缓冲区的长度</param>  
        /// <param name="lpFileName">INI文件名</param>  
        /// <returns>实际读取到的长度</returns>  
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In, Out] char[] lpReturnedString, uint nSize, string lpFileName);
        private static extern uint GetPrivateProfileString(string section, string key, string def, char[] retVal, int size, string filePath);
        /// <summary>  
        /// 获取某个指定节点(Section)中所有KEY和Value  
        /// </summary>  
        /// <param name="lpAppName">节点名称</param>  
        /// <param name="lpReturnedString">返回值的内存地址,每个之间用\0分隔</param>  
        /// <param name="nSize">内存大小(characters)</param>  
        /// <param name="lpFileName">Ini文件</param>  
        /// <returns>内容的实际长度,为0表示没有内容,为nSize-2表示内存大小不够</returns>  
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName); 
        #endregion
        /// <summary>
        /// 从INI文件中读取指定节点的内容
        /// </summary>
        /// <param name="section">INI节点</param>
        /// <param name="key">节点下的项</param>
        /// <param name="def">没有找到内容时返回的默认值</param>
        /// <param name="filePath">要读取的INI文件</param>
        /// <returns>读取的节点内容</returns>
        //public static string ReadIniData(string section, string key, string def, string filePath)
        //{
        //    StringBuilder temp = new StringBuilder(1024);
        //    GetPrivateProfileString(section, key, def, temp, 1024, filePath);
        //    return temp.ToString();
        //}
        #region 读Ini文件

        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                //if(temp.ToString()=="")
                //{
                //    throw new Exception("Please check 'section' or 'key'.");
                //}
                return temp.ToString();
            }
            else
            {
                //return String.Empty;
                throw new Exception("File not exists.");
            }
        }

        #endregion

        #region 写Ini文件,如果不存在key的话，会添加

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    //return false;
                    throw new Exception("Please check 'section' or 'key'.");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //return false;
                throw new Exception("File not exists.");
            }
        }
        #endregion
        #region 在INI文件中，指定节点写入指定的键及值。如果已经存在，则替换。如果没有则创建。
        /// <summary>  
        /// 在INI文件中，指定节点写入指定的键及值。如果已经存在，则替换。如果没有则创建。  
        /// </summary>  
        /// <param name="iniFile">INI文件</param>  
        /// <param name="section">节点</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <returns>操作是否成功</returns>  
        public static long INIWriteValue(string iniFile, string section, string key, string value)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称", "section");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("必须指定键名称", "key");
            }

            if (value == null)
            {
                throw new ArgumentException("值不能为null", "value");
            }

            return WritePrivateProfileString(section, key, value, iniFile);

        }
        #endregion
        #region 获取INI文件中指定节点(Section)中的所有条目(key=value形式)
        /// <summary>  
        /// 获取INI文件中指定节点(Section)中的所有条目(key=value形式)  
        /// </summary>  
        /// <param name="iniFile">Ini文件</param>  
        /// <param name="section">节点名称</param>  
        /// <returns>指定节点中的所有项目,没有内容返回string[0]</returns>  
        public static string[] INIGetAllItems(string iniFile, string section)
        {
            //返回值形式为 key=value,例如 Color=Red  
            uint MAX_BUFFER = 32767;    //默认为32767  

            string[] items = new string[0];      //返回值  

            //分配内存  
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));

            uint bytesReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, iniFile);

            if (!(bytesReturned == MAX_BUFFER - 2) || (bytesReturned == 0))
            {

                string returnedString = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);
                items = returnedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }

            Marshal.FreeCoTaskMem(pReturnedString);     //释放内存  

            return items;
        }
        #endregion

        /// <summary>  
        /// 获取INI文件中指定节点(Section)中的所有条目的Key列表  
        /// </summary>  
        /// <param name="iniFile">Ini文件</param>  
        /// <param name="section">节点名称</param>  
        /// <returns>如果没有内容,反回string[0]</returns>  
        public static string[] INIGetAllItemKeys(string iniFile, string section)
        {
            string[] value = new string[0];
            const int SIZE = 1024 * 10;

            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称", "section");
            }

            char[] chars = new char[SIZE];
            uint bytesReturned = GetPrivateProfileString(section, null, null, chars, SIZE, iniFile);

            if (bytesReturned != 0)
            {
                value = new string(chars).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            chars = null;

            return value;
        } 
        public static DataTable OpenCSV(string filePath)//从csv读取数据返回table
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        public static void OpenDialog(string file_path)
        {
            //打开文件对话框
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                sfd.Title = "打开文档";

                sfd.Filter = "Data Files(*.CSV)|*.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    file_path = sfd.FileName;
                }
            }         
        }
        //保存文件对话框，实际上是为了获得保存路径
        public static string SaveFileDialog()
        {
            //打开文件对话框
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save as";

                //sfd.Filter = "Picture Files(*.jpg)|*.pmg";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return null;
        }
        //public void SaveFile(string filename)
        //{
        //    SaveFileDialog savefiledialog = new SaveFileDialog();
        //    //savefiledialog.Filter = "*.jpg|*.p";
        //    savefiledialog.Title = "Save as";
        //    savefiledialog.FileName = filename;
        //    if(savefiledialog.ShowDialog()==DialogResult.OK)
        //    {
        //        if(savefiledialog.FileName=="")
        //        {
        //            MessageBox.Show("Please input file name.");
        //            return;
        //        }
        //    }
            
        //    //try
        //    //{
        //    //    Stream stream = File.OpenWrite(filename);
        //    //    using(StreamWriter writer=new StreamWriter(stream))
        //    //    {
        //    //        writer.Write();
        //    //    }
        //    //}
        //    //catch(IOException ex)
        //    //{
        //    //    MessageBox.Show(ex.Message, "Simple Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //    //}
        //}
        //读取初始时刻表数据
        public static void ImportTrainTimetable(string filepath,Graph g)
        {
            System.Text.Encoding encoding = GetType(filepath); //Encoding.ASCII
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //时间格式
            int minuteformatornot = int.Parse(DataReadWrite.ReadIniData("TimeFormat", "MinuteORNot", "", Application.StartupPath + "\\Paremeter.ini"));
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    IsFirst = false;
                }
                else
                {
                    aryLine = strLine.Split(',');
                    string train_name = aryLine[0];
                    string station_name = aryLine[1];
                    if (ReadTrainOrNot(train_name) == false)
                    {
                        continue;
                    }
                    if(g.GetArriveTimeByTrainStation(train_name, g.stationlist.Last().name) != -1)
                    {
                        continue;
                    }
                    //获取到达和离开时间、最小停站时间
                    int arrive_time = 0;//到达时间
                    int departure_time = 0 ;//离开时间
                    if(minuteformatornot==1)
                    {
                        arrive_time = g.TimeHMSChangeToMinute(aryLine[2]);//到达时间
                        departure_time = g.TimeHMSChangeToMinute(aryLine[3]); ;//离开时间
                    }
                    else
                    {
                        arrive_time = int.Parse(aryLine[2]) * 60;//到达时间
                        departure_time = int.Parse(aryLine[3]) * 60; ;//离开时间
                    }
                    int direction = int.Parse(aryLine[4]);
                    int dwell_time = Convert.ToInt32((departure_time-arrive_time));
                    if(train_name=="116")
                    {
                        int a = 0;
                        a = 1;
                    }
                    if(g.GetTrainByName(train_name)==null)
                    {
                        //创建列车
                        Train train = new Train();
                        train.name = train_name;
                        train.direction = direction;
                        train.begin_station_name = station_name;
                        //确定列车的类型
                        train.train_type = g.GetTrainTypeByName(train_name);
                        int index = g.trainlist.Count();
                        train.index = index;
                        g.trainlist.Add(train);
                        g.trainnametotrain.Add(train_name, train);
                    }              
                    //列车所经过的车站集合
                    Train _train = g.GetTrainByName(train_name);
                    //if (ReadStationOrNot(station_name) == false)
                    //{
                    //    string last_station_name=_train.past_stop_stations_name.Last();
                    //    int last_departuretime = g.GetDepartureTimeByTrainStation(train_name, last_station_name);
                    //    float last_mile = stationnametomile[last_station_name];//有记录的前一个车站的相关数据

                    //    string end_station_name = g.stationlist.Last().name;
                    //    float end_mile = stationnametomile[end_station_name];//所有需要读入车站的最后一个车站

                    //    float current_station_mile=stationnametomile[station_name];//当前所读到的车站

                    //    int end_time =(int) (((end_mile - last_mile) / (current_station_mile - last_mile)) * (arrive_time - last_departuretime) + last_departuretime);
                    //    //更改信息
                    //    station_name = end_station_name;
                    //    arrive_time = end_time;
                    //}
                    if (g.GetStationByName(station_name) == null)
                    {
                        throw new Exception("给定车站不在站名字典中");
                    }
                    _train.past_stations_name.Add(station_name);
                    //已经存在该key,则数据重复
                    if (g.GetArriveTimeByTrainStation(train_name,station_name) != -1)
                    {
                        throw new Exception("数据重复");
                    }                   
                    //否则创建该列车在这个车站的到发时刻、最小停站时间
                    else
                    {
                        KeyValuePair<string, string> key = new KeyValuePair<string, string>(train_name, station_name);
                        g.trainstationtoarrivetime.Add(key, arrive_time);
                        g.trainstationtodeparturetime.Add(key, departure_time);
                        g.trainstationtominimizedwelltime.Add(key, dwell_time);
                    }
                }
            }
            sr.Close();
            fs.Close();
            foreach (Train train in g.trainlist)
            {
                //train.end_station_name = train.past_stations_name.Last();
                train.begin_station_name = train.past_stations_name.First();
                train.end_station_name = train.past_stations_name.Last();

                ////根据列车的始末车站得到列车所经过的车站
                //Station start_station = g.GetStationByName(train.begin_station_name);
                //Station end_station = g.GetStationByName(train.end_station_name);
                //for (int i = start_station.index; i <= end_station.index; i++)
                //{
                //    train.past_stations_name.Add(g.stationlist[i].name);
                //}

                ////生成没有停站的车站的到发时刻
                //for (int i = 0; i < train.past_stop_stations_name.Count() - 1; i++)
                //{
                //    Station firststation = g.GetStationByName(train.past_stop_stations_name[i]);
                //    Station secondstation = g.GetStationByName(train.past_stop_stations_name[i + 1]);

                //    float deltax2 = secondstation.mileage - firststation.mileage;
                //    int deltatime2 = g.GetArriveTimeByTrainStation(train.name, secondstation.name) - g.GetDepartureTimeByTrainStation(train.name, firststation.name);
                //    for (int j = firststation.index + 1; j <= secondstation.index - 1; j++)
                //    {
                //        Station currentstation = g.stationlist[j];
                //        float deltax1 = currentstation.mileage - firststation.mileage;
                //        int arriveanddeparttime = Convert.ToInt32(g.GetDepartureTimeByTrainStation(train.name, firststation.name) + (deltax1 / deltax2) * deltatime2);
                //        KeyValuePair<string, string> key = new KeyValuePair<string, string>(train.name, currentstation.name);
                //        g.trainstationtoarrivetime.Add(key, arriveanddeparttime);
                //        g.trainstationtodeparturetime.Add(key, arriveanddeparttime);
                //        g.trainstationtominimizedwelltime.Add(key, 0);
                //    }
                //}
            }
            //清空数据
            stationnametomile.Clear();
        }
        //读取动车组数据
        public static void ImportStock(string filepath,Graph g)
        {
            System.Text.Encoding encoding = GetType(filepath); //Encoding.ASCII
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    IsFirst = false;
                }
                else
                {
                    aryLine = strLine.Split(',');
                    string stock_name = aryLine[0];
                    int cost = int.Parse(aryLine[1]);
                    int undertake_train_num = int.Parse(aryLine[2]);
                    Stock stock = new Stock();
                    stock.name = stock_name;
                    stock.cost = cost;
                    stock.max_undertake_train_num = undertake_train_num;
                    stock.depot = int.Parse(aryLine[3]);
                    int index = g.stocklist.Count();
                    stock.index = index;
                    g.stocklist.Add(stock);
                }
            }
            sr.Close();
            fs.Close();
            List<int[]> stock_type_list = new List<int[]>();
            //统计每种动车组分别有多少列
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach(Stock stock in g.stocklist)
            {
                int max_undertake_num = stock.max_undertake_train_num;
                if (dict.ContainsKey(max_undertake_num))
                {
                    dict[max_undertake_num]++;
                }
                else
                {
                    dict.Add(max_undertake_num, 1);
                    g.stock_cost_dict.Add(max_undertake_num,stock.cost);//添加cost的数据
                }


            }
            //把字典转成list
            foreach(KeyValuePair<int,int> pair in dict)
            {
                int[] element = new int[2];
                element[0] = pair.Key;
                element[1] = pair.Value;
                g.stock_type_list.Add(element);
            }


            //var minkeys = g.stock_type_list.Keys.Select(x => 
            //    new { x, y = g.stock_type_list[x] }).GroupBy(x => x.y).OrderBy(x => x.Key).First().Select(x => x.x);


        }

        //读取车辆段数据
        public static void ImportDepot(Graph g)
        {
            Depot depot_1 = new Depot(0);
            Depot depot_2 = new Depot(1);
            g.depotlist.Add(depot_1);
            g.depotlist.Add(depot_2);
        }

        //读取维修作业数据
        public static void ImportMaintenance(string filepath, Graph g)
        {
            System.Text.Encoding encoding = GetType(filepath); //Encoding.ASCII
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //时间格式
            int minuteformatornot = int.Parse(DataReadWrite.ReadIniData("TimeFormat", "MinuteORNot", "", Application.StartupPath + "\\Paremeter.ini"));
            int mostmaintenancenum = int.Parse(DataReadWrite.Read("maintenance","number"));
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    IsFirst = false;
                }
                else
                {
                    aryLine = strLine.Split(',');
                    if(g.GetStationByName(aryLine[6])!=null&&aryLine[6]!=g.stationlist.Last().name)
                    {
                        Maintenance m = new Maintenance();
                        m.id = int.Parse(aryLine[0]);
                        m.earlieststart = g.TimeHMSChangeToMinute(aryLine[1]);
                        m.lasteststart = g.TimeHMSChangeToMinute(aryLine[2]);
                        m.minimumduring = int.Parse(aryLine[3]);
                        m.firstrainspeedlimit = int.Parse(aryLine[4]);
                        m.secondtrainspeedlimit = int.Parse(aryLine[5]);
                        m.startstationname = aryLine[6];
                        g.maintenancelist.Add(m);

                        if(g.maintenancelist.Count()==mostmaintenancenum)
                        {
                            break;
                        }
                    }                
                }
            }
            sr.Close();
            fs.Close();
        }

        //判断列车是否需要读取
        public static bool ReadTrainOrNot(string train_name)
        {
            bool result=false;
            string value=ReadIniData("SolvingSizeTrain",train_name,"",Application.StartupPath+"\\Paremeter.ini");
            if(value=="1")
            {
                result=true;
            }
            else if(value==null)
            {
                throw new Exception("The train_name is not in ini file.");
            }
            else if(value=="0")
            {
                result=false;
            }
            return result;
        }
        //判断车站是否需要添加
        public static bool ReadStationOrNot(string station_name)
        {
            bool result = false;
            string value = ReadIniData("SolvingSizeStation", station_name, "", Application.StartupPath + "\\Paremeter.ini");
            if (value == "1")
            {
                result = true;
            }
            else if (value == null)
            {
                throw new Exception("The station_name is not in ini file.");
            }
            else if (value == "0")
            {
                result = false;
            }
            return result;
        }
        //读取车站的数据
        //保存所有车站（不论是否需要读入）的里程
        static Dictionary<string, float> stationnametomile = new Dictionary<string, float>();
        public static void ImportStation(string filepath, Graph g)
        {
            System.Text.Encoding encoding = GetType(filepath); //Encoding.ASCII;//
            //DataTable dt = new DataTable();
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //时间格式
            int minuteformatornot = int.Parse(DataReadWrite.ReadIniData("TimeFormat", "MinuteORNot", "", Application.StartupPath + "\\Paremeter.ini"));
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    IsFirst = false;
                }
                else
                {
                    aryLine = strLine.Split(',');
                    string station_name = aryLine[0];
                    string to_next_station_distance = aryLine[1]==""?"-1":aryLine[1];
                    string strmileage = aryLine[2];
                    //if(ReadStationOrNot(station_name)==false)
                    //{
                    //    stationnametomile.Add(station_name, float.Parse(strmileage));
                    //    continue;
                    //    //break;//
                    //}
                    stationnametomile.Add(station_name, float.Parse(strmileage));
                    int tracknum = int.Parse(aryLine[8]);
                    if (g.GetStationByName(station_name) == null)
                    {
                        //创建车站
                        Station station = new Station();
                        station.name = station_name;
                        station.tonextstationdistance = float.Parse(to_next_station_distance);
                        station.mileage = float.Parse(strmileage);
                        int index = g.stationlist.Count();
                        station.index = index;//车站的index
                        station.tracknumber = tracknum;//最多的股道数量
                        if(tracknum>g.maxtracknum)
                        {
                            g.maxtracknum = tracknum;
                        }
                        g.stationlist.Add(station);
                        g.stationnametostation.Add(station_name, station);
                        //添加站名到索引的字典数据
                        g.stationnametoindex.Add(station_name, index);
                    }
                    else
                    {
                        throw new Exception("站名重复");
                    }
                    //添加各类车的区间运行时分
                    //g.AddStationTrainTypeToNextRunTime(station_name, 0, timems_1);
                    //g.AddStationTrainTypeToNextRunTime(station_name, 1, timems_2);
                    //g.AddStationTrainTypeToNextRunTime(station_name, 2, timems_3);
                    //g.AddStationTrainTypeToNextRunTime(station_name, 3, timems_4);
                    //g.AddStationTrainTypeToNextRunTime(station_name, 4, timems_5);
                }
            }
            sr.Close();
            fs.Close();

            ////给车站的下一个车站的属性名赋值
            //for(int station_index=0;station_index<g.stationlist.Count()-1;station_index++)
            //{
            //    Station station = g.stationlist[station_index];
            //    Station nextstation = g.stationlist[station_index + 1];
            //    station.nextstationname = nextstation.name;
            //}
        }

        //设置车头间距
        public static void SetParemeter(Graph g)
        {
            //g.differentdirctionmeeting = int.Parse(DataReadWrite.ReadIniData("Headway", "Departure_Arrive_H", "", Application.StartupPath + "\\Paremeter.ini"));
            //g.samedirectionsetout = int.Parse(DataReadWrite.ReadIniData("Headway", "Departure_Arrive_L", "", Application.StartupPath + "\\Paremeter.ini"));
            g.departuredeparturehead = int.Parse(DataReadWrite.ReadIniData("Headway", "Departure_Departure", "", Application.StartupPath + "\\Paremeter.ini"));
            g.arrivearrivehead = int.Parse(DataReadWrite.ReadIniData("Headway", "Arrive_Arrive", "", Application.StartupPath + "\\Paremeter.ini"));
            //g.arrivedeparture = int.Parse(DataReadWrite.ReadIniData("Headway", "Arrive_Departure", "", Application.StartupPath + "\\Paremeter.ini"));
            //g.accelerate = int.Parse(DataReadWrite.ReadIniData("Acelerate", "acelerate", "", Application.StartupPath + "\\Paremeter.ini")) ;
            //g.decelerate = int.Parse(DataReadWrite.ReadIniData("Acelerate", "decelerate", "", Application.StartupPath + "\\Paremeter.ini"));
            //g.speed_reduce_number = int.Parse(DataReadWrite.ReadIniData("SpeedReduce", "number", "", Application.StartupPath + "\\Paremeter.ini"));
            
            //g.mostmaintenancenumber = int.Parse(DataReadWrite.ReadIniData("Maintenance", "number", "", Application.StartupPath + "\\Paremeter.ini"));
        }
        //设置晚点信息
        //public static void SetDelay(Model.GurobiDoubleTrackModel model)
        //{
        //    model.delaytrainname = DataReadWrite.ReadIniData("Delay", "trainname", "", Application.StartupPath + "\\Paremeter.ini");
        //    model.delaytime =int.Parse(DataReadWrite.ReadIniData("Delay", "delaytime", "", Application.StartupPath + "\\Paremeter.ini"));
        //    model.delayarrivestationname = DataReadWrite.ReadIniData("Delay", "stationname", "", Application.StartupPath + "\\Paremeter.ini");
        //}
        //设置故障信息
        //public static void SetBroken(Model.GurobiDoubleTrackModel model)
        //{
        //    model.disturbancefirststation = DataReadWrite.ReadIniData("Disturbance", "firststation", "", Application.StartupPath + "\\Paremeter.ini");
        //    model.disturbancestarttime = int.Parse(DataReadWrite.ReadIniData("Disturbance", "begintime", "", Application.StartupPath + "\\Paremeter.ini"));
        //    model.disturbanceendtime = int.Parse(DataReadWrite.ReadIniData("Disturbance", "endtime", "", Application.StartupPath + "\\Paremeter.ini"));
        //}
        //设置维修作业时间窗信息
        public static Maintenance ReadMaintancewindow()
        {
            Maintenance maintenace = new Maintenance();
            maintenace.earlieststart = int.Parse(DataReadWrite.ReadIniData("Maintenance", "earlieststart", "", DataReadWrite.paremeterfilepath));
            maintenace.lasteststart = int.Parse(DataReadWrite.ReadIniData("Maintenance", "lasteststart", "", DataReadWrite.paremeterfilepath));
            maintenace.minimumduring = int.Parse(DataReadWrite.ReadIniData("Maintenance", "minimumduring", "", DataReadWrite.paremeterfilepath));
            maintenace.startstationname = DataReadWrite.ReadIniData("Maintenance", "startstationname", "", DataReadWrite.paremeterfilepath);
            maintenace.firstrainspeedlimit = int.Parse(DataReadWrite.ReadIniData("Maintenance", "firsttrainspeedlimit", "",
                DataReadWrite.paremeterfilepath));
            maintenace.secondtrainspeedlimit = int.Parse(DataReadWrite.ReadIniData("Maintenance", "secondtrainspeedlimit", "",
                DataReadWrite.paremeterfilepath));
            return maintenace;
        }
       
        public static BrokenClass ReadBroken()
        {
            string firststation= DataReadWrite.ReadIniData("Disturbance", "firststation", "", Application.StartupPath + "\\Paremeter.ini");
            int begintime = int.Parse(DataReadWrite.ReadIniData("Disturbance", "begintime", "", Application.StartupPath + "\\Paremeter.ini"));
            int endtime = int.Parse(DataReadWrite.ReadIniData("Disturbance", "endtime", "", Application.StartupPath + "\\Paremeter.ini"));
            return new BrokenClass(firststation, begintime, endtime);
        }
        
        //清空文本的数据
        public static void ClearText(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Write);
                fs.SetLength(0);
                fs.Close();
            }
            catch (IOException e)
            {
                Console.Write(e.Message);
            }
        }

        //将时刻表数据输出到datagriview
        public static void ShowTimetable(Graph g,DataGridView datagriview)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Train_name");
            dt.Columns.Add("Station_name");
            dt.Columns.Add("Arrive_time");
            dt.Columns.Add("Departure_time");
            dt.Columns.Add("AT_XX:XX:XX");
            dt.Columns.Add("DT_XX:XX:XX");
            foreach(Train train in g.trainlist)
            {
                foreach(string station_name in train.past_stations_name)
                {
                    DataRow dr = dt.NewRow();
                    dr["Train_name"] = train.name;
                    dr["Station_name"] = station_name;
                    dr["Arrive_time"] = g.GetArriveTimeByTrainStation(train.name, station_name);
                    dr["Departure_time"] = g.GetDepartureTimeByTrainStation(train.name, station_name);
                    dr["AT_XX:XX:XX"]=g.MinuteChangeToTimeHM(g.GetArriveTimeByTrainStation(train.name, station_name)).ToString();
                    dr["DT_XX:XX:XX"] = g.MinuteChangeToTimeHM(g.GetDepartureTimeByTrainStation(train.name, station_name)).ToString();
                    dt.Rows.Add(dr);
                }
                datagriview.DataSource = dt;
            }
        }
        public static void ShowTimetable(Graph new_g,Graph old_g, DataGridView datagriview)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Train_name");
            dt.Columns.Add("Station_name");
            dt.Columns.Add("Arrive_time");
            dt.Columns.Add("Departure_time");
            dt.Columns.Add("AT_differ");
            dt.Columns.Add("DT_differ");
            foreach (Train train in new_g.trainlist)
            {
                foreach (string station_name in train.past_stations_name)
                {
                    DataRow dr = dt.NewRow();
                    dr["Train_name"] = train.name+"(T"+train.index+")";
                    dr["Station_name"] = station_name;
                    dr["Arrive_time"] = new_g.GetArriveTimeByTrainStation(train.name, station_name);
                    dr["Departure_time"] = new_g.GetDepartureTimeByTrainStation(train.name, station_name);
                    dr["AT_differ"] = new_g.GetArriveTimeByTrainStation(train.name, station_name) - old_g.GetArriveTimeByTrainStation(train.name, station_name);
                    dr["DT_differ"] = new_g.GetDepartureTimeByTrainStation(train.name, station_name) - old_g.GetDepartureTimeByTrainStation(train.name, station_name);
                    dt.Rows.Add(dr);
                }
                datagriview.DataSource = dt;
            }
        }
        //写入求解时间
        public static void SaveSolvingTime(string key,double time)
        {
            DataReadWrite.WriteIniData("SolvingTime", key, time.ToString(), Application.StartupPath + "\\Paremeter.ini");
        }
        //读取gurobi输出结果out.sol的数据
        public static void ReadSolFile(Dictionary<string,double> vardic)
        {
            try
            {
                TextReader bufread = new StreamReader("out.sol");
                bool is_first_line=true;
                string line = bufread.ReadLine();
                while(line!=null)
                {
                    if (line.Trim().Equals(""))
                    {
                        line = bufread.ReadLine();
                        continue;
                    }
                    if(is_first_line)
                    {
                        is_first_line=false;
                    }                   
                    else
                    {
                        string[] keyvalue = line.Split(' ');
                        string key = keyvalue[0];
                        double value = double.Parse(keyvalue[1]);
                        vardic.Add(key, value);
                    }
                    line = bufread.ReadLine();
                }
                bufread.Close();
            }
            catch(IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        //保存图片
        public static void SavePicture(PictureBox pb)
        {
            try
            {
                string file_path = DataReadWrite.SaveFileDialog();
                if (file_path == "")
                {
                    return;
                }
                pb.Image.Save(file_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //输出时刻表数据到csv
        public static void SaveTimetable(Graph g,Graph new_g,string filename)
        {
            try
            {
                ClearText(filename);
                //string file_path = DataReadWrite.SaveFileDialog();
                //if (file_path == "")
                //{
                //    return;
                //}
                StreamWriter wr = GetStreamWriter(filename);
                wr.WriteLine("trainname,station,arrivetime,departuretime,train_direction");
                if (new_g != null)
                {
                    foreach (Train train in g.trainlist)
                    {
                        int departure_time = new_g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name);
                        int arrive_time = new_g.GetArriveTimeByTrainStation(train.name, train.end_station_name);
                        TimeHM departure_time_hm = g.MinuteChangeToTimeHM(departure_time);
                        TimeHM arrive_time_hm = g.MinuteChangeToTimeHM(arrive_time);
                        wr.WriteLine(train.name + "," + train.begin_station_name + "," + null + "," + departure_time_hm.ToString() + "," + train.direction);
                        wr.WriteLine(train.name + "," + train.end_station_name + "," + arrive_time_hm.ToString() + "," + null + "," + train.direction);
                    }
                }
                
                wr.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //读数据
        public static string Read(string section,string key)
        {
            return DataReadWrite.ReadIniData(section,key,"",paremeterfilepath);
        }
        //输出时刻表、使用动车组关系到文件，做为模型的初始解
        public static void Savevariable(Graph g,Graph new_g)
        {
            StreamWriter sw = GetStreamWriter("OutputfromBDtoinitialmodelsolve_stock.txt");
            foreach (Train train in g.trainlist)
            {
                sw.WriteLine(train.index + "," + train.stock_index);
            }
            sw.Close();
            sw = GetStreamWriter("OutputfromBDtoinitialmodelsolve_timetable.txt");
            foreach (Train train in g.trainlist)
            {
                sw.WriteLine(train.index + "," + new_g.GetDepartureTimeByTrainStation(train.name, train.begin_station_name)
                    + "," + new_g.GetArriveTimeByTrainStation(train.name, train.end_station_name));
            }
            sw.Close();
        }
        //读取变量信息
        public static int[,] Get_train_use_stock_variable(Graph g)
        {
            StreamReader sr=GetStreamReader("OutputfromBDtoinitialmodelsolve_stock.txt");
            int[,] result = new int[g.stocklist.Count(), g.trainlist.Count()];
            string line="";
            while((line=sr.ReadLine())!=null)
            {
                string[] str_list = line.Split(',');
                int train_index = int.Parse(str_list[0]);
                int stock_index = int.Parse(str_list[1]);
                result[stock_index, train_index] = 1;
            }
            sr.Close();
            return result;
        }

        public static int[,] Get_initial_timetable(Graph g)
        {
            StreamReader sr = GetStreamReader("OutputfromBDtoinitialmodelsolve_timetable.txt");
            int[,] result = new int[g.trainlist.Count(), 2];
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] str_list = line.Split(',');
                int train_index = int.Parse(str_list[0]);
                int departure_time = int.Parse(str_list[1]);
                int arrival_time = int.Parse(str_list[2]);
                result[train_index, 0] = departure_time;
                result[train_index, 1] = arrival_time;
            }
            sr.Close();
            return result;
        }
        //写数据
        public static void Write(string section,string key,string value)
        {
            DataReadWrite.WriteIniData(section, key, value, paremeterfilepath);
        }
        //public static System.Text.Encoding encoding = GetType("Timetable.csv"); //Encoding.ASCII;//;
        public static StreamWriter GetStreamWriter(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, System.IO.FileMode.Open,
                    System.IO.FileAccess.Write);
                StreamWriter wr = new StreamWriter(fs, GetType("Timetable.csv"));
                return wr;
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }
        public static StreamReader GetStreamReader(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, System.IO.FileMode.Open,
                    System.IO.FileAccess.Read);

                StreamReader sr = new StreamReader(fs, GetType("Timetable.csv"));
                return sr;
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }

        public static StreamWriter CleanAndAppendText(string filename)
        {
            try
            {
                CleanFile(filename);
                StreamWriter sw_AT = new StreamWriter(filename, true, GetType("Timetable.csv"));
                return sw_AT;
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void CleanFile(string filename)
        {
            try
            {
                FileStream fs_AT = new FileStream(filename, FileMode.Open, FileAccess.Write);
                fs_AT.SetLength(0);//清空数据
                fs_AT.Close();
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
