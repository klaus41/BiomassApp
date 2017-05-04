using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Views.Converters;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class CustomTaskCell : ViewCell
    {
        Color color = Color.Default;
        public CustomTaskCell()
        {
            //SetColor();

            Label typeLabel = new Label();
            Label plannedDateLabel = new Label()
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            Label responsibleLabel = new Label()
            {

            };
            Label noLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            Label descriptionLabel = new Label();
            Label customerLabel = new Label();



            Grid mainGrid = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(noLabel, 0, 0);
            mainGrid.Children.Add(typeLabel, 1, 0);
            mainGrid.Children.Add(plannedDateLabel, 5, 0);
            mainGrid.Children.Add(customerLabel, 1, 1);

            mainGrid.Children.Add(responsibleLabel, 3, 0);

            mainGrid.Children.Add(descriptionLabel, 3, 1);

            Grid.SetRowSpan(noLabel, 2);
            Grid.SetRowSpan(plannedDateLabel, 2);
            Grid.SetColumnSpan(customerLabel, 2);
            Grid.SetColumnSpan(responsibleLabel, 2);
            Grid.SetColumnSpan(typeLabel, 2);
            Grid.SetColumnSpan(descriptionLabel, 2);



            //Grid mainGrid = new Grid
            //{
            //    Padding = new Thickness(10),
            //    RowDefinitions =
            //    {
            //        new RowDefinition { Height = GridLength.Auto },
            //        new RowDefinition { Height = GridLength.Auto },
            //        new RowDefinition { Height = GridLength.Auto },
            //        new RowDefinition { Height = GridLength.Auto }
            //    },
            //    ColumnDefinitions =
            //    {
            //        new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
            //        new ColumnDefinition { Width = new GridLength(3,GridUnitType.Star) },
            //        new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
            //    }
            //};

            //mainGrid.Children.Add(noLabel, 0, 0);
            //mainGrid.Children.Add(typeLabel, 1, 0);
            //mainGrid.Children.Add(plannedDateLabel, 4, 0);
            //mainGrid.Children.Add(customerLabel, 1, 2);

            //mainGrid.Children.Add(responsibleLabel, 1, 1);

            //mainGrid.Children.Add(descriptionLabel, 1, 3);

            //Grid.SetRowSpan(noLabel, 4);
            //Grid.SetRowSpan(plannedDateLabel, 4);
            //Grid.SetColumnSpan(customerLabel, 3);
            //Grid.SetColumnSpan(responsibleLabel, 3);
            //Grid.SetColumnSpan(typeLabel, 3);
            //Grid.SetColumnSpan(descriptionLabel, 5);

            mainGrid.BackgroundColor = color;
            View = mainGrid;

            noLabel.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.no);
            typeLabel.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.TaskType);
            plannedDateLabel.SetBinding(Label.TextProperty, new Binding("planned_Date", converter: new DateTimeToDateConverter()));
            responsibleLabel.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.responsible);
            descriptionLabel.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.text);
            customerLabel.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.CustomerName);

            mainGrid.SetBinding(Label.BackgroundColorProperty, new Binding("status", converter: new MaintenanceTaskRowColor()));


            if (Device.OS == TargetPlatform.iOS)
            {
                mainGrid.Margin = 0;
            }
            else
            {
                mainGrid.Margin = 10;
            }
            //MakeCustomCell();
            CreateMenu();
        }


        private void CreateMenu()
        {
            var pdfAction = new MenuItem { Text = "Vis dokument" };
            pdfAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            pdfAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                MaintenanceTask _task = (MaintenanceTask)mi.CommandParameter;
                MaintenancePage mp = new MaintenancePage();
                Debug.WriteLine("!!!!!!!!!!!!!!!!!!!" + _task.anlæg);
                mp.ShowPDF(_task);
            };

            var doneAction = new MenuItem { Text = "Udført" };
            doneAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            doneAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                MaintenanceTask _task = (MaintenanceTask)mi.CommandParameter;

                MaintenancePage mp = new MaintenancePage();
                mp.SetDone(_task);

            };
            var mapAction = new MenuItem { Text = "Kort" };
            mapAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            mapAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                MaintenanceTask _task = (MaintenanceTask)mi.CommandParameter;
                MaintenancePage mp = new MaintenancePage();
                mp.ShowOnMap(_task);
            };

            ContextActions.Add(pdfAction);
            ContextActions.Add(mapAction);
            ContextActions.Add(doneAction);
        }

        void OnMore(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            //Do something here... e.g. Navigation.pushAsync(new specialPage(item.commandParameter));
            //page.DisplayAlert("More Context Action", item.CommandParameter + " more context action", 	"OK");
        }

    }
}