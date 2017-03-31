using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class MaintenanceActivityEdit : ContentPage
    {
        public MaintenanceActivityEdit(MaintenanceActivity activity)
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Page" }
                }
            };
        }
    }
}
