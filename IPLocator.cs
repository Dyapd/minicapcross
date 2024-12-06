using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace test
{
    internal class IPLocator
    {

        public string ConnectionString()
        {
            //sid phone =192.168.83.144
            //string connectionString = $"Data Source=192.168.45.145,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            string connectionString = $"Data Source=192.168.1.84,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;Connection Timeout =30;";
            string testString = "Data Source=localhost,1433;Initial Catalog=Minicapstone;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;Integrated Security=True;";

            //if ikaw si sid, kenshin or charles. change the code bellow to:
            //return testString;
            //para matest ung database!
            //return to original after!!
            return connectionString;
            
        }
    }
}
