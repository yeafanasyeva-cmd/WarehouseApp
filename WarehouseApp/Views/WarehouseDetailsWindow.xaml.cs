using System.Windows;
using WarehouseApp.Models;

namespace WarehouseApp.Views
{
    public partial class WarehouseDetailsWindow : Window
    {
        public WarehouseDetailsWindow(Warehouse warehouse)
        {
            InitializeComponent();

            lblName.Text = warehouse.Name;
            lblAddress.Text = warehouse.Address ?? "Не указан";
            lblConditions.Text = warehouse.SpecialConditions ?? "Нет";
            lblDate.Text = warehouse.CreatedAt.ToString("dd.MM.yyyy");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}