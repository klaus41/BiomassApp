using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Views.Converters;
using Xamarin.Forms;

namespace Vedligehold.Views.CustomCells
{
    public class CustomTimeRegCell : ViewCell
    {
        Color color = Color.Default;

        public CustomTimeRegCell()
        {
            Label start = new Label();
            Label end = new Label()
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            };
            Label no = new Label();
            Label date = new Label()
            {
               HorizontalOptions = LayoutOptions.End
            };
            Label time = new Label()
            {
                HorizontalOptions = LayoutOptions.End

            };
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
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(no, 0, 0);
            Grid.SetRowSpan(no, 2);

            mainGrid.Children.Add(start, 1, 0);
            mainGrid.Children.Add(end, 1, 1);
            mainGrid.Children.Add(date, 4, 0);
            mainGrid.Children.Add(time, 4, 1);

            Grid.SetColumnSpan(end, 2);
            Grid.SetColumnSpan(date, 4);
            Grid.SetColumnSpan(time, 4);

            View = mainGrid;

            start.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.User);
            end.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Type);
            no.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.No);
            //date.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Time.Date);
            //time.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Time.ToString("HH:mm"));
            date.SetBinding(Label.TextProperty, new Binding("Time", converter: new DateTimeToDateConverter()));
            time.SetBinding(Label.TextProperty, new Binding("Time", converter: new DateTimeToTimeConverter()));
            
            mainGrid.SetBinding(Label.BackgroundColorProperty, new Binding("Type", converter: new TimeRegTypeToColorConverter()));
            if (Device.OS == TargetPlatform.iOS)
            {
                mainGrid.Margin = 0;
            }
            else
            {
                mainGrid.Margin = 10;
            }
        }
    }
}
