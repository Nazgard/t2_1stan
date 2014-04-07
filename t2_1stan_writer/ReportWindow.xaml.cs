using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using MigraDoc.RtfRendering;

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
        private TreeViewItem TVItem;


        public ReportWindow(TreeViewItem item)
        {
            InitializeComponent();

            Document document = CreateSample1(item);
            DocumentPreview1.Ddl = DdlWriter.WriteToString(document);

            TVItem = item;
        }

        public Document CreateSample1(TreeViewItem item)
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
                defectsdata.FlDefectTube = 1 AND
                defectsdata.NumberTube <> 0 AND
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
                WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d') = @A AND
                indexes.Id_WorkSmen = @B AND
                defectsdata.NumberTube <> 0
                GROUP BY defectsdata.DatePr
                LIMIT 1
            ";
            _mySqlCommand.Parameters.Clear();
            _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
            _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
            _mySqlCommand.Connection = _connection.MySqlConnection;
            _mySqlDataReader = _mySqlCommand.ExecuteReader();

            // Create a new MigraDoc document
            Document document = new Document();
            // Add a section to the document
            Section section = document.AddSection();
            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            while (_mySqlDataReader.Read())
            {
                Title = "Отчет за " + _mySqlDataReader.GetString(0) + " по смене " + _mySqlDataReader.GetString(1);
                paragraph.AddText("ОАО \"Северский трубный завод\"\r\nТЭСЦ-2 стан 73-219");
                paragraph.Format.Font.Size = "14";
                paragraph.Format.Alignment = ParagraphAlignment.Center;

                paragraph = section.AddParagraph();
                paragraph = section.AddParagraph();
                paragraph.AddText("Сводная информация по смене");
                paragraph.Format.Font.Size = "12";
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                document.LastSection.AddParagraph();
                Table table = new Table();
                table.Format.Font.Size = "11";
                table.Borders.Width = 0.5;

                Column column = table.AddColumn(Unit.FromCentimeter(8));
                //column.Format.Alignment = ParagraphAlignment.Center;

                table.AddColumn(Unit.FromCentimeter(10));

                var BottomPadding = 8;

                Row row = row = table.AddRow();
                row.BottomPadding = BottomPadding;
                Cell cell = row.Cells[0];
                cell.AddParagraph("Дата");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(0));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Смена");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(1));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Типоразмер труб");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(2));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Нормативные документы");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(3));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Контрольный образец");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(4));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Специалист ОКПП");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(5));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Уровень по МПР (FT)");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(6));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Специалист АСК ТЭСЦ-2");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(7));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Уровень по МПР (FT)");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(8));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Параметры настройки");
                cell = row.Cells[1];
                cell.AddParagraph();

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Автоматический контроль шва");
                cell = row.Cells[1];
                cell.AddParagraph();

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Установка");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(9));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Порог, мВ");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(10));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Ток, А");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(11));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Проконтролировано труб");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(12));

                row = table.AddRow();
                row.BottomPadding = BottomPadding;
                cell = row.Cells[0];
                cell.AddParagraph("Дефектных труб");
                cell = row.Cells[1];
                cell.AddParagraph(_mySqlDataReader.GetString(13));

                //table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

                document.LastSection.Add(table);
            }
            _mySqlDataReader.Close();
            _connection.Close();           
            
            return document;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            const bool unicode = true;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = CreateSample1(TVItem);

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...            
            var dlg = new SaveFileDialog();
            dlg.FileName = "ТЭСЦ-2 стан 73-219" + Title;
            dlg.DefaultExt = ".pdf";
            dlg.ShowDialog();
            pdfRenderer.PdfDocument.Save(dlg.FileName);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = "ТЭСЦ-2 стан 73-219" + Title;
            dlg.DefaultExt = ".rtf";
            dlg.ShowDialog();

            RtfDocumentRenderer rtf = new RtfDocumentRenderer();
            rtf.Render(CreateSample1(TVItem), dlg.FileName, null);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}