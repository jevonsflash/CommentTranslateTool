using System;
using System.Globalization;
using System.Windows.Data;

namespace Workshop.Converter
{
    public class Status2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color = string.Empty;
            if (value == null)
            {
                return color;
            }
            var currentStatus = (string)value;

            switch (currentStatus)
            {
                case "执行完成":
                    color = "Green";
                    break;
                case "执行失败":
                    color = "Red";
                    break;
                case "正在执行":
                    color = "Blue";
                    break;
                case "等待执行":
                    color = "Gold";
                    break;
                default:
                    color = "Blue";
                    break;
            }

            return color;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
