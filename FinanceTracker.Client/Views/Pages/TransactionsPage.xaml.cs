using FinanceTracker.Client.Services;
using FinanceTracker.Shared.DTOs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceTracker.Client.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : UserControl
    {
        private List<TransactionDto> _transactions = new();
        private List<CategoryDto> _categories = new();

        public TransactionsPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Загружаем категории для фильтра
                _categories = await ApiClient.GetAsync<List<CategoryDto>>("categories") ?? new();
                CategoryFilter.ItemsSource = _categories;

                // Добавляем пустой элемент "Все категории"
                _categories.Insert(0, new CategoryDto { Id = 0, Name = "Все категории" });
                CategoryFilter.SelectedIndex = 0;

                await LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private async Task LoadTransactionsAsync()
        {
            var url = "transactions";
            var queryParams = new List<string>();

            if (FromDatePicker.SelectedDate.HasValue)
                queryParams.Add($"from={FromDatePicker.SelectedDate.Value:yyyy-MM-dd}");

            if (ToDatePicker.SelectedDate.HasValue)
                queryParams.Add($"to={ToDatePicker.SelectedDate.Value:yyyy-MM-dd}");

            if (CategoryFilter.SelectedValue is int catId && catId > 0)
                queryParams.Add($"categoryId={catId}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            _transactions = await ApiClient.GetAsync<List<TransactionDto>>(url) ?? new();
            TransactionsGrid.ItemsSource = _transactions;
        }

        private async void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TransactionEditWindow(_categories.Where(c => c.Id > 0).ToList());
            if (dialog.ShowDialog() == true)
                await LoadTransactionsAsync();
        }

        private async void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            var selected = TransactionsGrid.SelectedItem as TransactionDto;
            if (selected == null)
            {
                MessageBox.Show("Выберите транзакцию");
                return;
            }

            var dialog = new TransactionEditWindow(
                _categories.Where(c => c.Id > 0).ToList(), selected);
            if (dialog.ShowDialog() == true)
                await LoadTransactionsAsync();
        }

        private async void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            var selected = TransactionsGrid.SelectedItem as TransactionDto;
            if (selected == null) return;

            if (MessageBox.Show("Удалить транзакцию?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    await ApiClient.DeleteAsync($"transactions/{selected.Id}");
                    await LoadTransactionsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private async void Filter_Changed(object sender, EventArgs e)
        {
            await LoadTransactionsAsync();
        }

        private async void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            CategoryFilter.SelectedIndex = 0;
            await LoadTransactionsAsync();
        }
    }
}
