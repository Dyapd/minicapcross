using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
#if ANDROID
using Android.App;
#endif

namespace test.AdminPages;

public partial class AdminItemDynamicPage : ContentPage
{
    List<DynamicItems> DynamicItems;
    public AdminItemDynamicPage()
	{
		InitializeComponent();
		DynamicItems = new List<DynamicItems>();
		BindingContext = this;

		LoadItems();
        populateDynamicPage();
    }

    public void populateDynamicPage()
    {
        ItemImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicItems[0].Image));
        ItemCategoryText.Text = DynamicItems[0].ICategory.ToString();
        ItemLocationText.Text = DynamicItems[0].Location.ToString();
        ItemDateAndTimeText.Text = DynamicItems[0].Date.ToString();
        ItemDescriptionText.Text = DynamicItems[0].Description.ToString();
        ItemStatusText.Text = DynamicItems[0].Status.ToString();
    }

    //removing the selected item
    public async void OnRemoveClick(object sender, EventArgs e)
	{
        bool answer = await DisplayAlert("Confirmation", "Are you sure you want to rescind this item? This will result in related claims being deleted too!", "Yes", "No");
        if (answer)
        {
            try
            {
                string connectionString = new IPLocator().ConnectionString();
                SqlConnection connection = new SqlConnection(connectionString);

                using (connection)
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Items WHERE Item_ID = @itemID";
                    command.Parameters.AddWithValue("@itemID", SessionVars.DynamicReportID);

                    command.ExecuteNonQuery();
                    DisplayAlert("Succesful deletion.", "The item has been successfuly removed!", "OK");
                }
            }

            catch (Exception ex)
            {
                DisplayAlert("Error in removing item!", ex.Message, "OK");
            }
        }
        
    }

    private List<DynamicItems> takeFromDatabaseItems()
    {
        List<DynamicItems> items = new List<DynamicItems>();
        try
        {

            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();


            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Item_ID, Item_Image.PathName(), Item_ICategory, Item_Date, Item_Description, Item_Location, Item_Status " +
                    "FROM Items WHERE Item_ID = @itemID";
                command.Parameters.AddWithValue("@itemID", SessionVars.DynamicItemID);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //check if there is a row
                    if (reader.HasRows)
                    {
                        //so more than oen row will be read
                        while (reader.Read())
                        {
                            byte[] imageBytes;

                            DateTime itemDate = reader.GetDateTime(3);
                            string date = itemDate.ToString("yyyy-MM-dd HH:mm");

                            string imgPath = reader["PathName"].ToString();

                            using(FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read ))
                            {
                                imageBytes = new byte[fs.Length];
                                fs.Read(imageBytes, 0, imageBytes.Length);
                            }

                            items.Add(new DynamicItems
                            {

                                ID = reader.GetInt32(0).ToString(),
                                Date = date,
                                ICategory = reader.GetString(2),
                                Location = reader.GetString(5),
                                Description = reader.GetString(4),
                                Image = imageBytes,
                                Status = reader.GetBoolean(6),
                            });
                        }
                    }
                }

            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error!", e.Message, "OK");
        }
        return items;
    }
    private void LoadItems()
    {
        List<DynamicItems> items = takeFromDatabaseItems();
        DynamicItems.Clear();
        foreach (DynamicItems item in items)
        {
            DynamicItems.Add(item);
        }

        DisplayAlert("Items Added Reports", $"{DynamicItems.Count} items have been added.", "OK");
        

    }
}