using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using System.Data;

namespace TOFI_project
{
    public partial class TransactionHistory : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);

        static string[] codes;
        static int userID_;

        public TransactionHistory(int userID)
        {
            InitializeComponent();
            userID_ = userID;
            DataTable dt = new DataTable();
            dt.Columns.Add("Transaction Date");
            dt.Columns.Add("Sum");
            dt.Columns.Add("Sender");
            dt.Columns.Add("Recipient");

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

            string query = "select * from Transaction where senderNumber in (select number from BankAccount where number = senderNumber or number = recepientNumber);";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow r = dt.NewRow();
                    r["Transaction Date"] = reader["transactionDate"].ToString().Split(" ")[0];
                    r["Sum"] = reader["sum"].ToString() + " " + codes[(int)reader["senderCurrencyID"]];
                    r["Sender"] = reader["senderNumber"].ToString();
                    r["Recipient"] = reader["recepientNumber"].ToString();
                    dt.Rows.Add(r);
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            dt.Columns.Add("Transaction Date");
            dt.Columns.Add("Sum");
            dt.Columns.Add("Sender");
            dt.Columns.Add("Recipient");

            string query = "select * from Transaction where senderNumber in (select number from BankAccount where number = senderNumber or number = recepientNumber);";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow r = dt.NewRow();
                    r["Transaction Date"] = reader["transactionDate"].ToString();
                    r["Sum"] = reader["sum"].ToString() + codes[(int)reader["senderCurrencyID"]];
                    r["Sender"] = reader["senderNumber"].ToString();
                    r["Recipient"] = reader["recepientNumber"].ToString();
                    dt.Rows.Add(r);
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            connection.Close();
        }
    }
}
