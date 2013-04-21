using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    /// <summary>
    /// Describes the type of data that is contained in a Buffer object's 
    /// data array.
    /// 
    /// The USHORT_IMAGE* types both represent binary image data, everything else represents
    /// textual data encoded in UTF8.
    /// </summary>
    public enum BufferType
    {
        ERROR,
        USHORT_IMAGE405,
        USHORT_IMAGE485,
        UTF8_ACCEL,
	UTF8_SPATIAL,
        UTF8_PHIDGETS,
        UTF8_NI6008,
        UPS,
        UTF8_VCOMM
    }
}
