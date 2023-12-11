using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Mail;

namespace TOFI_project
{
    public partial class Form1 : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";

        static string verCode = "";
        static int userID; 

        MySqlConnection connection = new MySqlConnection(constring);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection.Open();
            string query = "select * from user where email = '" + emailBox.Text + "';";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader["email"].ToString() == emailBox.Text)
                    {
                        if (reader["password_hash"].ToString() == passwordBox.Text)
                        {
                            Random random = new Random();
                            verCode = random.Next(100000, 999999).ToString();

                            try
                            {
                                MailMessage mail = new MailMessage();
                                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                                mail.From = new MailAddress("hlebchek.tofi@gmail.com");
                                mail.To.Add(emailBox.Text);
                                mail.Subject = "Двухфакторная аутентификация";
                                mail.Body = "Ваш код верификации для входа: " + verCode;
                                smtpClient.Port = 587;
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = new NetworkCredential("hlebchek.tofi@gmail.com", "lgxl xwio oziz fuqk");
                                smtpClient.EnableSsl = true;
                                smtpClient.Send(mail);

                                MessageBox.Show("Код отправлен на вашу почту", "Код", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                label3.Visible = true;
                                codeBox.Visible = true;
                                button2.Visible = true;
                                _ = int.TryParse(reader["userID"].ToString(), out userID);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Ошибка при отправке кода: " + ex.Message);
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show("Неправильный пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Данного аккаунта не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (codeBox.Text == verCode)
            {
                MessageBox.Show("Вход успешен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Main formMain = new Main(userID);
                formMain.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неправильный код верификации.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm2 = new();
            newForm2.Show();
        }
    }
}