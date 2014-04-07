using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly MySqlCommand _mySqlCommand1 = new MySqlCommand();
        public ArchiveWindow ArchiveWindow;
        private int _countDeffectsLine;
        private MySqlDataReader _mySqlDataReader;
        private MySqlDataReader _mySqlDataReader1;
        private Dictionary<string, string> _countDays;
        private Dictionary<string, string> _countDefectsDays;
        private Dictionary<string, string> _countDefectsMonths;
        private Dictionary<string, string> _countDefectsParts;
        private Dictionary<string, string> _countDefectsSmens;
        private Dictionary<string, string> _countDefectsYears;
        private Dictionary<string, string> _countMonths;
        private Dictionary<string, string> _countParts;
        private Dictionary<string, string> _countSmens;
        private Dictionary<string, string> _countYears;
        private bool _countsLoaded = true;

        public ArchiveControl()
        {
            _connection.Open();
        }

        public void First_TreeData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                _mySqlCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlDataReader = _mySqlCommand.ExecuteReader();

                while (_mySqlDataReader.Read())
                {
                    var item = new TreeViewItem
                    {
                        Uid = _mySqlDataReader.GetString(0),
                        Style = null,
                        Tag = "year",
                        Header = _mySqlDataReader.GetString(0)
                    };
                    item.Items.Add("*");
                    ArchiveWindow.treeView1.Items.Add(item);
                }
                _mySqlDataReader.Close();
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
                var item = (TreeViewItem) e.OriginalSource;
                item.Items.Clear();

                if (item.Tag.ToString() == "year")
                {
                    _mySqlCommand.CommandText = @"
                        SELECT
                        DISTINCT
                        MONTHNAME(defectsdata.DatePr),
                        YEAR(defectsdata.DatePr),
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m')
                        FROM
                        defectsdata
                        WHERE YEAR(defectsdata.DatePr) = @A
                    ";
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid);
                    _mySqlCommand.Connection = _connection.MySqlConnection;

                    

                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemMonth = new TreeViewItem
                        {
                            Uid =
                                _mySqlDataReader.GetString(1) + "-" + _mySqlDataReader.GetString(0) + "|" +
                                _mySqlDataReader.GetString(2),
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
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d')
                        FROM
                        defectsdata
                        WHERE
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m') = @A
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[1]);

                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemDays = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(1),
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
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'),
                        indexes.Id_WorkSmen
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE defectsdata.DatePr = @A
                        ORDER BY worksmens.NameSmen
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid);
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();

                    while (_mySqlDataReader.Read())
                    {
                        var itemSmens = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(2) + "|" + _mySqlDataReader.GetString(3) + "+" + _mySqlDataReader.GetString(2) + " / " + _mySqlDataReader.GetString(0),
                            Style = null,
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
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'),
                        indexes.Id_WorkSmen,
                        worksmens.NameSmen
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE defectsdata.DatePr = @A AND
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
                            Uid =
                                _mySqlDataReader.GetString(0) + "|" + _mySqlDataReader.GetString(1) + "|" +
                                _mySqlDataReader.GetString(2) + "|" + _mySqlDataReader.GetString(1) + " / " +
                                _mySqlDataReader.GetString(3) + " / плавка " + _mySqlDataReader.GetString(0),
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
                        WHERE defectsdata.DatePr = @A AND
                              worksmens.Id_WorkSmen = @B AND
                              defectsdata.NumberPart = @C
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("A", item.Uid.Split('|')[1]);
                    _mySqlCommand.Parameters.AddWithValue("B", item.Uid.Split('|')[2]);
                    _mySqlCommand.Parameters.AddWithValue("C", item.Uid.Split('|')[0]);
                    _mySqlDataReader = _mySqlCommand.ExecuteReader();


                    while (_mySqlDataReader.Read())
                    {
                        var itemTube = new TreeViewItem
                        {
                            Uid = _mySqlDataReader.GetString(3),
                            Tag = "tube",
                            Header = "Труба № " + _mySqlDataReader.GetString(0)
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

                        foreach (byte deffect in (byte[]) _mySqlDataReader.GetValue(4))
                        {
                            if (deffect != 0)
                            {
                                var redBrush = new SolidColorBrush
                                {
                                    Color = Colors.Red
                                };

                                var errorLine = new Line();

                                Canvas.SetLeft(errorLine, 40 + (j*4));
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

        public void info_router(TreeViewItem item)
        {
            try
            {
                switch (item.Tag.ToString())
                {
                    case "tube":
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                        Tube_Control(item);
                    }
                    break;
                    case "year":
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                        if (_countsLoaded)
                        {
                            ArchiveWindow.listBox1.Items.Clear();
                            ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid);
                            ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _countYears[item.Uid]);
                            ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _countDefectsYears[item.Uid]);
                            double cd = Convert.ToInt32(_countDefectsYears[item.Uid]);
                            double c = Convert.ToInt32(_countYears[item.Uid]);
                            double result = Math.Round(((cd/c)*100), 2);
                            ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + result);
                        }
                        else
                        {
                            MessageBox.Show("Статистика еще загружается, попробуйте позже.");
                        }
                    }
                    break;

                    case "month":
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                        if (_countsLoaded)
                        {
                            ArchiveWindow.listBox1.Items.Clear();
                            ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid.Split('|')[1]);
                            ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _countMonths[item.Uid.Split('|')[0]]);
                            ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" +
                                                             _countDefectsMonths[item.Uid.Split('|')[0]]);
                            double cd = Convert.ToInt32(_countDefectsMonths[item.Uid.Split('|')[0]]);
                            double c = Convert.ToInt32(_countMonths[item.Uid.Split('|')[0]]);
                            double result = Math.Round(((cd/c)*100), 2);
                            ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + result);
                        }
                        else
                        {
                            MessageBox.Show("Статистика еще загружается, попробуйте позже.");
                        }
                    }
                    break;

                    case "day":
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                        if (_countsLoaded)
                        {
                            ArchiveWindow.listBox1.Items.Clear();
                            ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid);
                            ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _countDays[item.Uid]);
                            ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _countDefectsDays[item.Uid]);
                            double cd = Convert.ToInt32(_countDefectsDays[item.Uid]);
                            double c = Convert.ToInt32(_countDays[item.Uid]);
                            double result = Math.Round(((cd/c)*100), 2);
                            ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + result);
                        }
                        else
                        {
                            MessageBox.Show("Статистика еще загружается, попробуйте позже.");
                        }
                    }
                    break;

                    case "smena":
                    {
                        ArchiveWindow.button1.IsEnabled = true;
                        if (_countsLoaded)
                        {
                            ArchiveWindow.listBox1.Items.Clear();
                            ArchiveWindow.listBox1.Items.Add("ВРЕМЯ: \t\t\t" + item.Uid.Split('+')[1]);
                            ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _countSmens[item.Uid.Split('+')[0]]);
                            ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" + _countDefectsSmens[item.Uid.Split('+')[0]]);
                            double cd = Convert.ToInt32(_countDefectsSmens[item.Uid.Split('+')[0]]);
                            double c = Convert.ToInt32(_countSmens[item.Uid.Split('+')[0]]);
                            double result = Math.Round(((cd/c)*100), 2);
                            ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + result);
                        }
                        else
                        {
                            MessageBox.Show("Статистика еще загружается, попробуйте позже.");
                        }
                    }
                    break;

                    case "part":
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                        if (_countsLoaded)
                        {
                            ArchiveWindow.listBox1.Items.Clear();
                            ArchiveWindow.listBox1.Items.Add("ПЛАВКА: \t\t" + item.Uid.Split('|')[0]);
                            ArchiveWindow.listBox1.Items.Add("ТРУБ: \t\t\t" + _countParts[item.Uid.Split('|')[0]]);
                            ArchiveWindow.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: \t" +
                                                             _countDefectsParts[item.Uid.Split('|')[0]]);
                            double cd = Convert.ToInt32(_countDefectsParts[item.Uid.Split('|')[0]]);
                            double c = Convert.ToInt32(_countParts[item.Uid.Split('|')[0]]);
                            double result = Math.Round(((cd/c)*100), 2);
                            ArchiveWindow.listBox1.Items.Add("ПРОЦЕНТ БРАКА: \t" + result);
                        }
                        else
                        {
                            MessageBox.Show("Статистика еще загружается, попробуйте позже.");
                        }
                    }
                    break;
                    default:
                    {
                        ArchiveWindow.button1.IsEnabled = false;
                    }
                    break;
                }
            }
            catch
            {
            }
        }

        public void bgworkercounter()
        {
            var backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _countsLoaded = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            _countYears = new Dictionary<string, string>();
            _countYears.Clear();
            _countYears = cyears();

            _countDefectsYears = new Dictionary<string, string>();
            _countDefectsYears.Clear();
            _countDefectsYears = cdyears();

            _countMonths = new Dictionary<string, string>();
            _countMonths.Clear();
            _countMonths = cmonths();

            _countDefectsMonths = new Dictionary<string, string>();
            _countDefectsMonths.Clear();
            _countDefectsMonths = cdmonths();

            _countDays = new Dictionary<string, string>();
            _countDays.Clear();
            _countDays = cdays();

            _countDefectsDays = new Dictionary<string, string>();
            _countDefectsDays.Clear();
            _countDefectsDays = cddays();

            _countSmens = new Dictionary<string, string>();
            _countSmens.Clear();
            _countSmens = csmens();

            _countDefectsSmens = new Dictionary<string, string>();
            _countDefectsSmens.Clear();
            _countDefectsSmens = cdsmens();

            _countParts = new Dictionary<string, string>();
            _countParts.Clear();
            _countParts = cparts();

            _countDefectsParts = new Dictionary<string, string>();
            _countDefectsParts.Clear();
            _countDefectsParts = cdparts();
        }

        public void count()
        {
            //bgworkercounter();
            _countYears = new Dictionary<string, string>();
            _countYears.Clear();
            _countYears = cyears();

            _countDefectsYears = new Dictionary<string, string>();
            _countDefectsYears.Clear();
            _countDefectsYears = cdyears();

            _countMonths = new Dictionary<string, string>();
            _countMonths.Clear();
            _countMonths = cmonths();

            _countDefectsMonths = new Dictionary<string, string>();
            _countDefectsMonths.Clear();
            _countDefectsMonths = cdmonths();

            _countDays = new Dictionary<string, string>();
            _countDays.Clear();
            _countDays = cdays();

            _countDefectsDays = new Dictionary<string, string>();
            _countDefectsDays.Clear();
            _countDefectsDays = cddays();

            _countSmens = new Dictionary<string, string>();
            _countSmens.Clear();
            _countSmens = csmens();

            _countDefectsSmens = new Dictionary<string, string>();
            _countDefectsSmens.Clear();
            _countDefectsSmens = cdsmens();

            _countParts = new Dictionary<string, string>();
            _countParts.Clear();
            _countParts = cparts();

            _countDefectsParts = new Dictionary<string, string>();
            _countDefectsParts.Clear();
            _countDefectsParts = cdparts();
        }


        public Dictionary<string, string> cyears()
        {
            var DictYears = new Dictionary<string, string>();
            DictYears.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                YEAR(defectsdata.DatePr)
                FROM
                defectsdata
                GROUP BY YEAR(defectsdata.DatePr)
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();
            

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictYears.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }

            _mySqlDataReader1.Close();
            

            return DictYears;
        }

        public Dictionary<string, string> cdyears()
        {
            var DictDYears = new Dictionary<string, string>();
            DictDYears.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                YEAR(defectsdata.DatePr)
                FROM
                defectsdata
                WHERE
                defectsdata.FlDefectTube = 1
                GROUP BY YEAR(defectsdata.DatePr)
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDYears.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();
            

            return DictDYears;
        }

        public Dictionary<string, string> cmonths()
        {
            var DictMonths = new Dictionary<string, string>();
            DictMonths.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT    
                Count(defectsdata.IndexData),
			    DATE_FORMAT(defectsdata.DatePr, '%Y-%M')
                FROM
                defectsdata
                GROUP BY MONTHNAME(defectsdata.DatePr)
                ORDER BY YEAR(defectsdata.DatePr), MONTH(defectsdata.DatePr)
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictMonths.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictMonths;
        }

        public Dictionary<string, string> cdmonths()
        {
            var DictDMonths = new Dictionary<string, string>();
            DictDMonths.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT    
                Count(defectsdata.IndexData),
			    DATE_FORMAT(defectsdata.DatePr, '%Y-%M')
                FROM
                defectsdata
                WHERE
                defectsdata.FlDefectTube = 1
                GROUP BY MONTHNAME(defectsdata.DatePr)
                ORDER BY YEAR(defectsdata.DatePr), MONTH(defectsdata.DatePr)
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDMonths.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictDMonths;
        }

        public Dictionary<string, string> cdays()
        {
            var DictDays = new Dictionary<string, string>();
            DictDays.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT    
                Count(defectsdata.IndexData),
                DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d')
                FROM
                defectsdata
                GROUP BY defectsdata.DatePr
                ORDER BY defectsdata.DatePr
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDays.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictDays;
        }

        public Dictionary<string, string> cddays()
        {
            var DictDays = new Dictionary<string, string>();
            DictDays.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT    
                Count(defectsdata.IndexData),
                DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d')
                FROM
                defectsdata
                WHERE
                defectsdata.FlDefectTube = 1
                GROUP BY defectsdata.DatePr
                ORDER BY defectsdata.DatePr
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDays.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictDays;
        }
        
        public Dictionary<string, string> csmens()
        {
            var DictSmens = new Dictionary<string, string>();
            DictSmens.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                CONCAT(DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'), '|', indexes.Id_WorkSmen)
                FROM
                defectsdata
                Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                GROUP BY defectsdata.DatePr, indexes.Id_WorkSmen
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictSmens.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictSmens;
        }

        public Dictionary<string, string> cdsmens()
        {
            var DictDSmens = new Dictionary<string, string>();
            DictDSmens.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                CONCAT(DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'), '|', indexes.Id_WorkSmen)
                FROM
                defectsdata
                Inner Join indexes ON defectsdata.IndexData = indexes.IndexData
                WHERE
                defectsdata.FlDefectTube = 1
                GROUP BY defectsdata.DatePr, indexes.Id_WorkSmen
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDSmens.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }

            _mySqlDataReader1.Close();

            return DictDSmens;
        }

        public Dictionary<string, string> cparts()
        {
            var DictParts = new Dictionary<string, string>();
            DictParts.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                defectsdata.NumberPart
                FROM
                defectsdata
                GROUP BY defectsdata.NumberPart
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictParts.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }
            _mySqlDataReader1.Close();

            return DictParts;
        }

        public Dictionary<string, string> cdparts()
        {
            var DictDParts = new Dictionary<string, string>();
            DictDParts.Clear();

            _mySqlCommand1.CommandText = @"
                SELECT
                Count(defectsdata.IndexData),
                defectsdata.NumberPart
                FROM
                defectsdata
                WHERE
                defectsdata.FlDefectTube = 1
                GROUP BY defectsdata.NumberPart
            ";
            _mySqlCommand1.Connection = _connection.MySqlConnection;
            _mySqlDataReader1 = _mySqlCommand1.ExecuteReader();

            while (_mySqlDataReader1.Read())
            {
                try
                {
                    DictDParts.Add(_mySqlDataReader1.GetString(1), _mySqlDataReader1.GetString(0));
                }
                catch
                {
                }
            }
            _mySqlDataReader1.Close();

            return DictDParts;
        }
    }
}