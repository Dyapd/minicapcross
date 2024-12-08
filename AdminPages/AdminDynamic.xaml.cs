using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
#if ANDROID
using Android.App;
#endif

namespace test;

public partial class AdminDynamic : TabbedPage
{
    Button currentButton; 
    public ObservableCollection<DynamicClaims> DynamicClaims { get; set; }
    public ObservableCollection<DynamicReports> DynamicReports { get; set; }
    public ObservableCollection<DynamicItems> DynamicItems { get; set; }

    public ICommand StatusClaim { get; set; }
    public ICommand RejectClaim { get; set; }
    public ICommand SaveClaim { get; set; }
    public ICommand DoneClaim { get; set; }

    public byte[] claimImage;
    string reportID, itemID;


    public AdminDynamic()
    {

        InitializeComponent();

        DynamicClaims = new ObservableCollection<DynamicClaims>();
        DynamicReports = new ObservableCollection<DynamicReports>();
        DynamicItems = new ObservableCollection<DynamicItems>();

        StatusClaim = new Command(OnStatusClicked);
        RejectClaim = new Command(OnRejectClicked);
        SaveClaim = new Command(OnSaveClicked);
        DoneClaim = new Command(OnDoneClicked);


        BindingContext = this;
        LoadItemsClaims();
        

    }

    


    private void OnDoneClicked(object obj)
    {
        string connectionString = new IPLocator().ConnectionString();
        
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using(connection)
            {
                connection.Open();
                string studentName = SessionVars.TakeStudentInfoAdmin(DynamicReports[0].StudentNumber);
                SqlCommand command = connection.CreateCommand();
                
                
                

                command.CommandText = "INSERT INTO Logs (Log_Category, Item_Category, Item_ID, Log_ICategory, Submitted_On, Received_By, Taken_Out)" +
                    " VALUES (@Category, @ItemCategory, @ItemID, @ICategory, @Submitted, @Received, @Taken)";

                command.Parameters.AddWithValue("@Category", "L");
                command.Parameters.AddWithValue("@ItemCategory", "I");
                command.Parameters.AddWithValue("@ItemID", DynamicItems[0].ID);
                command.Parameters.AddWithValue("@ICategory", DynamicItems[0].ICategory);
                command.Parameters.AddWithValue("@Submitted", DateTime.Parse(DynamicItems[0].Date));
                command.Parameters.AddWithValue("@Received", studentName);
                command.Parameters.AddWithValue("@Taken", DateTime.Now);

                command.ExecuteNonQuery();
                DisplayAlert("Done!", "Succesfully inserted into logs!", "OK");


                string claimID = DynamicClaims[0].ID.ToString();

                //delete the child row first!
                command.Parameters.Clear();
                command.CommandText = "DELETE FROM Claims WHERE Claims_ID = @ClaimIDF";
                command.Parameters.AddWithValue("@ClaimIDF", claimID);
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.CommandText = "DELETE FROM Items WHERE Item_ID = @ItemIDD";
                command.Parameters.AddWithValue("@ItemIDD", itemID);
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.CommandText = "DELETE FROM Reports WHERE Report_ID = @ReportIDD";
                command.Parameters.AddWithValue("@ReportIDD", reportID);
                command.ExecuteNonQuery();
                DisplayAlert("Done!", "Succesfully deleted corresponding rows!", "OK");
                Navigation.PopAsync();
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error on closing claim", e.Message, "OK");
        }
    }



