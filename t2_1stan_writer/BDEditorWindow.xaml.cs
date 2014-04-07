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
            button1.Click += new RoutedEventHandler(button1_Click);
            BDEditorControl = new BDEditorControl(this);

            treeView1.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeView1_SelectedItemChanged);
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Owner = this;
            window.Height = 200;
            window.Width = 200;

            Grid grid = new Grid();
            Label lbl1 = new Label();
            lbl1.Content = "name";
            grid.Children.Add(lbl1);

            window.ShowDialog();
        }

        void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BDEditorControl.SelectedItemControl((TreeViewItem)treeView1.SelectedItem);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = (TreeViewItem)treeView1.SelectedItem;
            BDEditorControl.delete_entry(selecteditem.Header.ToString());
        }
    }
}
