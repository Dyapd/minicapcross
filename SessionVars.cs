using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Data.SqlClient;

namespace test
{
    internal class SessionVars
    {
        public static string SessionId = "12345678";
        public static string DynamicClaim = "", DynamicReport="", DynamicItem="";

        public static void SetSessionId(string Email, string Password)
        {
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
            string query = "SELECT Student_ID FROM StudentUsers WHERE Student_Email = @Email AND Student_Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Password", Password);

                        SessionId = command.ExecuteScalar().ToString();
                    }
                }
                catch (Exception ex)
                {

              
                }
            }
        }
    }
}
