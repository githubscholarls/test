using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaTest.Domain.DTOS
{
    internal class RequestMsg
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<string> contacts { get; set; }

        public override string ToString()
        {
            return name + "  id:"+ id;
        }
    }
}
