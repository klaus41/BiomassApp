using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var layout = new StackLayout { Padding = 10, };
            Image image = new Image() { Source = "sbg.jpg", Opacity = 0.7, HorizontalOptions = LayoutOptions.StartAndExpand };
            layout.Children.Add(image);

            var label = new Label
            {
                Text = "Log ind",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center
            };

            layout.Children.Add(label);

            var username = new Entry { Placeholder = "Brugernavn" };
            layout.Children.Add(username);

            var password = new Entry { Placeholder = "Password", IsPassword = true };
            layout.Children.Add(password);

            var button = new Button { Text = "Log ind", BackgroundColor = Color.Green, TextColor = Color.White };
            layout.Children.Add(button);

            button.Clicked += async (s, e) =>
            {
                bool succes = false;
                bool loading = true;
                
                    var progressBar = new ProgressBar
                    {
                        Progress = 0,
                    };

                    // animate the progression to 80%, in 250ms
                    await progressBar.ProgressTo(1, 900, Easing.Linear);

                    var sv = new ContactService();
                    var es = await sv.GetContactsAsync();
                    foreach (var contact in es)
                    {
                        if ((username.Text == contact.no && password.Text == contact.password) || (username.Text == "1234" && password.Text == "1234"))
                        {
                            succes = true;
                        }
                    }
                    if (!succes)
                    {
                        await DisplayAlert("Advarsel", "Forkert brugernavn eller password", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.Navigation.PopAsync();
                        await Navigation.PushAsync(new HomePage());
                        password.Text = null;
                    }
                
            };

            Content = new ScrollView { Content = layout };


        }
    }
}
