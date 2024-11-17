using Microsoft.Maui.Controls;
using System.Data.SqlClient;

namespace test
{
    public partial class MainPage : ContentPage
    {
        private const string AdminContactNumber = "011504";
        private const string AdminPassword = "1234";
        string connectionString = "Data Source=localhost;Initial Catalog=Register;Integrated Security=True";

        public MainPage()
        {
            InitializeComponent();
        }
        private void OnLoginClicked(object sender, EventArgs e)
        {
            string enteredEmail = "Allen";
            string enteredPassword = "Payad";

            Navigation.PushAsync(new StudentDashboard());


            if (enteredEmail == AdminContactNumber && enteredPassword == AdminPassword)
            {
                //Insert Admin Page Method Here
            }
            else
            {
                if (CheckRegisteredAccount(enteredEmail, enteredPassword))
                {
                    //Insert Student Page Method Here
                }
                else
                {
                    // MessageBox.Show("Invalid credentials.");
                }
            }
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
        private void StudentDashboardLogin(string Student_Password, string Student_Email)
        {

        }
    }
}