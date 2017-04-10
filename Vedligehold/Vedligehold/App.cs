using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vedligehold.Database;
using Vedligehold.Models;
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
            checkedConnectionSettings();


        }

        private async void checkedConnectionSettings()
        {
            if (await Database.GetConnectionSetting(0) != null)
            {
                var s = await Database.GetConnectionSetting(0);
                ThreadManager tm = new ThreadManager();
                tm.StartSynchronizationThread();
            }
            else
            {
                ConnectionSettings settings = new ConnectionSettings()
                {
                    ID = 0,
                    BaseAddress = "http://vedligehold.biomass.eliteit.dk/"
                };
                await Database.SaveConnectionSetting(settings);
            }
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
