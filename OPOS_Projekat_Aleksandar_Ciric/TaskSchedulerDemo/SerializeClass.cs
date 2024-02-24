using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedulerDemo
{
    public class SerializeClass
    {
        [JsonProperty]
        private string? name;
        [JsonProperty]
        private int number;

        public SerializeClass(string name, int number)
        {
            this.name = name;
            this.number = number;
        }

        public SerializeClass()
        {

        }

        public override string ToString()
        {
            return this.name + " " + this.number;
        }
    }
}
