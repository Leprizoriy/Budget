using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace Budget
{
    public class BindableBase : INotifyPropertyChanged
    {
        private ImageSource logoImage;
        private ImageSource closeImage;

        public event PropertyChangedEventHandler PropertyChanged;
        public ImageSource LogoImage { get => logoImage; set => SetProperty(ref logoImage, value); }
        public ImageSource CloseImage { get => closeImage; set => SetProperty(ref closeImage, value); }

        protected void SetProperty<T>(ref T item, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(item, value))
            {
                item = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
