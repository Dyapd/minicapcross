using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
#if ANDROID
using Android.App;
#endif

namespace test.AdminPages;

public partial class AdminStudentEditPage : ContentPage
{
    List<Users> Users;
    public AdminStudentEditPage()
	{
		InitializeComponent();
        Users = new List<Users>();
        BindingContext = this;
        LoadItemsReports();
    }

    public async void OnConfirmClick(object sender, EventArgs s)
    {

        string connectionString = new IPLocator().ConnectionString();
        bool answer = await DisplayAlert("Confirmation", "Are you sure you want to rescind this application?", "Yes", "No");
        try
        {
            if (answer)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                using (connection)
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Reports WHERE Report_ID = @reportID";
                    command.Parameters.AddWithValue("@reportID", SessionVars.DynamicReportID);
                    command.ExecuteNonQuery();
                    DisplayAlert("Successful update!", "Rescinded report successfully!", "OK");

                }
                Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error in rescinding.", ex.Message, "OK");
        }

    }
    public void populateDynamicPage()
    {
        StudentNameText.Text = Users[0].StudentName.ToString();
        StudentNumberText.Text = Users[0].StudentNumber.ToString();
        StudentEmailText.Text = Users[0].StudentEmail.ToString();
        StudentPasswordText.Text = Users[0].StudentPassword.ToString();
      
    }

    private async Task<List<Users>> takeFromDatabaseUsers()
    {
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        List<Users> users = new List<Users>();
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();


                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT s.Student_ID, s2.Student_Name, s.Student_Email, s.Student_Password FROM StudentUsers s INNER JOIN StudentInfo s2 ON s2.Student_ID = s.Student_ID WHERE s.Student_ID = @studnum";

                command.Parameters.AddWithValue("@studnum", SessionVars.DynamicStudentID);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //await DisplayAlert("Test", "Report reader is running!.", "OK");
                        while (reader.Read())
                        {
                            //await DisplayAlert("Test2", reader.GetString(4), "OK");
                            //ClaimCategoryText.Text = reader.GetString(4);
                            DateTime reportDate = reader.GetDateTime(1);
                            string date = reportDate.ToString("yyyy-MM-dd HH:mm");
                            users.Add(new Users
                            {

                                StudentName = reader.GetInt32(0).ToString(),
                                StudentNumber = date,
                                StudentEmail = reader.GetString(2),
                                StudentPassword = reader.GetString(4),
                            });

                        }

                    }
                    else
                    {
                        await DisplayAlert("No Data Reports", "NoItems!.", "OK");
                    }


                }


            }
        }
        catch (Exception e)
        {
            await DisplayAlert("ERROR Reports", e.Message, "OK");
        }
        return users;

    }
    private async void LoadItemsReports()
    {
        List<Users> reports = await takeFromDatabaseUsers();
        Users.Clear();
        foreach (Users report in reports)
        {
            Users.Add(report);
        }

        //await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }
}