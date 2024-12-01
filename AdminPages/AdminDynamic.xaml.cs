using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;

namespace test;

public partial class AdminDynamic : TabbedPage
{

    public ObservableCollection<DynamicClaims> DynamicClaims { get; set; }
    public ObservableCollection<DynamicReports> DynamicReports { get; set; }
    public byte[] claimImage;
    string reportID;

    public AdminDynamic()
	{
        
        InitializeComponent();
        DynamicClaims = new ObservableCollection<DynamicClaims>();
        DynamicReports = new ObservableCollection<DynamicReports>();
        BindingContext = this;
        LoadItemsClaims();
        
    }

    //populate both contentpages
	private async void populateDynamicPage()
	{
        try
        {
            //for claim page
            ClaimCategoryText.Text = DynamicClaims[0].ICategory.ToString();
            ClaimStatusText.Text = DynamicClaims[0].Status.ToString();
            ClaimDescriptionText.Text = DynamicClaims[0].Category.ToString();


            // picture display
            ClaimImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicClaims[0].Image));

            //for item page

            //for report page
            ReportCategoryText.Text = DynamicReports[0].ICategory.ToString();
        }
        catch (Exception e)
        {
            DisplayAlert("PopulateTest", e.Message, "OK");
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
        DynamicClaims.Clear();
        foreach (DynamicReports report in reports)
        {
            DynamicReports.Add(report);
        }

        await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }

}