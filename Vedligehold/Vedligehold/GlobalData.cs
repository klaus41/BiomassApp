using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Views;
using Xamarin.Forms;

namespace Vedligehold
{
    public class GlobalData
    {
        private static GlobalData globaldata;
        private TabbedPage tabbedPage;
        private LoginPage loginPage;
        private string user;
        private DateTime searchDateTime;
        private string searchUserName;
        private TimeRegistrationModel timeRegisteredIn;
        private TimeRegistrationModel timeRegisteredOut;

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
        public string User
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

    }
}
