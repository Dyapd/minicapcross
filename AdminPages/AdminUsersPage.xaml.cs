using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace test.AdminPages;

public partial class AdminUsersPage : ContentPage
{
	public AdminUsersPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        AddBtn.IsEnabled = false;
        string studentIdStr = StudentIdInput.Text;

        if (string.IsNullOrWhiteSpace(studentIdStr))
        {
            await DisplayAlert("Error", "Student ID cannot be empty.", "OK");
            return;
        }

        if (studentIdStr.Length != 8)
        {
            await DisplayAlert("Error", "Student ID must be 8 digits.", "OK");
            return;
        }

        try
        {
            int studentId = int.Parse(studentIdStr);

            string name = StudentNameInput.Text;
            string gradeLevel = StudentGradeLevels.SelectedItem?.ToString();
            string section = StudentSections.SelectedItem?.ToString();
            string email = StudentEmailInput.Text;
            string password = StudentPasswordInput.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gradeLevel) || string.IsNullOrEmpty(section) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all required fields.", "OK");
                AddBtn.IsEnabled = true;
                return;
            }

            if (validateEmail(email))
            {
                await DisplayAlert("Error", "Invalid email format.", "OK");
                AddBtn.IsEnabled = true;
                return;
            }


            string connectionString = new IPLocator().ConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            using (connection)
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO StudentUsers (Student_ID, Student_Email, Student_Password) VALUES (@studentId, @email, @password); " +
                                                                    "INSERT INTO StudentInfo (Student_ID, Student_Name, Student_GradeLevel, Student_Section) VALUES (@studentId, @name, @gradeLevel, @section)", connection))
                {
                    command.Parameters.AddWithValue("@studentId", studentId);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@gradeLevel", gradeLevel);
                    command.Parameters.AddWithValue("@section", section);

                    await command.ExecuteNonQueryAsync();
                }

                await DisplayAlert("Success", "Student created successfully.", "OK");
                AddBtn.IsEnabled = true;
            }
        }
        catch (FormatException)
        {
            await DisplayAlert("Error", "Student ID must be numeric.", "OK");
            AddBtn.IsEnabled = true;
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627) // Duplicate key violation error code
            {
                await DisplayAlert("Error", "Student ID already exists. Please try again with a different ID.", "OK");
                AddBtn.IsEnabled = true;
                return;
            }
            else
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
                AddBtn.IsEnabled = true;
                return;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            AddBtn.IsEnabled = true;
            return;
        }
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