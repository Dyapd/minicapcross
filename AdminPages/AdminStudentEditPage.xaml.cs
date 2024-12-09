using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
        UserChangeBtn.IsEnabled = false;
        
        bool answer = await DisplayAlert("Confirmation", "Are you sure about this edit?", "Yes", "No");
        string email = EmailInput.Text;
        string password = PasswordInput.Text;
        string oldid = StudentNumberText.Text;
        string studid = NumberInput.Text;
        string studname = NameInput.Text;
        string gradeLevel = StudentGradeLevels.SelectedItem?.ToString();
        string section = StudentSections.SelectedItem?.ToString();

        if (validateEmail(email))
        {
            await DisplayAlert("Error", "Invalid email format.", "OK");
            UserChangeBtn.IsEnabled = true;
            return;
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(oldid) || string.IsNullOrEmpty(studid) || string.IsNullOrEmpty(studname) || string.IsNullOrEmpty(gradeLevel) || string.IsNullOrEmpty(section))
        {
            await DisplayAlert("Error", "Please fill in all required fields.", "OK");
            UserChangeBtn.IsEnabled = true;
            return;
        }

        if (studid.Length != 8 || !studid.All(char.IsDigit))
        {
            await DisplayAlert("Error", "New Student number must be 8 digits and contain only numbers.", "OK");
            UserChangeBtn.IsEnabled = true;

            return;
        }

        if (oldid.Length != 8 || !oldid.All(char.IsDigit))
        {
            await DisplayAlert("Error", "Old Student number must be 8 digits and contain only numbers.", "OK");
            UserChangeBtn.IsEnabled = true;

            return;
        }


        if (answer)
        {
            try
            {
                string connectionString = new IPLocator().ConnectionString();
                SqlConnection connection = new SqlConnection(connectionString);
                using (connection)
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                    "UPDATE StudentUsers SET Student_Email = @email, Student_Password = @password, Student_ID = @studid WHERE Student_ID = @oldid; " +
                    "UPDATE StudentInfo SET Student_Name = @name, Student_GradeLevel = @gradeLevel, Student_Section = @section, Student_ID = @studid WHERE Student_ID = @oldid",
                    connection))
                    {
                        command.Parameters.AddWithValue("@oldid", oldid);
                        command.Parameters.AddWithValue("@studid", studid);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@name", studname);
                        command.Parameters.AddWithValue("@gradeLevel", gradeLevel);
                        command.Parameters.AddWithValue("@section", section);
                        await command.ExecuteNonQueryAsync();
                    }





                    await DisplayAlert("Success", "Student edited successfully.", "OK");
                    UserChangeBtn.IsEnabled = true;

                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
                UserChangeBtn.IsEnabled = true;
                return;

            }
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
        SectionText.Text = Users[0].StudentSection.ToString();
        LevelText.Text = Users[0].StudentGrade.ToString();
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
                command.CommandText = "SELECT s.Student_ID, s2.Student_Name, s.Student_Email, s.Student_Password, s2.Student_GradeLevel, s2.Student_Section FROM StudentUsers s INNER JOIN StudentInfo s2 ON s2.Student_ID = s.Student_ID WHERE s.Student_ID = @studnum";

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
                                StudentGrade = reader.GetString(4),
                                StudentSection = reader.GetString(5)
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

    public bool validateEmail(string email)
    {
        //@ means start
        //^@ means all string except @
        // \s means no whitespace
        // \. means a dot
        // $ means end of string

        var pattern = @"^[^@\s] + @[^@\s] + \.[^@\s]+$";
        var regex = new Regex(pattern);
        bool match = regex.IsMatch(email);

        return match;
    }
}