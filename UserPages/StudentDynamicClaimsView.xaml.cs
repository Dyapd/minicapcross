using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
#if ANDROID
using Android.App;
#endif


namespace test.UserPages;

public partial class StudentDynamicClaimsView : ContentPage
{
    List<DynamicClaims> DynamicClaims;
    public byte[] claimImage;
  

    public StudentDynamicClaimsView()
	{
		InitializeComponent();
        DynamicClaims = new List<DynamicClaims>();
        LoadItemsClaims();


    }

    public async void OnRescindClick(object obj, EventArgs e)
    {

        string connectionString = new IPLocator().ConnectionString();
        bool answer = await DisplayAlert("Confirmation", "Are you sure you want to rescind this claim?", "Yes", "No");
        try
        {
            if (answer)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                using (connection)
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Claims WHERE Claims_ID = @claimID; "+
                        "UPDATE Reports SET Report_Status = 0 WHERE Report_ID = @reportID";
                    command.Parameters.AddWithValue("@claimID", DynamicClaims[0].ID.ToString());
                    command.Parameters.AddWithValue("@reportID", DynamicClaims[0].ReportId.ToString());
                    command.ExecuteNonQuery();
                    DisplayAlert("Successful update!", "Rescinded claim successfully!", "OK");

                }
                Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error in rescinding.", ex.Message, "OK");
        }

    }
    private async void populateDynamicPage()
    {
        try
        {
            //for claim page
            ClaimCategoryText.Text = DynamicClaims[0].ICategory.ToString();
            ClaimStatusText.Text = DynamicClaims[0].Status.ToString();
            ClaimDescriptionText.Text = DynamicClaims[0].Category.ToString();

            // picture display
            if (DynamicClaims[0].Image != null)
            {
                ClaimImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicClaims[0].Image));
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error in populating page!", e.Message, "OK");
        }
    }
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

        //await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }
}