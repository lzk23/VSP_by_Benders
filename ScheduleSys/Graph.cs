using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys
{
    public class Graph
    {
        public List<Train> trainlist;
        public List<Station> stationlist;
        public List<Stock> stocklist;
        public List<Depot> depotlist;
        public List<Maintenance> maintenancelist;
        public Dictionary<KeyValuePair<string, string>, int> trainstationtoarrivetime;
        public Dictionary<KeyValuePair<string, string>, int> trainstationtodeparturetime;
        public Dictionary<string, Train> trainnametotrain;
        public Dictionary<string, Station> stationnametostation;
        public Dictionary<KeyValuePair<string, int>, int> stationtraintypetonextruntime;
        public Dictionary<KeyValuePair<string, string>, int> trainstationtominimizedwelltime;
        public Dictionary<string, int> stationnametoindex;

        public List<int[]> stock_type_list;//每种动车组（以所能承担的数量为依据）分别有多少列
        public Dictionary<int, int> stock_cost_dict;

        public List<int[]> follow_list;
        public List<int[]> opp_list;

        public int maxtracknum;//最大的股道数量
        //车头间距参数
        public int differentdirctionmeeting;//会车
        public int samedirectionsetout;//连发
        public int departuredeparturehead;//不同时离开
        public int arrivearrivehead;//不同时到达
        public int arrivedeparture;
        public int accelerate;//加速
        public int decelerate;//减速
        public int speed_reduce_number;
        public int mostmaintenancenumber;
        //运行时间压缩
        public float runtimerateofcompression = 0;
        //public void Setcoefruntime(float value)
        //{
        //    runtimerateofcompression = value;
        //}

        //构建网络的存储变量
        //public Dictionary<int, List<int>> outnodelistdict;
        //public Dictionary<int, List<int>> intnodelistdict;

        public Graph()
        {
            trainlist = new List<Train>();
            stationlist = new List<Station>();
            stocklist = new List<Stock>();
            depotlist = new List<Depot>();
            maintenancelist = new List<Maintenance>();
            trainstationtoarrivetime = new Dictionary<KeyValuePair<string, string>, int>();
            trainstationtodeparturetime = new Dictionary<KeyValuePair<string, string>, int>();
            trainnametotrain = new Dictionary<string, Train>();
            stationnametostation = new Dictionary<string, Station>();
            stationtraintypetonextruntime = new Dictionary<KeyValuePair<string, int>, int>();
            trainstationtominimizedwelltime = new Dictionary<KeyValuePair<string, string>, int>();
            stationnametoindex = new Dictionary<string, int>();
            maxtracknum = 0;
            stock_type_list = new List<int[]>();
            stock_cost_dict = new Dictionary<int, int>();
        }
        public Graph(Graph g_)
        {
            trainlist = new List<Train>(g_.trainlist);
            stationlist = new List<Station>(g_.stationlist);
            
           // maintenancelist = new List<Maintenance>(g_.maintenancelist);

            trainstationtoarrivetime = new Dictionary<KeyValuePair<string, string>, int>();
            trainstationtodeparturetime = new Dictionary<KeyValuePair<string, string>, int>();

            trainnametotrain = new Dictionary<string, Train>(g_.trainnametotrain);
            stationnametostation = new Dictionary<string, Station>(g_.stationnametostation);
            stationtraintypetonextruntime = new Dictionary<KeyValuePair<string, int>, int>(g_.stationtraintypetonextruntime);
            trainstationtominimizedwelltime = new Dictionary<KeyValuePair<string, string>, int>(g_.trainstationtominimizedwelltime);
            stationnametoindex = new Dictionary<string, int>();

            follow_list = new List<int[]>();
            opp_list = new List<int[]>();

            //GenerationTrainPair();
        }
        //
        public void UpdateTime(Graph new_g)
        {
            this.trainstationtodeparturetime = new_g.trainstationtodeparturetime;
            this.trainstationtoarrivetime = new_g.trainstationtoarrivetime;
        }
        public double[] ChangeArriveTimeformat(Station station)
        {
            double[] result = new double[trainlist.Count()];
            foreach(Train train in station.arrive_train_list)
            {
                result[train.index] = GetArriveTimeByTrainStation(train.name,station.name);
            }
            return result;
        }
        public double[] ChangeDepartureTimeformat(Station station)
        {
            double[] result = new double[trainlist.Count()];
            foreach(Train train in station.departure_train_list)
            {
                result[train.index] = GetDepartureTimeByTrainStation(train.name,station.name);
            }
            return result;
        }
        //仅包含时间不包含其它信息
        public Graph(bool onlytime)
        {
            trainstationtoarrivetime = new Dictionary<KeyValuePair<string, string>, int>();
            trainstationtodeparturetime = new Dictionary<KeyValuePair<string, string>, int>();
        }
        public static void ImportTrainTimetable()
        {

        }
        //由列车名得到列车
        public Train GetTrainByName(string trainname)
        {
            return trainnametotrain.ContainsKey(trainname) ?
                trainnametotrain[trainname] : null;
        }
        //由车站名得到车站
        public Station GetStationByName(string stationname)
        {
            return stationnametostation.ContainsKey(stationname) ?
                stationnametostation[stationname] : null;
        }
        //由列车和车站得到到达和离开时间
        public int GetArriveTimeByTrainStation(string train_name,string station_name)
        {
            KeyValuePair<string, string> key = new KeyValuePair<string, string>(train_name,station_name);
            return trainstationtoarrivetime.ContainsKey(key) ? trainstationtoarrivetime[key] : -1;
        }
        public int GetDepartureTimeByTrainStation(string train_name, string station_name)
        {
            KeyValuePair<string, string> key = new KeyValuePair<string, string>(train_name, station_name);
            return trainstationtodeparturetime.ContainsKey(key) ? trainstationtodeparturetime[key] : -1;
        }
        //由车站和列车类型得到下一个车站的行驶时间
        public int GetToNextStationRunTime(string station_name,int train_type)
        {
            KeyValuePair<string,int> key=new KeyValuePair<string,int>(station_name,train_type);
            return stationtraintypetonextruntime.ContainsKey(key)?stationtraintypetonextruntime[key]:-1;
        }
        public int GetStationMinimizeDwellingTime(string train_name,string station_name)
        {
            KeyValuePair<string, string> key = new KeyValuePair<string, string>(train_name, station_name);
            if (trainstationtominimizedwelltime.ContainsKey(key))
            {
                return trainstationtominimizedwelltime[key];
            }
            else
            {
                throw new Exception("key不存在");
            }
        }
        //得到离开的顺序
        public int GetDepartureOrder(string train_name_1,string train_name_2,string station_name)
        {
            int result;
            int time_1 = GetDepartureTimeByTrainStation(train_name_1, station_name);
            int time_2 = GetDepartureTimeByTrainStation(train_name_2, station_name);
            if(time_1>time_2)
            {
                result=1;
            }
            else
            {
                result = 0;
            }
            return result;
        }
        //得到到达的顺序
        public int GetArriveOrder(string train_name_1,string train_name_2,string station_name)
        {
            int result;
            int time_1 = GetArriveTimeByTrainStation(train_name_1, station_name);
            int time_2 = GetArriveTimeByTrainStation(train_name_2, station_name);
            if (time_1 > time_2)
            {
                result = 1;
            }
            else
            {
                result = 0;
            }
            return result;
        }
        //得到区间的运行时间
        public int GetRunSectionTime(Station station,Train train)
        {
            if(station.name==train.past_stations_name.Last())
            {
                throw new Exception("error");
            }
            string nextstation_name = Getnextstationbytrainandstation(train, station.name);
            return (int)((GetArriveTimeByTrainStation(train.name, nextstation_name) - GetDepartureTimeByTrainStation(train.name, station.name))
               -runtimerateofcompression);
        }
        //对列车在车站的类型进行分类
        public void SeparetTrainInStation()
        {
            foreach(Station station in stationlist)
            {
                station.origin_train_list = new List<Train>();
                station.destination_train_list = new List<Train>();
                station.past_train_list = new List<Train>();
                station.arrive_train_list = new List<Train>();
                //station.exclude_origin_train_list = new List<Train>();
                station.departure_train_list = new List<Train>();
                foreach(Train train in trainlist)
                {
                    if(!train.past_stations_name.Contains(station.name))
                    {
                        continue;
                    }
                    if(train.past_stations_name[0]==station.name)
                    {
                        station.origin_train_list.Add(train);
                    }
                    else if(train.past_stations_name.Last()==station.name)
                    {
                        station.destination_train_list.Add(train);
                    }
                    else
                    {
                        station.past_train_list.Add(train);
                    }
                    
                    if(station.name!=train.past_stations_name[0])
                    {
                        station.arrive_train_list.Add(train);
                    }

                    if(station.name!=train.past_stations_name.Last())
                    {
                        station.departure_train_list.Add(train);
                    }                  
                }
                //需要保持列车之间的顺序关系，故不可用下面的方式
                //station.exclude_destinate_train_list = station.origin_train_list.Union(station.past_train_list).ToList();
                //station.exclude_origin_train_list = station.destination_train_list.Union(station.past_train_list).ToList() ;
            }
        }
        //由列车的车次确定列车的类型
        public TrainType GetTrainTypeByName(string train_name)
        {
            string letter = train_name.Substring(0, 1);
            switch(letter)
            {
                case "G":
                    return TrainType.High_Speed;
                //case "D":
                //    return TrainType.Motor;
                //case "T":
                //    return TrainType.Expecial;
                //case "K":
                //    return TrainType.High_Train;
                 
                default:
                    return TrainType.High_Speed;
            }
        }
        public void AddStationTrainTypeToNextRunTime(string station_name,int train_type,int time)
        {
            KeyValuePair<string, int> key = new KeyValuePair<string, int>(station_name, train_type);
            stationtraintypetonextruntime.Add(key, time);
        }
        public TimeHM MinuteChangeToTimeHM(int time)
        {
            int hour = (int)time / 60;
            int minute =time - hour * 60;
            TimeHM hms = new TimeHM(hour, minute);
            return hms;
        }
        public TimeHM StringChangeToTimeHMS(string time)
        {
            int hour =0;//小时
            int minute_second=0;//分钟的十位数字
            int minute_first=0;//分钟的各位数字
            int second_second=0;//秒的十位数字
            int second_first=0;//秒的各位数字
            if(time.IndexOf('.')==-1&&time.Length<=2)
            {
                hour=int.Parse(time);
            }
            else if(time.IndexOf('.')==-1&&time.Length>2)
            {
                throw new Exception("数据格式有误");
            }
            else 
            {
                string[] time_split=time.Split('.');
                hour=int.Parse(time_split[0]);
                if(time_split[1].Length==1)
                {
                    minute_second=int.Parse(time_split[1]);
                }
                if(time_split[1].Length==2)
                {
                    minute_second=int.Parse(time_split[1].Substring(0,1));
                    minute_first=int.Parse(time_split[1].Substring(1,1));
                }
                if(time_split[1].Length==3)
                {
                    minute_second=int.Parse(time_split[1].Substring(0,1));
                    minute_first=int.Parse(time_split[1].Substring(1,1));
                    second_first=int.Parse(time_split[1].Substring(2,1));
                }
                if(time_split[1].Length==4)
                {
                    minute_second=int.Parse(time_split[1].Substring(0,1));
                    minute_first=int.Parse(time_split[1].Substring(1,1));
                    second_first=int.Parse(time_split[1].Substring(2,1));
                    second_second=int.Parse(time_split[1].Substring(3,1));
                }
            }
                     
            int minute = minute_second * 10 + minute_first;
            int second = second_second * 10 + second_first;
            TimeHM timehms=new TimeHM(hour,minute);
            return timehms;
        }
        public TimeM StringChangeToTimeMS(string time)
        {
            int minute =0;//分钟
            int second_second=0;//秒的十位数字
            int second_first=0;//秒的各位数字
            if(time.IndexOf('.')==-1&&time.Length<=2)
            {
                minute=int.Parse(time);
            }
            else if(time.IndexOf('.')==-1&&time.Length>2)
            {
                throw new Exception("数据格式有误");
            }
            else 
            {
                string[] time_split=time.Split('.');
                minute=int.Parse(time_split[0]);
                if(time_split[1].Length==1)
                {
                    second_first=int.Parse(time_split[1].Substring(0,1));
                }
                if(time_split[1].Length==2)
                {
                    second_first=int.Parse(time_split[1].Substring(0,1));
                    second_second=int.Parse(time_split[1].Substring(1,1));
                }
            }                   
            int second = second_second * 10 + second_first;
            TimeM timems=new TimeM(minute);
            return timems;
        }
        public int TimeHMSChangeToMinute(string time)
        {
            if (time == "")
            {
                return -1;
            }
            if (IsNumFirstChar(time) == false)
            {
                return -1;
            }
            int hour = 0;//小时
            int minute_second = 0;//分钟的十位数字
            int minute_first = 0;//分钟的各位数字
            if (time.IndexOf(':') == -1 && time.Length <= 2)
            {
                hour = int.Parse(time);
            }
            else if (time.IndexOf(':') == -1 && time.Length > 2)
            {
                //return -1;
                throw new Exception("数据格式有误");
            }
            else
            {
                string[] time_split = time.Split('.', ':');
                hour = int.Parse(time_split[0]);
                if (time_split[1].Length == 1)
                {
                    minute_second = int.Parse(time_split[1]);
                }
                if (time_split[1].Length == 2)
                {
                    minute_second = int.Parse(time_split[1].Substring(0, 1));
                    minute_first = int.Parse(time_split[1].Substring(1, 1));
                }
            }

            int minute = minute_second * 10 + minute_first;
            return hour * 60 + minute;
        }
   
        //判断一个字符串第一个字符是否是数字
        public bool IsNumFirstChar(String str)
        {

            if (!Char.IsNumber(str, 0))
                return false;

            return true;
        }
        public int TimeMSChangeToSecond(string time)
        {
            if (time == "")
            {
                return -1;
            }
            int minute = 0;//分钟
            int second_second = 0;//秒的十位数字
            int second_first = 0;//秒的各位数字
            if (time.IndexOf('.') == -1 && time.Length <= 2)
            {
                minute = int.Parse(time);
            }
            else if (time.IndexOf('.') == -1 && time.Length > 2)
            {
                throw new Exception("数据格式有误");
            }
            else
            {
                string[] time_split = time.Split('.');
                minute = int.Parse(time_split[0]);
                if (time_split[1].Length == 1)
                {
                    second_second = int.Parse(time_split[1].Substring(0, 1));
                }
                if (time_split[1].Length == 2)
                {
                    second_second = int.Parse(time_split[1].Substring(0, 1));
                    second_first = int.Parse(time_split[1].Substring(1, 1));
                }
            }
            int second = second_second * 10 + second_first;
            return minute * 60 + second;
        }
        //获取调度的时段，开始时间和结束时间
        public int[] GetScheduleSpan()
        {
            int begin_time = 1000000;
            int end_time = 0;
            foreach (Train train in trainlist)
            {
                string begin_station_name = train.past_stations_name[0];
                string end_station_name = train.past_stations_name.Last();
                if (begin_time > GetDepartureTimeByTrainStation(train.name, begin_station_name))
                {
                    begin_time = GetDepartureTimeByTrainStation(train.name, begin_station_name);
                }
                if (end_time < GetArriveTimeByTrainStation(train.name, end_station_name))
                {
                    end_time = GetArriveTimeByTrainStation(train.name, end_station_name);
                }
            }
            int[] TimeSpan = new int[2] { begin_time, end_time };
            return TimeSpan;
        }
        public int GetStationIndexByName(string name)
        {
            if(stationnametoindex.ContainsKey(name))
            {
                return stationnametoindex[name];
            }
            else
            {
                throw new Exception("The name of station don't exit in the dictionary");
            }
        }
        public string Getnextstationbytrainandstation(Train train,string station_name)
        {
            if(station_name==train.past_stations_name.Last())
            {
                throw new Exception("Error");
            }
            for(int station_index=0;station_index<train.past_stations_name.Count()-1;station_index++)
            {
                if(station_name==train.past_stations_name[station_index])
                {
                    return train.past_stations_name[station_index + 1];
                }
            }
            return null;
        }
        //change timetable to only_time_graph

        public void GenerationFollowPair()
        {

        }

        public void GenerationTrainPair()
        {
            for (int train_index_1 = 0; train_index_1 < trainlist.Count(); train_index_1++)
            {
                Train train_1 = trainlist[train_index_1];
                for (int train_index_2 = train_index_1+1; train_index_2 < trainlist.Count(); train_index_2++)
                {
                    Train train_2 = trainlist[train_index_2];
                    int[] pair = new int[2];
                    pair[0] = train_index_1;
                    pair[1] = train_index_2;
                    if(train_1.direction!=train_2.direction)
                    {                      
                        opp_list.Add(pair);
                    }
                    else
                    {
                        follow_list.Add(pair);
                    }
                }
            }
        }

    }
    public class Stock
    {
        public int index;
        public string name;
        public int cost;
        public int depot;
        public int max_undertake_train_num;
        //public List<int> train_index_list;
        //public Stock()
        //{
        //    train_index_list = new List<int>();
        //}
    }

    public class Depot
    {
        public int id;
        public int start_node;
        public int end_node;

        public Depot(int id)
        {
            this.id = id;
        }
    }
    public class Train
    {
        public int index;//id
        public string name;//车次
        public int direction;//方向
        public int stock_index;//使用的动车组索引
        public TrainType train_type;//列车类型：高速、快速、普快、货车
        public List<string> past_stations_name;//列车所经过车站的名称集合
        public List<string> past_stop_stations_name;
        public string begin_station_name;
        public string end_station_name;
        public bool trainselect;//当前列车是否被选中
        public int earliestdeparturetime;//最早可发车时间
        public int latestdeparturetime;//最晚可发车时间
        public Train()
        {
            stock_index = -1;
            past_stations_name = new List<string>();
            past_stop_stations_name = new List<string>();
            trainselect = false;
        }    
    }
    public class Station
    {
        public int index;//id
        public string name;//车次
        public float tonextstationdistance;
        //public string nextstationname;//下一个车站的名称
        public int tracknumber;
        public float mileage;
        public List<Train> origin_train_list;//始发的列车集合
        public List<Train> destination_train_list;//终到的列车集合
        public List<Train> past_train_list;//经过的列车集合
        public List<Train> departure_train_list;//排除终到
        public List<Train> arrive_train_list;//排除始发

        //public List<string> origin_train_name_list;//始发的列车集合
        //public List<string> destination_train_name_list;//终到的列车集合
        //public List<string> past_train_name_list;//经过的列车集合
        //public List<string> exclude_destinate_train_list;//排除终到
        //public List<string> exclude_origin_train_list;//排除始发
    }
    //public class Section
    //{
    //    public Station start_station;
    //    public Station end_station;
    //    public int distance;
    //}
    //小时、分钟、秒
    public class TimeHM
    {
        public int hour;
        public int minute;
        public TimeHM(int hour,int minute)
        {
            this.hour=hour;
            this.minute=minute;
        }
        public string ToString()
        {
            string result = "";
            string hour_string = hour < 10 ? "0" + hour : hour.ToString();
            string minite_string = minute < 10 ? "0" + minute : minute.ToString();
            result = hour_string + ":" + minite_string;
            return result;
        }
        //public static bool operator >=(TimeHMS time_1,TimeHMS time_2)
        //{
        //    bool result=false;
        //    if(time_1.hour>time_2.hour)
        //    {
        //        result=true;
        //    }
        //    else if(time_1.hour==time_2.hour)
        //    {
        //        if(time_1.minute>time_2.minute)
        //        {
        //            result = true;
        //        }
        //        else if(time_1.minute==time_2.minute)
        //        {
        //            if(time_1.second>=time_2.second)
        //            {
        //                result = true;
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
    //分钟、秒
    public class TimeM
    {
        public int minute;
        public TimeM(int minute)
        {
            this.minute=minute;
        }
        public string ToString()
        {
            return "00-" + minute;
        }
    }

    public enum TrainType:int
    {
        High_Speed=0,//
        Motor=High_Speed,//动车
        Straight=1,//直达
        Expecial=Straight,//特快
        High_Train=2,      
        Normal_Speed=3,//代表普快
        Freight_Train=4
    }
    public class BrokenClass
    {
        public int begintime;
        public string firststation;
        public int endtime;
        public BrokenClass(string firststation,int begintime,int endtime)
        {
            this.firststation = firststation;
            this.begintime = begintime;
            this.endtime = endtime;
        }
    }
    public class Maintenance
    {
        public int id;
        public int earlieststart;
        public int lasteststart;
        public int minimumduring;
        public string startstationname;
        public int firstrainspeedlimit;
        public int secondtrainspeedlimit;
        public Maintenance()
        {

        }
    }
}
