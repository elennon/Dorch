using Dorch.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Dorch.Model
{
    public class Team : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string Location { get; set; }
        public byte[] TeamImage { get; set; }
        public string PlayerTargetNumber { get; set; }
        public ICollection<Player> Players { get; set; }


        public ImageSource ImgSource        // returns TeamImage byte[] as image source or default if null
        {
            get
            {
                return ReadImage.GetTeamImage(this.TeamImage);
            }
            protected set { }
        }




        //ImageSource _ImgSource;
        //[Ignore]
        //public ImageSource ImgSource
        //{
        //    get
        //    {
        //        return (this.userImage);
        //    }
        //    set
        //    {
        //        SetProperty(ref this.userImage, value);
        //    }
        //}

        //protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        //{
        //    if (object.Equals(storage, value)) return false;

        //    storage = value;
        //    this.NotifyPropertyChanged(propertyName);
        //    return true;
        //}
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }



    //[Table("Teams")]
    //public class Team : INotifyPropertyChanged
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int TeamId { get; set; }
    //    public string Name { get; set; }
    //    public string Location { get; set; }
    //    public string Image { get; set; }

    //    //[Ignore]
    //    //public ImageSource ImageSource { get; set; }

    //    ImageSource userImage;
    //    [Ignore]
    //    public ImageSource ImageSource
    //    {
    //        get
    //        {
    //            return (this.userImage);
    //        }
    //        set
    //        {
    //            SetProperty(ref this.userImage, value);
    //        }
    //    }

    //    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)        
    //    {
    //        if (object.Equals(storage, value)) return false;

    //        storage = value;
    //        this.NotifyPropertyChanged(propertyName);
    //        return true;
    //    }
    //    #region INotifyPropertyChanged Members

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private void NotifyPropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //    #endregion
    //}
}
