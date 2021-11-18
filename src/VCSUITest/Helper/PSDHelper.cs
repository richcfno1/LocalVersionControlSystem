using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.PSD;

namespace LocalVersionControlSystem.Helper
{
    public class PSDHelper
    {
        private PsdFile _file;

        public PSDHelper(string path)
        {
            //GB2312 = 936
            _file = new PsdFile(936);
            _file.Load(path);
        }
        public PSDHelper(Stream stream)
        {
            //GB2312 = 936
            _file = new PsdFile(936);
            _file.Load(stream);
        }

        public Bitmap GetImage()
        {
            return ImageDecoder.DecodeImage(_file);
        }

        public List<Bitmap> GetLayers()
        {
            List<Bitmap> result = new List<Bitmap>();
            foreach (Layer l in _file.Layers)
                result.Add(ImageDecoder.DecodeImage(l));
            return result;
        }
    }
}
