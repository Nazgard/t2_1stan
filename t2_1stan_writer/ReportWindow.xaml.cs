using System;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    /// <summary>
    ///     Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private readonly Connection _connection = new Connection();
        private readonly MySqlCommand _mySqlCommand = new MySqlCommand();
        private MySqlDataReader _mySqlDataReader;
        private string guid = Guid.NewGuid().ToString() + ".report";


        public ReportWindow(TreeViewItem item)
        {
            InitializeComponent();

            get_info(item);
            xps();
        }

        private void xps()
        {
            var package = Package.Open(guid, FileMode.Create);
            var doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write(grid1);
            doc.Close();
            package.Close();

            var xpsDocument = new XpsDocument(guid, FileAccess.Read);
            DocumentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
        }

        public void get_info(TreeViewItem item)
        {
            _connection.Open();
            _mySqlCommand.CommandText = @"SELECT
                DATE_FORMAT(defectsdata.DatePr,'%Y-%m-%d'),
                worksmens.NameSmen,
                sizetubes.SizeTube,
                gosts.NameGost,
                controlsamples.NameControlSample,
                o1.Surname,
                o1.LevelMD,
                o2.Surname,
                o2.LevelMD,
                device.NameDevice,
                defectsdata.Porog,
                defectsdata.Current,
                Count(defectsdata.IndexData),
                (SELECT
                    Count(defectsdata.IndexData)
                    FROM
                    defectsdata
                    Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                    Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                    WHERE
                    defectsdata.FlDefectTube =  1 AND
                    defectsdata.NumberTube <>  0 AND
                    defectsdata.DatePr = '2013-12-25' AND
                    indexes.Id_WorkSmen = 3
                )
                FROM
                defectsdata
                Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                Inner Join sizetubes ON sizetubes.Id_SizeTube = indexes.Id_SizeTube
                Inner Join gosts ON gosts.Id_Gost = indexes.Id_Gost
                Inner Join controlsamples ON controlsamples.Id_ControlSample = indexes.Id_ControlSample
                Inner Join operators AS o1 ON o1.Id_Operator = indexes.Id_Operator1
                Inner Join operators AS o2 ON o2.Id_Operator = indexes.Id_Operator2
                Inner Join device ON device.Id_Device = indexes.Id_Device
                WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                indexes.Id_WorkSmen = @B AND
                defectsdata.NumberTube <>  0
                GROUP BY defectsdata.DatePr
            ";
            _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
            _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
            _mySqlCommand.Connection = _connection.MySqlConnection;
            _mySqlDataReader = _mySqlCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                Title = "Отчет за " + _mySqlDataReader.GetString(0) + " по смене " + _mySqlDataReader.GetString(1);
                label5.Content = _mySqlDataReader.GetString(0);
                label6.Content = _mySqlDataReader.GetString(1);
                label7.Content = _mySqlDataReader.GetString(2);
                label8.Content = _mySqlDataReader.GetString(3);
                label9.Content = _mySqlDataReader.GetString(4);
                label10.Content = _mySqlDataReader.GetString(5);
                label11.Content = _mySqlDataReader.GetString(6);
                label12.Content = _mySqlDataReader.GetString(7);
                label13.Content = _mySqlDataReader.GetString(8);
                label14.Content = "";
                label15.Content = "";
                label16.Content = _mySqlDataReader.GetString(9);
                label17.Content = _mySqlDataReader.GetString(10);
                label18.Content = _mySqlDataReader.GetString(11);
                label19.Content = _mySqlDataReader.GetString(12);
                label20.Content = _mySqlDataReader.GetString(13);
            }
            _mySqlDataReader.Close();
            _connection.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = Title;
            dlg.DefaultExt = ".xps";
            dlg.ShowDialog();

            try
            {
                Package package = Package.Open(dlg.FileName, FileMode.Create);
                var doc = new XpsDocument(package);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

                writer.Write(grid1);
                doc.Close();
                package.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DocumentViewer.Document = null;
            try
            {
                string[] txtFiles = Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString()), "*.report");
                foreach (var txtFile in txtFiles)
                {
                    System.IO.File.Delete(txtFile);
                }
            }
            catch
            {

            }
        }
    }
}