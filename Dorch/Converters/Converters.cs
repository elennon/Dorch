using Coding4Fun.Toolkit.Controls;
using Dorch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Dorch.Converters
{
    public class MessageTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var messa = (Message)value;

            // Checks the modulus, whether the index is odd or even
            if (messa.SenderId == ((App)Application.Current).UserId)
            {
                if (parameter == null)
                    return 1; // no parameter - return opacity
                else
                {
                    if (parameter.ToString() == "direction") // return chat "triangle" direction
                    {
                        return ChatBubbleDirection.LowerRight;
                    }
                    else  // return aligment
                    {
                        return "65,0,0,0"; //HorizontalAlignment.Right;
                    }
                }
            }
            else
            {
                if (parameter == null)
                    return .8;
                else
                {
                    if (parameter.ToString() == "direction")
                    {
                        return ChatBubbleDirection.UpperLeft;
                    }
                    else
                    {
                        return "15,0,0,0"; //HorizontalAlignment.Left;
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemClickedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as ItemClickEventArgs;

            if (args != null)
                return args.ClickedItem;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)      // CultureInfo language
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
