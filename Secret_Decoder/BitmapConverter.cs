using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Secret_Decoder {
    class BitmapConverter {
        private Bitmap bitmap;

        public BitmapConverter(Bitmap map) {
            bitmap = map;
        }//end constructor

        public ImageSource ConvertBitmapToImageSource() {
            //CREATE NEW MEMORYSTREAM AND READ DATA INTO BITMAP
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                //SET STREAM POSITION
                memory.Position = 0;

                //CREATE NEW BITMAPIMAGE
                BitmapImage BI = new BitmapImage();
                //BEGIN CONVERSION
                BI.BeginInit();
                BI.StreamSource = memory;
                //CACHE ENTIRE IMAGE AT LOAD TIME
                BI.CacheOption = BitmapCacheOption.OnLoad;
                //STOP
                BI.EndInit();

                return BI;
            }
        }
    }
}
