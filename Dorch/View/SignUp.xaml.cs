using Dorch.Common;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.Security.Credentials;
using Dorch.Model;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Dorch.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUp : Page
    {
        private NavigationHelper navigationHelper;
        public static SignUp signUpPage;
        public TextBox txtNme, txtNum;
        public Popup popup;
        private MobileServiceUser user;

        //private async System.Threading.Tasks.Task AuthenticateAsync()
        //{
        //    while (user == null)
        //    {
        //        string message;
        //        try
        //        {
        //            // Change 'MobileService' to the name of your MobileServiceClient instance.
        //            // Sign-in using Facebook authentication.
        //            user = await App.MobileService
        //                .LoginAsync(MobileServiceAuthenticationProvider.Google);
        //            message =
        //                string.Format("You are now signed in - {0}", user.UserId);
        //        }
        //        catch (InvalidOperationException)
        //        {
        //            message = "You must log in. Login Required";
        //        }

        //        var dialog = new MessageDialog(message);
        //        dialog.Commands.Add(new UICommand("OK"));
        //        await dialog.ShowAsync();
        //    }
        //}


        // Define a method that performs the authentication process
        // using a Facebook sign-in. 
        //private async System.Threading.Tasks.Task AuthenticateAsync()
        //{
        //    string message;
        //    // This sample uses the Facebook provider.
        //    var provider = "Google";

        //    // Use the PasswordVault to securely store and access credentials.
        //    PasswordVault vault = new PasswordVault();
        //    PasswordCredential credential = null;

        //    while (credential == null)
        //    {
        //        try
        //        {
        //            // Try to get an existing credential from the vault.
        //            credential = vault.FindAllByResource(provider).FirstOrDefault();
        //        }
        //        catch (Exception)
        //        {
        //            // When there is no matching resource an error occurs, which we ignore.
        //        }

        //        if (credential != null)
        //        {
        //            // Create a user from the stored credentials.
        //            user = new MobileServiceUser(credential.UserName);
        //            credential.RetrievePassword();
        //            user.MobileServiceAuthenticationToken = credential.Password;

        //            // Set the user from the stored credentials.
        //            App.MobileService.CurrentUser = user;

        //            try
        //            {
        //                // Try to return an item now to determine if the cached credential has expired.
        //                await App.MobileService.GetTable<RequestPending>().Take(1).ToListAsync();
        //            }
        //            catch (MobileServiceInvalidOperationException ex)
        //            {
        //                if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //                {
        //                    // Remove the credential with the expired token.
        //                    vault.Remove(credential);
        //                    credential = null;
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                // Login with the identity provider.
        //                user = await App.MobileService.LoginAsync(provider);


        //                // Create and store the user credentials.
        //                credential = new PasswordCredential(provider,
        //                    user.UserId, user.MobileServiceAuthenticationToken);
        //                vault.Add(credential);
        //            }
        //            catch (MobileServiceInvalidOperationException ex)
        //            {
        //                message = "You must log in. Login Required";
        //            }
        //        }
        //        message = string.Format("You are now logged in - {0}", user.UserId);
        //        var dialog = new MessageDialog(message);
        //        dialog.Commands.Add(new UICommand("OK"));
        //        await dialog.ShowAsync();
        //    }
        //}

        public SignUp()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            signUpPage = this;
            txtNum = txtNumber; txtNme = txtUserName;
            popup = ppPopup;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            // Login the user and then load data from the mobile service.
            //await AuthenticateAsync();

            //// Hide the login button and load items from the mobile service.
            //this.ButtonLogin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ////RefreshTodoItems();
        }
        //#region NavigationHelper registration

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    this.navigationHelper.OnNavigatedTo(e);
        //}

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    this.navigationHelper.OnNavigatedFrom(e);
        //}

        //#endregion
    }
}
