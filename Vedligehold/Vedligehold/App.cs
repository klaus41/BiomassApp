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
            //database.DeleteAllTimeReg();
            GlobalData gd = GlobalData.GetInstance;
            gd.TabbedPage.Children.Add(gd.LoginPage);
            //gd.TabbedPage.Children.Add(new HomePage());
            //gd.TabbedPage.Children.Add(new MaintenancePage());
            //gd.TabbedPage.Children.Add(new SettingsPage());
            //gd.TabbedPage.Children.Add(new TimeRegistrationPage());
            MainPage = gd.TabbedPage;
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
