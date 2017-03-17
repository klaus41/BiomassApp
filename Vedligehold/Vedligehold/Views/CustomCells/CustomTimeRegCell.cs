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
               HorizontalOptions = LayoutOptions.Start
            };
            Label time = new Label()
            {
                HorizontalOptions = LayoutOptions.Center

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
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(4,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(no, 0, 0);
            Grid.SetRowSpan(no, 2);

            mainGrid.Children.Add(start, 1, 0);
            mainGrid.Children.Add(end, 1, 1);
            mainGrid.Children.Add(date, 3, 0);
            Grid.SetColumnSpan(date, 3);
            Grid.SetColumnSpan(time, 4);

            mainGrid.Children.Add(time, 3, 1);
            View = mainGrid;

            start.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.User);
            end.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Type);
            no.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.No);
            //date.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Time.Date);
            //time.SetBinding<TimeRegistrationModel>(Label.TextProperty, i => i.Time.ToString("HH:mm"));
            date.SetBinding(Label.TextProperty, new Binding("Time", converter: new DateTimeToDateConverter()));
            time.SetBinding(Label.TextProperty, new Binding("Time", converter: new DateTimeToTimeConverter()));
            mainGrid.SetBinding(Label.BackgroundColorProperty, new Binding("Time", converter: new EndTimeToColorConverter()));
            mainGrid.Margin = 10;

        }
    }
}
