using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TOFI_project
{
    public partial class Credit : Form
    {
        static string server = "sql11.freesqldatabase.com";
        static string database = "sql11671897";
        static string username = "sql11671897";
        static string password = "LdMIXqLdtS";
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

        public void button1_Click(object sender, EventArgs e)
        {
            if (sumBox.Text.Length > 0 && monthsBox.Text.Length > 0 && comboBox1.SelectedIndex != -1)
            {
                if (!Regex.IsMatch(monthsBox.Text, "^\\d+$"))
                {
                    MessageBox.Show("Количество месяцев может состоять только из цифр.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(sumBox.Text, "[+-]?([0-9]*[.])?\\d\\d+"))
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
                        if (Convert.ToDouble(sumBox.Text, CultureInfo.InvariantCulture) >= Convert.ToDouble(reader["minSum"], CultureInfo.InvariantCulture) && Convert.ToDouble(reader["maxSum"], CultureInfo.InvariantCulture) >= Convert.ToDouble(sumBox.Text, CultureInfo.InvariantCulture))
                        {
                            if (Convert.ToInt32(monthsBox.Text) >= Convert.ToInt32(reader["minMonths"]) && Convert.ToInt32(reader["maxMonths"]) >= Convert.ToInt32(monthsBox.Text)) 
                            {
                                double finalSum = double.Round((Convert.ToDouble(reader["interestRate"], CultureInfo.InvariantCulture) / 1200 * Convert.ToInt32(monthsBox.Text)) * Convert.ToDouble(sumBox.Text, CultureInfo.InvariantCulture) + Convert.ToDouble(sumBox.Text, CultureInfo.InvariantCulture), 2);
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

        public void button2_Click(object sender, EventArgs e)
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
                //credit score
                string scorequery = $"select * from User where UserID = {userID_};";
                MySqlCommand scorecmd = new MySqlCommand(scorequery, connection);
                MySqlDataReader scorereader = scorecmd.ExecuteReader();
                int totalScore = 0;

                if (scorereader.HasRows)
                {
                    while (scorereader.Read())
                    {
                        if (Convert.ToInt32(scorereader["age"]) >= 18 && Convert.ToInt32(scorereader["age"]) < 26)
                        {
                            totalScore += 4;
                        }
                        else if (Convert.ToInt32(scorereader["age"]) >= 26 && Convert.ToInt32(scorereader["age"]) < 36)
                        {
                            totalScore += 3;
                        }
                        else if (Convert.ToInt32(scorereader["age"]) >= 36 && Convert.ToInt32(scorereader["age"]) < 46)
                        {
                            totalScore += 2;
                        }
                        else if (Convert.ToInt32(scorereader["age"]) >= 46 && Convert.ToInt32(scorereader["age"]) < 56)
                        {
                            totalScore += 1;
                        }
                        else if (Convert.ToInt32(scorereader["age"]) >= 56)
                        {
                            totalScore += 0;
                        }

                        if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) < 4800)
                        {
                            totalScore -= 5;
                        }
                        else if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) >= 4800 && Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) < 7200)
                        {
                            totalScore -= 3;
                        }
                        else if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) >= 7200 && Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) < 10800)
                        {
                            totalScore -= 1;
                        }
                        else if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) >= 10800 && Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) < 13200)
                        {
                            totalScore += 1;
                        }
                        else if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) >= 13200 && Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) < 18000)
                        {
                            totalScore += 3;
                        }
                        else if (Convert.ToDouble(scorereader["income_per_year"], CultureInfo.InvariantCulture) >= 18000)
                        {
                            totalScore += 5;
                        }

                        if (Convert.ToInt32(scorereader["family"]) == 1)
                        {
                            totalScore += 3;
                        }
                        else if (Convert.ToInt32(scorereader["family"]) == 3)
                        {
                            totalScore -= 2;
                        }
                        else if (Convert.ToInt32(scorereader["family"]) == 4)
                        {
                            totalScore += 1;
                        }
                        else if (Convert.ToInt32(scorereader["family"]) == 5)
                        {
                            totalScore += 2;
                        }

                        if (Convert.ToInt32(scorereader["education"]) == 1)
                        {
                            totalScore -= 10;
                        }
                        else if (Convert.ToInt32(scorereader["education"]) == 2)
                        {
                            totalScore -= 7;
                        }
                        else if (Convert.ToInt32(scorereader["education"]) == 3)
                        {
                            totalScore -= 3;
                        }
                        else if (Convert.ToInt32(scorereader["education"]) == 4)
                        {
                            totalScore -= 1;
                        }
                        else if (Convert.ToInt32(scorereader["education"]) == 5)
                        {
                            totalScore += 1;
                        }
                        else if (Convert.ToInt32(scorereader["education"]) == 6)
                        {
                            totalScore += 4;
                        }

                        if (Convert.ToInt32(scorereader["savings"]) == 1)
                        {
                            totalScore -= 2;
                        }
                        else if (Convert.ToInt32(scorereader["savings"]) == 2)
                        {
                            totalScore += 1;
                        }
                        else if (Convert.ToInt32(scorereader["savings"]) == 3)
                        {
                            totalScore += 3;
                        }
                        else if (Convert.ToInt32(scorereader["savings"]) == 4)
                        {
                            totalScore += 5;
                        }
                    }

                    if (totalScore <= 0)
                    {
                        MessageBox.Show("Ваш кредитный рейтинг слишком низок для выдачи Вам кредита.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("С Вашим кредитным рейтингов все в порядке, продолжаем...", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                scorereader.Close();


                double finalSum = 0;
                string query = $"select * from CreditType where creditID = {comboBox1.SelectedIndex + 1};";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture) >= Convert.ToDouble(reader["minSum"], CultureInfo.InvariantCulture) && Convert.ToDouble(reader["maxSum"], CultureInfo.InvariantCulture) >= Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture))
                        {
                            if (Convert.ToInt32(textBox1.Text) >= Convert.ToInt32(reader["minMonths"]) && Convert.ToInt32(reader["maxMonths"]) >= Convert.ToInt32(textBox1.Text))
                            {
                                finalSum = double.Round((Convert.ToDouble(reader["interestRate"], CultureInfo.InvariantCulture) / 1200 * Convert.ToInt32(textBox1.Text)) * Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture) + Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture), 2);
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

                    string makeLoan = $"insert Loan (sum, monthlyPayment, startDate, endDate, userID, currencyID) select '{Convert.ToString(finalSum, CultureInfo.InvariantCulture)}', '{Convert.ToString(double.Round(finalSum / Convert.ToInt32(textBox1.Text), 2), CultureInfo.InvariantCulture)}', current_date(), ADDDATE(current_date(), INTERVAL {textBox1.Text} Month), {userID_}, currencyID from BankAccount where number = {accounts[comboBox3.SelectedIndex].Split(" ")[0]};";
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
