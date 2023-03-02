using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.Algorithm
{
    class ArriveAndDepartureEvent
    {
        Graph new_g;
        public Dictionary<string, List<ConflictAD>> stationnametoconflict;
        public ArriveAndDepartureEvent(Graph _new_g)
        {
            new_g = _new_g;
        }
        //将时刻表转换成到达和离开的事件
        public void ChangeToEventAndCheck()
        {
            stationnametoconflict = new Dictionary<string, List<ConflictAD>>();
            Dictionary<string, List<EventAD>> stationnametoarriveevent = new Dictionary<string, List<EventAD>>();
            Dictionary<string, List<EventAD>> stationnametodepartureevent = new Dictionary<string, List<EventAD>>();
            
            foreach(Station station in new_g.stationlist)
            {
                foreach(Train train in new_g.trainlist)
                {
                    if(!train.past_stations_name.Contains(station.name))
                    {
                        continue;
                    }
                    int arrive_time = new_g.GetArriveTimeByTrainStation(train.name, station.name);
                    int departure_time = new_g.GetDepartureTimeByTrainStation(train.name, station.name);
                    //到达事件
                    List<EventAD> eventlist_arrive_temp;
                    if(stationnametoarriveevent.ContainsKey(station.name))
                    {
                        eventlist_arrive_temp = stationnametoarriveevent[station.name];
                        eventlist_arrive_temp.Add(new EventAD(train.name, arrive_time));
                    }  
                    else
                    {
                        eventlist_arrive_temp = new List<EventAD>();
                        eventlist_arrive_temp.Add(new EventAD(train.name, arrive_time));
                        stationnametoarriveevent.Add(station.name, eventlist_arrive_temp);
                    }
                    //离开事件
                    List<EventAD> eventlist_departure_temp;
                    if (stationnametodepartureevent.ContainsKey(station.name))
                    {
                        eventlist_departure_temp = stationnametodepartureevent[station.name];
                        eventlist_departure_temp.Add(new EventAD(train.name, departure_time));
                    }
                    else
                    {
                        eventlist_departure_temp = new List<EventAD>();
                        eventlist_departure_temp.Add(new EventAD(train.name, departure_time));
                        stationnametodepartureevent.Add(station.name, eventlist_departure_temp);
                    }
                }
            }

            //排序和检查冲突
            foreach(Station station in new_g.stationlist)
            {
                stationnametoarriveevent[station.name] = stationnametoarriveevent[station.name].OrderBy(e => e.occurtime).ToList();
                stationnametodepartureevent[station.name] = stationnametodepartureevent[station.name].OrderBy(e => e.occurtime).ToList();
                CheckConflict(stationnametoarriveevent[station.name], true, station.name);
                CheckConflict(stationnametodepartureevent[station.name],false,station.name);
            }
            
        }
        public void CheckConflict(List<EventAD> eventlist,bool arrive,string stationname)
        {
            List<ConflictAD> conflictadlist;
            bool arrive_conflict = true;
            int headtime = new_g.arrivearrivehead;
            for (int i = 0; i < eventlist.Count()-1; i++)
            {
                for (int j = i+1; j < eventlist.Count(); j++)
                {
                    int occurtime_1 = eventlist[i].occurtime;
                    int occurtime_2 = eventlist[j].occurtime;
                    if(!arrive)
                    {
                        arrive_conflict = false;
                        headtime = new_g.departuredeparturehead;
                    }
                    if (occurtime_1 + headtime > occurtime_2)
                    {
                        if (stationnametoconflict.ContainsKey(stationname))
                        {
                            conflictadlist = stationnametoconflict[stationname];
                            conflictadlist.Add(new ConflictAD(eventlist[i], eventlist[j], arrive_conflict));
                        }
                        else
                        {
                            conflictadlist = new List<ConflictAD>();
                            conflictadlist.Add(new ConflictAD(eventlist[i], eventlist[j], arrive_conflict));
                            stationnametoconflict.Add(stationname, conflictadlist);
                        }
                    }
                }
            }
        }
    }
    class ConflictAD
    {
        public bool arrive_conflict;//是否到达冲突，是为true,否则为false
        public EventAD first_event;
        public EventAD second_event;
        public ConflictAD(EventAD first_event, EventAD second_event,bool arrive_conflict)
        {
            this.arrive_conflict = arrive_conflict;
            this.first_event = first_event;
            this.second_event = second_event;
        }
    }
    //事件定义
    class EventAD
    {
        public string trainname;
        public int occurtime;//到达或离开的时间
        public string stationname;
        public EventAD(string train_name,int occurtime)
        {
            this.trainname = train_name;
            this.occurtime = occurtime;
        }
    }
}
