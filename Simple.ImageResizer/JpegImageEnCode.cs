using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ImageResizer
{
    public sealed class JpegImageEncode: ImageEnCode
    {
        public int QualityLevel { get; set; }
    }
}
