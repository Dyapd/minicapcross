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
            string stringConnection = new IPLocator().ConnectionString();
            SqlConnection connection = new SqlConnection(stringConnection); 
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
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }
    }
}