    private void OnStatusClicked(object obj)
    {
        string stringConnection = new IPLocator().ConnectionString();
        SqlConnection connection = new SqlConnection(stringConnection);
        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                if (DynamicClaims[0].Status)
                {
                    
                    command.CommandText = "UPDATE Claims SET Claim_Status = 0 WHERE Claims_ID = @ClaimID";
                    command.Parameters.AddWithValue("@ClaimID", DynamicClaims[0].ID.ToString());
                    command.ExecuteNonQuery();
                    DisplayAlert("Error status clicked", "Changed approval status of claim to false!", "OK");
                    LoadItemsClaims();
                }
                else
                {
                    command.CommandText = "UPDATE Claims SET Claim_Status = 1 WHERE Claims_ID = @ClaimID";
                    command.Parameters.AddWithValue("@ClaimID", DynamicClaims[0].ID.ToString());
                    command.ExecuteNonQuery();
                    DisplayAlert("Error status clicked", "Changed approval status of claim to true!", "OK");
                    LoadItemsClaims();

                }




            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error status clicked", e.Message, "OK");
        }

    }
    private async void OnRejectClicked(object obj)
    {
        string connectionString = new IPLocator().ConnectionString();
        try
        {
            SqlConnection connection = new SqlConnection((string)connectionString);

            bool answer = await DisplayAlert("Confirmation","Are you sure you want to reject this application?", "Yes", "No");

            if (answer)
            {
                using (connection)
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Claims WHERE Claims_ID = @ClaimID";
                    command.Parameters.AddWithValue("@ClaimID", DynamicClaims[0].ID.ToString());

                    command.ExecuteNonQuery();
                    DisplayAlert("Error status clicked", "Sucessfully deleted! Redirecting to claims page.", "OK");
                    Navigation.PopAsync();

                }
            }
            
      
        }
        catch (Exception e)
        {
            DisplayAlert("Error rejection clicked", e.Message, "OK");
        }

    }

    //for sending emails possibly
    private async void OnSaveClicked(object obj)
    {
        var emailMessage = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "This is a test email.",
            To = new List<string> { "allenpayad1@gmail.com" }
        };

        try
        {
            await Email.ComposeAsync(emailMessage);
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle the exception when email is not supported on the device.
        }
        catch (Exception ex)
        {
            // Handle other types of exceptions.
        }
    }





    //populate both contentpages
    private async void populateDynamicPage()
    {
        try
        {
            //for claim page
            ClaimStatusText.Text = DynamicClaims[0].Status.ToString();
            ClaimDescriptionText.Text = DynamicClaims[0].Description.ToString();
            
            //put in done claiming if here
            bool resultbool = DynamicClaims[0].Status;
            if (resultbool)
            {
                if (currentButton != null)
                {
                    DynamicButtons.Remove(currentButton);
                }

                var saveButton = new Button

                
                {
                    Text = "Item Claimed?",
                    BackgroundColor = Color.FromHex("#D32F2F"),
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    HeightRequest = 45,
                    Command = DoneClaim
                };
                DynamicButtons.Children.Insert(1, saveButton);

                currentButton = saveButton;
            }
            else
            {
                if (currentButton != null)
                {
                    DynamicButtons.Remove(currentButton);
                }
            }


            // picture display
            if (DynamicClaims[0].Image != null)
            {
                ClaimImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicClaims[0].Image));
            }
            

            //for item page
            ItemImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicItems[0].Image));
            ItemCategoryText.Text = DynamicItems[0].ICategory.ToString();
            ItemLocationText.Text = DynamicItems[0].Location.ToString();
            ItemDateAndTimeText.Text = DynamicItems[0].Date.ToString();
            ItemDescriptionText.Text = DynamicItems[0].Description.ToString();
            ItemStatusText.Text = DynamicItems[0].Status.ToString();

            //for report page
            if (DynamicClaims[0].Image != null)
            {
                ReportImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicReports[0].Image));
            }
                

            ReportCategoryText.Text = DynamicReports[0].ICategory.ToString();
            ReportLocationText.Text = DynamicReports[0].Location.ToString();
            ReportDateAndTimeText.Text = DynamicReports[0].Date.ToString();
            ReportDescriptionText.Text = DynamicReports[0].Description.ToString();
            ReportStatusText.Text = DynamicReports[0].Status.ToString();


        }
        catch (Exception e)
        {
           DisplayAlert("Error in populating report page!", e.Message, "OK");
        }


    }

    //claim report only
    private async Task<List<DynamicClaims>> takeFromDatabaseClaim()
    {
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        List<DynamicClaims> claims = new List<DynamicClaims>();
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();


                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Claim_Category, Claims_ID, Claim_Status, Claim_ICategory, Claim_Description, " +
                    "Student_Number, Claim_Image, Report_ID, Item_ID  FROM Claims WHERE Claims_ID = @claimID";

                command.Parameters.AddWithValue("@claimID", Convert.ToInt32(SessionVars.DynamicClaim));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //await DisplayAlert("Test", "Reader is running!.", "OK");
                        while (reader.Read())
                        {
                            //await DisplayAlert("Test2", reader.GetString(4), "OK");
                            //ClaimCategoryText.Text = reader.GetString(4);
                            reportID = reader.GetInt32(7).ToString();
                            itemID = reader.GetInt32(8).ToString();
                            claims.Add(new DynamicClaims
                            {

                                ID = reader.GetInt32(1).ToString(),
                                Category = reader.GetString(0),
                                Status = reader.GetBoolean(2),
                                ICategory = reader.GetString(3),
                                Description = reader.GetString(4),
                                StudentNumber = reader.GetString(5),
                                ReportId = reader.GetInt32(7).ToString(),
                                ItemId = reader.GetInt32(8).ToString(),
                                Image = reader.IsDBNull(6) ? null : reader["Claim_Image"] as byte[]
                            });

                        }

                    }
                    else
                    {
                        await DisplayAlert("No Data Claim", "NoItems!.", "OK");
                    }


                }


            }
        }
        catch (Exception e)
        {
            await DisplayAlert("ERROR CLaim", e.Message, "OK");
        }
        LoadItemsReports();
        LoadItemsItems();
        return claims;

    }
    private async void LoadItemsClaims()
    {
        List<DynamicClaims> claims = await takeFromDatabaseClaim();
        DynamicClaims.Clear();
        foreach (DynamicClaims claim in claims)
        {
            DynamicClaims.Add(claim);
        }

        //await DisplayAlert("Items Added", $"{DynamicClaims.Count} items have been added.", "OK");
        populateDynamicPage();

    }

    private async Task<List<DynamicReports>> takeFromDatabaseReport()
    {
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        List<DynamicReports> reports = new List<DynamicReports>();
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();


                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Report_ID, Report_Date, Report_ICategory, Report_Description, " +
                    "Report_Location, Student_Number, Report_Image, Report_Status FROM Reports WHERE Report_ID = @ReportID";

                command.Parameters.AddWithValue("@ReportID", reportID);

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
                            reports.Add(new DynamicReports
                            {

                                ID = reader.GetInt32(0).ToString(),
                                Date = date,
                                ICategory = reader.GetString(2),
                                Location = reader.GetString(4),
                                Description = reader.GetString(3),
                                StudentNumber = reader.GetString(5),
                                Image = reader.IsDBNull(6) ? null : reader["Report_Image"] as byte[],
                                Status = reader.GetBoolean(7),
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
        return reports;

    }
    private async void LoadItemsReports()
    {
        List<DynamicReports> reports = await takeFromDatabaseReport();
        DynamicReports.Clear();
        foreach (DynamicReports report in reports)
        {
            DynamicReports.Add(report);
        }

        //await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }

    private async Task<List<DynamicItems>> takeFromDatabaseItems()
    {
        List<DynamicItems> items = new List<DynamicItems>();
        try
        {

            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
           

            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                await connection.OpenAsync();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Item_ID, Item_Image, Item_ICategory, Item_Date, Item_Description, Item_Location, Item_Status " +
                    "FROM Items WHERE Item_ID = @itemID";
                command.Parameters.AddWithValue("@itemID", itemID);

                using(SqlDataReader reader = command.ExecuteReader())
                {
                    //check if there is a row
                    if (reader.HasRows) 
                    {
                        //so more than oen row will be read
                        while (reader.Read())
                        {
                            DateTime itemDate = reader.GetDateTime(3);
                            string date = itemDate.ToString("yyyy-MM-dd HH:mm");
                            items.Add(new DynamicItems
                            {

                                ID = reader.GetInt32(0).ToString(),
                                Date = date,
                                ICategory = reader.GetString(2),
                                Location = reader.GetString(5),
                                Description = reader.GetString(4),
                                Image = reader.IsDBNull(1) ? null : reader["Item_Image"] as byte[],
                                Status = reader.GetBoolean(6),
                            });
                        }
                    }
                }

            }
        }
        catch (Exception e)
        {
            await DisplayAlert("Error!", e.Message, "OK");
        }
        return items;
    }
    private async void LoadItemsItems()
    {
        List<DynamicItems> items = await takeFromDatabaseItems();
        DynamicItems.Clear();
        foreach (DynamicItems item in items)
        {
            DynamicItems.Add(item);
        }

        //await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }
    
    
    
    

}


