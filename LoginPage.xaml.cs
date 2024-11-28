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
        string connectionString;

        //192.168.1.6
        public MainPage()
        {
            InitializeComponent();
            
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();

            string enteredEmail;
            string enteredPassword;

            enteredEmail = userEntry.Text;
            enteredPassword = userPass.Text;

            string adacorrecttemp = "adcred";
            string stcorrecttemp = "stcred";



            //TEMPORARY HARD CODE FOR TESTING!!
            if (enteredemail == adacorrecttemp && enteredpassword == adacorrecttemp)
            {
                application.current.mainpage = new navigationpage(new admindashboard());
                await navigation.pushasync(new admindashboard());
            }
            else if (enteredemail == stcorrecttemp && enteredpassword == stcorrecttemp)
            {
                application.current.mainpage = new navigationpage(new studentdashboard());
                await navigation.pushasync(new studentdashboard());
            }
            else
            {
                this.showpopup(new newpage1());
            }


            //ADD THIS IF WANT TO TEST REAL FUNCTIONALTIY!
            //NOTE NEED TO IMPORT DATABASE FROM THE FILES
            //PROPER CREDENTIALS ARE user id= recadm; password = pass

            //if (CheckAdminAccount(enteredEmail, enteredPassword))
            //{
            //    Application.Current.MainPage = new NavigationPage(new AdminDashboard());
            //    await Navigation.PushAsync(new AdminDashboard());
            //}
            //else if (CheckRegisteredAccount(enteredEmail, enteredPassword))
            //{
            //    SessionVars.SetSessionId(enteredEmail, enteredPassword);
            //    await DisplayAlert(SessionVars.SessionId, "", "OK!");
            //    Application.Current.MainPage = new NavigationPage(new StudentDashboard());
             
            //    await Navigation.PushAsync(new StudentDashboard());
            //}
            //else if (!(CheckRegisteredAccount(enteredEmail, enteredPassword) && (CheckAdminAccount(enteredEmail, enteredPassword))))
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