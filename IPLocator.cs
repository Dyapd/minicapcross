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
            string connectionString = "Data Source=192.168.45.144,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            return connectionString;
        }
    }
}
