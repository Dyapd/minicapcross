﻿using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using test.UserPages;
using Microsoft.IdentityModel.Tokens;





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

            if (DeviceInfo.Platform != DevicePlatform.Android )
            {
                PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
                pointerGestureRecognizer.PointerEntered += (s, e) =>
                {
                    //mageenter 
                    LoginBtn.BackgroundColor = Colors.SteelBlue;
                };
                pointerGestureRecognizer.PointerExited += (s, e) =>
                {
                    //mageexit
                    LoginBtn.BackgroundColor = Colors.SlateBlue;
                };

                LoginBtn.GestureRecognizers.Add(pointerGestureRecognizer);
            }
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            LoginBtn.IsEnabled = false;
            userEntry.IsEnabled = false;
            userPass.IsEnabled = false;

            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();

            string enteredEmail;
            string enteredPassword;

            enteredEmail = userEntry.Text;
            enteredPassword = userPass.Text;

            string adacorrecttemp = "adcred";
            string stcorrecttemp = "stcred";

            if (string.IsNullOrWhiteSpace(enteredEmail) || string.IsNullOrWhiteSpace(enteredPassword))
            {
                DisplayAlert("Error", "Fields must not be empty!", "OK");
                LoginBtn.IsEnabled = true;
                userEntry.IsEnabled = true;
                userPass.IsEnabled = true;
                userEntry.Text = "";
                userPass.Text = "";
                return;
            }


            //temporary hard code for testing!!

            //if (enteredEmail == adacorrecttemp && enteredPassword == adacorrecttemp)
            //{
            //    Application.Current.MainPage = new NavigationPage(new AdminDashboard());
            //    await Navigation.PushAsync(new AdminDashboard());
            //}
            //else if (enteredEmail == stcorrecttemp && enteredPassword == stcorrecttemp)
            //{
            //    if (DeviceInfo.Platform == DevicePlatform.Android)
            //    {
            //        Application.Current.MainPage = new NavigationPage(new StudentDashboard());
            //        await Navigation.PushAsync(new StudentDashboard());
            //    }
            //    else if (DeviceInfo.Platform != DevicePlatform.Android)
            //    {
            //        Application.Current.MainPage = new NavigationPage(new StudentDashboardWindows());
            //        await Navigation.PushAsync(new StudentDashboardWindows());
            //    }
            //}
            //else
            //{
            //    this.ShowPopup(new NewPage1());
            //}


            //ADD THIS IF WANT TO TEST REAL FUNCTIONALTIY!
            //NOTE NEED TO IMPORT DATABASE FROM THE FILES
            //PROPER CREDENTIALS look in the database adminusers and studusers!!!

            if (await CheckAdminAccount(enteredEmail, enteredPassword))
            {
                var toast = Toast.Make("Logging in as admin", ToastDuration.Short);
                toast.Show();
                Application.Current.MainPage = new NavigationPage(new AdminDashboard());
                await Navigation.PushAsync(new AdminDashboard());


            }
            else if (await CheckRegisteredAccount(enteredEmail, enteredPassword))
            {

                SessionVars.SetSessionId(enteredEmail, enteredPassword);
                var toast = Toast.Make("Logging in as " + SessionVars.TakeStudentInfo(), ToastDuration.Short);
                toast.Show();
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    Application.Current.MainPage = new NavigationPage(new StudentDashboard());
                    await Navigation.PushAsync(new StudentDashboard());
                }
                else if (DeviceInfo.Platform != DevicePlatform.Android)
                {
                    Application.Current.MainPage = new NavigationPage(new StudentDashboardWindows());
                    await Navigation.PushAsync(new StudentDashboardWindows());
                }





            }
            else
            {
                this.ShowPopup(new NewPage1());
                LoginBtn.IsEnabled = true;
                userEntry.IsEnabled = true;
                userPass.IsEnabled = true;
                userEntry.Text = "";
                userPass.Text = "";
                return;
            }

        }
        private async Task<bool> CheckRegisteredAccount(string enteredEmail, string enteredPassword)
        {
            string query = "SELECT 1 FROM StudentUsers WHERE Student_Email = @Email AND Student_Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
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
                    DisplayAlert("testerror", ex.Message, "OK");
                    return false;
                }
            }
        }

        private async Task<bool> CheckAdminAccount(string enteredEmail, string enteredPassword)
        {
            string query = "SELECT 1 FROM AdminUsers WHERE Admin_Email = @Email AND Admin_Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
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