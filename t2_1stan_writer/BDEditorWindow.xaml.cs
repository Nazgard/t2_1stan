﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MessageBox = System.Windows.Forms.MessageBox;

namespace t2_1stan_writer
{
    /// <summary>
    ///     Логика взаимодействия для BDEditorWindow.xaml
    /// </summary>
    public partial class BDEditorWindow : Window
    {
        private readonly Connection _connection = new Connection();
        private readonly MySqlCommand _mySqlCommand = new MySqlCommand();
        private MySqlDataReader _mySqlDataReader;

        public BDEditorWindow()
        {
            InitializeComponent();
        }


        //===========================================================================
        //===========================================================================
        //Специалисты
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        operators
                        SET active = 0
                        WHERE Id_Operator = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg1.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg1();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt1 = TextBox1.Text;
                var txt2 = TextBox2.Text;
                var txt3 = TextBox3.Text;

                if (txt1.Trim() != "" && txt2.Trim() != "" && txt3.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO operators (Surname, LevelMD, LevelUSD) VALUES (@Surname, @LevelMD, @LevelUSD)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.AddWithValue("Surname", txt1);
                    _mySqlCommand.Parameters.AddWithValue("LevelMD", txt2);
                    _mySqlCommand.Parameters.AddWithValue("LevelUSD", txt3);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg1();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                }                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg1()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    operators.Id_Operator AS 'ID',
                    operators.Surname AS 'ФИО',
                    operators.LevelMD AS 'Уровень MD',
                    operators.LevelUSD AS 'Уровень USD'
                    FROM
                    operators
                    WHERE operators.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg1.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        //===========================================================================
        //===========================================================================
        //СМЕНЫ
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                    UPDATE
                    worksmens
                    SET active = 0
                    WHERE Id_WorkSmen = @id
                ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg2.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg2();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt4 = TextBox4.Text;

