using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace zapier_bham_dotnet.Data
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public int Qty { get; set; }

        public string SalesPerson { get; set; }
    }
}
