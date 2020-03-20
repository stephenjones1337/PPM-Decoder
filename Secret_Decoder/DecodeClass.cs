using System;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace Secret_Decoder {
    class DecodeClass {
        //VARS
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        private int increment;
        private int xPos;
        private int yPos;
        private int exceptionCount = 0;
        private char stopChar = (char)4;
        private Bitmap bmp;

        //CONSTRUCTOR
        public DecodeClass() {
            //construct
        }//end constructor

        //METHODS
        public StringBuilder Decoder(Bitmap bitmap, StringBuilder stringBuilder, TextBox txtBox) {
            //NOTE: DESIGNATED STOP GLYPH - GLYPH: ¸ | CODE: U+00B8 | DECIMAL: 0184 | HTML: &cedil; | NUMBER: 0120
            //SET FOR GLOBAL
            bmp = bitmap;
            if(SetupDecoder(bmp, txtBox)) {

                //IF SETSTART RETURNS 0 , 0 SEND ERROR TO OUTPUT BOX
                if(xPos < 0 && yPos <  0) {
                    stringBuilder.Clear();
                    stringBuilder.Append("ERROR: THE IMAGE YOU DECODED DOESN'T HAVE A MESSAGE OR YOU DIDN'T SET THE X AND Y POSITIONS CORRECTLY");
                    return stringBuilder;
                }//end if

                //IF STRINGBUILDER HAS WORDS CLEAR
                if(stringBuilder.Length > 0) {
                    stringBuilder.Clear();
                }//end if

                //GRAB FIRST LETTER
                stringBuilder.Append(GrabLetter());

                for(int index = 0; index <= 255; index++) {
                    //GRAB NEXT POINT
                    NextPoint();

                    if(stringBuilder[stringBuilder.Length - 1] == stopChar) {
                        //IF CHAR IS STOP CHAR END STRING
                        return stringBuilder;
                    } else {
                        //APPEND(STORE) GRABBED LETTERS
                        stringBuilder.Append(GrabLetter());
                    } //end if                
                }//end for            

                return stringBuilder;
            } else {
                stringBuilder.Clear();
                stringBuilder.Append("ERROR: THE IMAGE YOU DECODED DOESN'T HAVE A MESSAGE OR YOU DIDN'T SET THE X AND Y POSITIONS CORRECTLY");
                return stringBuilder;
            }
        }//end method

        private bool SetupDecoder(Bitmap bmp, TextBox txtbox) {
            try {

                increment = (int)Math.Round(((double)bmp.Width/5) + ((double)bmp.Height/5)/5);
                if(increment < 1) {
                    increment = 1;
                }//end if

                return SetStart(txtbox);
            } catch {
                MessageBox.Show("Invalid hash", "Error");
                return false;
            }
        }//end method

        private string GrabLetter() {
            //GRAB BLUE
            Color col = bmp.GetPixel(xPos, yPos);

            //RETURN CHAR VERSION OF THE INT            
            return $"{(char)col.B}";
        }//end method

        private void NextPoint() {
            //X POS PLUS ((W/5) + (H/5)/5)
            xPos += increment;

            if(xPos >= bmp.Width) {
                xPos = (xPos - bmp.Width);
                //IF Y POSITION >= BITMAP HEIGHT MINUS ONE SET Y POSITION TO ZERO, ELSE ADD ONE TO THE POSITION
                yPos = yPos >= bmp.Height - 1 ? yPos = 0 : yPos += 1;
            }//end if
        }//end method

        //TODO: CREATE NEW GETSTART METHOD
        private bool SetStart(TextBox txtbox) {
            try {
                string[] xANDy = Unhash(txtbox.Text).Split(',');                
                xPos = int.Parse(xANDy[0]);
                yPos = int.Parse(xANDy[1]);
                if (xPos < 0 || yPos < 0)
                    return false;
                else
                    return true;
            } catch(Exception) {
                ExceptionClass ex = new ExceptionClass();
                ex.FeedMessage("The Coordinates Entered Is Not Correct", "Error");
                exceptionCount++;

                return false;
            }//end try/catch
        }//end method

        //THIS WILL DECRYPT THE HASHED MESSAGE SENT FROM THE ENCODER PROGRAM
        public static string Unhash(string encryptedText) {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }//end method
    }//end class
}//end namespace
