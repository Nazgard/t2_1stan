using System.Windows;
using System.Windows.Controls;

namespace t2_1stan_writer
{
    /// <summary>
    ///     Логика взаимодействия для ArchiveWindow.xaml
    /// </summary>
    public partial class ArchiveWindow
    {
        private readonly ArchiveControl _ac = new ArchiveControl();


        public ArchiveWindow()
        {
            InitializeComponent();

            _ac.ArchiveWindow = this;
            _ac.First_TreeData();
        }

        private void trw_Expanded(object sender, RoutedEventArgs e)
        {
            _ac.Expander(e);
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _ac.Tube_Control((TreeViewItem) treeView1.SelectedItem);
            _ac.Info((TreeViewItem) treeView1.SelectedItem);
        }
    }
}