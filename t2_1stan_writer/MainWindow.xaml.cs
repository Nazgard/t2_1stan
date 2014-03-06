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

namespace t2_1stan_writer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Parameters parameters = new Parameters();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            comboBox1.ItemsSource = parameters.get_db_worksmens();
            comboBox2.ItemsSource = parameters.get_db_timeintervalsmens();
            comboBox3.ItemsSource = parameters.get_db_surnames();
            comboBox4.ItemsSource = parameters.get_db_surnames();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            comboBox7.ItemsSource  = parameters.get_db_gosts();
            comboBox5.ItemsSource  = parameters.get_db_sizetubes();
            comboBox8.ItemsSource  = parameters.get_db_controlsamples();
            comboBox9.ItemsSource  = parameters.get_db_listdefects();
            comboBox11.ItemsSource = parameters.get_db_device();
        }

        private void textBox4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
