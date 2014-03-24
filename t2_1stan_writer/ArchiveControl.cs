using System;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace t2_1stan_writer
{
    class ArchiveControl
    {
        Connection connection = new Connection();
        public ArchiveWindow AW;
        private string current_year;
        private string current_month;
        private string current_day;
        private string current_smena;
        private string current_part;
        private int count_deffects_line = 0;
        MySqlCommand myCommand = new MySqlCommand();
        MySqlDataReader MyDataReader;

        public void First_TreeData()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            myCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
            myCommand.Connection = connection.myConnection;
            
            try
            {
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = "year";
                    item.Header = MyDataReader.GetString(0);
                    item.Items.Add("*");
                    AW.treeView1.Items.Add(item);
                }
                MyDataReader.Close();
                connection.close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                AW.Close();
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        public void Expander(RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            try
            {
                connection.open();
                TreeViewItem item = (TreeViewItem)e.OriginalSource;
                item.Items.Clear();

                if (item.Tag.ToString() == "year")
                {
                    myCommand.CommandText = @"
                        SELECT
                        DISTINCT
                        MONTHNAME(defectsdata.DatePr)
                        FROM
                        defectsdata
                        WHERE YEAR(defectsdata.DatePr) = @A
                    ";
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", item.Header.ToString());
                    myCommand.Connection = connection.myConnection;

                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        TreeViewItem itemMonth = new TreeViewItem();
                        itemMonth.Tag = "month";
                        itemMonth.Header = MyDataReader.GetString(0);
                        itemMonth.Items.Add("*");
                        item.Items.Add(itemMonth);
                    }
                    MyDataReader.Close();

                    current_year = item.Header.ToString();
                }

                if (item.Tag.ToString() == "month")
                {
                    AW.listBox1.Items.Clear();
                    AW.listBox1.Items.Add("ВРЕМЯ: " + current_year + "-" + item.Header.ToString());
                    myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year);
                    myCommand.Parameters.AddWithValue("B", item.Header.ToString());
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ТРУБ: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();

                    myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B
                    ";
                    myCommand.Connection = connection.myConnection;
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();


                    myCommand.CommandText = @"
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
                    ";
                    myCommand.Connection = connection.myConnection;
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ПРОЦЕНТ БРАКА: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();


                    //=============================

                    myCommand.CommandText = @"
                        SELECT
                        DISTINCT
                        DAY(defectsdata.DatePr)
                        FROM
                        defectsdata
                        WHERE   MONTHNAME(defectsdata.DatePr) = @A AND
                                YEAR (defectsdata.DatePr) = @B
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", item.Header.ToString());
                    myCommand.Parameters.AddWithValue("B", current_year);

                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        TreeViewItem itemDays = new TreeViewItem();
                        itemDays.Tag = "day";
                        itemDays.Header = MyDataReader.GetString(0);
                        itemDays.Items.Add("*");
                        item.Items.Add(itemDays);
                    }
                    MyDataReader.Close();

                    current_month = item.Header.ToString();
                }

                if (item.Tag.ToString() == "day")
                {
                    AW.listBox1.Items.Clear();
                    AW.listBox1.Items.Add("ВРЕМЯ: " + current_year + "-" + current_month + "-" + item.Header.ToString());
                    myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B AND
                        DAY(defectsdata.DatePr) = @C
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year);
                    myCommand.Parameters.AddWithValue("B", current_month);
                    myCommand.Parameters.AddWithValue("C", string.Format("{0:00}", Convert.ToInt32(item.Header.ToString())));
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ТРУБ: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();

                    myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B AND
                        DAY(defectsdata.DatePr) = @C
                    ";
                    myCommand.Connection = connection.myConnection;
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();


                    myCommand.CommandText = @"
                        SELECT 
                        concat(round(( (SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        defectsdata.FlDefectTube =  1 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B AND
                        DAY(defectsdata.DatePr) = @C)  / Count(defectsdata.IndexData) * 100 ),2),'%') 
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B AND
                        DAY(defectsdata.DatePr) = @C
                    ";
                    myCommand.Connection = connection.myConnection;
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        AW.listBox1.Items.Add("ПРОЦЕНТ БРАКА: " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();


                    //=============================
                    myCommand.CommandText = @"
                        SELECT DISTINCT
                        worksmens.NameSmen
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                        ORDER BY worksmens.NameSmen
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}", Convert.ToInt32(item.Header.ToString())));
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        TreeViewItem itemSmens = new TreeViewItem();
                        itemSmens.Tag = "smena";
                        itemSmens.Header = MyDataReader.GetString(0);
                        itemSmens.Items.Add("*");
                        item.Items.Add(itemSmens);
                    }
                    MyDataReader.Close();

                    current_day = item.Header.ToString();
                }

                if (item.Tag.ToString() == "smena")
                {
                    myCommand.CommandText = @"
                        SELECT DISTINCT
                        defectsdata.NumberPart
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                              worksmens.NameSmen = @B  
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}", Convert.ToInt32(current_day)));
                    myCommand.Parameters.AddWithValue("B", item.Header.ToString());
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        TreeViewItem itemPart = new TreeViewItem();
                        itemPart.Tag = "part";
                        itemPart.Header = MyDataReader.GetString(0);
                        itemPart.Items.Add("*");
                        item.Items.Add(itemPart);
                    }
                    MyDataReader.Close();

                    current_smena = item.Header.ToString();
                }

                if (item.Tag.ToString() == "part")
                {
                    myCommand.CommandText = @"
                        SELECT
                        defectsdata.NumberTube
                        FROM
                        indexes
                        INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                              worksmens.NameSmen = @B AND
                              defectsdata.NumberPart = @C
                    ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}", Convert.ToInt32(current_day)));
                    myCommand.Parameters.AddWithValue("B", current_smena);
                    myCommand.Parameters.AddWithValue("C", Convert.ToInt32(item.Header.ToString()));
                    MyDataReader = myCommand.ExecuteReader();

          
                    while (MyDataReader.Read())
                    {
                        TreeViewItem itemTube = new TreeViewItem();
                        itemTube.Tag = "tube";
                        itemTube.Header = "Труба № " + MyDataReader.GetString(0);
                        item.Items.Add(itemTube);
                    }
                    MyDataReader.Close();

                    current_part = item.Header.ToString();
                }

                if (item.Tag.ToString() == "tube")
                {
                    myCommand.CommandText = @"
                        
                    ";
                }
                connection.close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                AW.Close();
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }


        public void info_for_year(string time)
        {
            connection.open();
            AW.listBox1.Items.Clear();
            AW.listBox1.Items.Add("ВРЕМЯ: " + time);
            myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A
                    ";
            myCommand.Connection = connection.myConnection;
            myCommand.Parameters.Clear();
            myCommand.Parameters.AddWithValue("A", time);
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                AW.listBox1.Items.Add("ТРУБ: " + MyDataReader.GetString(0));
            }
            MyDataReader.Close();

            myCommand.CommandText = @"
                        SELECT
                        Count(defectsdata.IndexData)
                        FROM
                        defectsdata
                        WHERE
                        defectsdata.FlDefectTube =  1 AND
                        defectsdata.NumberTube <>  0 AND
                        YEAR(defectsdata.DatePr) = @A
                    ";
            myCommand.Connection = connection.myConnection;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                AW.listBox1.Items.Add("ДЕФЕКТНЫХ ТРУБ: " + MyDataReader.GetString(0));
            }
            MyDataReader.Close();


            myCommand.CommandText = @"
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
                    ";
            myCommand.Connection = connection.myConnection;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                AW.listBox1.Items.Add("ПРОЦЕНТ БРАКА: " + MyDataReader.GetString(0));
            }
            MyDataReader.Close();
            connection.close();
        }

        public void Tube_Control(TreeViewItem item)
        {
            try
            {
                if (item.Tag.ToString() == "tube")
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    myCommand.CommandText = @"
                        SELECT
                        defectsdata.IndexData,
                        defectsdata.NumberPart,
                        defectsdata.NumberTube,
                        defectsdata.NumberSegments,
                        defectsdata.DataSensors,
                        DATE_FORMAT(defectsdata.DatePr, '%Y-%m-%d'),
                        defectsdata.TimePr,
                        defectsdata.Porog,
                        defectsdata.Current,
                        defectsdata.FlDefectTube,
                        worksmens.NameSmen,
                        operators.Surname
                        FROM
                        defectsdata
                        INNER JOIN indexes ON indexes.IndexData = defectsdata.IndexData
                        INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                        INNER JOIN operators ON indexes.Id_Operator1 = operators.Id_Operator AND
                                                indexes.Id_Operator2 = operators.Id_Operator
                        WHERE 
                        YEAR(defectsdata.DatePr) = @A AND
                        MONTHNAME(defectsdata.DatePr) = @B AND
                        DAY(defectsdata.DatePr) = @C AND
                        worksmens.NameSmen = @D AND 
                        defectsdata.NumberPart = @E AND
                        defectsdata.NumberTube = @F
                    ";
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year);
                    myCommand.Parameters.AddWithValue("B", current_month);
                    myCommand.Parameters.AddWithValue("C", current_day);
                    myCommand.Parameters.AddWithValue("D", current_smena);
                    myCommand.Parameters.AddWithValue("E", Convert.ToInt32(current_part));
                    myCommand.Parameters.AddWithValue("F", Convert.ToInt32(item.Header.ToString().Substring(8)));

                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();
                    
                    while (MyDataReader.Read())
                    {
                        AW.label1.Content = "Номер плавки\t" + MyDataReader.GetString(1);
                        AW.label2.Content = "Номер трубы\t" + MyDataReader.GetString(2);
                        AW.label3.Content = "Дата проведения Н.К.\t" + MyDataReader.GetString(5);
                        AW.label4.Content = "Время проведения Н.К.\t" + MyDataReader.GetString(6);
                        AW.label5.Content = "Длина трубы (метры)\t\t " + Math.Round((MyDataReader.GetDouble(3) / 6), 2).ToString();
                        AW.label7.Content = "Время проведения Н.К.\t" + MyDataReader.GetString(6);
                        AW.rectangle1.Width = MyDataReader.GetDouble(3) * 4;

                        for (int i = 0; i < count_deffects_line; i++)
                        {
                            AW.canvas1.Children.Remove((UIElement)AW.canvas1.FindName("errorLine" + i.ToString()));
                            try { AW.canvas1.UnregisterName("errorLine" + i.ToString()); }
                            catch { }
                        }
                        count_deffects_line = 0;

                        int j = 0;

                        foreach (byte deffect in (byte[])MyDataReader.GetValue(4))
                        {
                            if (deffect != 0)
                            {
                                SolidColorBrush redBrush = new SolidColorBrush();
                                redBrush.Color = Colors.Red;

                                Line errorLine = new Line();

                                Canvas.SetLeft(errorLine, 40 + (j * 4));
                                errorLine.X1 = 0;
                                errorLine.X2 = 0;
                                errorLine.Y1 = 151;
                                errorLine.Y2 = 151 + 70;
                                errorLine.StrokeThickness = 4;
                                errorLine.Stroke = redBrush;
                                errorLine.Fill = redBrush;
                                AW.canvas1.RegisterName("errorLine" + count_deffects_line.ToString(), errorLine);
                                AW.canvas1.Children.Add(errorLine);
                                count_deffects_line++;
                            }
                            j++;
                        }
                        AW.label6.Content = "Кол-во дефектных сегментов\t " + count_deffects_line;
                    }
                    connection.close();
                    MyDataReader.Close();
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }

}
