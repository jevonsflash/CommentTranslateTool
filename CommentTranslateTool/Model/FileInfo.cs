using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace Workshop.Model
{
    public class FileInfo : ViewModelBase
    {
        public string Name { get; set; }
        public string Path { get; set; }



        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged();

            }
        }


        private int _progress;

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

    }
}
