using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;
using Vedligehold.Views.CustomCells;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class StatisticsPage : ContentPage
    {
        Color color;
        ListView lv;
        Statistic[] statsGlobal;

        public StatisticsPage(Statistic[] stats)
        {
            statsGlobal = stats;
            InitializeComponent();

            //MakeToolBar();
            NavigationPage.SetHasNavigationBar(this, false);
            MakeListView(stats);
            //MakeGrid(stats);        
        }
        private void MakeToolBar()
        {
            ToolbarItems.Add(new ToolbarItem("Hjem", "filter.png", async () =>
            {
                if (this.GetType() != typeof(HomePage))
                {
                    await Navigation.PushAsync(new HomePage());
                }
            }));
            ToolbarItems.Add(new ToolbarItem("Statistik", "filter.png", async () =>
            {
                string data = null;
                try
                {
                    PDFService pds = new PDFService();
                    data = await pds.GetPDF("A00005");
                    HomePage hp = new HomePage();
                    hp.StatButtonMethod();
                }
                catch
                {
                    await DisplayAlert("Forbindelse", "Enheden har ingen forbindelse til NAV", "OK");
                }
            }));

            ToolbarItems.Add(new ToolbarItem("Opgaver", "filter.png", async () =>
            {
                if (this.GetType() != typeof(MaintenancePage))
                {
                    await Navigation.PushAsync(new MaintenancePage());
                }
            }));

            ToolbarItems.Add(new ToolbarItem("Indstillinger", "filter.png", async () =>
            {
                if (this.GetType() != typeof(SettingsPage))
                {
                    await Navigation.PushAsync(new SettingsPage());
                }
            }));

        }

        private void MakeListView(Statistic[] stats)
        {
            var temp = new DataTemplate(typeof(CustomStatCell));
            Application.Current.Properties["gridrowindex"] = 1;

            lv = new ListView();

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    lv
                }
            };

            lv.HasUnevenRows = true;
            lv.ItemTemplate = temp;

            lv.ItemsSource = stats;
            lv.IsPullToRefreshEnabled = true;
            lv.Refreshing += Lv_Refreshing;
            //lv.ItemTapped += Lv_ItemTapped;

        }
        async void Lv_Refreshing(object sender, EventArgs e)
        {
            Statistic[] _stats = null;

            while (_stats == null)
            {
                var sv = new StatisticService();
                var es = await sv.GetStatsAsync(statsGlobal[0].SupplNo);
                _stats = es;
            }
            MakeListView(_stats);

            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }

        private void MakeGrid(Statistic[] stats)
        {
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection(),
                ColumnDefinitions = new ColumnDefinitionCollection(),
                ColumnSpacing = 0,
                RowSpacing = 1,

            };


           
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
