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
using System.Windows.Shapes;
using System.Threading;

namespace t2_1stan_writer
{
    /// <summary>
    /// Логика взаимодействия для ArchiveWindow.xaml
    /// </summary>
    public partial class ArchiveWindow : Window
    {
        ArchiveControl AC = new ArchiveControl();


        public ArchiveWindow()
        {
            InitializeComponent();

            AC.AW = this;
            AC.First_TreeData();
        }

        private void trw_Expanded(object sender, RoutedEventArgs e)
        {
            AC.Expander(e);
        }
    }
}
