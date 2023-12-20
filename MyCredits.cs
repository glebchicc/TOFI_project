using MySql.Data.MySqlClient;
using System.Data;

namespace TOFI_project
{
    public partial class MyCredits : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);

        static int userID_;
        static string[] codes;
        List<string> credits = new List<string>();
        List<string> accounts = new List<string>();

        public MyCredits(int userID)
        {
            InitializeComponent();
            userID_ = userID;

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
                MessageBox.Show("У вас нет созданных счетов. Создайте счет для возможности кредитования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            numberReader.Close();

            DataTable dt = new DataTable();
            dt.Columns.Add("Номер кредита");
            dt.Columns.Add("Остаток по кредиту");
            dt.Columns.Add("Ежемесячный платеж");
            dt.Columns.Add("Дата начала");
            dt.Columns.Add("Дата окончания");

            string query = "select * from Loan;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow r = dt.NewRow();
                    r["Номер кредита"] = reader["loanID"].ToString();
                    r["Остаток по кредиту"] = reader["sum"].ToString() + " " + codes[(int)reader["currencyID"]];
                    r["Ежемесячный платеж"] = reader["monthlyPayment"].ToString() + " " + codes[(int)reader["currencyID"]];
                    r["Дата начала"] = reader["startDate"].ToString().Split(" ")[0];
                    r["Дата окончания"] = reader["endDate"].ToString().Split(" ")[0];
                    dt.Rows.Add(r);

                    credits.Add(reader["loanID"].ToString());
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                comboBox1.DataSource = credits;
                comboBox2.DataSource = accounts;
            }

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection.Open();
            if (comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
            {
                string accountquery = $"select currencyID from BankAccount where number = {accounts[comboBox2.SelectedIndex].Split(" ")[0]}";
                MySqlCommand accountcmd = new MySqlCommand(accountquery, connection);
                int currency = Convert.ToInt32(accountcmd.ExecuteScalar());

                string loanquery = $"select currencyID from Loan where loanID = {comboBox1.Items[comboBox1.SelectedIndex]}";
                MySqlCommand loancmd = new MySqlCommand(loanquery, connection);
                int loanCurrency = Convert.ToInt32(loancmd.ExecuteScalar());

                if (currency == loanCurrency)
                {
                    string balancequery = $"select balance from BankAccount where number = {accounts[comboBox2.SelectedIndex].Split(" ")[0]}";
                    MySqlCommand balancecmd = new MySqlCommand(balancequery, connection);
                    double balance = Convert.ToDouble(balancecmd.ExecuteScalar());

                    string monthquery = $"select monthlyPayment from Loan where loanID = {comboBox1.Items[comboBox1.SelectedIndex]}";
                    MySqlCommand monthcmd = new MySqlCommand(monthquery, connection);
                    double monthPayment = Convert.ToDouble(monthcmd.ExecuteScalar());

                    string sumquery = $"select sum from Loan where loanID = {comboBox1.Items[comboBox1.SelectedIndex]}";
                    MySqlCommand sumcmd = new MySqlCommand(sumquery, connection);
                    double sumleft = Convert.ToDouble(sumcmd.ExecuteScalar());

                    if (sumleft > 0)
                    {
                        if (balance >= monthPayment)
                        {
                            if (monthPayment > sumleft)
                            {
                                string minusBalance = $"update BankAccount set balance = balance - {sumleft} where number = {accounts[comboBox2.SelectedIndex].Split(" ")[0]}";
                                MySqlCommand minusBalanceCMD = new MySqlCommand(minusBalance, connection);
                                minusBalanceCMD.ExecuteNonQuery();

                                string minusMonth = $"update Loan set sum = sum - {sumleft} where loanID = {comboBox1.Items[comboBox1.SelectedIndex]}";
                                MySqlCommand minusMonthCMD = new MySqlCommand(minusMonth, connection);
                                minusMonthCMD.ExecuteNonQuery();
                            }
                            else
                            {
                                string minusBalance = $"update BankAccount set balance = balance - {monthPayment} where number = {accounts[comboBox2.SelectedIndex].Split(" ")[0]}";
                                MySqlCommand minusBalanceCMD = new MySqlCommand(minusBalance, connection);
                                minusBalanceCMD.ExecuteNonQuery();

                                string minusMonth = $"update Loan set sum = sum - {monthPayment} where loanID = {comboBox1.Items[comboBox1.SelectedIndex]}";
                                MySqlCommand minusMonthCMD = new MySqlCommand(minusMonth, connection);
                                minusMonthCMD.ExecuteNonQuery();
                            }

                            MessageBox.Show("Ежемесячный платеж внесен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            accounts = new List<string>();
                            string numberQuery = "select * from BankAccount where userID = " + userID_;
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
                                MessageBox.Show("У вас нет созданных счетов. Создайте счет для возможности кредитования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            numberReader.Close();

                            DataTable dt = new DataTable();
                            dt.Columns.Add("Номер кредита");
                            dt.Columns.Add("Остаток по кредиту");
                            dt.Columns.Add("Ежемесячный платеж");
                            dt.Columns.Add("Дата начала");
                            dt.Columns.Add("Дата окончания");
                            

                            string query = "select * from Loan;";
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            MySqlDataReader reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    DataRow r = dt.NewRow();
                                    r["Номер кредита"] = reader["loanID"].ToString();
                                    r["Остаток по кредиту"] = reader["sum"].ToString() + " " + codes[(int)reader["currencyID"]];
                                    r["Ежемесячный платеж"] = reader["monthlyPayment"].ToString() + " " + codes[(int)reader["currencyID"]];
                                    r["Дата начала"] = reader["startDate"].ToString().Split(" ")[0];
                                    r["Дата окончания"] = reader["endDate"].ToString().Split(" ")[0];
                                    dt.Rows.Add(r);

                                    credits.Add(reader["loanID"].ToString());
                                }
                                dataGridView1.DataSource = dt;
                                dataGridView1.AllowUserToAddRows = false;
                                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                                comboBox1.DataSource = credits;
                                comboBox2.DataSource = accounts;
                            }
                            connection.Close();
                        }
                        else
                        {
                            MessageBox.Show("Этот кредит уже оплачен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            connection.Close();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("На вашем счете недостаточно средств.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Оплата кредита принимается только в валюте кредита.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connection.Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Для выплаты требуется заполнить все поля формы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
                return;
            }
        }
    }
}
