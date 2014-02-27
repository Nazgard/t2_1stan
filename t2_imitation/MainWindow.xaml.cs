using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace t2_imitation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool position_defectoscope = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            position_defectoscope = false;

            Canvas.SetTop(rectangle3, 12);
            Canvas.SetTop(rectangle5, 62);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            position_defectoscope = true;

            Canvas.SetTop(rectangle3, 32);
            Canvas.SetTop(rectangle5, 82);
        }
    }
}
