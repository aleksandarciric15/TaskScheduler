using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskSchedulerDemo
{
    public class Klasa1
    {
        [JsonProperty]
        public int broj { get; set; }
        [JsonProperty]
        public string? name { get; set; }
        public Klasa1()
        {
            broj = 1;
            name = " ";
        }

        public Klasa1(int broj, string name)
        {
            this.broj = broj;
            this.name = name;
        }
        public override string ToString()
        {
            return this.name + " " + this.broj;
        }
    }
}
