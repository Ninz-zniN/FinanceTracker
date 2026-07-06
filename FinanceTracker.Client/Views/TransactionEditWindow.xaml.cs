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
using System.Windows.Shapes;

namespace FinanceTracker.Client.Views
{
    /// <summary>
    /// Логика взаимодействия для TransactionEditWindow.xaml
    /// </summary>
    public partial class TransactionEditWindow : Window
    {
        private readonly List<CategoryDto> _categories;
        private readonly TransactionDto? _existing;

        public TransactionEditWindow(List<CategoryDto> categories, TransactionDto? existing = null)
        {
            InitializeComponent();
            _categories = categories;
            _existing = existing;

            CategoryBox.ItemsSource = _categories;

            if (_existing != null)
            {
                AmountBox.Text = _existing.Amount.ToString();
                DatePicker.SelectedDate = _existing.Date.ToDateTime(TimeOnly.MinValue);
                NoteBox.Text = _existing.Note;

                // Выбираем тип
                foreach (ComboBoxItem item in TypeBox.Items)
                {
                    if (item.Tag?.ToString() == _existing.Type)
                    {
                        TypeBox.SelectedItem = item;
                        break;
                    }
                }

                // Выбираем категорию
                CategoryBox.SelectedItem = _categories.FirstOrDefault(c => c.Id == _existing.CategoryId);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(AmountBox.Text, out var amount))
                {
                    MessageBox.Show("Введите корректную сумму");
                    return;
                }

                var selectedType = (ComboBoxItem)TypeBox.SelectedItem;
                var type = selectedType.Tag?.ToString() ?? "Expense";

                var selectedCategory = CategoryBox.SelectedItem as CategoryDto;
                if (selectedCategory == null)
                {
                    MessageBox.Show("Выберите категорию");
                    return;
                }

                if (_existing == null)
                {
                    // Создание
                    var dto = new CreateTransactionDto
                    {
                        Amount = amount,
                        Date = DateOnly.FromDateTime(DatePicker.SelectedDate ?? DateTime.Today),
                        Type = type,
                        Note = NoteBox.Text,
                        CategoryId = selectedCategory.Id
                    };
                    await ApiClient.PostAsync<CreateTransactionDto, TransactionDto>("transactions", dto);
                }
                else
                {
                    // Обновление
                    var dto = new UpdateTransactionDto
                    {
                        Amount = amount,
                        Date = DateOnly.FromDateTime(DatePicker.SelectedDate ?? DateTime.Today),
                        Type = type,
                        Note = NoteBox.Text,
                        CategoryId = selectedCategory.Id
                    };
                    await ApiClient.PutAsync<UpdateTransactionDto, TransactionDto>(
                        $"transactions/{_existing.Id}", dto);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
