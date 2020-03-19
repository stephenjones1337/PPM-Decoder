using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace Secret_Decoder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //THIS THE DECODER
        //CALL THE CLASSES      
        OpenFileDialog openFileDialog = new OpenFileDialog();
        FileClass fileClass = new FileClass();
        DecodeClass decodeClass = new DecodeClass();
        StringBuilder builtString = new StringBuilder();

        public MainWindow() {
            InitializeComponent();
        }

        //EVENTS
        private void Open_Click(object sender, RoutedEventArgs e) {           
            //CHECK IF LOADED OR NOT
            if(fileClass.LoadFile(openFileDialog, ImgPicture, decodeClass)) {
                //ENABLE DECODE BUTTON IF FILE SUCCESSFULLY LOADED
                TxtCoords.IsEnabled = true;
                TxtCoords.Clear();
                TxtCoords.Focus();
            }//end if

        }//end event

        private void BtnDecode_Click(object sender, RoutedEventArgs e) {
            //START DECODE PROCESS
            decodeClass.Decoder(fileClass.LoadedBitmap, builtString, TxtCoords);
            

            //CALL TextOutputWindow AND CREATE A NEW WINDOW
            TextOutputWindow text = new TextOutputWindow();
            //SHOW WINDOW
            text.Show();
            //OUTPUT DECODED MESSAGE TO NEW WINDOW
            text.TxtOutputBox.Text = builtString.ToString();
        }//end event

        //METHODS
        private void DoEvents() {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }//end event

        private void Help_Click(object sender, RoutedEventArgs e) {
            //SETUP MESSAGE TEXT
            string messageBoxText = 
                "• Open the 'File' drop down menu and click 'Open'\n\n" +
                "• Search for the appropriate .PPM file (P3 or P6) you wish to open\n\n" +
                "• Click the 'Decode Button'\n" +
                "(You may need to wait some depending on how large the file is)\n\n" +
                "• Once the operation is complete your message will appear in the designated 'Output Box'";

            //GIVE WINDOW TITLE
            string caption = "Instructions";

            //GIVE WINDOW AN OK BUTTON
            MessageBoxButton button = MessageBoxButton.OK;

            //CREATE BUTTON WITH GIVEN VARIABLES
            System.Windows.MessageBox.Show(messageBoxText, caption, button);
        }//end event
    }//end class
}//end namespace
