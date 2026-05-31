using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Views
{
    public partial class VisitManagerWindow : Window
    {
        private DatabaseAdapter dbAdapter;

        public VisitManagerWindow(DatabaseAdapter adapter)
        {
            InitializeComponent();
            dbAdapter = adapter;
            LoadVisits();
            LoadActiveRents();
        }

        private void LoadVisits()
        {
            dgVisits.ItemsSource = dbAdapter.GetAllVisits();
        }

        private void LoadActiveRents()
        {
            var rents = dbAdapter.GetActiveRents();
            cmbRents.ItemsSource = rents;
            if (rents.Any())
                cmbRents.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRents.SelectedItem == null)
            {
                MessageBox.Show("Выберите аренду", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new Window
            {
                Title = "Добавление посещения",
                Width = 400,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var grid = new Grid { Margin = new Thickness(10) };

            for (int i = 0; i < 9; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var txtFirstName = new TextBox { Margin = new Thickness(0, 5, 0, 5), Height = 30 };
            var txtLastName = new TextBox { Margin = new Thickness(0, 5, 0, 5), Height = 30 };
            var txtCompany = new TextBox { Margin = new Thickness(0, 5, 0, 5), Height = 30 };
            var txtCarNumber = new TextBox { Margin = new Thickness(0, 5, 0, 5), Height = 30 };

            var lblFirstName = new TextBlock { Text = "Имя:", Margin = new Thickness(0, 5, 0, 5) };
            Grid.SetRow(lblFirstName, 0);
            grid.Children.Add(lblFirstName);

            Grid.SetRow(txtFirstName, 1);
            grid.Children.Add(txtFirstName);

            var lblLastName = new TextBlock { Text = "Фамилия:", Margin = new Thickness(0, 5, 0, 5) };
            Grid.SetRow(lblLastName, 2);
            grid.Children.Add(lblLastName);

            Grid.SetRow(txtLastName, 3);
            grid.Children.Add(txtLastName);

            var lblCompany = new TextBlock { Text = "Компания:", Margin = new Thickness(0, 5, 0, 5) };
            Grid.SetRow(lblCompany, 4);
            grid.Children.Add(lblCompany);

            Grid.SetRow(txtCompany, 5);
            grid.Children.Add(txtCompany);

            var lblCarNumber = new TextBlock { Text = "Номер машины:", Margin = new Thickness(0, 5, 0, 5) };
            Grid.SetRow(lblCarNumber, 6);
            grid.Children.Add(lblCarNumber);

            Grid.SetRow(txtCarNumber, 7);
            grid.Children.Add(txtCarNumber);

            var btnSave = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(5) };
            var btnCancel = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5) };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            stackPanel.Children.Add(btnSave);
            stackPanel.Children.Add(btnCancel);

            Grid.SetRow(stackPanel, 8);
            grid.Children.Add(stackPanel);

            dialog.Content = grid;

            btnSave.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Заполните имя и фамилию", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedRent = cmbRents.SelectedItem as RentHistory;
                if (selectedRent != null)
                {
                    if (dbAdapter.AddVisit(selectedRent.Id, txtFirstName.Text, txtLastName.Text, txtCompany.Text, txtCarNumber.Text))
                    {
                        MessageBox.Show("Посещение успешно добавлено", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisits();
                        dialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            };

            btnCancel.Click += (s, args) => dialog.Close();

            dialog.ShowDialog();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var visit = dgVisits.SelectedItem as Visit;
            if (visit != null)
            {
                if (MessageBox.Show("Удалить посещение?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (dbAdapter.DeleteVisit(visit.Id))
                    {
                        MessageBox.Show("Посещение удалено", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisits();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите посещение", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}