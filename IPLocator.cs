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
            return connectionString;
        }
    }
}
