using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Maui.Views;

#if ANDROID
using Android.Provider;
#endif
namespace test
{
    public partial class MainPage : ContentPage
    {
        private const string AdminContactNumber = "admin";
        private const string AdminPassword = "pass";
        private const string NormalUser = "normal";

        string connectionString = "Data Source=192.168.1.6,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";

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

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM StudentUsers";
                    command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                lbl.Text = $"Error: {ex.Message}";
                Console.WriteLine(ex.Message);

            }

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
                this.ShowPopup(new NewPage1());
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