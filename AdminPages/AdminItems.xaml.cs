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
        public ICommand ButtonCommand { get; set; }
        public AdminItems()
        {
            InitializeComponent();
            DynamicItems = new ObservableCollection<DynamicItems>();
            ButtonCommand = new Command<string>(OnButtonClicked);
            BindingContext = this;
            LoadItems();
            ButtonHighlighters();
        }

        public void ButtonHighlighters()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
            {
                PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
                pointerGestureRecognizer.PointerEntered += (s, e) =>
                {
                    //mageenter 
                    AddBtn.BackgroundColor = Colors.SteelBlue;
                };
                pointerGestureRecognizer.PointerExited += (s, e) =>
                {
                    //mageexit
                    AddBtn.BackgroundColor = Color.FromArgb("#1565C0");
                };

                AddBtn.GestureRecognizers.Add(pointerGestureRecognizer);
            }
            
        }

        //this one refreshes data once page is popped!!
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private void OnButtonClicked(string obj)
        {
            SessionVars.DynamicItemID = obj;
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
                    "Item_ID, Item_Status, Item_ICategory, Item_Description" +
                    " FROM Items";

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
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "OK");
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