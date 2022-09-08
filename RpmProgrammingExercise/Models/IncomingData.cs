using System;
using System.Collections.Generic;
using System.Text;

namespace RpmProgrammingExercise.Models
{
    public class IncomingData
    {
        public Request request { get; set; }
        public List<Series> series { get; set; }
    }
    public class Request
    {
        public string command { get; set; }
        public string series_id { get; set; }
    }

    public class Series
    {
        public string series_id { get; set; }
        public string name { get; set; }
        public string units { get; set; }
        public string f { get; set; }
        public string unitsshort { get; set; }
        public string description { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
        public string iso3166 { get; set; }
        public string geography { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public DateTime updated { get; set; }
        public List<List<object>> data { get; set; }
    }
}
