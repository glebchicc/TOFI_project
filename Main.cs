﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TOFI_project
{
    public partial class Main : Form
    {
        static int userID;

        public Main(int userID_)
        {
            userID = userID_;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Accounts formAccounts = new Accounts(userID);
            formAccounts.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Transaction transaction = new Transaction(userID);
            transaction.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MyCredits credit = new MyCredits(userID);
            credit.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Credit credit = new Credit(userID);
            credit.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TransactionHistory transactionHistory = new TransactionHistory(userID);
            transactionHistory.ShowDialog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}