using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using Android.Provider;

namespace test
{
    public partial class MainPage : ContentPage
    {
        private const string AdminContactNumber = "admin";
        private const string AdminPassword = "pass";
        private const string NormalUser = "normal";

        string connectionString = "Data Source=localhost;Initial Catalog=Register;Integrated Security=True";

        public MainPage()
        {
            InitializeComponent();
        }
        private void OnLoginClicked(object sender, EventArgs e)
        {
            string enteredEmail;
            string enteredPassword;

            enteredEmail = userEntry.Text;
            enteredPassword = userPass.Text;

            if (CheckAdminAccount(enteredEmail, enteredPassword))
            {
                Navigation.PushAsync(new AdminDashboard());
            }
            else if (CheckRegisteredAccount(enteredEmail, enteredPassword))
            {
                Navigation.PushAsync(new StudentDashboard());
            }
            else
            {
                
            }
            


            //if (enteredEmail == AdminContactNumber && enteredPassword == AdminPassword)
            //{
            //    Navigation.PushAsync(new StudentDashboard());
            //}
            //else if (enteredEmail == NormalUser && enteredPassword == AdminPassword)
            //{
            //    Navigation.PushAsync(new AdminDashboard());
            //}

            //else
            //{
            //    if (CheckRegisteredAccount(enteredEmail, enteredPassword))
            //    {
            //        //Insert Student Page Method Here
            //    }
            //    else
            //    {
            //        // MessageBox.Show("Invalid credentials.");
            //    }
            //}




        }
        private bool CheckRegisteredAccount(string enteredEmail, string enteredPassword)
        {
            string query = "SELECT 1 FROM StudentUsers WHERE Student_Email = @Email AND Student_Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", enteredEmail);
                        command.Parameters.AddWithValue("@Password", enteredPassword);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                    
                }
                catch (Exception ex)
                {

                    return false;
                }
            }
        }

        private bool CheckAdminAccount(string enteredEmail, string enteredPassword)
        {
            string query = "SELECT 1 FROM AdminUsers WHERE Admin_Email = @Email AND Admin_Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", enteredEmail);
                        command.Parameters.AddWithValue("@Password", enteredPassword);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }

                }
                catch (Exception ex)
                {

                    return false;
                }
            }
        }


        private void StudentDashboardLogin(string Student_Password, string Student_Email)
        {

        }
    }
}