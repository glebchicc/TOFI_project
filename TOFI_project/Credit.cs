using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TOFI_project
{
    public partial class Credit : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);

        static int userID_;
        static string[] codes;
        List<string> creditTypes = new List<string>();
        List<string> accounts = new List<string>();

        public Credit(int userID)
        {
            InitializeComponent();
            userID_ = userID;

            connection.Open();
            DataTable dt = new DataTable();
            dt.Columns.Add("Interest rate");
            dt.Columns.Add("Minimum sum");
            dt.Columns.Add("Maximum sum");
            dt.Columns.Add("Minimum months");
            dt.Columns.Add("Maximum months");

            string query = "select * from CreditType;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow r = dt.NewRow();
                    r["Interest Rate"] = reader["interestRate"].ToString() + "%";
                    r["Minimum sum"] = reader["minSum"].ToString();
                    r["Maximum sum"] = reader["maxSum"].ToString();
                    r["Minimum months"] = reader["minMonths"].ToString();
                    r["Maximum months"] = reader["maxMonths"].ToString();
                    dt.Rows.Add(r);

                    creditTypes.Add($"{reader["creditID"]} ({reader["interestRate"]}%)");
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                comboBox1.DataSource = creditTypes;
                comboBox2.DataSource = creditTypes;
            }
            reader.Close();

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

            string numberQuery = "select * from BankAccount where userID = " + userID_ + " and currencyID = 1";
            MySqlCommand numbercmd = new MySqlCommand(numberQuery, connection);
            var numberReader = numbercmd.ExecuteReader();
            if (numberReader.HasRows)
            {
                while (numberReader.Read())
                {
                    accounts.Add(numberReader["number"].ToString() + " (" + numberReader["balance"] + " " + codes[(int)numberReader["currencyID"]] + ")");
                }
            }
            else
            {
                MessageBox.Show("У вас нет созданных счетов. Создайте счет для перевода средств.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            comboBox3.DataSource = accounts;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sumBox.Text.Length > 0 && monthsBox.Text.Length > 0 && comboBox1.SelectedIndex != -1)
            {
                if (!Regex.IsMatch(monthsBox.Text, "^\\d+$"))
                {
                    MessageBox.Show("Количество месяцев может состоять только из цифр.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(sumBox.Text, "^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$"))
                {
                    MessageBox.Show("Сумма кредита должна быть целым или вещественным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                connection.Open();
                string query = $"select * from CreditType where creditID = {comboBox1.SelectedIndex + 1};";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToDouble(sumBox.Text) > Convert.ToDouble(reader["minSum"]) && Convert.ToDouble(reader["maxSum"]) > Convert.ToDouble(sumBox.Text))
                        {
                            if (Convert.ToInt32(monthsBox.Text) > Convert.ToInt32(reader["minMonths"]) && Convert.ToInt32(reader["maxMonths"]) > Convert.ToInt32(monthsBox.Text)) 
                            {
                                double finalSum = double.Round((Convert.ToDouble(reader["interestRate"]) / 1200 * Convert.ToInt32(monthsBox.Text)) * Convert.ToDouble(sumBox.Text) + Convert.ToDouble(sumBox.Text), 2);
                                resultText.Text = $"Финальная сумма кредита - {finalSum}, ежемесячная выплата - {double.Round(finalSum / Convert.ToInt32(monthsBox.Text), 2)}";
                            }
                            else
                            {
                                MessageBox.Show("Указанный срок не вписывается в выбранный кредит.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                connection.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Указанная сумма не вписывается в выбранный кредит.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            connection.Close();
                            return;
                        }
                    }
                }
                reader.Close();

                connection.Close();
            }
            else
            {
                MessageBox.Show("Все поля формы должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0 && textBox1.Text.Length > 0 && comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
            {
                if (!Regex.IsMatch(textBox1.Text, "^\\d+$"))
                {
                    MessageBox.Show("Количество месяцев может состоять только из цифр.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(textBox2.Text, "^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$"))
                {
                    MessageBox.Show("Сумма кредита должна быть целым или вещественным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                connection.Open();
                double finalSum = 0;
                string query = $"select * from CreditType where creditID = {comboBox1.SelectedIndex + 1};";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToDouble(textBox2.Text) > Convert.ToDouble(reader["minSum"]) && Convert.ToDouble(reader["maxSum"]) > Convert.ToDouble(textBox2.Text))
                        {
                            if (Convert.ToInt32(textBox1.Text) > Convert.ToInt32(reader["minMonths"]) && Convert.ToInt32(reader["maxMonths"]) > Convert.ToInt32(textBox1.Text))
                            {
                                finalSum = double.Round((Convert.ToDouble(reader["interestRate"]) / 1200 * Convert.ToInt32(textBox1.Text)) * Convert.ToDouble(textBox2.Text) + Convert.ToDouble(textBox2.Text), 2);
                                //resultText.Text = $"Финальная сумма кредита - {finalSum}, ежемесячная выплата - {double.Round(finalSum / Convert.ToInt32(textBox1.Text), 2)}";
                            }
                            else
                            {
                                MessageBox.Show("Указанный срок не вписывается в выбранный кредит.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                connection.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Указанная сумма не вписывается в выбранный кредит.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            connection.Close();
                            return;
                        }
                    }
                    reader.Close();
                    string getMoney = $"UPDATE BankAccount set balance = balance + {textBox2.Text} where number = {accounts[comboBox3.SelectedIndex].Split(" ")[0]}";
                    MySqlCommand getcmd = new MySqlCommand(getMoney, connection);
                    getcmd.ExecuteNonQuery();

                    string makeLoan = $"insert loan (sum, monthlyPayment, startDate, endDate, userID) values ('{Convert.ToString(finalSum, CultureInfo.InvariantCulture)}', '{Convert.ToString(double.Round(finalSum / Convert.ToInt32(textBox1.Text), 2), CultureInfo.InvariantCulture)}', current_date(), ADDDATE(current_date(), INTERVAL {textBox1.Text} Month), {userID_});";
                    MySqlCommand makecmd = new MySqlCommand(makeLoan, connection);
                    makecmd.ExecuteNonQuery();

                    MessageBox.Show("Кредит успешен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Все поля формы должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
