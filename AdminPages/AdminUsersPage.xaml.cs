using System.Data.SqlClient;

namespace test.AdminPages;

public partial class AdminUsersPage : ContentPage
{
	public AdminUsersPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        int studentId = int.Parse(StudentIdInput.Text);

        if (studentId.ToString().Length != 8)
        {
            await DisplayAlert("Error", "Student ID must be 8 digits.", "OK");
            return;
        }

        string name = StudentNameInput.Text;
        string gradeLevel = StudentGradeLevels.SelectedItem.ToString();
        string section = StudentSections.SelectedItem.ToString();
        string email = StudentEmailInput.Text;
        string password = StudentPasswordInput.Text;

        if (!email.Contains("@email.com"))
        {
            await DisplayAlert("Error", "Invalid email format.", "OK");
            return;
        }

        try
        {

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gradeLevel) || string.IsNullOrEmpty(section) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            if (!email.EndsWith("@email.com"))
            {
                await DisplayAlert("Error", "Invalid email format. Email must end with '@email.com'.", "OK");
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
                //pops page to go to previous
                await Navigation.PopAsync();
            }
        }
        catch (FormatException)
        {
            await DisplayAlert("Error", "Student ID must be numeric.", "OK");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627) // Duplicate key violation error code
            {
                await DisplayAlert("Error", "Student ID already exists. Please try again with a different ID.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }
    }
}