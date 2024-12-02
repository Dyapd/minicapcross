using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Windows.Input;
using static test.DataHolders.DataholderNotificationLog;

namespace test.Pages
{
    public partial class AdminItems : ContentPage
    {
        
        public ObservableCollection<DynamicItems> DynamicItems { get; set; }
        ICommand ButtonCommand { get; }
        public AdminItems()
        {
            InitializeComponent();
            DynamicItems = new ObservableCollection<DynamicItems>();
            ButtonCommand = new Command<string>(OnDetailsClicked);
            BindingContext = this;
        }

        private void OnDetailsClicked(string obj)
        {
            //Navigation.PushAsync(new AdminDynamicPage());
            
        }

        public void OnAddItemBtnClicked(object args, EventArgs e)
        {
            Navigation.PushAsync(new AdminSubmittedPage());
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
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT " +
                    "Claim_Category, Claims_ID, Claim_Status, Claim_ICategory, Claim_Description, " +
                    "Student_Number, Claim_Image  FROM Items";

                    command.Parameters.AddWithValue("@claimID", Convert.ToInt32(SessionVars.DynamicClaim));

                    using (SqlDataReader reader =  command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                items.Add(new DynamicItems
                                {

                                    ID = reader.GetInt32(1).ToString(),
                                    Status = reader.GetBoolean(2),
                                    ICategory = reader.GetString(3),
                                    Description = reader.GetString(4),
                                    StudentNumber = reader.GetString(5),
                                    Image = reader.IsDBNull(6) ? null : reader["Item_Image"] as byte[]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DisplayAlert("Error", e.Message, "OK");
            }
            return items;
        }
    }

}