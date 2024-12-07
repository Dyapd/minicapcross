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
        public static string SessionId = "";
        public static string DynamicClaim = "";
        public static string DynamicReportID = "";
        public static string DynamicItemID = "";
        public static string DynamicStudentID = "";


        public static void SetSessionId(string Email, string Password)
        {
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
            string query = "SELECT Student_ID FROM StudentUsers WHERE Student_Email = @Email AND Student_Password = @Password";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
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
        public static string TakeStudentInfo()
        {
            string studName;
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
            string query = "SELECT Student_Name FROM StudentInfo WHERE Student_ID  = @Student_Number";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Number", SessionId);

                        studName = command.ExecuteScalar().ToString();
                        return studName;

                    }
                }
                catch (Exception ex)
                {
                   
                    return null;
                }
            }
        }

        public static string TakeStudentInfoAdmin(string studnum)
        {
            string studName;
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
            string query = "SELECT Student_Name FROM StudentInfo WHERE Student_ID  = @Student_Number";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Number", studnum);

                        studName = command.ExecuteScalar().ToString();
                        return studName;

                    }
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }

    }
}
