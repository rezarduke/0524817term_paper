using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotSkype.Model
{
    public class YWeather
    {
        public Query query { get; set; }
    }
    public class Condition
    {
        public string code { get; set; }
        public string date { get; set; }
        public string temp { get; set; }
        public string text { get; set; }
    }

    public class Item
    {
        public Condition condition { get; set; }
    }

    public class Channel
    {
        public Item item { get; set; }
    }

    public class Results
    {
        public Channel channel { get; set; }
    }

    public class Query
    {
        public int count { get; set; }
        public string created { get; set; }
        public string lang { get; set; }
        public Results results { get; set; }
    }

   
}