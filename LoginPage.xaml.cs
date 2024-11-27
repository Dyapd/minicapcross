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
        string connectionString = "Data Source=localhost,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
        //192.168.1.6
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string enteredEmail;
            string enteredPassword;

            enteredEmail = userEntry.Text;
            enteredPassword = userPass.Text;

            string adacorrecttemp = "adcred";
            string stcorrecttemp = "stcred";

            //for testing the query if it works
            //try
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //    {
            //        connection.Open();
            //        var command = connection.CreateCommand();
            //        command.CommandText = "SELECT * FROM StudentUsers";
            //        command.ExecuteReader();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    lbl.Text = $"Error: {ex.Message}";
            //    Console.WriteLine(ex.Message);

            //}

            //TEMPORARY HARD CODE FOR TESTING!!
            if (enteredEmail == adacorrecttemp && enteredPassword == adacorrecttemp)
            {
                Application.Current.MainPage = new NavigationPage(new AdminDashboard());
                await Navigation.PushAsync(new AdminDashboard());
            }
            else if (enteredEmail == stcorrecttemp && enteredPassword == stcorrecttemp)
            {
                Application.Current.MainPage = new NavigationPage(new StudentDashboard());
                await Navigation.PushAsync(new StudentDashboard());
            }
            else
            {
                this.ShowPopup(new NewPage1());
                
            }


            //ADD THIS IF WANT TO TEST REAL FUNCTIONALTIY!
            //NOTE NEED TO IMPORT DATABASE FROM THE FILES
            //PROPER CREDENTIALS ARE user id= recadm; password = pass

            //if (CheckAdminAccount(enteredEmail, enteredPassword)) 
            //{
            //  Application.Current.MainPage = new NavigationPage(new AdminDashboard());
            //    Navigation.PushAsync(new AdminDashboard());
            //}
            //else if (CheckRegisteredAccount(enteredEmail, enteredPassword))
            //{
            //      Application.Current.MainPage = new NavigationPage(new StudentDashboard());
            //    Navigation.PushAsync(new StudentDashboard());
            //}
            //else
            //{
            //    this.ShowPopup(new NewPage1());
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

        public void OnLoginBtnEntered(object sender, EventArgs e)
        {
            LoginBtn.BackgroundColor = Colors.SteelBlue;
        }
        public void OnLoginBtnExited(object sender, EventArgs e)
        {
            LoginBtn.BackgroundColor = Colors.SlateBlue;
        }

    }
}