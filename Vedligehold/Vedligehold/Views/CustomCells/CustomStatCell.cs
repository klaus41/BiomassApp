using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views.CustomCells
{
    class CustomStatCell : ViewCell
    {
        Color color = Color.Default;
        public CustomStatCell()
        {
            SetColor();

            Label type = new Label();
            Label anlægsbeskrivelse = new Label();

            Label done = new Label();
            Label image = new Label();
            Label hidden = new Label();

            //Image image = new Image();

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
                    new ColumnDefinition { Width = new GridLength(4,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(image, 0, 0);
            Grid.SetRowSpan(image, 2);

            mainGrid.Children.Add(type, 1, 0);
            mainGrid.Children.Add(anlægsbeskrivelse, 1, 1);

            //mainGrid.Children.Add(done, 4, 0);
            //Grid.SetRowSpan(done, 2);

            mainGrid.BackgroundColor = color;
            View = mainGrid;

            image.SetBinding<Statistic>(Label.TextProperty, i => i.mdr);

            type.SetBinding<Statistic>(Label.TextProperty, i => i.gns_torstof_raa);
            type.HorizontalOptions = LayoutOptions.EndAndExpand;
            anlægsbeskrivelse.SetBinding<Statistic>(Label.TextProperty, i => i.gns_torstof_afg);
            anlægsbeskrivelse.HorizontalOptions = LayoutOptions.EndAndExpand;

            //done.SetBinding<Statistic>(Label.TextProperty, i => i.done);

            CreateMenu();
        }

        private void SetColor()
        {
            int rowindex = Convert.ToInt32(Application.Current.Properties["gridrowindex"]);

            if (rowindex % 2 == 0)
            {
                color = Color.Default;
            }
            else
            {
                color = Color.FromRgb(224, 224, 224);
            }

            rowindex = rowindex + 1;
            Application.Current.Properties["gridrowindex"] = rowindex;
        }

        private void CreateMenu()
        {
            var moreAction = new MenuItem { Text = "More" };
            moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            moreAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                System.Diagnostics.Debug.WriteLine("More Context Action clicked: " + mi.CommandParameter);
            };

            var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // Red background
            deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            deleteAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                System.Diagnostics.Debug.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
            };
            this.ContextActions.Add(moreAction);
            ContextActions.Add(deleteAction);
        }
    }
}