                if (txt4.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO worksmens (NameSmen) VALUES (@NameSmen)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameSmen", txt4);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg2();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                }                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg2()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    worksmens.Id_WorkSmen AS 'ID',
                    worksmens.NameSmen AS 'Название смены'
                    FROM
                    worksmens
                    WHERE worksmens.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg2.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Типоразмеры трубы
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                    UPDATE
                    sizetubes
                    SET active = 0
                    WHERE Id_SizeTube = @id
                ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg3.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg3();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt5 = TextBox5.Text;

                if (txt5.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO sizetubes (SizeTube) VALUES (@SizeTube)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("SizeTube", txt5);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg3();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                } 
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg3()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    sizetubes.Id_SizeTube AS 'ID',
                    sizetubes.SizeTube  AS 'Диаметр трубы'
                    FROM
                    sizetubes
                    WHERE sizetubes.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg3.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Контрольные образцы
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click3(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        controlsamples
                        SET active = 0
                        WHERE Id_ControlSample = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg4.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg4();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt6 = TextBox6.Text;
                var txt7 = TextBox7.Text;
                var txt8 = TextBox8.Text;

                if (txt6.Trim() != "" && txt7.Trim() != "" && txt8.Trim() != "" && ComboBox1.SelectedIndex != -1)
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO controlsamples (NameControlSample, Id_SizeTube, DepthMin, DepthMax) VALUES (@NameControlSample, @Id_SizeTube, @DepthMin, @DepthMax)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameControlSample", txt6);
                    _mySqlCommand.Parameters.AddWithValue("Id_SizeTube",
                        ((KeyValuePair<int, string>)ComboBox1.SelectedItem).Key);
                    _mySqlCommand.Parameters.AddWithValue("DepthMin", txt7);
                    _mySqlCommand.Parameters.AddWithValue("DepthMax", txt8);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg4();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                }                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg4()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    controlsamples.Id_ControlSample AS 'ID',
                    controlsamples.NameControlSample AS 'Название контрольного образца',
                    sizetubes.SizeTube AS 'Диаметр трубы',
                    controlsamples.DepthMin AS 'Минимальный диаметр',
                    controlsamples.DepthMax AS 'Максимальный диаметр'
                    FROM
                    controlsamples
                    Inner Join sizetubes ON sizetubes.Id_SizeTube = controlsamples.Id_SizeTube
                    WHERE controlsamples.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg4.ItemsSource = dset.Tables[0].DefaultView;
                ComboBox1.ItemsSource = get_db_sizetubes();
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Dictionary<int, string> get_db_sizetubes()
        {
            _connection.Open();
            var sizetubes = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_SizeTube, SizeTube FROM sizetubes", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                sizetubes.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return sizetubes;
        }

        //===========================================================================
        //===========================================================================
        //Нормативные документы
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click4(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        gosts
                        SET active = 0
                        WHERE Id_Gost = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg5.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg5();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt9 = TextBox9.Text;

                if (txt9.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO gosts (NameGost) VALUES (@NameGost)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameGost", txt9);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg5();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                }                 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg5()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    gosts.Id_Gost AS 'ID',
                    gosts.NameGost AS 'Название ГОСТа'
                    FROM
                    gosts
                    WHERE gosts.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg5.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Дефектоскопы
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click5(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        device    
                        SET active = 0
                        WHERE Id_Device = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg6.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg6();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click5(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt10 = TextBox10.Text;

                if (txt10.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO device (NameDevice) VALUES (@NameDevice)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameDevice", txt10);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg6();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                } 
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg6()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    device.Id_Device AS 'ID',
                    device.NameDevice AS 'Название дефектоскопа'
                    FROM
                    device
                    WHERE device.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg6.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Датчики
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click9(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        sensors    
                        SET active = 0
                        WHERE Id_Sensor = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg10.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg10();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click9(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt12 = TextBox12.Text;

                if (txt12.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO sensors (NameSensor) VALUES (@NameSensor)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameSensor", txt12);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg10();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                }                 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg10()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    sensors.Id_Sensor AS 'ID',
                    sensors.NameSensor AS 'Название датчика'
                    FROM
                    sensors
                    WHERE sensors.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg10.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Соответсвия
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click6(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        bufferdata    
                        SET active = 0
                        WHERE Id = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg7.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg7();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click6(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"
                    INSERT INTO bufferdata (Id_Gost, Id_SizeTube) VALUES (@Id_Gost, @Id_SizeTube)
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;
                _mySqlCommand.Parameters.Clear();
                _mySqlCommand.Parameters.AddWithValue("Id_Gost",
                    ((KeyValuePair<int, string>) ComboBox2.SelectedItem).Key);
                _mySqlCommand.Parameters.AddWithValue("Id_SizeTube",
                    ((KeyValuePair<int, string>) ComboBox3.SelectedItem).Key);
                _mySqlCommand.ExecuteNonQuery();
                _connection.Close();
                fill_dg7();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg7()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    bufferdata.Id 'ID',
                    sizetubes.SizeTube 'Диаметр трубы',
                    gosts.NameGost 'Название ГОСТа'
                    FROM
                    bufferdata
                    Inner Join gosts ON gosts.Id_Gost = bufferdata.Id_Gost
                    Inner Join sizetubes ON sizetubes.Id_SizeTube = bufferdata.Id_SizeTube
                    WHERE bufferdata.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg7.ItemsSource = dset.Tables[0].DefaultView;
                ComboBox2.ItemsSource = get_db_gosts();
                ComboBox3.ItemsSource = get_db_sizetubes();
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Dictionary<int, string> get_db_gosts()
        {
            _connection.Open();
            var gosts = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_Gost, NameGost FROM gosts", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                gosts.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return gosts;
        }

        //===========================================================================
        //===========================================================================
        //Временные интервалы смен
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click7(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        timeintervalsmens    
                        SET active = 0
                        WHERE Id_TimeIntervalSmen = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg8.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg8();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click7(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = DateTimeUpDown1.Text + "-" + DateTimeUpDown2.Text;
                var txt1 = DateTimeUpDown1.Text;
                var txt2 = DateTimeUpDown2.Text;

                if (txt1.Trim() != "" && txt2.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO timeintervalsmens (TimeIntervalSmen) VALUES (@TimeIntervalSmen)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("TimeIntervalSmen", txt);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg8();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg8()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    timeintervalsmens.Id_TimeIntervalSmen AS 'ID',
                    timeintervalsmens.TimeIntervalSmen AS 'Время'
                    FROM
                    timeintervalsmens
                    WHERE timeintervalsmens.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg8.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //===========================================================================
        //===========================================================================
        //Виды искусственых дефектов
        //===========================================================================
        //===========================================================================

        private void MenuItem_Click8(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        UPDATE
                        listdefects    
                        SET active = 0
                        WHERE Id_NameDefect = @id
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    var dataRowView = (DataRowView) dg9.CurrentItem;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg9();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void Button_Click8(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt11 = TextBox11.Text;

                if (txt11.Trim() != "")
                {
                    _connection.Open();
                    _mySqlCommand.CommandText = @"
                        INSERT INTO listdefects (NameDefect) VALUES (@NameDefect)
                    ";
                    _mySqlCommand.Connection = _connection.MySqlConnection;
                    _mySqlCommand.Parameters.Clear();
                    _mySqlCommand.Parameters.AddWithValue("NameDefect", txt11);
                    _mySqlCommand.ExecuteNonQuery();
                    _connection.Close();
                    fill_dg9();
                }
                else
                {
                    MessageBox.Show("Все поля обязательны для заполнения и не могут быть пустыми");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void fill_dg9()
        {
            try
            {
                _connection.Open();
                _mySqlCommand.CommandText = @"SELECT
                    listdefects.Id_NameDefect AS 'ID',
                    listdefects.NameDefect AS 'Искусственный дефект'
                    FROM
                    listdefects
                    WHERE listdefects.active = 1
                ";
                _mySqlCommand.Connection = _connection.MySqlConnection;

                var dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);

                var dset = new DataSet();
                dataAdapter.Fill(dset);

                dg9.ItemsSource = dset.Tables[0].DefaultView;
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                //MessageBox.Show(TabControl.SelectedIndex.ToString());
                switch (TabControl.SelectedIndex)
                {
                    case 0:
                        {
                            fill_dg1();
                        }
                        break;
                    case 1:
                        {
                            fill_dg2();
                        }
                        break;
                    case 2:
                        {
                            fill_dg3();
                        }
                        break;
                    case 3:
                        {
                            fill_dg4();
                        }
                        break;
                    case 4:
                        {
                            fill_dg5();
                        }
                        break;
                    case 5:
                        {
                            fill_dg6();
                        }
                        break;
                    case 6:
                        {
                            fill_dg10();
                        }
                        break;
                    case 7:
                        {
                            fill_dg7();
                        }
                        break;
                    case 8:
                        {
                            fill_dg8();
                        }
                        break;
                    case 9:
                        {
                            fill_dg9();
                        }
                        break;
                }
            }            
        }
    }
}