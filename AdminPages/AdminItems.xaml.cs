using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace test.Pages
{
    public partial class AdminItems : ContentPage
    {
        
        public ObservableCollection<ItemModel> ItemList { get; set; }

        public AdminItems()
        {
            InitializeComponent();

            
            ItemList = new ObservableCollection<ItemModel>
            {
                new ItemModel { ItemID = "1", ItemImage = "item1.jpg", Category = "Electronics", Status = "Available" },
                new ItemModel { ItemID = "2", ItemImage = "item2.jpg", Category = "Clothing", Status = "Out of Stock" },
                new ItemModel { ItemID = "3", ItemImage = "item3.jpg", Category = "Books", Status = "Available" },
                new ItemModel { ItemID = "4", ItemImage = "item4.jpg", Category = "Accessories", Status = "Available" }
            };

            
            BindingContext = this;
        }

        
        public async void OnAddItemBtnClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new AdminSubmittedPage());
        }
    }

    
    public class ItemModel
    {
        public string ItemID { get; set; }
        public string ItemImage { get; set; }  
        public string Category { get; set; }
        public string Status { get; set; }
    }
}