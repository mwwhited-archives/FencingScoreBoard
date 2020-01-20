using FencingScoreBoard.WpCommonf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace FencingScoreBoard.Wpf
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        private readonly SerialPortProvider _serial; // = new SerialPortProvider();

        public SerialPortViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> SerialPorts { get; set; }
    }
}
