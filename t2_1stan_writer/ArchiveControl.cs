using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Controls;
using System.Windows;
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
        MySqlCommand myCommand = new MySqlCommand();
        MySqlDataReader MyDataReader;
        List<string> list = new List<string>();

        public void First_TreeData()
        {
            list.Clear();
            myCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
            myCommand.Connection = connection.myConnection;

            try
            {
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    list.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
                connection.close();

                foreach (string year in list)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = "year";
                    item.Header = year.ToString();
                    item.Items.Add("*");
                    AW.treeView1.Items.Add(item);
                }
            }
            catch
            {
                AW.Close();
            }            
        }

        public void Expander(RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)e.OriginalSource;
                item.Items.Clear();

                if (item.Tag.ToString() == "year")
                {
                    list.Clear();
                    myCommand.CommandText = @"
                    SELECT
                    DISTINCT
                    MONTHNAME(defectsdata.DatePr)
                    FROM
                    defectsdata
                    WHERE YEAR(defectsdata.DatePr) = @A
                ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", item.Header.ToString());

                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        list.Add(MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    connection.close();

                    foreach (string month in list)
                    {
                        TreeViewItem itemMonth = new TreeViewItem();
                        itemMonth.Tag = "month";
                        itemMonth.Header = month.ToString();
                        itemMonth.Items.Add("*");
                        item.Items.Add(itemMonth);
                    }
                    current_year = item.Header.ToString();
                }

                if (item.Tag.ToString() == "month")
                {
                    list.Clear();
                    myCommand.CommandText = @"
                    SELECT
                    DISTINCT
                    DAY(defectsdata.DatePr)
                    FROM
                    defectsdata
                    WHERE MONTHNAME(defectsdata.DatePr) = @A
                ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", item.Header.ToString());

                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        list.Add(MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    connection.close();

                    foreach (string day in list)
                    {
                        TreeViewItem itemDays = new TreeViewItem();
                        itemDays.Tag = "day";
                        itemDays.Header = day.ToString();
                        itemDays.Items.Add("*");
                        item.Items.Add(itemDays);
                    }
                    current_month = item.Header.ToString();
                }

                if (item.Tag.ToString() == "day")
                {
                    list.Clear();
                    myCommand.CommandText = @"
                    SELECT DISTINCT
                    worksmens.NameSmen
                    FROM
                    indexes
                    INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                    INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                    WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                ";
                    myCommand.Connection = connection.myConnection;
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}", Convert.ToInt32(item.Header.ToString())));
                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        list.Add(MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    connection.close();

                    foreach (string smena in list)
                    {
                        TreeViewItem itemSmens = new TreeViewItem();
                        itemSmens.Tag = "smena";
                        itemSmens.Header = smena.ToString();
                        itemSmens.Items.Add("*");
                        item.Items.Add(itemSmens);
                    }
                    current_day = item.Header.ToString();
                }

                if (item.Tag.ToString() == "smena")
                {
                    list.Clear();
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
                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        list.Add(MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    connection.close();

                    foreach (string part in list)
                    {
                        TreeViewItem itemPart = new TreeViewItem();
                        itemPart.Tag = "part";
                        itemPart.Header = part.ToString();
                        itemPart.Items.Add("*");
                        item.Items.Add(itemPart);
                    }

                    current_smena = item.Header.ToString();
                }

                if (item.Tag.ToString() == "part")
                {
                    list.Clear();
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
                    connection.open();
                    MyDataReader = myCommand.ExecuteReader();

                    while (MyDataReader.Read())
                    {
                        list.Add("Труба № " + MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    connection.close();

                    foreach (string tube in list)
                    {
                        TreeViewItem itemTube = new TreeViewItem();
                        itemTube.Tag = "tube";
                        itemTube.Header = tube.ToString();
                        item.Items.Add(itemTube);
                    }
                }
            }
            catch
            {
                AW.Close();
            }
        }


    }
}
