using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;

namespace test;

public partial class AdminDynamic : TabbedPage
{

    public ObservableCollection<DynamicClaims> DynamicClaims { get; set; }
    public byte[] claimImage;

    public AdminDynamic()
	{
        
        InitializeComponent();
        DynamicClaims = new ObservableCollection<DynamicClaims>();
        BindingContext = this;
        LoadItems();
        //populateDynamicPage();
    }

	private void populateDynamicPage()
	{
        ClaimCategoryText.Text = DynamicClaims[0].Category.ToString();

    }

	private async Task<List<DynamicClaims>> takeFromDatabase()
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
                        while (reader.Read())
                        {
                            claims.Add(new DynamicClaims
                            {
                                ID = reader.GetInt32(1).ToString(),
                                Category = reader.GetString(0),
                                Status = reader.GetBoolean(2),
                                ICategory = reader.GetInt32(3).ToString(),
                                Description = reader.GetString(4),
                                StudentNumber = reader.GetString(5),
                                ReportId = reader.GetString(7),
                                ItemId = reader.GetString(8),
                                Image = reader.IsDBNull(6) ? null : reader["Claim_Image"] as byte[]
                        });
                        }
                        
                    }
                    else
                    {
                        await DisplayAlert("No Data", "NoItems!.", "OK");
                    }
                    

                }
                

            }
        }
        catch (Exception e)
        {
            await DisplayAlert("ERROR", e.Message, "OK");
        }
        return claims;
		
	}

	private async void LoadItems()
	{
        List<DynamicClaims> claims = await takeFromDatabase();
        DynamicClaims.Clear();
        foreach (DynamicClaims claim in claims)
        {
            DynamicClaims.Add(claim);
        }

        await DisplayAlert("Items Added", $"{DynamicClaims.Count} items have been added.", "OK");
    }
}