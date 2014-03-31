using System;
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
            label1.Content = "";
            label2.Content = "";
            label3.Content = "";
            label4.Content = "";
            label5.Content = "";
            label6.Content = "";
            label7.Content = "";
            label8.Content = "";
            label9.Content = "";
        }

        private void trw_Expanded(object sender, RoutedEventArgs e)
        {
            _ac.Expander(e);
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                _ac.Tube_Control((TreeViewItem)treeView1.SelectedItem);
                _ac.Info((TreeViewItem)treeView1.SelectedItem);
            }
            catch
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ReportWindow = new ReportWindow();
            ReportWindow.Show();
        }
    }
}