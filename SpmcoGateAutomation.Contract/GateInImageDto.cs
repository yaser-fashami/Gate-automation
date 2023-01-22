using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Contract
{
    public class GateInImageDto : //INotifyPropertyChanged, 
        IDisposable
    {

        public ImageInfoDto? _plateImage;
        //private bool _isFinished;
        //private bool _isDataGathered;

        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public void Dispose() => _plateImage?.Dispose();

        public ImageInfoDto? PlateImage
        {
            get { return _plateImage; }
            set
            {
                _plateImage = value;
                //if (value != null)
                //{
                //    IsDataGathered = true;
                //}
            }
        }
        //public bool IsDataGathered
        //{
        //    get => _isDataGathered;
        //    set
        //    {
        //        // if (_isDataGathered != value)
        //        {
        //            _isDataGathered = value;
        //            if (value)
        //                OnPropertyChanged(nameof(IsDataGathered));
        //        }
        //    }
        //}
        //public bool IsFinished
        //{
        //    get => _isFinished;
        //    set
        //    {
        //        // if (_isFinished != value)
        //        {
        //            _isFinished = value;
        //            if (value)
        //                OnPropertyChanged(nameof(IsFinished));
        //        }
        //    }
        //}
    }
}
