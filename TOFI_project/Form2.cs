using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace TOFI_project
{
    public partial class Form2 : Form
    {
        static string server = "localhost";
        static string database = "TOFI";
        static string username = "root";
        static string password = "root";
        static string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";

        static string verCode = "";

        MySqlConnection connection = new MySqlConnection(constring);

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (emailBox.Text.Length > 0 && passwordBox.Text.Length > 0 && nameBox.Text.Length > 0 && surnameBox.Text.Length > 0 && patronimycBox.Text.Length > 0 && ageBox.Text.Length > 0 && incomeBox.Text.Length > 0 && comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && documentNumberBox.Text.Length > 0)
            {
                if (!Regex.IsMatch(emailBox.Text, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
                {
                    MessageBox.Show("Введите корректный email.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(passwordBox.Text, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$"))
                {
                    MessageBox.Show("Пароль должен содержать как минимум одну заглавную и строчную букву, одну цифру и один специальный символ, а также состоять минимум из 8 символов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(nameBox.Text, "^[\\p{L} \\.'\\-]+$") || !Regex.IsMatch(surnameBox.Text, "^[\\p{L} \\.'\\-]+$") || !Regex.IsMatch(patronimycBox.Text, "^[\\p{L} \\.'\\-]+$"))
                {
                    MessageBox.Show("Имя, фамилия и отчество может состоять только из букв, точек, апострофов, дефисов и пробелов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Regex.IsMatch(ageBox.Text, "^\\d+$"))
                {
                    MessageBox.Show("Возраст может состоять только из цифр.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if(!Regex.IsMatch(ageBox.Text, "^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$"))
                {
                    MessageBox.Show("Доход в год должен быть целым или вещественным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox4.SelectedIndex == 0 && !Regex.IsMatch(documentNumberBox.Text, "(MC|AB|BM|HB|KH|MP|KB|PP|SP|DP)\\d{7}"))
                {
                    MessageBox.Show("Указанный паспорт не является валидным.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (comboBox4.SelectedIndex == 1 && !Regex.IsMatch(ageBox.Text, "\\d{7}"))
                {
                    MessageBox.Show("Указанный ВНЖ не является валидным.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (comboBox4.SelectedIndex == 2 && !Regex.IsMatch(ageBox.Text, "[A-Z]{2}\\d{7}"))
                {
                    MessageBox.Show("Указанный ПМЖ не является валидным.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox4.SelectedIndex == 0 && comboBox5.SelectedIndex != 0)
                {
                    MessageBox.Show("Паспорт РБ и не гражданство РБ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (comboBox4.SelectedIndex != 0 && comboBox5.SelectedIndex == 0)
                {
                    MessageBox.Show("Гражданство РБ и вид на жительство.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Random random = new Random();
                verCode = random.Next(100000, 999999).ToString();

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("hlebchek.tofi@gmail.com");
                    mail.To.Add(emailBox.Text);
                    mail.Subject = "Двухфакторная аутентификация";
                    mail.Body = "Ваш код верификации для создания аккаунта: " + verCode;
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("hlebchek.tofi@gmail.com", "lgxl xwio oziz fuqk");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mail);

                    MessageBox.Show("Код отправлен на вашу почту", "Код", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    label11.Visible = true;
                    codeBox.Visible = true;
                    button2.Visible = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке кода: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Все поля формы должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (codeBox.Text == verCode)
            {
                try
                {
                    connection.Open();
                    string query = $"insert User(email, password_hash, name, surname, patronimyc, age, income_per_year, family, education, savings, document_type, document_number, citizenship) values ('{emailBox.Text}', '{passwordBox.Text}', '{nameBox.Text}', '{surnameBox.Text}', '{patronimycBox.Text}', {ageBox.Text}, {incomeBox.Text}, {comboBox1.SelectedIndex}, {comboBox2.SelectedIndex}, {comboBox3.SelectedIndex}, {comboBox4.SelectedIndex}, '{documentNumberBox.Text}', {comboBox5.SelectedIndex});";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    connection.Close();
                    this.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Ошибка при попытке зарегистрировать новый аккаунт.\n" + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Неправильный код верификации.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
