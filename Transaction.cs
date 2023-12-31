﻿using MySql.Data.MySqlClient;
using System.Globalization;
using static TOFI_project.CreateAccount;
using System.Text.RegularExpressions;

namespace TOFI_project
{
    public partial class Transaction : Form
    {
        static string server = "sql11.freesqldatabase.com";
        static string database = "sql11671897";
        static string username = "sql11671897";
        static string password = "LdMIXqLdtS";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);
        public List<string> accounts = new List<string>();
        static string[] codes;

        public Transaction(int userID_)
        {
            InitializeComponent();
            

            connection.Open();
            string conutquery = "select COUNT(*) from Currency";
            MySqlCommand countcmd = new MySqlCommand(conutquery, connection);
            int count;
            int.TryParse(countcmd.ExecuteScalar().ToString(), out count);
            codes = new string[count + 1];
            for (int i = 1; i <= count; i++)
            {
                string curQuery = "select codeOfCurrency from Currency where currencyID = '" + i + "';";
                MySqlCommand curcmd = new MySqlCommand(curQuery, connection);
                codes[i] = curcmd.ExecuteScalar().ToString();
            }

            string numberQuery = "select * from BankAccount where userID = " + userID_;
            MySqlCommand numbercmd = new MySqlCommand(numberQuery, connection);
            var reader = numbercmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    accounts.Add(reader["number"].ToString() + " (" + reader["balance"] + " " + codes[(int)reader["currencyID"]] + ")");
                }
            }
            else
            {
                MessageBox.Show("У вас нет созданных счетов. Создайте счет для перевода средств.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            comboBox1.DataSource = accounts;
            connection.Close();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            int senderCurrency = -1;
            int recipientCurrency = -1;

            if (!(recipientBox.Text.Length > 0 && moneyBox.Text.Length > 0))
            {
                MessageBox.Show("Заполните все поля формы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (accounts[comboBox1.SelectedIndex].Split(" ")[0] == recipientBox.Text)
            {
                MessageBox.Show("Номера счетов отправителя и получателя не могут совпадать.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Regex.IsMatch(moneyBox.Text, "[+-]?([0-9]*[.])?\\d\\d+"))
            {
                MessageBox.Show("Сумма отправляемых средств должна быть целым или вещественным положительным числом c точкой-разделителем.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new TestException("Сумма отправляемых средств должна быть целым или вещественным положительным числом c точкой-разделителем.");
                //return;
            }
            if (Convert.ToDouble(moneyBox.Text, CultureInfo.InvariantCulture) <= 0)
            {
                MessageBox.Show("Сумма отправляемых средств должна быть больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connection.Open();
            string senderQuery = "select * from BankAccount where number = " + accounts[comboBox1.SelectedIndex].Split(" ")[0];
            MySqlCommand sendercmd = new MySqlCommand(senderQuery, connection);
            var reader = sendercmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (Convert.ToDouble(reader["balance"], CultureInfo.InvariantCulture) < Convert.ToDouble(moneyBox.Text, CultureInfo.InvariantCulture))
                    {
                        MessageBox.Show("У Вас недостаточно средств для данного перевода.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        return;
                    }
                    senderCurrency = Convert.ToInt32(reader["currencyID"]);
                }
            }
            else
            {
                MessageBox.Show("Такого номера отправителя не существует. Попробуйте закрыть и открыть данную страницу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
                return;
            }
            reader.Close();

            string recipientQuery = $"select * from BankAccount where number = {recipientBox.Text}";
            MySqlCommand countcmd = new MySqlCommand(recipientQuery, connection);
            var recipientReader = countcmd.ExecuteReader();
            if (recipientReader.HasRows)
            {
                while(recipientReader.Read())
                {
                    recipientCurrency = Convert.ToInt32(recipientReader["currencyID"]);
                }
            }
            else
            {
                MessageBox.Show("Такого номера получателя не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
                return;
            }
            recipientReader.Close();

            if (senderCurrency != -1 && recipientCurrency != -1)
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                if (senderCurrency == recipientCurrency)
                {
                    string sendMoney = $"UPDATE BankAccount set balance = balance - {moneyBox.Text} where number = {accounts[comboBox1.SelectedIndex].Split(" ")[0]}";
                    MySqlCommand sendcmd = new MySqlCommand(sendMoney, connection);
                    sendcmd.ExecuteNonQuery();

                    string getMoney = $"UPDATE BankAccount set balance = balance + {moneyBox.Text} where number = {recipientBox.Text}";
                    MySqlCommand getcmd = new MySqlCommand(getMoney, connection);
                    getcmd.ExecuteNonQuery();

                    string saveInfo = $"INSERT Transaction (sum, transactionDate, senderNumber, recepientNumber, senderCurrencyID, recipientCurrencyID) values ({moneyBox.Text}, current_date(), {accounts[comboBox1.SelectedIndex].Split(" ")[0]}, {recipientBox.Text}, {senderCurrency}, {recipientCurrency})";
                    MySqlCommand savecmd = new MySqlCommand(saveInfo, connection);
                    savecmd.ExecuteNonQuery();

                    MessageBox.Show("Перевод средств успешен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else if (senderCurrency == 1 && (recipientCurrency == 2 || recipientCurrency == 3)) 
                {
                    string multi = $"Select exchangeRate from Currency where currencyID = {recipientCurrency};";
                    MySqlCommand multicmd = new MySqlCommand(multi, connection);
                    double multiplixer = Convert.ToDouble(multicmd.ExecuteScalar(), CultureInfo.InvariantCulture);
                    double newMoney = Convert.ToDouble(moneyBox.Text, CultureInfo.InvariantCulture) / multiplixer;

                    string sendMoney = $"UPDATE BankAccount set balance = balance - {moneyBox.Text} where number = {accounts[comboBox1.SelectedIndex].Split(" ")[0]}";
                    MySqlCommand sendcmd = new MySqlCommand(sendMoney, connection);
                    sendcmd.ExecuteNonQuery();

                    string getMoney = $"UPDATE BankAccount set balance = balance + {double.Round(newMoney, 2).ToString(nfi)} where number = {recipientBox.Text}";
                    MySqlCommand getcmd = new MySqlCommand(getMoney, connection);
                    getcmd.ExecuteNonQuery();

                    string saveInfo = $"INSERT Transaction (sum, transactionDate, senderNumber, recepientNumber, senderCurrencyID, recipientCurrencyID) values ({moneyBox.Text}, current_date(), {accounts[comboBox1.SelectedIndex].Split(" ")[0]}, {recipientBox.Text}, {senderCurrency}, {recipientCurrency})";
                    MySqlCommand savecmd = new MySqlCommand(saveInfo, connection);
                    savecmd.ExecuteNonQuery();

                    MessageBox.Show("Перевод средств успешен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else if ((senderCurrency == 2 || senderCurrency == 3) && recipientCurrency == 1)
                {
                    string multi = $"Select exchangeRate from Currency where currencyID = {senderCurrency};";
                    MySqlCommand multicmd = new MySqlCommand(multi, connection);
                    double multiplixer = Convert.ToDouble(multicmd.ExecuteScalar(), CultureInfo.InvariantCulture);
                    double newMoney = Convert.ToDouble(moneyBox.Text, CultureInfo.InvariantCulture) * multiplixer;

                    string sendMoney = $"UPDATE BankAccount set balance = balance - {moneyBox.Text} where number = {accounts[comboBox1.SelectedIndex].Split(" ")[0]}";
                    MySqlCommand sendcmd = new MySqlCommand(sendMoney, connection);
                    sendcmd.ExecuteNonQuery();

                    string getMoney = $"UPDATE BankAccount set balance = balance + {double.Round(newMoney, 2).ToString(nfi)} where number = {recipientBox.Text}";
                    MySqlCommand getcmd = new MySqlCommand(getMoney, connection);
                    getcmd.ExecuteNonQuery();

                    string saveInfo = $"INSERT Transaction (sum, transactionDate, senderNumber, recepientNumber, senderCurrencyID, recipientCurrencyID) values ({moneyBox.Text}, current_date(), {accounts[comboBox1.SelectedIndex].Split(" ")[0]}, {recipientBox.Text}, {senderCurrency}, {recipientCurrency})";
                    MySqlCommand savecmd = new MySqlCommand(saveInfo, connection);
                    savecmd.ExecuteNonQuery();

                    MessageBox.Show("Перевод средств успешен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Мультивалютные переводы недоступны. Переведите средства с валютного счета в беларуский и повторите попытку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connection.Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Ошибка при сравнении валют счетов. Попробуйте перезайти в окно переводов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
                return;
            }
        }
    }
}
