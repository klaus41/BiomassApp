﻿using System;
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
        static GlobalData gd = GlobalData.GetInstance;

        public App()
        {
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
                gd.Done = false;
                var s = await Database.GetConnectionSetting(0);

                gd.ConnectionSettings = s;
            }
            else
            {
                ConnectionSettings settings = new ConnectionSettings()
                {
                    ID = 0,
                    BaseAddress = "http://opgaver.eliteit.dk/"
                };
                await Database.SaveConnectionSetting(settings);
                gd.ConnectionSettings = settings;
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
            string s = gd.ConnectionSettings.LastUser;
        }

        protected override void OnSleep()
        {
            gd.Done = true;
        }

        protected override void OnResume()
        {
            gd.Done = false;
        }
    }
}
