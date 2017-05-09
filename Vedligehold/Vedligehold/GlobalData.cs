using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services.Synchronizers;
using Vedligehold.Views;
using Xamarin.Forms;

namespace Vedligehold
{
    public class GlobalData
    {
        private static GlobalData globaldata;
        private TabbedPage tabbedPage;
        private LoginPage loginPage;
        private bool isLoggedIn = false;
        private SalesPerson user;
        private DateTime searchDateTime;
        private DateTime searchDateTimeLast;
        private string searchUserName;
        private TimeRegistrationModel timeRegisteredIn;
        private TimeRegistrationModel timeRegisteredOut;
        private string baseAddress;
        private bool done;
        ConnectionSettings cs = new ConnectionSettings();

        private GlobalData() { }

        public static GlobalData GetInstance
        {
            get
            {
                if (globaldata == null)
                {
                    globaldata = new GlobalData();
                }
                return globaldata;
            }
        }
        public TabbedPage TabbedPage
        {
            get
            {
                if (tabbedPage == null)
                {
                    tabbedPage = new TabbedPage();
                }

                return tabbedPage;
            }
        }
        public LoginPage LoginPage
        {
            get
            {
                if (loginPage == null)
                {
                    loginPage = new LoginPage();
                }
                return loginPage;
            }
        }
        public SalesPerson User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }
        public DateTime SearchDateTime
        {
            get
            {
                return searchDateTime;
            }
            set
            {
                searchDateTime = value;
            }
        }
        public DateTime SearchDateTimeLast
        {
            get
            {
                return searchDateTimeLast;
            }
            set
            {
                searchDateTimeLast = value;
            }
        }
        public string SearchUserName
        {
            get
            {
                return searchUserName;
            }
            set
            {
                searchUserName = value;
            }
        }
        public TimeRegistrationModel TimeRegisteredIn
        {
            get
            {
                return timeRegisteredIn;
            }
            set
            {
                timeRegisteredIn = value;
            }
        }
        public TimeRegistrationModel TimeRegisteredOut
        {
            get
            {
                return timeRegisteredOut;
            }
            set
            {
                timeRegisteredOut = value;
            }
        }
        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }
            set
            {
                isLoggedIn = value;
            }
        }

        public string BaseAddress
        {
            get
            {
                return baseAddress;
            }
            set
            {
                baseAddress = value;
            }
        }

        public bool Done
        {
            get
            {
                return done;
            }
            set
            {
                done = value;
            }
        }

        public ConnectionSettings ConnectionSettings
        {
            get
            {
                if (cs.BaseAddress == null)
                {
                    GetConSet();   
                }
                return cs;
            }
            set
            {
                cs = value;
            }
        }

        private async void GetConSet()
        {
            cs = await App.Database.GetConnectionSetting(0);
        }
    }
}
