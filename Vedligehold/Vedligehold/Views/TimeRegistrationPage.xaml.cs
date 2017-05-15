using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;
using Vedligehold.Services;
using Vedligehold.Services.Synchronizers;
using Vedligehold.Views.CustomCells;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class TimeRegistrationPage : ContentPage
    {
        List<JobRecLine> jobList;
        List<JobRecLine> jobItemsSourceList;
        ListView lv;
        MaintenanceDatabase db = App.Database;
        GlobalData gd = GlobalData.GetInstance;
        MaintenanceTask taskGlobal;

        Grid gridInfo;
        Grid grid;

        Label asset;
        Label type;
        Label text;
        Label assetDescription;
        Label header;

        Button doneButton;
        Button jobLineButton;
        public TimeRegistrationPage(MaintenanceTask task)
        {
            taskGlobal = task;
            Title = "Tidsregistrering";

            var temp = new DataTemplate(typeof(CustomCaseCell));

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp
            };
            lv.ItemTapped += Lv_ItemTapped;



            MakeGrid();
            UpdateItemsSource();

            StackLayout layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                    {
                        gridInfo,
                        lv,
                        grid
                    }

            };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            Content = layout;

            MessagingCenter.Subscribe<JobRecLine>(this, "hi", (sender) =>
            {
                Navigation.PushModalAsync(new JobRecLineUpdateForm(sender));
            });
        }

        private void MakeGrid()
        {
            doneButton = new Button() { Text = "Tilbage", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            jobLineButton = new Button() { Text = "Registrer tid", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            doneButton.Clicked += DoneButton_Clicked;
            jobLineButton.Clicked += JobLineButton_Clicked;

            asset = new Label()
            {
                Text = "Anlæg: " + taskGlobal.anlæg,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 14,
            };
            assetDescription = new Label()
            {
                Text = "Beskrivelse: " + taskGlobal.anlægsbeskrivelse,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 14,
            };
            type = new Label()
            {
                Text = "Type: " + taskGlobal.TaskType,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 14,
            };
            text = new Label()
            {
                Text = "Tekst: " + taskGlobal.text,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 14,
            };
            header = new Label()
            {
                Text = "Opgave nr. " + taskGlobal.no,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            };


            grid = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                },
                VerticalOptions = LayoutOptions.EndAndExpand,
            };

            gridInfo = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                },
                BackgroundColor = Color.FromRgb(135, 206, 250),

            };

            if (taskGlobal.status == "Completed")
            {
                gridInfo.BackgroundColor = Color.FromRgb(135, 206, 250);
            }
            else
            {
                gridInfo.BackgroundColor = Color.FromRgb(205, 201, 201);
                header.TextColor = Color.Black;
                asset.TextColor = Color.Black;
                assetDescription.TextColor = Color.Black;
                type.TextColor = Color.Black;
                text.TextColor = Color.Black;
            }

            gridInfo.Margin = 10;

            gridInfo.Children.Add(header, 0, 0);
            Grid.SetColumnSpan(header, 2);
            gridInfo.Children.Add(asset, 0, 1);
            gridInfo.Children.Add(assetDescription, 0, 2);
            gridInfo.Children.Add(type, 1, 1);
            gridInfo.Children.Add(text, 1, 2);

            grid.Children.Add(jobLineButton, 0, 0);
            grid.Children.Add(doneButton, 0, 1);

        }

        private void JobLineButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new JobRecLineForm(taskGlobal));
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var action = ((ListView)sender).SelectedItem;
            JobRecLine c = (JobRecLine)action;

            this.Navigation.PushModalAsync(new JobRecLineDetail(c));
        }

        private async void UpdateItemsSource()
        {
            jobList = null;


            jobList = await App.Database.GetJobRecLinesAsync();

            Resources resource = null;

            List<Resources> rl = await db.GetResourcesAsync();
            resource = rl.Where(x => x.Name == gd.User.Name).FirstOrDefault();

            List<JobRecLine> jsl = jobList.Where(x => x.MaintenanceTaskNo == taskGlobal.no.ToString()).ToList();
            jobItemsSourceList = jsl.Where(x => x.No == resource.No).ToList();
            lv.ItemsSource = jobItemsSourceList;

        }

        protected override void OnAppearing()
        {
            UpdateItemsSource();
        }


    }
}
