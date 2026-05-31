using System.Linq;
using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Views
{
    public partial class UserMainWindow : Window
    {
        private User _currentUser;
        private IDatabaseFacade _facade;

        public UserMainWindow(User user, IDatabaseFacade facade)
        {
            InitializeComponent();
            _currentUser = user;
            _facade = facade;
            LoadData();
        }

        private void LoadData()
        {
            dgWarehouses.ItemsSource = _facade.Warehouses.GetAllWarehouses();
            LoadMyRequests();
        }

        private void LoadMyRequests()
        {
            var myRequests = _facade.Rents.GetUserRentRequests(_currentUser.Id);
            dgMyRequests.ItemsSource = myRequests;

            int pendingCount = myRequests.Count(r => r.Status == "pending");
            int approvedCount = myRequests.Count(r => r.Status == "approved");
            int rejectedCount = myRequests.Count(r => r.Status == "rejected");

            lblStatus.Text = $"Всего заявок: {myRequests.Count} | В ожидании: {pendingCount} | Одобрено: {approvedCount} | Отклонено: {rejectedCount}";
        }

        private void BtnCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            var selectedWarehouse = dgWarehouses.SelectedItem as Warehouse;
            if (selectedWarehouse == null)
            {
                MessageBox.Show("Выберите помещение из списка", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new CreateRequestDialog(selectedWarehouse);
            if (dialog.ShowDialog() == true)
            {
                if (_facade.Rents.AddRentRequest(selectedWarehouse.Id, _currentUser.Id, dialog.SpecialConditions))
                {
                    MessageBox.Show("Заявка на аренду отправлена на одобрение", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMyRequests();
                }
                else
                {
                    MessageBox.Show("Ошибка при создании заявки", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMyRequests();
            MessageBox.Show("Список заявок обновлён", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var warehouse = dgWarehouses.SelectedItem as Warehouse;
            if (warehouse != null)
            {
                var detailsWindow = new WarehouseDetailsWindow(warehouse);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите помещение", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}