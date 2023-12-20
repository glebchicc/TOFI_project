namespace TOFI_project
{
    partial class Transaction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label5 = new System.Windows.Forms.Label();
            this.recipientBox = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.moneyBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(341, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "Введите номер счета, на который хотите перевести средства";
            // 
            // recipientBox
            // 
            this.recipientBox.Location = new System.Drawing.Point(10, 29);
            this.recipientBox.Name = "recipientBox";
            this.recipientBox.Size = new System.Drawing.Size(235, 23);
            this.recipientBox.TabIndex = 18;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Без образования",
            "Начальное образование",
            "Основное общее",
            "Среднее общее",
            "Среднее профессиональное",
            "Высшее образование"});
            this.comboBox1.Location = new System.Drawing.Point(10, 88);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(235, 23);
            this.comboBox1.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(366, 15);
            this.label9.TabIndex = 30;
            this.label9.Text = "Выберите номер счета, с которого вы хотите перевести средства";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(369, 15);
            this.label1.TabIndex = 33;
            this.label1.Text = "Введите сумму переводимых средств (валюта счета отправителя)";
            // 
            // moneyBox
            // 
            this.moneyBox.Location = new System.Drawing.Point(8, 150);
            this.moneyBox.Name = "moneyBox";
            this.moneyBox.Size = new System.Drawing.Size(235, 23);
            this.moneyBox.TabIndex = 32;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 186);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 23);
            this.button1.TabIndex = 34;
            this.button1.Text = "Совершить перевод";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Transaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 226);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.moneyBox);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.recipientBox);
            this.Name = "Transaction";
            this.Text = "Transaction";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label5;
        private TextBox recipientBox;
        private ComboBox comboBox1;
        private Label label9;
        private Label label1;
        private TextBox moneyBox;
        private Button button1;
    }
}