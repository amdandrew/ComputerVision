using System;
using System.Globalization;

namespace VideoFeatureMatching.Converters
{
    public class NotConverter : BaseConverter<bool, bool>
    {
        public override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}