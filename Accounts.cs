using MySql.Data.MySqlClient;
using System.Data;

namespace TOFI_project
{
    public partial class Accounts : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        MySqlConnection connection = new MySqlConnection(constring);

        static string[] codes;
        static int userID_;

        public Accounts(int userID)
        {
            InitializeComponent();
            userID_ = userID;
            DataTable dt = new DataTable();
            dt.Columns.Add("Number");
            dt.Columns.Add("Balance");
            dt.Columns.Add("Validity period");
            dt.Columns.Add("Currency");

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

            string query = "select * from BankAccount where userID = '" + userID + "';";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow r = dt.NewRow();
                    r["Number"] = reader["Number"].ToString();
                    r["Balance"] = reader["Balance"].ToString();
                    r["Validity period"] = reader["validity_period"].ToString().Split(" ")[0];
                    r["Currency"] = codes[(int)reader["currencyID"]];
                    dt.Rows.Add(r);
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateAccount createAccount = new CreateAccount(userID_);
            createAccount.Show();
            this.Close();
        }
    }
}
