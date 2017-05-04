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
    public class CustomCaseCell : ViewCell
    {
        Color color = Color.Default;
        public CustomCaseCell()
        {
            Label type = new Label();
            Label WorkType = new Label();
            Label anlægsbeskrivelse = new Label()
            {
                //FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                //HorizontalOptions = LayoutOptions.End
            };
            Image colorImg = new Image()
            {
                VerticalOptions = LayoutOptions.End
            };

            Label done = new Label()
            {
                //HorizontalOptions = LayoutOptions.CenterAndExpand,
                //VerticalOptions = LayoutOptions.CenterAndExpand
            };
            Label image = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                //VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Grid mainGrid = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    //new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2,GridUnitType.Star) }
                }
            };

            mainGrid.Children.Add(image, 1, 0);
            //Grid.SetRowSpan(image, 2);
            mainGrid.Children.Add(WorkType, 2, 0);
            mainGrid.Children.Add(type, 3, 0);
            mainGrid.Children.Add(anlægsbeskrivelse, 0, 0);

            //mainGrid.Children.Add(done, 4, 0);
            //Grid.SetRowSpan(done, 2);
            //Grid.SetColumnSpan(type, 2);
            mainGrid.BackgroundColor = color;
            View = mainGrid;

            image.SetBinding<JobRecLine>(Label.TextProperty, i => i.Unit_of_Measure_Code);
            WorkType.SetBinding<JobRecLine>(Label.TextProperty, i => i.WorkType);
            type.SetBinding<JobRecLine>(Label.TextProperty, i => i.Description);
            anlægsbeskrivelse.SetBinding<JobRecLine>(Label.TextProperty, i => i.Quantity);

            //done.SetBinding<JobRecLine>(Label.TextProperty, i => i.Journal_Batch_Name);
            mainGrid.BackgroundColor = Color.FromRgb(205, 201, 201);

            if (Device.OS == TargetPlatform.iOS)
            {
                mainGrid.Margin = 0;
            }
            else
            {
                mainGrid.Margin = 10;
            }
            CreateMenu();
        }
        private void CreateMenu()
        {
            var moreAction = new MenuItem { Text = "Rediger" };
            moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            moreAction.Clicked += (sender, e) =>
            {
                var mi = ((MenuItem)sender);
                JobRecLine _recLine = (JobRecLine)mi.CommandParameter;
                MessagingCenter.Send<JobRecLine>(_recLine, "hi");
                //(new JobRecLineUpdateForm(_recLine));

            };


            this.ContextActions.Add(moreAction);
        }
    }
}