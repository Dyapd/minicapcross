using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DataHolders
{
    internal class DataHolderPickerReports
    {
        public class Reports()
        {
            public string Category { get; set; }
            public string ID { get; set; }
            public string CategoryAndID => $"{Category}{ID}";
        }

        
    }
}
