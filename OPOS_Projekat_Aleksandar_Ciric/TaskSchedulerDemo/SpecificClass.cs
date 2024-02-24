using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace TaskSchedulerDemo
{
    public class SpecificClass : SerializeClass
    {
        [JsonProperty]
        private string? specific;
        [JsonProperty]
        protected string? test;
        public int broj;
        public SpecificClass(string specific, string name, int numb): base (name, numb)
        {
            this.specific = specific;
            test = "50";
        }

        public SpecificClass()
        {

        }

        public override string ToString()
        {
            return this.specific + " " + base.ToString();
        }
    }
}
