using System.Windows;
using System.Windows.Controls;

namespace t2_1stan_writer
{
    /// <summary>
    /// Логика взаимодействия для BDEditorWindow.xaml
    /// </summary>
    public partial class BDEditorWindow : Window
    {
        private BDEditorControl BDEditorControl;

        public BDEditorWindow()
        {
            InitializeComponent();
            BDEditorControl = new BDEditorControl(this);

            treeView1.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeView1_SelectedItemChanged);
        }

        void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BDEditorControl.SelectedItemControl((TreeViewItem)treeView1.SelectedItem);
        }
    }
}
