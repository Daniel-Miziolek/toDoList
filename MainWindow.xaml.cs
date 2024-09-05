using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace toDoList
{
    public partial class MainWindow : Window
    {
        private List<Task> tasks = [];
        private List<DateTime> dates = [];
        private readonly DatabaseHelper dbHelper = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadDate();
            comboBoxType.ItemsSource = Enum.GetValues(typeof(TaskType));
        }

        private void LoadDate()
        {
            tasks = dbHelper.GetTask();
            dataGrid.ItemsSource = tasks;

            dates = dbHelper.GetBlackoutDates();
            foreach (var date in dates)
            {
                calender.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (startDatePicker.SelectedDate == null || endDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Obie daty (początkowa i końcowa) muszą być wybrane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime startDate = startDatePicker.SelectedDate.Value.Date;
                DateTime endDate = endDatePicker.SelectedDate.Value.Date;

                if (startDate > endDate)
                {
                    MessageBox.Show("Data początkowa nie może być późniejsza niż data końcowa!", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool isBlackout = IsDateBlackoutDate(calender, startDate.Date);

                if (isBlackout)
                {
                    var result = MessageBox.Show("Ta data jest już zajęta. Czy chcesz dodać zadanie z tą datą?", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                AddTaskToDataGrid(startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTaskToDataGrid(DateTime startDate, DateTime endDate)
        {
            var task = new Task()
            {
                Title = textBoxTitle.Text,
                Description = textBoxDescription.Text,
                Type = (TaskType)Enum.Parse(typeof(TaskType), comboBoxType.Text),
                StartDate = startDate,
                EndDate = endDate,
            };

            dbHelper.InsertTask(task);
            tasks.Add(task);

            textBoxTitle.Text = "Title";
            textBoxDescription.Text = "Description";
            comboBoxType.Text = string.Empty;
            startDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;

            dbHelper.InsertBlackoutDate(startDate);
            calender.BlackoutDates.Add(new CalendarDateRange(startDate));

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = tasks;
        }

        private static bool IsDateBlackoutDate(System.Windows.Controls.Calendar calendar, DateTime date)
        {
            return calendar.BlackoutDates.Any(range => date >= range.Start && date <= range.End);
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxTitle.Text == "Title")
            {
                textBoxTitle.Text = "";
            }
        }

        private void GotFocus2(object sender, RoutedEventArgs e)
        {
            if (textBoxDescription.Text == "Description")
            {
                textBoxDescription.Text = "";
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Task selectedItem)
            {
                dbHelper.DeleteTask(selectedItem);
                tasks.Remove(selectedItem);

                dbHelper.DeleteBlackoutDate(selectedItem.StartDate);

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = tasks;

                calender.BlackoutDates.Remove(new CalendarDateRange(selectedItem.StartDate));
            }
            else
            {
                MessageBox.Show("Wybierz element do usunięcia", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Minimazing_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
    }
}
