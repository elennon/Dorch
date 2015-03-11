using Coding4Fun.Toolkit.Controls;
using Dorch.Model;
using Dorch.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dorch.Converters
{
    //public class MessageTypeConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
            
    //        var messa = (Message)value;

    //        // Checks the modulus, whether the index is odd or even
    //        if (messa.SenderId == ((App)Application.Current).UserId)
    //        {
    //            if (parameter == null)
    //                return 1; // no parameter - return opacity
    //            else
    //            {
    //                if (parameter.ToString() == "direction") // return chat "triangle" direction
    //                {
    //                    return ChatBubbleDirection.LowerRight;
    //                }
    //                else  // return aligment
    //                {
    //                    return HorizontalAlignment.Right;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (parameter == null)
    //                return .8;
    //            else
    //            {
    //                if (parameter.ToString() == "direction")
    //                {
    //                    return ChatBubbleDirection.UpperLeft;
    //                }
    //                else
    //                {
    //                    return HorizontalAlignment.Left;
    //                }
    //            }
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
