using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vedligehold.Database;
using Vedligehold.Views;
using Xamarin.Forms;

namespace Vedligehold
{
    public class App : Application
    {
        static MaintenanceDatabase database;

        public App()
        {
            //MainPage = new NavigationPage(new LoginPage());
            MainPage = new NavigationPage(new LoginPage());
        }
        public static MaintenanceDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new MaintenanceDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("MaintenanceSQLite.db3"));
                }
                return database;
            }
        }

        public int ResumeAtMaintenanceId { get; set; }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
