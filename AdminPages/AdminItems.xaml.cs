using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace test.Pages
{
    public partial class AdminItems : ContentPage
    {
        
        public ObservableCollection<ItemModel> ItemList { get; set; }
        ICommand ButtonCommand { get; }
        public AdminItems()
        {
            InitializeComponent();
            ButtonCommand = new Command<string>(OnDetailsClicked);


            ItemList = new ObservableCollection<ItemModel>
            {
                new ItemModel { ItemID = "1", ItemImage = "item1.jpg", Category = "Electronics", Status = "Available" },
                new ItemModel { ItemID = "2", ItemImage = "item2.jpg", Category = "Clothing", Status = "Out of Stock" },
                new ItemModel { ItemID = "3", ItemImage = "item3.jpg", Category = "Books", Status = "Available" },
                new ItemModel { ItemID = "4", ItemImage = "item4.jpg", Category = "Accessories", Status = "Available" }
            };

            
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


    }

    
    public class ItemModel
    {
        public string ItemID { get; set; }
        public string ItemImage { get; set; }  
        public string Category { get; set; }
        public string Status { get; set; }
    }
}