using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    internal class ArchiveControl
    {
        private readonly Connection _connection = new Connection();
        private readonly MySqlCommand _mySqlCommand = new MySqlCommand();
        public ArchiveWindow ArchiveWindow;
        private int _countDeffectsLine;
        private MySqlDataReader _mySqlDataReader;

        public void First_TreeData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    var item = new TreeViewItem
                    {
                        Style = null,
                        Tag = "year",
                        Header = _mySqlDataReader.GetString(0)
                    };
                    item.Items.Add("*");
                    ArchiveWindow.treeView1.Items.Add(item);
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }
            catch
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                ArchiveWindow.Close();
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void Expander(RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                _connection.Open();
                var item = (TreeViewItem) e.OriginalSource;
                item.Items.Clear();

                if (item.Tag.ToString() == "year")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT
                        DISTINCT
                        MONTHNAME(defectsdata.DatePr),
                        YEAR(defectsdata.DatePr)
                        FROM
                        defectsdata
                        WHERE YEAR(defectsdata.DatePr) = @A
                    ";
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Header.ToString());
                    _mySqlCommand.Connection = _connection.MySqlConnection;

                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemMonth = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(1),
                            Style = null,
                            Tag = "month",
                            Header = _mySqlDataReader.GetString(0)
                        };
                        itemMonth.Items.Add("*");
                        item.Items.Add(itemMonth);
                    }
                    _mySqlDataReader.Close();
                }

                if (item.Tag.ToString() == "month")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT
                        DISTINCT
                        DAY(defectsdata.DatePr),                        
                        YEAR(defectsdata.DatePr),
                        MONTHNAME(defectsdata.DatePr)
                        FROM
                        defectsdata
                        WHERE   MONTHNAME(defectsdata.DatePr) = @A AND
                                YEAR (defectsdata.DatePr) = @B
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Header.ToString());
                    _mySqlCommand.Parameters.AddWithValue("B", item.Uid);

                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemDays = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(1) + "-" + _mySqlDataReader.GetString(2),
                            Style = null,
                            Tag = "day",
                            Header = _mySqlDataReader.GetString(0)
                        };
                        itemDays.Items.Add("*");
                        item.Items.Add(itemDays);
                    }
                    _mySqlDataReader.Close();
                }

                if (item.Tag.ToString() == "day")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT DISTINCT
                        worksmens.NameSmen,
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d'),
                        worksmens.Id_WorkSmen
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                        ORDER BY worksmens.NameSmen
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid + "-" + string.Format("{0:00}", Convert.ToInt32(item.Header)));
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemSmens = new TreeViewItem
                        {
                            Style = null,
                            Uid = _mySqlDataReader.GetString(1) + "|" + _mySqlDataReader.GetString(2),
                            Tag = "smena",
                            Header = _mySqlDataReader.GetString(0)
                        };
                        itemSmens.Items.Add("*");
                        item.Items.Add(itemSmens);
                    }
                    _mySqlDataReader.Close();
                }

                if (item.Tag.ToString() == "smena")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT DISTINCT
                        defectsdata.NumberPart,
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d'),
                        worksmens.Id_WorkSmen
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                              worksmens.Id_WorkSmen = @B  
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
                    _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemPart = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(1) + "|" + _mySqlDataReader.GetString(2),
                            Style = null,
                            Tag = "part",
                            Header = _mySqlDataReader.GetString(0)
                        };
                        itemPart.Items.Add("*");
                        item.Items.Add(itemPart);
                    }
                    _mySqlDataReader.Close();
                }

                if (item.Tag.ToString() == "part")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT
                        defectsdata.NumberTube,
                        defectsdata.FlDefectTube,
                        defectsdata.TimePr,
                        defectsdata.IndexData
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                              worksmens.Id_WorkSmen = @B AND
                              defectsdata.NumberPart = @C
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
                    _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
                    _mySqlCommand.Parameters.AddWithValue("C", item.Header);
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();


                    while (_mySqlDataReader.Read())
                    {
                        var itemTube = new TreeViewItem
                        {
                            Uid     = _mySqlDataReader.GetString(3),
                            Tag     = "tube",
                            Header  = "Труба № " + _mySqlDataReader.GetString(0)
                        };
                        if (_mySqlDataReader.GetInt32(1) == 1)
                        {
                            var redBrush = new SolidColorBrush
                            {
                                Color = Colors.Red
                            };
                            itemTube.Foreground = redBrush;
                        }
                        item.Items.Add(itemTube);
                    }
                    _mySqlDataReader.Close();
                }
                _connection.Close();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void Tube_Control(TreeViewItem item)
        {
            try
            {
                if (item.Tag.ToString() == "tube")
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    _mySqlCommand.CommandText = @"
                        SELECT
                        defectsdata.IndexData,
                        defectsdata.NumberPart,
                        defectsdata.NumberTube,
                        defectsdata.NumberSegments,
                        defectsdata.DataSensors,
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'),
                        defectsdata.TimePr,
                        worksmens.NameSmen,
                        o1.Surname,
                        o2.Surname
                        FROM
                        defectsdata
                        INNER JOIN indexes ON indexes.IndexData = defectsdata.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        INNER JOIN operators o1 ON o1.Id_Operator = indexes.Id_Operator1
                        INNER JOIN operators o2 ON o2.Id_Operator = indexes.Id_Operator2
                        WHERE 
                        defectsdata.IndexData = @A
                        LIMIT 1
                    ";
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid);

                    _connection.Open();
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        ArchiveWindow.label1.Content = "Номер плавки\t" + _mySqlDataReader.GetString(1);
                        ArchiveWindow.label2.Content = "Номер трубы\t" + _mySqlDataReader.GetString(2);
                        ArchiveWindow.label3.Content = "Дата проведения Н.К.\t" + _mySqlDataReader.GetString(5);
                        ArchiveWindow.label4.Content = "Время проведения Н.К.\t" + _mySqlDataReader.GetString(6);
                        ArchiveWindow.label5.Content = "Длина трубы (метры)\t\t " +
                                                       Math.Round((_mySqlDataReader.GetDouble(3)/6), 2);
                        ArchiveWindow.label7.Content = "Рабочая смена\t" + _mySqlDataReader.GetString(7);
                        ArchiveWindow.label8.Content = "Специалист ОКПП\t" + _mySqlDataReader.GetString(9);
                        ArchiveWindow.label9.Content = "Специалист АСК ТЭСЦ-2\t" + _mySqlDataReader.GetString(8);


                        for (int i = 0; i < _countDeffectsLine; i++)
                        {
                            var line = (Line) ArchiveWindow.canvas1.FindName("errorLine" + i);
                            ArchiveWindow.canvas1.Children.Remove(line);
                            try
                            {
                                ArchiveWindow.canvas1.UnregisterName("errorLine" + i);
                            }
                                // ReSharper disable EmptyGeneralCatchClause
                            catch
                                // ReSharper restore EmptyGeneralCatchClause
                            {
                            }
                        }
                        _countDeffectsLine = 0;

                        /*_mySqlDataReaderValue4 = _mySqlDataReader.GetValue(4);
                        _da.Completed += _da_Completed;
                        _da.From = ArchiveWindow.rectangle1.Width;
                        _da.To = _mySqlDataReader.GetDouble(3)*4;
                        _da.Duration = TimeSpan.FromMilliseconds(500);
                        ArchiveWindow.rectangle1.BeginAnimation(FrameworkElement.WidthProperty, _da);*/
                        ArchiveWindow.rectangle1.Width = _mySqlDataReader.GetDouble(3)*4;

                        int j = 0;

                        foreach (byte deffect in (byte[])_mySqlDataReader.GetValue(4))
                        {
                            if (deffect != 0)
                            {
                                var redBrush = new SolidColorBrush
                                {
                                    Color = Colors.Red
                                };

                                var errorLine = new Line();

                                Canvas.SetLeft(errorLine, 40 + (j * 4));
                                errorLine.Opacity = 1;
                                errorLine.X1 = 0;
                                errorLine.X2 = 0;
                                errorLine.Y1 = 151;
                                errorLine.Y2 = 151 + 70;
                                errorLine.StrokeThickness = 4;
                                errorLine.Stroke = redBrush;
                                errorLine.Fill = redBrush;
                                ArchiveWindow.canvas1.RegisterName("errorLine" + _countDeffectsLine, errorLine);
                                ArchiveWindow.canvas1.Children.Add(errorLine);
                                /*_da1.From = 0;
                                _da1.To = 1;
                                _da1.Duration = TimeSpan.FromMilliseconds(2000);
                                errorLine.BeginAnimation(UIElement.OpacityProperty, _da1);*/
                                _countDeffectsLine++;
                            }
                            j++;
                        }

                        ArchiveWindow.label6.Content = "Кол-во дефектных сегментов\t " + _countDeffectsLine;
                    }
                    _connection.Close();
                    _mySqlDataReader.Close();
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public void Info(TreeViewItem item)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (item.Tag.ToString() == "year")
            {
                _connection.Open();
                ArchiveWindow.listBox1.Items.Clear();
                ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Header);
                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("A", item.Header.ToString());
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();

                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();


                _mySqlCommand.CommandText = @"
                        SELECT 
                        concat(round(( (SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND 
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A)  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }

            if (item.Tag.ToString() == "month")
            {
                _connection.Open();
                ArchiveWindow.listBox1.Items.Clear();
                _mySqlCommand.CommandText = @"
                        SELECT    
                        Count(defectsdata.IndexData),
                        YEAR(defectsdata.DatePr),
                        MONTHNAME(defectsdata.DatePr)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B
                        GROUP BY MONTHNAME(defectsdata.DatePr)
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("A", item.Uid);
                _mySqlCommand.Parameters.AddWithValue("B", item.Header.ToString());
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + _mySqlDataReader.GetString(1) + "-" + _mySqlDataReader.GetString(2));
                    ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();

                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();


                _mySqlCommand.CommandText = @"
                        SELECT 
                        concat(round(( (SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B )  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }

            if (item.Tag.ToString() == "day")
            {
                _connection.Open();
                ArchiveWindow.listBox1.Items.Clear();
                
                _mySqlCommand.CommandText = @"
                    SELECT
                    Count(defectsdata.IndexData)
                    FROM
                    defectsdata
                    WHERE
                    defectsdata.NumberTube <>  0 AND
                    DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                    LIMIT 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("A", item.Uid + "-" + string.Format("{0:00}", Convert.ToInt32(item.Header)));
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid + "-" + item.Header);

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();

                _mySqlCommand.CommandText = @"
                    SELECT
                    Count(defectsdata.IndexData)
                    FROM
                    defectsdata
                    WHERE
                    defectsdata.FlDefectTube =  1 AND
                    defectsdata.NumberTube <>  0 AND
                    DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();


                _mySqlCommand.CommandText = @"
                    SELECT 
                    concat(round(( (SELECT
                    Count(defectsdata.IndexData)
                    FROM
                    defectsdata
                    WHERE
                    defectsdata.NumberTube <>  0 AND
                    defectsdata.FlDefectTube =  1 AND
                    DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A)  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                    FROM
                    defectsdata
                    WHERE
                    defectsdata.NumberTube <>  0 AND
                    DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                    LIMIT 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }

            if (item.Tag.ToString() == "smena")
            {
                _connection.Open();
                ArchiveWindow.listBox1.Items.Clear();
                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData),
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d'),
                        worksmens.NameSmen
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B
                        GROUP BY (defectsdata.DatePr)
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
                _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + _mySqlDataReader.GetString(1) + " / " + _mySqlDataReader.GetString(2));
                    ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();

                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();


                _mySqlCommand.CommandText = @"
                        SELECT 
                        concat(round(( (SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        defectsdata.FlDefectTube =  1 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B)  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }

            if (item.Tag.ToString() == "part")
            {
                _connection.Open();
                ArchiveWindow.listBox1.Items.Clear();
                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData),
                        worksmens.NameSmen    
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B AND
                        defectsdata.NumberPart = @C
                        GROUP BY defectsdata.DatePr
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[0]);
                _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[1]);
                _mySqlCommand.Parameters.AddWithValue("C", item.Header);
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid.Split('|')[0] + " / " + _mySqlDataReader.GetString(1) + " / Плавка № " + item.Header);
                    ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();

                _mySqlCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B AND
                        defectsdata.NumberPart = @C
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _mySqlDataReader.GetString(0));
                }
                _mySqlDataReader.Close();


                _mySqlCommand.CommandText = @"
                        SELECT 
                        concat(round(( (SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        defectsdata.FlDefectTube =  1 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B AND
                        defectsdata.NumberPart = @C)  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                        FROM
                        defectsdata
                        Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                        Inner Join worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                        worksmens.Id_WorkSmen = @B AND
                        defectsdata.NumberPart = @C
                        LIMIT 1
                    ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    try
                    {
                        ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + _mySqlDataReader.GetString(0));
                    }
                    catch (Exception)
                    {
                        ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t");
                    }
                }
                _mySqlDataReader.Close();
                _connection.Close();
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}