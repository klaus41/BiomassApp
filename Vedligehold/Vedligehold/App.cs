using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vedligehold.Database;
using Vedligehold.Services;
using Vedligehold.Views;
using Xamarin.Forms;

namespace Vedligehold
{
    public class App : Application
    {
        static MaintenanceDatabase database;

        public App()
        {
            GlobalData gd = GlobalData.GetInstance;
            if (gd.IsLoggedIn)
            {
                MainPage = gd.TabbedPage;
            }
            else
            {
                gd.TabbedPage.Children.Add(gd.LoginPage);
                MainPage = gd.TabbedPage;
            }
            ThreadManager tm = new ThreadManager();
            tm.StartSynchronizationThread();
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
