using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Views
{
    public partial class AddWarehouseDialog : Window
    {
        private DatabaseAdapter dbAdapter;
        private Warehouse editingWarehouse;

        public AddWarehouseDialog(DatabaseAdapter adapter, Warehouse warehouse = null)
        {
            InitializeComponent();
            dbAdapter = adapter;
            editingWarehouse = warehouse;

            if (editingWarehouse != null)
            {
                Title = "Редактирование помещения";
                txtName.Text = editingWarehouse.Name;
                txtAddress.Text = editingWarehouse.Address;
                txtConditions.Text = editingWarehouse.SpecialConditions;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название помещения обязательно", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (editingWarehouse != null)
            {
                editingWarehouse.Name = txtName.Text;
                editingWarehouse.Address = txtAddress.Text;
                editingWarehouse.SpecialConditions = txtConditions.Text;

                if (dbAdapter.UpdateWarehouse(editingWarehouse))
                {
                    MessageBox.Show("Информация о помещении успешно изменена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при изменении", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var warehouse = new Warehouse
                {
                    Name = txtName.Text,
                    Address = txtAddress.Text,
                    SpecialConditions = txtConditions.Text
                };

                if (dbAdapter.AddWarehouse(warehouse))
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}