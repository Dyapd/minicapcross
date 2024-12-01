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
        //public static string GetLocalIpAddress()
        //{
        //    string localIp = string.Empty;


        //    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        //    {

        //        if (ni.OperationalStatus == OperationalStatus.Up)
        //        {

        //            IPInterfaceProperties ipProperties = ni.GetIPProperties();


        //            foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
        //            {

        //                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //                {
        //                    localIp = ip.Address.ToString();
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return localIp;
        //}

        public string ConnectionString()
        {
            //string localIp = GetLocalIpAddress();
            //string connectionString = $"Data Source=192.168.45.145,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            string connectionString = $"Data Source=localhost,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=False;TrustServerCertificate=True;Connection Timeout =30;";
            return connectionString;
        }
    }
}
