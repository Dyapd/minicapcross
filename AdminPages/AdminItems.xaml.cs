using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Windows.Input;
using static test.DataHolders.DataholderNotificationLog;
using test.AdminPages;

namespace test.Pages
{
    public partial class AdminItems : ContentPage
    {
        
        public ObservableCollection<DynamicItems> DynamicItems { get; set; }
        public ICommand ButtonCommand { get; }
        public AdminItems()
        {
            InitializeComponent();
            DynamicItems = new ObservableCollection<DynamicItems>();
            ButtonCommand = new Command<string>(OnDetailsClicked);
            BindingContext = this;
            LoadItems();
        }

        private void OnDetailsClicked(string obj)
        {
            Navigation.PushAsync(new AdminItemDynamicPage());
            
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
                    "Item_ID, Item_Status, Item_ICategory, Item_Description, " +
                    "Item_Image FROM Items";

                    using (SqlDataReader reader =  await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                items.Add(new DynamicItems
                                {

                                    ID = reader.GetInt32(0).ToString(),
                                    Status = reader.GetBoolean(1),
                                    ICategory = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Image = reader.IsDBNull(4) ? null : reader["Item_Image"] as byte[]
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

        private async void LoadItems()
        {
            List<DynamicItems> items = await takeFromDatabaseItems();
            DynamicItems.Clear();
            foreach (DynamicItems item in items)
            {
                DynamicItems.Add(item);
            }
            
        }
    }

}