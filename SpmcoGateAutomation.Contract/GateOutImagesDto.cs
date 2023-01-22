using System.ComponentModel;
using System.Drawing;

namespace SpmcoGateAutomation.Contract
{
    public class GateOutImagesDto : IDisposable
    {
        private ImageInfoDto? _leftImage;
        private ImageInfoDto? _rightImage;
        private ImageInfoDto? _backImage;
        private ImageInfoDto? _plateImage;
        public void Dispose()
        {
            _plateImage?.Dispose();
            _leftImage?.Dispose();
            _rightImage?.Dispose();
            _backImage?.Dispose();
        }

        public ImageInfoDto? LeftImage
        {
            get => _leftImage;
            set {_leftImage = value;}
        }
        public ImageInfoDto? RightImage
        {
            get => _rightImage;
            set { _rightImage = value;}
        }
        public ImageInfoDto? BackImage
        {
            get => _backImage;
            set { _backImage = value;}
        }
        public ImageInfoDto? PlateImage
        {
            get => _plateImage;
            set {  _plateImage = value; }
        }
    }
}
