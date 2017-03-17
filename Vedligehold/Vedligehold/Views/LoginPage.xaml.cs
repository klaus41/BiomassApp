using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Vedligehold.Models;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class LoginPage : ContentPage
    {
        bool usernameText = false;
        bool passwordText;
        Button button;
        Entry username;
        Entry password;
        GlobalData gd = GlobalData.GetInstance;

        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var layout = new StackLayout { Padding = 10, };
            BackgroundColor = Color.White;
            Image image = new Image() { Source = "eistor.png", Opacity = 0.7, HorizontalOptions = LayoutOptions.StartAndExpand };
            Title = "Log ind";
            var label = new Label
            {
                Text = "Log ind",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center
            };

            //layout.Children.Add(label);

            username = new Entry { Placeholder = "Brugernavn" };
            layout.Children.Add(username);

            password = new Entry { Placeholder = "Password", IsPassword = true };
            layout.Children.Add(password);

            button = new Button { Text = "Log ind", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            layout.Children.Add(button);
            layout.Children.Add(image);

            password.TextChanged += Password_TextChanged;
            username.TextChanged += Username_TextChanged;

            button.Clicked += async (s, e) =>
            {
                bool succes = false;
                bool loading = true;
                while (loading)
                {
                    ActivityIndicator ai = new ActivityIndicator()
                    {
                        IsRunning = true
                    };
                    layout.Children.Add(ai);
                    button.IsEnabled = false;

                    var sv = new SalesPersonService();
                    try
                    {
                        SalesPerson person = await sv.GetSalesPersonAsync(username.Text);
                        succes = true;
                        gd.User = person.Code;
                        gd.SearchUserName = person.Code;
                    }
                    catch
                    {
                      
                    }
                    if (!succes)
                    {
                        layout.Children.Remove(ai);
                        await DisplayAlert("Advarsel", "Forkert brugernavn eller password", "OK");
                    }
                    else
                    {
                        gd.SearchDateTime = DateTime.Today;

                        gd.TabbedPage.Children.Add(new HomePage());
                        gd.TabbedPage.Children.Add(new TimeRegistrationPage());
                        gd.TabbedPage.Children.Add(new MaintenancePage());
                        gd.TabbedPage.Children.Add(new SettingsPage());

                        gd.TabbedPage.Children.Remove(gd.LoginPage);
                        //await Navigation.PushAsync(new HomePage());
                        //await Navigation.PushAsync(new TabbedPage
                        //{
                        //    Children =
                        //    {
                        //        new HomePage(),
                        //        new MaintenancePage(),
                        //        new SettingsPage(),
                        //        new TimeRegistrationPage()
                        //    },

                        //});
                        //this.Navigation.RemovePage(this.Navigation.NavigationStack[0]);

                        password.Text = null;
                    }
                    loading = false;
                    button.IsEnabled = true;

                    layout.Children.Remove(ai);
                }
            };

            Content = new ScrollView { Content = layout };

        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            usernameText = true;
            if (username.Text == null)
            {
                usernameText = false;
            }
            if (usernameText && passwordText)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordText = true;
            if (password.Text == null)
            {
                passwordText = false;
            }
            if (usernameText && passwordText)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        protected override void OnAppearing()
        {
            //bool usernameText = false;
            bool passwordText = false;
            button.IsEnabled = false;
        }
    }
}
