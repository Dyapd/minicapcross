using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Security.Claims;
using static test.DataHolders.DataholderNotificationLog;


namespace test;

public partial class ClaimPage : ContentPage
{
    string connectionString;
    string reportCategory;
    byte[] leftImageBytes;
    public ObservableCollection<Items> Items { get; set; }
    public ObservableCollection<Items2> Items2 { get; set; }


    public ClaimPage()
    {
        InitializeComponent();
        //DisplayAlert("Test", SessionVars.SessionId, "OK");
        Items = new ObservableCollection<Items>();
        Items2 = new ObservableCollection<Items2>();
        BindingContext = this;
        LoadItems();
    }
    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(DescriptionInput.Text))
        {
            DisplayAlert("Validation Error!", "Please fill in all required fields.", "OK");
            ClaimBtn.IsEnabled = true;
            return;
        }

        SaveToDatabase();
        Navigation.PopAsync();
    }

    //this changes the picture on the left depending on the selection

    private void OnPickerSelectedIndexChangedLeft(object sender, EventArgs e)
    {
        
        try
        {
            rightImage.Source = null;
            comboBox.SelectedItem = null;
            leftImage.Source = null;
            
            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();
            
            var selectedOption = comboBoxLeft.SelectedItem as Items;
            reportCategory = selectedOption.ICategory;
            if (string.IsNullOrEmpty(selectedOption.ID))
            {
                DisplayAlert("Error", "No option selected!", "OK!");

                return;
            }




            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Report_Image FROM Reports WHERE Report_ID = @SelectedID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SelectedID", selectedOption.ID);

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
                LoadItems2();
            }
            else
            {
                DisplayAlert("Error", "No image found for the selected option!", "OK!");
                leftImage.Source = null;
                LoadItems2();
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
            rightImage.Source = null;
       
            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();

            var selectedItem2 = comboBox.SelectedItem as Items2;
            var selectedOption = selectedItem2.ICategory;

            if (string.IsNullOrEmpty(selectedOption))
            {
            DisplayAlert("Error", "No option selected!", "OK!");
            return;
            }

        

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Item_Image FROM Items WHERE Item_ID = @SelectedID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SelectedID", selectedItem2.ID);

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

    private  void SaveToDatabase()
    {
        try
        {
            IPLocator ip = new IPLocator();
            connectionString = ip.ConnectionString();
            var selectedReport = comboBoxLeft.SelectedItem as Items;
            var selectedImage = comboBox.SelectedItem as Items2;

            //this is way easier than editing a lot of code late-production :(
            var category = "defunct";
            var description = DescriptionInput.Text;
            var studentNumber = SessionVars.SessionId;
            var ReportID = selectedReport.ID;
            var ClaimID = selectedImage.ID;

            if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(studentNumber))
            {
                 DisplayAlert("Error", "All fields must have values!", "OK!");
                return;
            }

            if (leftImageBytes == null)
            {
                insertWithoutImage( category,  description,  ReportID,  ClaimID);
            }
            else
            {
                insertWithImage(category, description, ReportID, ClaimID);
            }
        }
        catch (Exception e)
        {
            DisplayAlert("ERROR", e.Message, "OK");
        }
    }

    private void insertWithoutImage(string category, string description, string reportID, string claimID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Claims (Claim_Category, Claim_Status, Claim_ICategory, Claim_Description, Student_Number, Report_Category, Report_ID, Item_Category, Item_ID)
                                VALUES (@Category, @Status, @ICategory, @Description, @Student, @RCategory, @RID, @ITCategory, @IID)" +
                                "UPDATE Reports SET Report_Status = 1 WHERE Report_ID = @reportID;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Category", "C");
                command.Parameters.AddWithValue("@Status", false);
                command.Parameters.AddWithValue("@ICategory", category);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Student", SessionVars.SessionId);
                command.Parameters.AddWithValue("@RCategory", "R");
                command.Parameters.AddWithValue("@RID", reportID);
                command.Parameters.AddWithValue("@ITCategory", "I");
                command.Parameters.AddWithValue("@IID", claimID);

                command.Parameters.AddWithValue("@reportID", reportID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    DisplayAlert("Success", "Claim has been recorded successfully, without image.", "OK");
                }
                else
                {
                    DisplayAlert("Failure", "Claim failed to insert.", "OK");
                }

            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error without image insertion!", e.Message, "OK");
        }
    }

    private void insertWithImage(string category, string description, string reportID, string claimID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Claims (Claim_Category, Claim_Status, Claim_ICategory, Claim_Description, Student_Number, Claim_Image, Report_Category, Report_ID, Item_Category, Item_ID)
                                VALUES (@Category, @Status, @ICategory, @Description, @Student, @Image, @RCategory, @RID, @ITCategory, @IID);" +
                                "UPDATE Reports SET Report_Status = 1 WHERE Report_ID = @reportID;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Category", "C");
                command.Parameters.AddWithValue("@Status", false);
                command.Parameters.AddWithValue("@ICategory", category);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Student", SessionVars.SessionId);
                command.Parameters.AddWithValue("@Image", leftImageBytes); 
                command.Parameters.AddWithValue("@RCategory", "R");
                command.Parameters.AddWithValue("@RID", reportID);
                command.Parameters.AddWithValue("@ITCategory", "I");
                command.Parameters.AddWithValue("@IID", claimID);

                command.Parameters.AddWithValue("@reportID", reportID);

                connection.Open();



                int rowsAffected =  command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    DisplayAlert("Success", "Claim has been recorded successfully.", "OK");
                }
                else
                {
                    DisplayAlert("Failure", "Claim failed to insert.", "OK");
                }

            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error without image insertion!", e.Message, "OK");
        }
    }
    //ahead methods are for the use of binding tables to the picker tags!!
    public List<Items> ReadDataNotificationLog()
    {
        List<Items> items = new List<Items>();

        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Report_ID, Report_Category, Report_Location, Report_ICategory FROM Reports WHERE Student_Number = @SessionVars AND Report_Status = 0";
                command.Parameters.AddWithValue("@SessionVars", SessionVars.SessionId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Items
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Category = reader.GetString(1),
                            Location = reader.GetString(2),
                            ICategory = reader.GetString(3)
                        });
                    }
                }

            }
        }
        catch (Exception ex)
        {

        }
        return items;

    }

    private void LoadItems()
    {
        List<Items> items = ReadDataNotificationLog();
        Items.Clear();
        foreach (Items item in items)
        {
            Items.Add(item);
        }
    }

    public List<Items2> ReadNotificationLog2()
    {
        List<Items2> items2 = new List<Items2>();

        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT i.Item_ID, i.Item_Category, i.Item_Location, i.Item_ICategory FROM Items i " +
                    "JOIN Reports r ON i.Item_ICategory = r.Report_ICategory " +
                    "WHERE i.Item_ICategory = @reportCategory";
                command.Parameters.AddWithValue("@reportCategory", reportCategory);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items2.Add(new Items2
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Category = reader.GetString(1),
                            Location = reader.GetString(2),
                            ICategory = reader.GetString(3)
                        });;
                    }
                }

            }
        }
        catch (Exception ex)
        {
            DisplayAlert("ERROR", ex.Message, "Ok");
        }
        return items2;

    }

    private  void LoadItems2()
    {
        List<Items2> items =  ReadNotificationLog2();
        Items2.Clear();
        foreach (Items2 item in items)
        {
            Items2.Add(item);
        }
    }
}
