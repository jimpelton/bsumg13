using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uGCapture;

namespace gui
{
    public partial class ImageDisplayControl : UserControl
    {

        private static byte[] pixelWorkBuffer = new byte[2592 * 1944 * 3];


        public ImageDisplayControl()
        {
            InitializeComponent();
        }

        public void UpdateContents(DataSet<byte> dat)
        {
            updateImages(dat);
        } 

        private void updateImages(DataSet<byte> dat)
        {
            byte[] i405 = dat.lastData[BufferType.USHORT_IMAGE405];
            byte[] i485 = dat.lastData[BufferType.USHORT_IMAGE485];

            //test goodness
            /*
            BinaryReader b = new BinaryReader(File.Open("data_485_1000.raw",FileMode.Open));
            i405 = b.ReadBytes(2 * 2592 * 1944);
            b.Close();
            b.Dispose();
            */

            //ImageDisplay iee = Guimain.guiImageDisplay;
            if (i405 != null)
            {
                pictureBox1.Image = ConvertCapturedRawImage(i405);
                pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            if (i485 != null)
            {
                pictureBox2.Image = ConvertCapturedRawImage(i485);
                pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
        }

        static unsafe public Bitmap ConvertCapturedRawImage(byte[] indata)
        {
            Bitmap bitmap = null;

            unsafe
            {
                fixed (byte* ptr = indata)
                {

                    int a = 1;
                    int b = 0;
                    while (a < indata.Length)
                    {
                        pixelWorkBuffer[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        pixelWorkBuffer[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        pixelWorkBuffer[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        a += 2;
                    }

                    fixed (byte* ptr2 = pixelWorkBuffer)
                    {
                        IntPtr scan0 = new IntPtr(ptr2);
                        bitmap = new Bitmap(2592, 1944, // Image size
                                            2592 * 3, // Scan size
                                            PixelFormat.Format24bppRgb, scan0);
                    }
                }
            }
            return bitmap;
        }
    }
}
