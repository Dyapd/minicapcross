using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace test.Pages
{
    public partial class AdminReportPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public AdminReportPage()
        {
            InitializeComponent();

            // Initialize the Items collection
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            // Set the BindingContext to the current page
            BindingContext = this;
        }
    }
}
