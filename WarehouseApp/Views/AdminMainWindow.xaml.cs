using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Views
{
    public partial class AdminMainWindow : Window
    {
        private User currentUser;
        private DatabaseAdapter dbAdapter;

        public AdminMainWindow(User user, DatabaseAdapter adapter)
        {
            InitializeComponent();
            currentUser = user;
            dbAdapter = adapter;
            LoadData();

            dgRequests.SelectionChanged += (s, e) => UpdateButtons();
            dgWarehouses.SelectionChanged += (s, e) => UpdateButtons();
        }

        private void LoadData()
        {
            dgRequests.ItemsSource = dbAdapter.GetPendingRequests();
            dgWarehouses.ItemsSource = dbAdapter.GetAllWarehouses();
        }

        private void UpdateButtons()
        {
            btnApprove.IsEnabled = dgRequests.SelectedItem != null;
            btnDeleteRequest.IsEnabled = dgRequests.SelectedItem != null;
            btnEdit.IsEnabled = dgWarehouses.SelectedItem != null;
            btnDelete.IsEnabled = dgWarehouses.SelectedItem != null;
        }

        private void BtnAddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWarehouseDialog(dbAdapter);
            if (addWindow.ShowDialog() == true)
            {
                MessageBox.Show("Помещение успешно добавлено в систему", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            var request = dgRequests.SelectedItem as RentHistory;
            if (request != null)
            {
                if (dbAdapter.ApproveRentRequest(request.Id, currentUser.Id))
                {
                    MessageBox.Show("Заявка одобрена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
            }
        }

        private void BtnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = dgRequests.SelectedItem as RentHistory;
            if (request != null)
            {
                if (MessageBox.Show("Отклонить заявку?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (dbAdapter.RejectRentRequest(request.Id))
                    {
                        MessageBox.Show("Заявка отклонена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var warehouse = dgWarehouses.SelectedItem as Warehouse;
            if (warehouse != null)
            {
                var editWindow = new AddWarehouseDialog(dbAdapter, warehouse);
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var warehouse = dgWarehouses.SelectedItem as Warehouse;
            if (warehouse != null)
            {
                if (MessageBox.Show($"Удалить помещение \"{warehouse.Name}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (dbAdapter.DeleteWarehouse(warehouse.Id))
                    {
                        MessageBox.Show("Помещение успешно удалено", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                }
            }
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            var visitsWindow = new VisitManagerWindow(dbAdapter);
            visitsWindow.ShowDialog();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}