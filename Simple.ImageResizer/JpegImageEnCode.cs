using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Simple.ImageResizer
{
    public class JpegImageEncode : ImageEncode
    {
        public int QualityLevel { get; set; }
        public int GetQualityLevel
        {
            get { return QualityLevel; }
        }

        internal override BitmapEncoder GetBitmapEncoder()
        {
            return  new JpegBitmapEncoder
            {
                QualityLevel = GetQualityLevel
            };
        }
    }

}
