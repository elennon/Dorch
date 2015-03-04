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
    public class Player : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string PlayerName { get; set; }
        public string PhNumber { get; set; }       
        public bool IsPicked { get; set; }
        public string InOutImage {  get; set; }

        private byte[] _imgByte;
        public byte[] Image
        {
            get { return _imgByte; }
            set { _imgByte = value; NotifyPropertyChanged("Image"); }
        }
                
        //[Ignore]
        public virtual ICollection<Team> Teams { get; set; }

        public ImageSource PlayerImage        
        {
            get
            {
                return ReadImage.GetImage(this.Image);
            }
            protected set { NotifyPropertyChanged("PlayerImage"); }
        }

        
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


    //[Table("Players")]
    //public class Player : INotifyPropertyChanged
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int PlayerId { get; set; }
    //    public string Name { get; set; }
    //    public string PhNumber { get; set; }
    //    public string Image { get; set; }

    //    public bool IsRegular { get; set; }
    //    public bool IsPicked { get; set; }

    //    [ForeignKey(typeof(Team))]
    //    public int TeamId { get; set; }
        
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
