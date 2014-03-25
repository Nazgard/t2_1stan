using System.Windows;
using System.Windows.Controls;

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

            AC.ArchiveWindow = this;
            AC.First_TreeData();
        }

        private void trw_Expanded(object sender, RoutedEventArgs e)
        {
            AC.Expander(e);
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            AC.Tube_Control((TreeViewItem)treeView1.SelectedItem);
            AC.info((TreeViewItem)treeView1.SelectedItem);
        }
    }
}
