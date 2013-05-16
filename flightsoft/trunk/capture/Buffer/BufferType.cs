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
        USHORT_IMAGE405=0,  
        USHORT_IMAGE485=1,  
        UTF8_ACCEL=2,
        UTF8_SPATIAL=3,
        UTF8_PHIDGETS=4,
        UTF8_NI6008=5,
        UTF8_UPS=6,
        UTF8_VCOMM=7,
        EMPTY_CYCLE = 8,//cycle the writer once, not writing anything.
	    UTF8_LOG=9  
    }
}
