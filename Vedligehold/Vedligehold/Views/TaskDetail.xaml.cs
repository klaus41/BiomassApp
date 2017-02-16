using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class TaskDetail : ContentPage
    {
        public TaskDetail(MaintenanceTask task)
        {
            InitializeComponent();
            Labl.Text = task.anlæg;
        }
    }
}
