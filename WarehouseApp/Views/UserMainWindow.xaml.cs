using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Views
{
    public partial class UserMainWindow : Window
    {
        private User currentUser;
        private DatabaseAdapter dbAdapter;

        public UserMainWindow(User user, DatabaseAdapter adapter)
        {
            InitializeComponent();
            currentUser = user;
            dbAdapter = adapter;
            LoadWarehouses();
        }

        private void LoadWarehouses()
        {
            dgWarehouses.ItemsSource = dbAdapter.GetAllWarehouses();
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
                if (dbAdapter.AddRentRequest(selectedWarehouse.Id, currentUser.Id, dialog.SpecialConditions))
                {
                    MessageBox.Show("Заявка на аренду отправлена на одобрение", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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