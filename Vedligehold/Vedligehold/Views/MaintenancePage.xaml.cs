using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class MaintenancePage : ContentPage
    {
        public MaintenancePage(MaintenanceTask[] tasks)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection(),
                ColumnDefinitions = new ColumnDefinitionCollection(),
                ColumnSpacing = 0,
                RowSpacing = 1
            };
            Color color;

            foreach (var task in tasks)
            {
                if (Array.IndexOf(tasks, task) % 2 == 0)
                {
                    color = Color.FromRgb(211, 211, 211);
                    //color = Color.FromRgb(173, 255, 47);
                }
                else
                {
                    color = Color.Default;
                    //color = Color.Green;
                }

                grid.Children.Add(new Label
                {
                    Text = "Vedligeholdelsesliste",
                    //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 4, 0, 1);

                grid.Children.Add(new Label
                {
                    Text = task.no.ToString(),
                    BackgroundColor = color
                }, 0, Array.IndexOf(tasks, task) + 2);

                grid.Children.Add(new Label
                {
                    Text = task.type,
                    BackgroundColor = color
                }, 1, Array.IndexOf(tasks, task) + 2);

                grid.Children.Add(new Label
                {
                    Text = task.anlæg,
                    BackgroundColor = color
                }, 2, Array.IndexOf(tasks, task) + 2);

                grid.Children.Add(new Label
                {
                    Text = task.text,
                    BackgroundColor = color
                }, 3, Array.IndexOf(tasks, task) + 2);


                grid.Children.Add(new Label
                {
                    Text = "Nummer"
                }, 0, 1);

                grid.Children.Add(new Label
                {
                    Text = "Type"
                }, 1, 1);

                grid.Children.Add(new Label
                {
                    Text = "Anlæg"
                }, 2, 1);

                grid.Children.Add(new Label
                {
                    Text = "Tekst"
                }, 3, 1);


                // Accomodate iPhone status bar.
                this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

                // Build the page.
                this.Content = grid;
            }
        }
    }
}

