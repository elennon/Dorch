using Dorch.Common;
using Dorch.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Dorch.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : BindablePage
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static MainPage Current;
        
        ImageSource userImage;
        public ImageSource UserImage
        {
            get
            {
                return (this.userImage);
            }
            private set
            {
                this.userImage = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            Current = this;
            //img = this.imgTest;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            
            //imgTest.Source =

            //imgTest.Source = new BitmapImage(new Uri(@"C:\Data\Users\DefApps\APPDATA\Local\Packages\1d25d3a2-be75-4482-85fe-07742f11370f_9p85fwwdzb3vm\LocalState\Assets\WP_20150202_001.jpg"));//"ms-appx:///Assets/fca.png"));
            //imgTest.Source = new BitmapImage(new Uri("ms-appdata:///local/Assets/WP_20150202_001.jpg"));
            //imgTest.Source = new BitmapImage(new Uri(@"C:\Data\Users\DefApps\APPDATA\Local\Packages\1d25d3a2-be75-4482-85fe-07742f11370f_9p85fwwdzb3vm\LocalState\Assets\wp_ss_20141123_0001.png"));

            //StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            //var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);

            //var uri = new System.Uri("ms-appx:///Assets/bin.png");
            //StorageFolder lbb = await StorageFolder.GetFolderFromPathAsync("ms-appx:///Assets/bin.png");
            //StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            //string hytr = file.Path;
            //BitmapImage bitmapImage = new BitmapImage();
            //var gui = this.BaseUri.ToString();
            //bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/bin.png");
            //imgTest.Source = bitmapImage;

            //StorageFile sf =await dataFolder.GetFileAsync("bin.png");
             

        }//C:\Data\Users\DefApps\APPDATA\Local\Packages\1d25d3a2-be75-4482-85fe-07742f11370f_9p85fwwdzb3vm\LocalState\Assets\wp_ss_20141123_0001.png

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
