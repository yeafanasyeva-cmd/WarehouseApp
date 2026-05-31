using System.Windows;
using WarehouseApp.Models;

namespace WarehouseApp.Views
{
    public partial class CreateRequestDialog : Window
    {
        public Warehouse SelectedWarehouse { get; private set; }
        public string SpecialConditions { get; private set; }

        public CreateRequestDialog(Warehouse warehouse)
        {
            InitializeComponent();
            SelectedWarehouse = warehouse;
            DataContext = this;
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SpecialConditions = txtSpecialConditions.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}