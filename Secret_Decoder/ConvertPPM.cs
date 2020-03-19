using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Secret_Decoder {
    class ConvertPPM {
        ExceptionClass ex = new ExceptionClass();
        private Bitmap myMap;
        private Color[,] colArr;
        private BinaryReader reader;

        public string File { get; set; }

        public bool IncorrectLoad { get; set; }

        public ConvertPPM(Image img) {
            myMap = new Bitmap(img);
            colArr = new Color[myMap.Width, myMap.Height];
        }//end constructor

        public ConvertPPM(string file) {
            File = file;
        }//end constructor

        #region BITMAP STUFF
        public Bitmap ConvertToBitmap() {
            reader = new BinaryReader(new FileStream(File, FileMode.Open));
            char[] id = reader.ReadChars(2);

            //CHECK FIRST CHARS TO CHECK PPM TYPE
            if(id[1] == '3') {
                //DO UNCOMPRESSED ASCII STUFF
                return RawAsciiReader();
            } else if (id[1] == '6') {
                //DO UNCOMPRESSED BINARY STUFF
                return RawBinaryReader();
            } else if (id[1] == '7') {
                //DO COMPRESSED ASCII STUFF
                return DecompressRawAsciiReader();
            } else if (id[1] == '8') { 
                //DO COMPRESSED BINARY STUFF
                return DecompressRawBinaryReader();
            } else {
                //SEND EXCEPTION WINDOW AND LOAD YOU MESSED UP PICTURE
                ex.LoadedP1();
                Bitmap bitmap = new Bitmap(@"C:\users\MCA\pictures\messedup.jpg"); 
                IncorrectLoad = true;
                return bitmap;
            }//end if
        }//end method
        #endregion

        #region ASCII STUFF
        private Bitmap RawAsciiReader() {
            Bitmap bitmap;
            string widths = "", heights = "";
            int width, height;
            char temp;

            //SKIP CHAR \n
            temp = reader.ReadChar();

            //SKIP LINE 2
            while((temp = reader.ReadChar()) != '\n') { };

            //GRAB WIDTH AND HEIGHT OF IMAGE
            while((temp = reader.ReadChar()) != ' ') widths += temp;

            while((temp = reader.ReadChar()) >= '0' && temp <= '9') heights += temp;

            //CHECK COLOR SPECS
            if(reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') return null;

            //SKIP CHAR \n
            reader.ReadChar();

            //SAVE BITMAP SIZE
            width = int.Parse(widths);
            height = int.Parse(heights);

            //CREATE BITMAP
            bitmap = new Bitmap(width, height);

            //FILL BITMAP
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    int r = Convert.ToInt32(RGBGrabber()),
                        g = Convert.ToInt32(RGBGrabber()),
                        b = Convert.ToInt32(RGBGrabber());
                    Color colors = Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, colors);
                }//end for
            }//end for
            return bitmap;
        }//end method

        private string RGBGrabber() {
            string colVal = "";
            char temp;

            while((temp = reader.ReadChar()) != '\n') {
                colVal += temp;
            }//end while

            return colVal;
        }//end method

        private Bitmap DecompressRawAsciiReader() {
            //TODO: DECOMPRESS P7 INTO P3 AND THEN READ IT
            Bitmap bitmap;
            string widths = "", heights = "";
            int width, height;
            char temp;

            //SKIP CHAR \n
            temp = reader.ReadChar();

            //SKIP LINE 2
            while((temp = reader.ReadChar()) != '\n') { };

            //GRAB WIDTH AND HEIGHT OF IMAGE
            while((temp = reader.ReadChar()) != ' ') widths += temp;

            while((temp = reader.ReadChar()) >= '0' && temp <= '9') heights += temp;

            //CHECK COLOR SPECS
            if(reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') return null;

            //SKIP CHAR \n
            reader.ReadChar();

            //SAVE BITMAP SIZE
            width = int.Parse(widths);
            height = int.Parse(heights);

            //CREATE BITMAP
            bitmap = new Bitmap(width, height);

            Queue<Color> colors = AddColorsFromAscii();

            //FILL BITMAP
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    //repeating function goes here
                    bitmap.SetPixel(x, y, colors.Dequeue());
                }//end for
            }//end for
            return bitmap;
        }//end method

        private Queue<Color> AddColorsFromAscii() {
            char temp;
            Queue<Color> colList = new Queue<Color>();

            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                string repeatNum = "";
                while((temp = reader.ReadChar()) != '\n') {
                    repeatNum += temp;
                }
                int r = Convert.ToInt32(RGBGrabber()),
                    g = Convert.ToInt32(RGBGrabber()),
                    b = Convert.ToInt32(RGBGrabber()),
                    copyColor = int.Parse(repeatNum);

                for (int i = 0; i < copyColor; i++) {
                    colList.Enqueue(Color.FromArgb(r, g, b));
                }

            }
            return colList;
        }
        private Queue<Color> AddColorsFromBinary() {
            int repeatNum;

            Queue<Color> colList = new Queue<Color>();

            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                repeatNum = Convert.ToInt32(reader.ReadByte());
                
                byte r = reader.ReadByte(),
                     g = reader.ReadByte(),
                     b = reader.ReadByte();
                for (int i = 0; i < repeatNum; i++) {
                    colList.Enqueue(Color.FromArgb(r, g, b));
                }

            }
            return colList;
        }
        #endregion

        #region BINARY STUFF
        private Bitmap RawBinaryReader() {
            Bitmap bitmap;
            string widths = "", heights = "";
            int width, height;
            char temp;

            //SKIP CHAR \n
            reader.ReadChar();

            //SKIP LINE 2
            while((temp = reader.ReadChar()) != '\n') { };

            //GRAB WIDTH AND HEIGHT OF IMAGE
            while((temp = reader.ReadChar()) != ' ') widths += temp;

            while((temp = reader.ReadChar()) >= '0' && temp <= '9') heights += temp;

            //CHECK COLOR SPECS
            if(reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') return null;

            //SKIP CHAR \n
            reader.ReadChar();

            //SAVE BITMAP SIZE
            width = int.Parse(widths);
            height = int.Parse(heights);

            //CREATE BITMAP
            bitmap = new Bitmap(width, height);

            //FILL BITMAP
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {

                    int r = Convert.ToInt32(reader.ReadByte()),
                        g = Convert.ToInt32(reader.ReadByte()),
                        b = Convert.ToInt32(reader.ReadByte());

                    Color colors = Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, colors);

                }//end for
            }//end for

            return bitmap;
        }//end method

        private Bitmap DecompressRawBinaryReader() {
            //TODO: DECOMPRESS P8 INTO P6 AND THEN READ IT
            Bitmap bitmap;
            string widths = "", heights = "";
            int width, height;
            char temp;

            //SKIP CHAR \n
            reader.ReadChar();

            //SKIP LINE 2
            while((temp = reader.ReadChar()) != '\n') { };

            //GRAB WIDTH AND HEIGHT OF IMAGE
            while((temp = reader.ReadChar()) != ' ') widths += temp;

            while((temp = reader.ReadChar()) >= '0' && temp <= '9') heights += temp;

            //CHECK COLOR SPECS
            if(reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') return null;

            //SKIP CHAR \n
            reader.ReadChar();

            //SAVE BITMAP SIZE
            width = int.Parse(widths);
            height = int.Parse(heights);

            //CREATE BITMAP
            bitmap = new Bitmap(width, height);

            Queue<Color> colors = AddColorsFromBinary();

            //FILL BITMAP
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {

                    bitmap.SetPixel(x, y, colors.Dequeue());

                }//end for
            }//end for

            return bitmap;
        }//end method
        #endregion
    }
}
