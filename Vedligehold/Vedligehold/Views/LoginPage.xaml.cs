﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Vedligehold.Database;
using Vedligehold.Models;
using Vedligehold.Services;
using Vedligehold.Services.Synchronizers;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class LoginPage : ContentPage
    {
        bool usernameText = false;
        bool passwordText;
        Button button;
        Button connectionSettingsButton;
        Entry username;
        Entry password;

        ServiceFacade facade = ServiceFacade.GetInstance;
        SynchronizerFacade syncFacade = SynchronizerFacade.GetInstance;
        GlobalData gd = GlobalData.GetInstance;
        MaintenanceDatabase db = App.Database;
        public LoginPage()
        {

            NavigationPage.SetHasNavigationBar(this, false);

            var layout = new StackLayout { Padding = 10, };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }


            BackgroundColor = Color.White;
            Image image = new Image() { Source = "eistor.png", Opacity = 0.7, HorizontalOptions = LayoutOptions.StartAndExpand };
            Title = "Log ind";
            var label = new Label
            {
                Text = "Log ind",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center
            };

            username = new Entry { Placeholder = "Brugernavn" };


            layout.Children.Add(username);

            password = new Entry { Placeholder = "Password", IsPassword = true };
            layout.Children.Add(password);
            connectionSettingsButton = new Button { Text = "Opkoblingsindstillinger", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, VerticalOptions = LayoutOptions.End };
            button = new Button { Text = "Log ind", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            layout.Children.Add(button);
            layout.Children.Add(connectionSettingsButton);
            layout.Children.Add(image);

            password.TextChanged += Password_TextChanged;
            username.TextChanged += Username_TextChanged;

            connectionSettingsButton.Clicked += async (s, e) =>
            {
                await Navigation.PushModalAsync(new ConnectionSettingsPage());
            };
            button.Clicked += async (s, e) =>
            {
                bool succes = false;
                bool loading = true;
                bool connection = false;
                while (loading)
                {
                    ActivityIndicator ai = new ActivityIndicator()
                    {
                        IsRunning = true
                    };
                    layout.Children.Add(ai);
                    button.IsEnabled = false;
                    connectionSettingsButton.IsEnabled = false;

                    try
                    {
                        if (!await syncFacade.MaintenanceTaskSynchronizer.HasConnectionToNAV())
                        {
                            connection = false;
                        }
                        else
                        {
                            connection = true;
                        }
                        SalesPerson person = await facade.SalesPersonService.GetSalesPersonAsync(username.Text.ToUpper());

                        if (password.Text == person.Password)
                        {
                            succes = true;
                        }
                        gd.User = person;
                        gd.SearchUserName = person.Code;
                    
                    }
                    catch
                    {
                        
                    }
                    if (!succes && connection)
                    {
                        layout.Children.Remove(ai);
                        await DisplayAlert("Advarsel", "Forkert brugernavn eller password", "OK");
                    }
                    else if (!connection)
                    {
                        layout.Children.Remove(ai);
                        await DisplayAlert("Advarsel", "Enheden har ikke forbindelse til NAV", "OK");
                    }
                    else
                    {
                        gd.SearchDateTime = DateTime.Today.AddDays(-7);
                        gd.SearchDateTimeLast = DateTime.Today.AddDays(7);
                        gd.IsLoggedIn = true;

                        gd.TabbedPage.Children.Add(new HomePage());
                        gd.TabbedPage.Children.Add(new MeetLeavePage());
                        gd.TabbedPage.Children.Add(new MaintenancePage());
                        gd.TabbedPage.Children.Add(new SettingsPage());

                        gd.TabbedPage.Children.Remove(gd.LoginPage);

                        password.Text = null;
                        if (Device.OS != TargetPlatform.iOS)
                        {
                            facade.ThreadManager.StartSynchronizationThread();
                        }
                    }
                    loading = false;
                    button.IsEnabled = true;
                    connectionSettingsButton.IsEnabled = true;

                    layout.Children.Remove(ai);
                }
            };

            Content = new ScrollView { Content = layout };

        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckUserName();
        }

        private void CheckUserName()
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
            CheckPassword();
        }

        private void CheckPassword()
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
            passwordText = false;
            button.IsEnabled = false;
            CheckPassword();
            CheckUserName();
        }
    }
}
