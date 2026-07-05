using FinanceTracker.Client.Services;
using FinanceTracker.Client.Views.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceTracker.Client.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentArea.Content = new TransactionsPage();
        }

        private void Transactions_Click(object sender, RoutedEventArgs e)
            => ContentArea.Content = new TransactionsPage();

        private void Debts_Click(object sender, RoutedEventArgs e)
            => ContentArea.Content = new DebtsPage();

        private void Reports_Click(object sender, RoutedEventArgs e)
            => ContentArea.Content = new ReportPage();
    }
}
