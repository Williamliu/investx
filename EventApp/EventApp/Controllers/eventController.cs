using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using EventApp.Models;
using System.Net;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace EventApp.Controllers
{
    public class eventController : Controller
    {
        // GET: event
        public ActionResult getEvent(eventViewModel evt, string submitButton)
        {
            //Pevent webpage remember the old value when render partial view
            ModelState.Clear();

            switch (submitButton)
            {
                case "First":
                    evt.navi.pageNumber = 1;
                    break;
                case "Previous":
                    evt.navi.pageNumber--;
                    break;
                case "Next":
                    evt.navi.pageNumber++;
                    break;
                case "Last":
                    evt.navi.pageNumber = evt.navi.pageTotal;
                    break;
                case "Search":
                case "Go":
                    break;
            }

            evt.naviSafeCheck();

            getDataFromEventFul(evt);

            // when we can search criteria,  the pageNumber keep old value but pageTotal already changed.  
            // in this case,  it will return nothing,  so we need to request the real last page(new pageTotal)
            if (evt.navi.pageNumber > evt.navi.pageTotal)
            {
                evt.navi.pageNumber = evt.navi.pageTotal;
                getDataFromEventFul(evt);
            }
            return PartialView(evt);
        }

        // pass class object :  send by reference,  so don't need return
        public void getDataFromEventFul(eventViewModel evt)
        {
            string result = string.Empty;

            //Important: if request header include: Expect: 100-continue. always return error 417 
            //I use  fiddler4  to compare the header.    remove Expect:100-continue ,  it works fine
            //So I search how to using c# code to remove Expect: 100-continue 
            ServicePointManager.Expect100Continue = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.eventful.com/json/events/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("app_key", "dKqgHG6c7xhNs2Kp"),
                    new KeyValuePair<string, string>("keywords", evt.navi.keywords),
                    new KeyValuePair<string, string>("location", evt.navi.location),
                    new KeyValuePair<string, string>("date",    "Future"),
                    new KeyValuePair<string, string>("page_number",     evt.navi.pageNumber.ToString()),
                    new KeyValuePair<string, string>("page_size",       evt.navi.pageSize.ToString())
                };

                var content = new FormUrlEncodedContent(pairs);

                var response = client.PostAsync("search", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    evt.jsonParse(result);
                }
            }


            /*
            // These code works fine as well
            using (WebClient client = new WebClient())
            {
                NameValueCollection postKeyValue = new NameValueCollection();
                postKeyValue["app_key"]     = "dKqgHG6c7xhNs2Kp";
                postKeyValue["keywords"]    = evt.navi.keywords;
                postKeyValue["location"]    = "San Diego";
                postKeyValue["date"]        = "Future";
                postKeyValue["page_number"] = evt.navi.pageNumber.ToString();
                postKeyValue["page_size"]   = evt.navi.pageSize.ToString();
                byte[] respBytes = client.UploadValues("http://api.eventful.com/json/events/search", "POST", postKeyValue);
                result = ASCIIEncoding.UTF8.GetString(respBytes);
                evt.jsonParse(result);
            }
            */
        }

    }
}