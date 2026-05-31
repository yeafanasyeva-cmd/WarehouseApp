using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Views
{
    public partial class AddWarehouseDialog : Window
    {
        private IDatabaseFacade _facade;
        private Warehouse _editingWarehouse;

        public AddWarehouseDialog(IDatabaseFacade facade, Warehouse warehouse = null)
        {
            InitializeComponent();
            _facade = facade;
            _editingWarehouse = warehouse;

            if (_editingWarehouse != null)
            {
                Title = "Редактирование помещения";
                txtName.Text = _editingWarehouse.Name;
                txtAddress.Text = _editingWarehouse.Address;
                txtConditions.Text = _editingWarehouse.SpecialConditions;
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

            if (_editingWarehouse != null)
            {
                _editingWarehouse.Name = txtName.Text;
                _editingWarehouse.Address = txtAddress.Text;
                _editingWarehouse.SpecialConditions = txtConditions.Text;

                if (_facade.Warehouses.UpdateWarehouse(_editingWarehouse))
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

                if (_facade.Warehouses.AddWarehouse(warehouse))
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