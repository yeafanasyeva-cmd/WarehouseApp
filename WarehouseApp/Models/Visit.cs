using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseApp.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int RentHistoryId { get; set; }
        public string WarehouseName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CarNumber { get; set; }
        public DateTime VisitDate { get; set; }
    }
}