using System.Windows;

namespace Secret_Decoder {
    class ExceptionClass {
        public ExceptionClass() {
            //constructor
        }//end constructor

        public void LoadedP1() {
            //CREATE MESSAGE
            string messageBoxText =
                "You just tried to load a .PPM that wasn't a P3 or P6";

            //GIVE WINDOW TITLE
            string caption = "ERROR WARNING";

            //GIVE WINDOW AN OK BUTTON
            MessageBoxButton button = MessageBoxButton.OK;

            //SETUP WINDOW WITH GIVEN DATA
            System.Windows.MessageBox.Show(messageBoxText, caption, button);
        }//end method

        public void FeedMessage(string messageBoxText, string caption) {
            MessageBoxButton button = MessageBoxButton.OK;

            System.Windows.MessageBox.Show(messageBoxText, caption, button);
        }//end method


    }//end class
}//end namespace
