using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dorch.Common
{
    public static class ReadImage               //  this class reads an image in local storage file in a bitmap to pass to the view
    {
        public static ImageSource GetImage(byte[] pic)
        {
            if (pic == null)
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/person-icon.png")); 
            }
            else
            {
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {

                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes((byte[])pic);
                        writer.StoreAsync().GetResults();
                    }
                    var image = new BitmapImage();
                    image.SetSource(ms);
                    return image;
                }
            }
        }

        public static ImageSource GetTeamImage(byte[] pic)
        {
            //List<string> defaultTeamPics = new List<string> { "ms-appx:///Assets/Charlton.png", "ms-appx:///Assets/fca.png", 
            //                "ms-appx:///Assets/Carpi.png", "ms-appx:///Assets/Malaga.png" };
            //List<string> defaultTeamPics = new List<string> { "ms-appx:///Assets/grpp2.png", "ms-appx:///Assets/grup33.png",
                                  //};
            //Random rnd = new Random();
            //int rdN = rnd.Next(0, defaultTeamPics.Count);
            if (pic == null)
            {

                return new BitmapImage(new Uri("ms-appx:///Assets/grpp2.png"));
                //return new BitmapImage(new Uri("ms-appx:///Assets/grp2.png"));
            }
            else
            {
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes((byte[])pic);
                        writer.StoreAsync().GetResults();
                    }
                    var image = new BitmapImage();
                    image.SetSource(ms);
                    return image;
                }
            }
        }

        public static async Task<byte[]> GetImageBytes(object fileObj)
        {
            StorageFile file = (StorageFile)fileObj;            
            var bitmapImage = new BitmapImage();

            using (var stream = await file.OpenReadAsync())
            {
                await bitmapImage.SetSourceAsync(stream);
            }
            byte[] fileBytes = null;
            if (file != null)
            {
                using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                }
            }
            return fileBytes;
        }









   
        public static async Task<ImageSource> MakeImage(string fileName, StorageFolder folder)
        {           
            BitmapImage bitmapImage = null;
            StorageFile file = null;
            bool exists = false;
            try
            {                
                try
                {
                    file = await folder.GetFileAsync(fileName);     // 1st check is for if the file is a default pic and needs to move from app assets folder to isolated storage
                
                    exists = true;
                }
                catch (FileNotFoundException ex)
                {
                    CopyToStorage(fileName);
                }
                catch (Exception ex) 
                {
                    return null;
                }
                file = await folder.GetFileAsync(fileName);
                exists = true;
            }
            catch (Exception ex)  // if the file was picked from the file picker and app uninstalled or file deleted from isolated storage, return a default
            {
                exists = false;                
            }

            if (exists)
            {
                bitmapImage = new BitmapImage();

                using (var stream = await file.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                return bitmapImage;
            }
            else
            {
                CopyToStorage(fileName);
                file = await folder.GetFileAsync(fileName);
                bitmapImage = new BitmapImage();

                using (var stream = await file.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                return bitmapImage;
                //return await GetDefaultPic();
            }
        }

        private async static Task<ImageSource> GetDefaultPic()
        {
            



            StorageFolder appFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile defaultFile = await appFolder.GetFileAsync("Charlton.png");
            var bitmapImage = new BitmapImage();

            using (var stream = await defaultFile.OpenReadAsync())
            {
                await bitmapImage.SetSourceAsync(stream);
            }
            return bitmapImage;
        }



        private static async void CopyToStorage(string item)
        {
            try
            {
                string uri = "ms-appx:///Assets/" + item;
                var urii = new System.Uri(uri);
                StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(urii);
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
                try
                {
                    StorageFile t = await dataFolder.GetFileAsync(item);
                }
                catch (FileNotFoundException ex)
                {
                    CopyFile(file, dataFolder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async void CopyFile(StorageFile stfile, StorageFolder dest)
        {
            await stfile.CopyAsync(dest);
        }
    }
}
