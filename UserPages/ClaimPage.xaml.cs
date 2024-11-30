using Microsoft.Data.SqlClient;

namespace test;

public partial class ClaimPage : ContentPage
{
    string connectionString;
    byte[] leftImageBytes;

    public ClaimPage()
    {
        InitializeComponent();
    }
    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        SaveToDatabase();
    }

    //this changes the picture on the left depending on the selection

    private async void OnPickerSelectedIndexChangedLeft(object sender, EventArgs e)
    {
        
        try
        {
            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();

            var selectedOption = comboBoxLeft.SelectedItem.ToString();

            if (string.IsNullOrEmpty(selectedOption))
            {
                test.Text = "No option selected.";
                return;
            }

            test.Text = $"You selected: {selectedOption}";



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Report_Image FROM Reports WHERE Report_ID = @SelectedID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SelectedID", selectedOption);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    leftImageBytes = reader["Report_Image"] as byte[];
                }
            }

            if (leftImageBytes != null)
            {
                var imageSource = ImageSource.FromStream(() => new MemoryStream(leftImageBytes));
                leftImage.Source = imageSource;
            }
            else
            {
                test.Text = "No image found for the selected option.";
                leftImage.Source = null;
            }
        }
        catch (Exception ev)
        {
            DisplayAlert("Error!", ev.Message, "OK!");
        }
    }

    //this changes the picture on the right depending on the selection

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        byte[] imageBytes = null;
        try
        {

       
        IPLocator ip = new IPLocator();
        connectionString = ip.ConnectionString();

        var selectedOption = comboBox.SelectedItem.ToString();

        if (string.IsNullOrEmpty(selectedOption))
        {
            test.Text = "No option selected.";
            return;
        }

        test.Text = $"You selected: {selectedOption}";

        

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Item_Image FROM Items WHERE Item_ID = @SelectedID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SelectedID", selectedOption);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                imageBytes = reader["Item_Image"] as byte[];
            }
        }

        if (imageBytes  != null)
        {
            var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            rightImage.Source = imageSource;
        }
        else
        {
            test.Text = "No image found for the selected option.";
            rightImage.Source = null;
        }
        }
        catch (Exception ev)
        {
            DisplayAlert("Error!", ev.Message, "OK!");
        }
    }

    private void SaveToDatabase()
    {
        IPLocator ip = new IPLocator();
        connectionString = ip.ConnectionString();

        var category = CategoryInput.Text;
        var description = DescriptionInput.Text;
        var studentNumber = SessionVars.SessionId;

        if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(studentNumber))
        {
            test.Text = "Please fill in all required fields.";
            return;
        }

        


        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO Reports (Report_Category, Report_Description, Student_Number, Report_Image) VALUES (@Category, @Description, @StudentNumber, @Image)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@StudentNumber", studentNumber);
            command.Parameters.AddWithValue("@Image", leftImageBytes);

            connection.Open();
            command.ExecuteNonQuery();
        }

        test.Text = "NASEND NA PUTANGINAMO.";
    }

    
}
