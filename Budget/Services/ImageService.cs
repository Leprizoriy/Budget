using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Budget.Services
{
    public class ImageService
    {
        public ImageSource InitImage(string heroImagePath)
        {
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.CacheOption = BitmapCacheOption.OnLoad;
            bmi.UriSource = new Uri(heroImagePath, UriKind.Absolute);
            bmi.EndInit();

            return bmi;
        }
        
    }
}
