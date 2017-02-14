using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vedligehold.Views;
using Xamarin.Forms;

namespace Vedligehold
{
    public class App : Application
    {
        public App()
        {
            //tabbedpage
            //MainPage = new CarouselPage
            //{
            //    Children =
            //    {
            //        new HomePage(),
            //        new Page1("Hello!"),
            //        new ListView(),

            //    }
            //};
            MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.Green };
        }

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
