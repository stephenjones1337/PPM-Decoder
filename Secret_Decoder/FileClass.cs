using System.Drawing;
using System.Windows.Forms;

namespace Secret_Decoder {
    class FileClass {
        //PROPERTIES
        public string PreImagePath { get; private set; }

        public Bitmap LoadedBitmap { get; private set; }

        public bool IsLoaded {get; private set; }

        //constructors
        public FileClass() { }//end constructor

        //METHODS
        public bool LoadFile(OpenFileDialog openFileDialog, System.Windows.Controls.Image ImgPicture, DecodeClass decode) { 
            //SETUP OFD INFORMATION WHEN CALLED
            openFileDialog.Filter = "PPM file (*.ppm)|*.ppm";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Image Selector";            

            //IF YOU LOAD A CORRECT IMAGE AND HIT OK/OPEN
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                //CHECK IF IMAGE WAS LOADED BEFORE
                if(PreImagePath == openFileDialog.FileName) {
                    return true;
                }//end if

                //SAVE OPENED FILENAME TO CHECK ON SECOND LOAD
                PreImagePath = openFileDialog.FileName;

                //GRAB FILENAME AND SEND TO ConvertPPM CLASS
                ConvertPPM ppm = new ConvertPPM(openFileDialog.FileName);    

                //CONVERT PPM TO BITMAP
                LoadedBitmap = ppm.ConvertToBitmap();                

                //SAVE PICTURE TO IMAGE BOX
                if (LoadedBitmap != null) {

                    BitmapConverter bmpcon = new BitmapConverter(LoadedBitmap);
                    ImgPicture.Source = bmpcon.ConvertBitmapToImageSource();
                }

                //IF YOU TRIED TO LOAD P1
                if(ppm.IncorrectLoad) {
                    return false;
                }//end if               
                
                //DISPOSE OF PICTURE
                openFileDialog.Dispose();

                return true;
            }//end if
            
            return false;
        }//end method
    }//end class
}//end namespace
