using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Secret_Decoder {
    /// <summary>
    /// Interaction logic for TextOutputWindow.xaml
    /// </summary>
    public partial class TextOutputWindow : Window {
        public TextOutputWindow() {
            InitializeComponent();            
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e) {
            //CLOSE THIS WINDOW
            this.Close();
        }
    }
}
