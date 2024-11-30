using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DataHolders
{
    public class DataholderNotificationLog
    {
        public class Items
        { 
            public string Category { get; set; }
            public string ID {  get; set; }

            public string CategoryAndID => $"{Category}{ID}";
        }

        public class LeftImagePicker
        {
            public string Category { get; set; }
            public string ID { get; set; }

            public string CategoryAndID => $"{Category}{ID}";
        }

        public class Items2
        {
            public string Category { get; set; }
            public string ID { get; set; }
            public string CategoryAndID => $"{Category}{ID}";
        }

        
    }
}
