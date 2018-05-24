using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntranceApp
{
    #region Field
    public class Visitors
    {
       // public int Id { get; set; }
        public string FullName { get; set; }
        public string RFID { get;  set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public double Balance { get; }
        #endregion


        #region constructor
        public Visitors( string name,int telephoneNumber,string emailaddress,double balance,string rfid)
        {
            //this.Id = id;
            this.FullName = name;
            this.PhoneNumber = telephoneNumber;
            this.EmailAddress = emailaddress;
            this.Balance = balance;
            this.RFID = rfid;
        }
        #endregion

        public string[] Info()
        {
            string[] info = new string[5];
            info[0] = FullName;
            info[1] = EmailAddress;
            return info;
        }
    }
}