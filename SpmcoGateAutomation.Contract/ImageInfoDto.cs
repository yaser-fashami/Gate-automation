using System.Drawing;

namespace SpmcoGateAutomation.Contract
{
    public class ImageInfoDto : IDisposable
    {
        public ImageInfoDto(Image image, string path,string imageName)
        {
            Image = image;
            Path = path;
            ImageName = imageName;
        }
        public string Path { get; set; }
        public string ImageName { get; set; }
        public Image Image { get; }

        public void Dispose() => Image?.Dispose();
    }
}
