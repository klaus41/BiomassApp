using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class CustomTaskCell : ViewCell
    {
        Color color = Color.Default;
        public CustomTaskCell()
        {
            SetColor();

            Label type = new Label();
            Label anlægsbeskrivelse = new Label()
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            };
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
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(image, 0, 0);
            Grid.SetRowSpan(image, 2);

            mainGrid.Children.Add(type, 1, 0);
            mainGrid.Children.Add(anlægsbeskrivelse, 1, 1);

            mainGrid.Children.Add(done, 4, 0);
            Grid.SetRowSpan(done, 2);

            mainGrid.BackgroundColor = color;
            View = mainGrid;

            image.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.no);

            type.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.type);
            anlægsbeskrivelse.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.anlægsbeskrivelse);
            done.SetBinding<MaintenanceTask>(Label.TextProperty, i => i.done);

            //MakeCustomCell();
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

        private void MakeCustomCell()
        {
            StackLayout cellWrapper = new StackLayout();
            StackLayout horizontalLayout = new StackLayout();
            Label no = new Label();
            Label type = new Label();
            Label anlæg = new Label();
            Label anlægsbeskrivelse = new Label();
            Label text = new Label();

            //set bindings
            no.SetBinding(Label.TextProperty, "no");
            type.SetBinding(Label.TextProperty, "type");
            anlæg.SetBinding(Label.TextProperty, "anlæg");
            anlægsbeskrivelse.SetBinding(Label.TextProperty, "anlægsbeskrivelse");
            text.SetBinding(Label.TextProperty, "text");

            //Set properties for desired design
            cellWrapper.BackgroundColor = Color.FromHex("#eee");
            horizontalLayout.Orientation = StackOrientation.Horizontal;
            anlægsbeskrivelse.HorizontalOptions = LayoutOptions.FillAndExpand;
            text.HorizontalOptions = LayoutOptions.EndAndExpand;

            //add views to the view hierarchy
            //horizontalLayout.Children.Add(no);
            horizontalLayout.Children.Add(type);
            //horizontalLayout.Children.Add(anlæg);
            horizontalLayout.Children.Add(anlægsbeskrivelse);
            horizontalLayout.Children.Add(text);

            cellWrapper.Children.Add(horizontalLayout);
            View = cellWrapper;
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

        void OnMore(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            //Do something here... e.g. Navigation.pushAsync(new specialPage(item.commandParameter));
            //page.DisplayAlert("More Context Action", item.CommandParameter + " more context action", 	"OK");
        }

    }
}