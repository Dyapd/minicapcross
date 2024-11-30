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
                DisplayAlert("Error", "No option selected!", "OK!");

                return;
            }




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
                DisplayAlert("Error", "No image found for the selected option!", "OK!");
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
            DisplayAlert("Error", "No option selected!", "OK!");
            return;
        }

        

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
                DisplayAlert("Error", "No image found for the selected option!", "OK!");
                rightImage.Source = null;
        }
        }
        catch (Exception ev)
        {
            DisplayAlert("Error!", ev.Message, "OK!");
        }
    }

    private async void SaveToDatabase()
    {
        try
        {


            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();

            var category = CategoryInput.SelectedItem.ToString();
            var description = DescriptionInput.Text;
            var studentNumber = SessionVars.SessionId;
            var ReportID = comboBoxLeft.SelectedItem.ToString();
            var ClaimID = comboBox.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(studentNumber))
            {
                DisplayAlert("Error", "All fields must have values!", "OK!");
                return;
            }




            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Claims (Claim_Category, Claim_Status, Claim_ICategory, Claim_Description, Student_Number, Claim_Image, Report_Category, Report_ID, Item_Category, Item_ID)
                                VALUES (@Category, @Status, @ICategory, @Description, @Student, @Image, @RCategory, @RID, @ITCategory, @IID)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Category", "C");
                command.Parameters.AddWithValue("@Status", false);
                command.Parameters.AddWithValue("@ICategory", category);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Student", SessionVars.SessionId);
                command.Parameters.AddWithValue("@Image", leftImageBytes);
                command.Parameters.AddWithValue("@RCategory", "R");
                command.Parameters.AddWithValue("@RID", ReportID);
                command.Parameters.AddWithValue("@ITCategory", "I");
                command.Parameters.AddWithValue("@IID", ClaimID);

                connection.Open();
                


                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Success", "Claim has been recorded successfully.", "OK");
                }
                else
                {
                    await DisplayAlert("Failure", "Claim failed to insert.", "OK");
                }

            }

        }
        catch (Exception e)
        {
            await DisplayAlert("ERROR", e.Message, "OK");
        }
    }

    
}
