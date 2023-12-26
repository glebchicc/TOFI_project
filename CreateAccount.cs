using MySql.Data.MySqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TOFI_project
{
    public partial class CreateAccount : Form
    {
        static int userID;
        static string server = "sql11.freesqldatabase.com";
        static string database = "sql11671897";
        static string username = "sql11671897";
        static string password = "LdMIXqLdtS";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);

        public class TestException : Exception
        {
            public TestException(string message) : base(message) { }
        }

        public CreateAccount(int userID_)
        {
            InitializeComponent();
            userID = userID_;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 && BalanceBox.Text.Length > 0) 
            {
                if (!Regex.IsMatch(BalanceBox.Text, "[+-]?([0-9]*[.])?[0-9]+"))
                {
                    MessageBox.Show("Начальный баланс должен быть целым или вещественным положительным числом c точкой-разделителем.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new TestException("Начальный баланс должен быть целым или вещественным положительным числом.");
                    //return;
                }

                if (Convert.ToDouble(BalanceBox.Text, CultureInfo.InvariantCulture) < 0)
                {
                    MessageBox.Show("Начальный баланс должен быть целым или вещественным положительным числом с точкой-разделителем.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new TestException("Начальный баланс должен быть целым или вещественным положительным числом.");
                    //return;
                }

                try
                {
                    long number;
                    connection.Open();
                    while (true)
                    {
                        Random random = new Random();
                        string number1 = random.Next(11111111, 99999999).ToString();
                        string number2 = random.Next(11111111, 99999999).ToString();
                        number1 += number2;
                        long.TryParse(number1, out number);
                        string checkQuery = "select COUNT(*) from BankAccount where number = " + number;
                        MySqlCommand checkcmd = new MySqlCommand(checkQuery, connection);
                        if (Convert.ToInt32(checkcmd.ExecuteScalar()) > 0)
                        {
                            continue;
                            throw new TestException("Такой номер аккаунта уже существует.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    string query = $"insert BankAccount (balance, number, validity_period, userID, currencyID) values ({BalanceBox.Text}, {number}, ADDDATE(current_date(), INTERVAL 4 Year), {userID}, {comboBox1.SelectedIndex + 1});";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Создание счета успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Accounts formAccounts = new Accounts(userID);
                    formAccounts.Show();
                    connection.Close();
                    this.Close();
                    return;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Ошибка при попытке создать новый счет.\n" + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Все поля формы должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new TestException("Все поля формы должны быть заполнены.");
                //return;
            }
        }
    }
}
