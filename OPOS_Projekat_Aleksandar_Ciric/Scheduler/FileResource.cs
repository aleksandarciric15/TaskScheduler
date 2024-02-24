using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Scheduler
{
    [Serializable]
    public class FileResource : Resource
    {
        [JsonProperty]
        private string path {get; set;}

        public FileResource(string path)
        {
            this.path = path;
        }

        public override string getResource()
        {
            return path;
        }


        public string ToString()
        {
            return this.path;    
        }
    }
}
