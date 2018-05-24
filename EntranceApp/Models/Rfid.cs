using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceApp.Models
{
    public class Rfid
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsCardLinked { get; set; }

        public int? VistorId { get; set; }

        public Rfid(int id,string code,bool isCardLinked,int? visitorId)
        {
            Id = id;
            Code = code;
            IsCardLinked = isCardLinked;
            VistorId = visitorId;
        }
    }
}
