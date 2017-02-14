using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage(Statistic[] stats)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection(),
                ColumnDefinitions = new ColumnDefinitionCollection(),
                ColumnSpacing = 0,
                RowSpacing = 1,

            };


            Color color;

            foreach (var stat in stats)
            {
                if (Array.IndexOf(stats, stat) % 2 == 0)
                {
                    color = Color.FromRgb(211, 211, 211);
                }
                else
                {
                    color = Color.Default;
                }

                grid.Children.Add(new Label
                {
                    Text = "Leverandørnummer: " + stats[0].SupplNo,
                    //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 4, 0, 1);

                grid.Children.Add(new Label
                {
                    Text = stat.mdr,
                    BackgroundColor = color
                }, 0, Array.IndexOf(stats, stat) + 2);

                grid.Children.Add(new Label
                {
                    Text = stat.maengde_raa.ToString(),
                    BackgroundColor = color
                }, 1, Array.IndexOf(stats, stat) + 2);

                grid.Children.Add(new Label
                {
                    Text = stat.gns_torstof_raa.ToString(),
                    BackgroundColor = color
                }, 2, Array.IndexOf(stats, stat) + 2);

                grid.Children.Add(new Label
                {
                    Text = stat.maengde_afg.ToString(),
                    BackgroundColor = color
                }, 3, Array.IndexOf(stats, stat) + 2);

            }

            //grid.Children.Add(new Label
            //{
            //    Text = "Leverandørnummer: " + stats[0].SupplNo,
            //    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            //    HorizontalOptions = LayoutOptions.Center
            //}, 0, 3, 0, 1);

            grid.Children.Add(new Label
            {
                Text = "Måned"
            }, 0, 1);

            grid.Children.Add(new Label
            {
                Text = "Mængde rå"
            }, 1, 1);

            grid.Children.Add(new Label
            {
                Text = "Gennemsnit rå"
            }, 2, 1);

            grid.Children.Add(new Label
            {
                Text = "Mængde afgasset"
            }, 3, 1);


            // Accomodate iPhone status bar.
            this.Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 5);

            // Build the page.
            this.Content = grid;



        }
    }
}
