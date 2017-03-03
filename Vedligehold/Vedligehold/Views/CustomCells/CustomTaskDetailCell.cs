using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views.CustomCells
{
    class CustomTaskDetailCell : ViewCell
    {
        Color color = Color.Default;
        public CustomTaskDetailCell()
        {
            SetColor();

            Label type = new Label();
            Label value = new Label();

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
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(type, 0, 0);
            mainGrid.Children.Add(value, 1, 0);

            mainGrid.BackgroundColor = color;
            View = mainGrid;

            type.SetBinding<TaskDetailModel>(Label.TextProperty, i => i.type);

            value.SetBinding<TaskDetailModel>(Label.TextProperty, i => i.value);

            //MakeCustomCell();
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

    }
}
