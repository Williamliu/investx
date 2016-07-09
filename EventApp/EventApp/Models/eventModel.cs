using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace EventApp.Models
{
    public class eventViewModel
    {
        public eventNavi navi { get; set; }
        public List<eventEntity> events { get; set; }
        public eventViewModel()
        {
            this.navi = new eventNavi { pageNumber = 1, pageSize = 20, pageTotal = 0, keywords = "Hotel", location="San Diago" };
            this.events = new List<eventEntity>();
        }

        public void naviSafeCheck()
        {
            this.navi.pageSize = this.navi.pageSize <= 0 ? 20 : this.navi.pageSize;
            this.navi.pageNumber = this.navi.pageNumber <= 0 ? 1 : this.navi.pageNumber;
            this.navi.pageNumber = this.navi.pageNumber > this.navi.pageTotal? this.navi.pageTotal : this.navi.pageNumber;
            this.navi.pageNumber = this.navi.pageNumber > (int)Math.Ceiling((decimal)this.navi.itemTotal / this.navi.pageSize) ? (int)Math.Ceiling((decimal)this.navi.itemTotal / this.navi.pageSize) : this.navi.pageNumber;
            this.navi.keywords = string.IsNullOrEmpty(this.navi.keywords) ? "" : this.navi.keywords.Trim();
            this.navi.location = string.IsNullOrEmpty(this.navi.location) ? "" : this.navi.location.Trim();

        }

        public void jsonParse(string evtStr)
        {
            JObject evtJson = JObject.Parse(evtStr);
            this.navi.pageNumber = evtJson.Value<int>("page_number");
            this.navi.pageSize = evtJson.Value<int>("page_size");
            this.navi.pageTotal = evtJson.Value<int>("page_count");
            this.navi.itemTotal = evtJson.Value<int>("total_items");

            this.events.Clear();
            if(evtJson.Value<JObject>("events")!=null) {
                if (evtJson.Value<JObject>("events").Value<JArray>("event") != null)
                {
                    JArray evtJarr = evtJson.Value<JObject>("events").Value<JArray>("event");
                    foreach (JObject theEvent in evtJarr)
                    {
                        eventEntity evtObj = new eventEntity();
                        evtObj.title = theEvent.Value<string>("title");
                        evtObj.location = theEvent.Value<string>("venue_address");
                        evtObj.location += (string.IsNullOrEmpty(evtObj.location) ? "" : ", ") + theEvent.Value<string>("city_name");
                        evtObj.location += (string.IsNullOrEmpty(evtObj.location) ? "" : ", ") + theEvent.Value<string>("region_abbr");
                        evtObj.location += (string.IsNullOrEmpty(evtObj.location) ? "" : ", ") + theEvent.Value<string>("country_abbr");
                        evtObj.location += (string.IsNullOrEmpty(evtObj.location) ? "" : " ") + theEvent.Value<string>("postal_code");

                        evtObj.event_start = theEvent.Value<string>("start_time");
                        evtObj.event_end = theEvent.Value<string>("stop_time");

                        if (theEvent.Value<JObject>("image") != null)
                            if (theEvent.Value<JObject>("image").Value<JObject>("thumb") != null)
                                evtObj.imageURL = theEvent.Value<JObject>("image").Value<JObject>("thumb").Value<string>("url");

                        #region Perfomers: there is issue: sometimes JObject ; sometimes JArray;  it is bad message format
                        var performers = theEvent.Value<JObject>("performers");
                        if (performers != null)
                        {
                            try
                            {
                                var performer = performers.Value<JObject>("performer");
                                if (performer != null)
                                {
                                    performer perObj = new performer();

                                    perObj.id = performer.Value<string>("id");
                                    perObj.name = performer.Value<string>("name");
                                    perObj.url = performer.Value<string>("url");
                                    perObj.short_bio = performer.Value<string>("short_bio");
                                    perObj.linker = performer.Value<string>("linker");
                                    perObj.creator = performer.Value<string>("creator");

                                    evtObj.performers.Add(perObj);
                                }
                            }
                            catch
                            {
                                try
                                {
                                    var performerArr = performers.Value<JArray>("performer");
                                    foreach(JObject perJson in performerArr)
                                    {
                                        performer perObj = new performer();

                                        perObj.id = perJson.Value<string>("id");
                                        perObj.name = perJson.Value<string>("name");
                                        perObj.url = perJson.Value<string>("url");
                                        perObj.short_bio = perJson.Value<string>("short_bio");
                                        perObj.linker = perJson.Value<string>("linker");
                                        perObj.creator = perJson.Value<string>("creator");

                                        evtObj.performers.Add(perObj);
                                    }
                                }
                                catch { }

                            }
                        }
                        #endregion

                        this.events.Add(evtObj);
                    }
                }
            }

        }
    }

    public class eventNavi
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int pageTotal { get; set; }
        public int itemTotal { get; set; }
        public string keywords { get; set; }
        public string location { get; set; }
    }

    public class eventEntity
    {
        public eventEntity()
        {
            this.performers = new List<performer>();
        }
        public string title { get; set; }
        public string location { get; set; }
        public string event_start { get; set; }
        public string event_end { get; set; }
        public string imageURL { get; set; }

        public List<performer> performers { get; set; }
    }

    public class performer
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string short_bio { get; set; }
        public string creator { get; set; }
        public string linker { get; set; }
    }

    public static class MyJSON {
        public static T getValue<T>(JObject jObj, string propertyName)
        {
            T ret = default(T);
            try
            {
                ret = jObj.Value<T>(propertyName);
            }
            catch
            {

            }
            return ret;
        }
    }
}
