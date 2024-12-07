using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
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
        bool answer = await DisplayAlert("Confirmation", "Are you sure about this edit?", "Yes", "No");


        try
        {
            string stringConnection = new IPLocator().ConnectionString();
            SqlConnection connection = new SqlConnection(stringConnection);
            using (connection)
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO StudentUsers (Student_ID, Student_Email, Student_Password) VALUES (@studentId, @email, @password); " +
                                                                "INSERT INTO StudentInfo (Student_ID, Student_Name, Student_GradeLevel, Student_Section) VALUES (@studentId, @name, @gradeLevel, @section)", connection))
                {
                    //command.Parameters.AddWithValue("@studentId", studentId);
                    //command.Parameters.AddWithValue("@email", email);
                    //command.Parameters.AddWithValue("@password", password);
                    //command.Parameters.AddWithValue("@name", name);
                    //command.Parameters.AddWithValue("@gradeLevel", gradeLevel);
                    //command.Parameters.AddWithValue("@section", section);

                    await command.ExecuteNonQueryAsync();
                }

                await DisplayAlert("Success", "Student created successfully.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }

    }

    private async void OnDeleteClick(object sender, EventArgs events)
    {
        bool answer = await DisplayAlert("Confirmation", "Are you sure you want to delete this student user?", "Yes", "No");
        if (answer)
        {
            try
            {
                string connectionString = new IPLocator().ConnectionString();
                SqlConnection connection = new SqlConnection(connectionString);

                using (connection)
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM StudentInfo WHERE Student_ID = @StudID";
                    command.Parameters.AddWithValue("@StudID", SessionVars.DynamicStudentID);

                    command.ExecuteNonQuery();
                    DisplayAlert("Succesful deletion.", "User has been successfuly removed!", "OK");
                    await Navigation.PopAsync();

                }
            }

            catch (Exception ex)
            {
                DisplayAlert("Error in removing item!", ex.Message, "OK");
            }
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
 

                            users.Add(new Users
                            {

                                StudentNumber = reader.GetInt64(0).ToString(),
                                StudentName = reader.GetString(1),
                                StudentEmail = reader.GetString(2),
                                StudentPassword = reader.GetString(3),
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