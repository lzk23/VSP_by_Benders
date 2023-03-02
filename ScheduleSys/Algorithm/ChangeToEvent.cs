using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleSys.Algorithm
{
    class ChangeToEvent
    {
        Graph g;
        public List<Event> alleventlist;//按发车时间排序的离散事件
        public bool slack;
        //public List<ConflictEventPair> conflictpairs;//冲突事件对集合
        public ChangeToEvent(Graph g)
        {
            this.g = g;
            alleventlist = new List<Event>();
            slack = bool.Parse(DataReadWrite.ReadIniData("Slack", "slackornot", "", Application.StartupPath + "\\Paremeter.ini"));
            ChangeGraphToEvent();
        }
        //将时刻表转化成事件
        public void ChangeGraphToEvent()
        {
            foreach(Train train in g.trainlist)
            {
                for(int station_index=0;station_index<train.past_stations_name.Count()-1;station_index++)
                {
                    Station station=g.GetStationByName(train.past_stations_name[station_index]);
                    int begin_time = g.GetDepartureTimeByTrainStation(train.name, station.name);
                    int end_time = g.GetArriveTimeByTrainStation(train.name, train.past_stations_name[station_index+1]);
                    alleventlist.Add(new Event(begin_time, end_time, station.name, train.past_stations_name[station_index + 1], train.name));
                }
            }          
        }
        //顺延疏解
        public void KeepOrderFreeConflict()
        {
            List<ConflictEventPair> conflictpairs = new List<ConflictEventPair>();
            Dictionary<string, List<Event>> stationnametoeventlist = new Dictionary<string, List<Event>>();
            foreach (Event e in alleventlist)
            {
                if (stationnametoeventlist.ContainsKey(e.start_station_name))
                {
                    stationnametoeventlist[e.start_station_name].Add(e);
                }
                else
                {
                    List<Event> eventlist_temp = new List<Event>();
                    stationnametoeventlist.Add(e.start_station_name, eventlist_temp);
                    stationnametoeventlist[e.start_station_name].Add(e);
                }
            }
            //排序
            List<string> station_name_keys = stationnametoeventlist.Keys.ToList();
            foreach (string station_name in station_name_keys)
            {
                stationnametoeventlist[station_name] = stationnametoeventlist[station_name].OrderBy(e => e.begin_time).ToList();
            }
            //顺延时间
            foreach (string station_name in stationnametoeventlist.Keys)
            {
                for (int i = 0; i < stationnametoeventlist[station_name].Count() - 1; i++)
                {
                    Event event_1 = stationnametoeventlist[station_name][i];
                    Event event_2 = stationnametoeventlist[station_name][i + 1];
                    if (event_1.begin_time + g.departuredeparturehead > event_2.begin_time)
                    {
                        event_2.begin_time = event_1.begin_time + g.departuredeparturehead;
                    }
                    if(event_1.end_time + g.arrivearrivehead > event_2.end_time)
                    {
                        event_2.end_time = event_1.end_time + g.arrivearrivehead;
                    }                  
                }
            }
        }
        //只检查发车时间冲突和到达时间冲突
        public List<ConflictEventPair> CheckConflictModel(int bendersking)
        {
            int departuretime_slack = 0;
            int arrivetime_slack = 0;
            if(slack==true&&bendersking!=3)
            {
                departuretime_slack = int.Parse(DataReadWrite.ReadIniData("Slack", "arriveslack", "", Application.StartupPath + "\\Paremeter.ini"));
                arrivetime_slack = int.Parse(DataReadWrite.ReadIniData("Slack", "departureslack", "", Application.StartupPath + "\\Paremeter.ini"));
            }
            //分区间
            List<ConflictEventPair> conflictpairs = new List<ConflictEventPair>();         
            Dictionary<string, List<Event>> stationnametoeventlist = new Dictionary<string, List<Event>>();
            foreach (Event e in alleventlist)
            {
                if(stationnametoeventlist.ContainsKey(e.start_station_name))
                {
                    stationnametoeventlist[e.start_station_name].Add(e);
                }
                else
                {
                    List<Event> eventlist_temp = new List<Event>();
                    stationnametoeventlist.Add(e.start_station_name, eventlist_temp);
                    stationnametoeventlist[e.start_station_name].Add(e);
                }
            }
            //排序
            List<string> station_name_keys = stationnametoeventlist.Keys.ToList();
            foreach(string station_name in station_name_keys)
            {
                stationnametoeventlist[station_name]=stationnametoeventlist[station_name].OrderBy(e => e.begin_time).ToList();
            }
            foreach (string station_name in stationnametoeventlist.Keys)
            {
                
                for(int i=0;i<stationnametoeventlist[station_name].Count()-1;i++)
                {
                    Event event_1 = stationnametoeventlist[station_name][i];
                    Event event_2 = stationnametoeventlist[station_name][i + 1];
                    if (event_1.train_name == "K518/5" && event_2.train_name == "1230/27" && station_name == "徐州")
                    {
                        int a = 0;
                        a = 1;
                    }
                    if (event_1.begin_time + g.departuredeparturehead - departuretime_slack > event_2.begin_time ||
                    event_1.end_time + g.arrivearrivehead - arrivetime_slack > event_2.end_time ||
                    event_1.end_time > event_2.end_time)
                    {
                        ConflictEventPair eventpair = new ConflictEventPair(event_1, event_2);
                        conflictpairs.Add(eventpair);
                        if (event_1.begin_time + g.departuredeparturehead - departuretime_slack > event_2.begin_time)
                        {
                            eventpair.begintime_conflict = true;
                        }
                        if (event_1.end_time + g.arrivearrivehead - arrivetime_slack > event_2.end_time && event_1.end_time < event_2.end_time)
                        {
                            eventpair.endtime_conflict = true;
                        }
                        if (event_1.end_time > event_2.end_time)
                        {
                            eventpair.order_comflict = true;
                        }
                        if (bendersking == 2)
                        {
                            if (event_1.begin_time + g.departuredeparturehead > event_2.begin_time)
                            {
                                eventpair.begintime_conflict = true;
                            }
                            if (event_1.end_time + g.arrivearrivehead > event_2.end_time)
                            {
                                eventpair.endtime_conflict = true;
                            }
                            if (event_1.end_time > event_2.end_time)
                            {
                                eventpair.order_comflict = true;
                            }
                        }
                    }                 
                }
            }
            return conflictpairs;
        }

        ////检测列车与维修作业之间的冲突
        //public 
        public static void ReportConflict(List<ConflictEventPair> conpairs)
        {
            DataShow.Message message = new DataShow.Message();
            foreach(ConflictEventPair conflict in conpairs)
            {
                //Station first_station = g.GetStationByName(conflict.first_event.start_station_name);
                //Station second_station = g.GetStationByName(conflict.first_event.end_station_name);
                //Train train_1 = g.GetTrainByName(conflict.first_event.train_name);
                //Train train_2 = g.GetTrainByName(conflict.second_event.train_name);
                //message.textBox_message.AppendText(train_1.name + "," + train_2.name + "," + first_station.name+",("+conflict.begintime_conflict.ToString()+","+
                //    conflict.endtime_conflict.ToString()+","+conflict.order_comflict.ToString()+")"+"\r\n");
                //message.textBox_message.AppendText(train_1.name + "," + train_2.name + "," + first_station.name +"\r\n");
                message.textBox_message.AppendText(conflict.first_event.train_name + "," +
                   conflict.second_event.train_name + "," + conflict.first_event.start_station_name + "\r\n");
            }
            message.Show();
        }


        #region
        //下面这个检查冲突，把运行时间和最小停站时间、车头间距都检查了
        //public void CheckConflict()
        //{
        //    List<ConflictEventPair> conflictpairs = new List<ConflictEventPair>();
        //    Dictionary<string, Event> trainendeventdic = new Dictionary<string, Event>();//记录相同列车号的上一个事件的结束事件
        //    Dictionary<string, Event> sectionendeventdic = new Dictionary<string, Event>();//记录每个车站的结束时间
        //    DataShow.Message message = new DataShow.Message();
        //    message.textBox_message.AppendText("There are " + alleventlist.Count() + " events\r\n");
        //    ConflictEventPair eventpair;//冲突对
        //    foreach (Event e in alleventlist)
        //    {
        //        //列车上一个事件的结束时间,并判断是否满足最小停站时间
        //        if (trainendeventdic.ContainsKey(e.train_name))
        //        {
        //            int earlystarttime_dwell = trainendeventdic[e.train_name].end_time + g.GetStationMinimizeDwellingTime(e.train_name, e.start_station_name);
        //            if (e.begin_time < earlystarttime_dwell)
        //            {
        //                ConflictEventPair conflict_pair_dwell = new ConflictEventPair(trainendeventdic[e.train_name], e);
        //                conflict_pair_dwell.dwelling_conflict = true;
        //                //conflictpairs.Add(conflict_pair_dwell);
        //                message.textBox_message.AppendText(e.train_name + " from " + e.start_station_name + " to " + e.end_station_name + " dwelling time is conflict," +
        //            earlystarttime_dwell + " - " + e.begin_time + "\r\n");
        //            }
        //        }
        //        //if(!sectionendeventdic.ContainsKey(e.start_station_name))
        //        //{
        //        //    sectionendeventdic.Add(e.start_station_name, e);
        //        //    continue;
        //        //}

        //        //是否满足发车时间要求
        //        if (sectionendeventdic.ContainsKey(e.start_station_name))
        //        {
        //            eventpair = new ConflictEventPair(sectionendeventdic[e.start_station_name], e);
        //            int earlystarttime_start = sectionendeventdic[e.start_station_name].begin_time + g.departuredeparturehead;
        //            if (e.begin_time < earlystarttime_start)
        //            {
        //                eventpair.begintime_conflict = true;
        //                message.textBox_message.AppendText(e.train_name + " from " + e.start_station_name + " to " + e.end_station_name + " begin time is conflict," +
        //            earlystarttime_start + " - " + e.begin_time + "\r\n");
        //            }
        //            //是否满足结束时间要求
        //            int earlystarttime_end = sectionendeventdic[e.end_station_name].end_time + g.arrivearrivehead;
        //            if (e.end_time < earlystarttime_end)
        //            {
        //                eventpair.endtime_conflict = true;
        //                message.textBox_message.AppendText(e.train_name + " from " + e.start_station_name + " to " + e.end_station_name + " end time is conflict," +
        //            earlystarttime_end + " - " + e.begin_time + "\r\n");
        //            }
        //            //如果存在冲突，则加入冲突对集合中
        //            if (eventpair.begintime_conflict == true || eventpair.endtime_conflict == true)
        //            {
        //                conflictpairs.Add(eventpair);
        //            }
        //        }

        //        //更新列车的前一个时间
        //        if (trainendeventdic.ContainsKey(e.train_name))
        //        {
        //            trainendeventdic[e.train_name] = e;
        //        }
        //        else
        //        {
        //            trainendeventdic.Add(e.train_name, e);
        //        }
        //        //更新区间的最后一个事件
        //        if (sectionendeventdic.ContainsKey(e.start_station_name))
        //        {
        //            sectionendeventdic[e.start_station_name] = e;
        //        }
        //        else
        //        {
        //            sectionendeventdic.Add(e.start_station_name, e);
        //        }
        //    }
        //    message.textBox_message.AppendText(conflictpairs.Count() + " conflict\r\n");
        //    message.Show();
        //}
        #endregion
    }
    class Event
    {
        public int begin_time;
        public int end_time;
        public string start_station_name;
        public string end_station_name;
        public string train_name;
        public Event()
        {

        }
        public Event(int begin_time,int end_time,string start_station_name,
            string end_station_name,string train_name)
        {
            this.begin_time = begin_time;
            this.end_time = end_time;
            this.start_station_name = start_station_name;
            this.end_station_name = end_station_name;
            this.train_name = train_name;
        }
    }
    //冲突事件对
    class ConflictEventPair
    {
        public Event first_event;
        public Event second_event;
        public bool dwelling_conflict;
        public bool begintime_conflict;
        public bool endtime_conflict;
        public bool order_comflict;
        public ConflictEventPair()
        {
            first_event = new Event();
            second_event = new Event();
        }
        public ConflictEventPair(Event first,Event second)
        {
            first_event = first;
            second_event = second;
            dwelling_conflict = false;
            begintime_conflict = false;
            endtime_conflict = false;
            order_comflict = false;
        }
    }

}
