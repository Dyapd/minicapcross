using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DataHolders
{
    //add more public class models for more bindings
    public class DataholderNotificationLog
    {
        //items is reused for both studentdashboard and for the claims page left picker!!

        public class StudentNotification
        {
            public string ID { get; set; }
            public string Category { get; set; }
            public string CategoryAndId => $"{Category}{ID}";
        }
        public class Items
        { 
            public string Category { get; set; }


            public string ID {  get; set; }

            public string Location { get; set; }
            public string ICategory { get; set; }

            public Boolean Status { get; set; }

            //combine strings
            public string CategoryAndID => $"{Category}{ID}";


        }
        //used for the right image in claims
        public class Items2
        {
            public string Category { get; set; }
            public string ID { get; set; }
            public string Location { get; set; }
            public string ICategory { get; set; }
            public string CategoryAndID => $"{Category}{ID}";
        }

        public class DynamicClaims
        {
            public string Category { get; set; }
            public string ID { get; set; }
            public Boolean Status { get; set; }
            public string ICategory { get; set; }
            public string Description { get;  set; }
            public string StudentNumber { get; set; }
            public byte[] Image { get; set; }
            public string ReportId { get; set; }
            public string ItemId { get; set; }
        }

        public class DynamicReports
        {
            public string ID { get; set; }
            public string Date { get; set; }
            public string ICategory { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string StudentNumber { get; set; }
            public byte[] Image { get; set; }
            public Boolean Status { get; set; }
        }
    }
}
