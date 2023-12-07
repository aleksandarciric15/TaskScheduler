using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSDokan
{
    public class File
    {
        private byte[] data;
        private string name;
        private DateTime created;

        public File(string name, DateTime created)
        {
            data = Array.Empty<byte>();
            this.name = name;
            this.created = created;
        }

        public byte[] Data { get { return data; } set { data = value; } }

        public string Name { get { return name; } set { name = value; } }

        public DateTime Created { get { return created; } set { created = value; } }
    }
}
