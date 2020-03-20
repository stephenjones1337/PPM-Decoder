using System;
using System.Windows;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Secret_Decoder {
    class colValue {
        public int R;
        public int G;
        public int B;
        public bool full;
        public colValue() {
            full = false;
        }
        public void Clear() {
            R = -1;
            G = -1;
            B = -1;
            full = false;
        }
        public Color MakeColor() {
            return Color.FromArgb(R,G,B);
        }
    }
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
            try {

                //CHECK FIRST CHARS TO CHECK PPM TYPE
                if(id[1] == '3') {
                    //DO UNCOMPRESSED ASCII STUFF
                    return RawAsciiReader();
                } else if (id[1] == '6') {
                    //DO UNCOMPRESSED BINARY STUFF
                    return RawBinaryReader();
                } else if (id[1] == '7') {
                    //DO COMPRESSED ASCII STUFF
                    return DecompressRleAsciiReader();
                } else if (id[1] == '8') { 
                    //DO COMPRESSED BINARY STUFF
                    return DecompressRleBinaryReader();
                }  else if (id[1] == '9') {
                    //DO COMPRESSED ASCII STUFF
                    return DecompressLzwAsciiReader();
                } else if (id[1] == '1') { 
                    //DO COMPRESSED BINARY STUFF
                    return DecompressLzwBinaryReader();
                } else {
                    //SEND EXCEPTION WINDOW AND LOAD YOU MESSED UP PICTURE
                    ex.LoadedP1();
                    return null;
                }//end if
            } catch {
                MessageBox.Show("Error decompressing - Corrupt data","Error");
                return null;
            }
        }//end method
        #endregion

        #region ASCII READERS
        private Bitmap RawAsciiReader() {
            //CREATE BITMAP
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {
                    int r = Convert.ToInt32(RGBGrabber()),
                        g = Convert.ToInt32(RGBGrabber()),
                        b = Convert.ToInt32(RGBGrabber());
                    Color colors = Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, colors);
                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }//end method
        private Bitmap DecompressRleAsciiReader() {
            //TODO: DECOMPRESS P7 INTO P3 AND THEN READ IT
            //CREATE BITMAP
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            Queue<Color> colors = AddColorsRleAscii();

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {
                    //repeating function goes here
                    bitmap.SetPixel(x, y, colors.Dequeue());
                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }//end method
        private Bitmap DecompressLzwAsciiReader() {
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            Queue<Color> colors = AddColorsLzwAscii();

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {
                    //repeating function goes here
                    bitmap.SetPixel(x, y, colors.Dequeue());
                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }
        #endregion

        #region BINARY READERS
        private Bitmap RawBinaryReader() {
            //CREATE BITMAP
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {

                    int r = Convert.ToInt32(reader.ReadByte()),
                        g = Convert.ToInt32(reader.ReadByte()),
                        b = Convert.ToInt32(reader.ReadByte());

                    Color colors = Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, colors);

                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }//end method
        private Bitmap DecompressRleBinaryReader() {
            //CREATE BITMAP
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            Queue<Color> colors = AddColorsRleBinary();

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {

                    bitmap.SetPixel(x, y, colors.Dequeue());

                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }//end method
        private Bitmap DecompressLzwBinaryReader() {
            //CREATE BITMAP
            Bitmap bitmap = ReadHeader();

            if (bitmap == null) return null;

            Queue<Color> colors = AddColorsLzwBinary();

            //FILL BITMAP
            for(int y = 0; y < bitmap.Height; y++) {
                for(int x = 0; x < bitmap.Width; x++) {

                    bitmap.SetPixel(x, y, colors.Dequeue());

                }//end for
            }//end for
            reader.Close();
            return bitmap;
        }
        #endregion
        #region TOOLS
        private Bitmap ReadHeader() {
            string widths = "", heights = "";
            int width, height;
            char temp;

            //SKIP CHAR \n
            while(reader.ReadChar() != '\n') { };

            //SKIP LINE 2
            while(reader.ReadChar() != '\n') { };

            //GRAB WIDTH AND HEIGHT OF IMAGE
            while((temp = reader.ReadChar()) != ' ') 
                widths += temp;

            while((temp = reader.ReadChar()) >= '0' && temp <= '9') 
                heights += temp;

            //CHECK COLOR SPECS
            if(reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') 
                return null;

            //SKIP CHAR \n
            reader.ReadChar();

            //SAVE BITMAP SIZE
            width = int.Parse(widths);
            height = int.Parse(heights);

            //CREATE BITMAP
            return new Bitmap(width, height);
        }
        private string RGBGrabber() {
            string colVal = "";
            char temp;

            while((temp = reader.ReadChar()) != '\n') {
                colVal += temp;
            }//end while

            return colVal;
        }//end method
        private Queue<Color> AddColorsRleAscii() {
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
        private Queue<Color> AddColorsRleBinary() {
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
        private Queue<Color> AddColorsLzwBinary() {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            List<int> compressed = new List<int>();

            PopulateDictionary(dictionary);

            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                compressed.Add(reader.ReadByte());                
            }

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach(int k in compressed) {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];
                decompressed.Append(entry);

                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            char[] splitter = {'\n'};
            string toSplit = decompressed.ToString();
            string[] split = toSplit.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            return FillColorQueue(split);
        }
        private Queue<Color> AddColorsLzwAscii() {
            string numStr = "";
            char temp;
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            List<int> compressed = new List<int>();
            PopulateDictionary(dictionary);

            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                if ((temp = reader.ReadChar()) != ',') {
                    numStr += temp;
                } else {
                    compressed.Add(int.Parse(numStr));
                    numStr = "";
                }
            }

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach(int k in compressed) {                
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }
            char[] splitter = {'\n'};
            string toSplit = decompressed.ToString();
            string[] split = toSplit.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            return FillColorQueue(split);
        }

        private void PopulateDictionary(Dictionary<int, string> dictionary) {
            for (int i = 0; i < 128; i++) {
                dictionary.Add(i,((char)i).ToString());
            }
        }
        private void AddToColValue(colValue colValue, int color, string value) {
            if (color == 1) {
                colValue.R = int.Parse(value);
            }else if (color == 2) {
                colValue.G = int.Parse(value);
            }else if (color == 3) {
                colValue.B = int.Parse(value);
                colValue.full = true;
            }
        }
        private Queue<Color> FillColorQueue(string[] colors) {
            Queue<Color> colorQ = new Queue<Color>();
            for (int i = 0; i < colors.Length; i+=3) {
                colorQ.Enqueue(Color.FromArgb(int.Parse(colors[i]), int.Parse(colors[i+1]), int.Parse(colors[i+2])));
            }
            return colorQ;
        }
        #endregion
    }
}
