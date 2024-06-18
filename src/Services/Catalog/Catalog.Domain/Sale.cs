using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain
{
    public class Sale
    {
        public Guid Id { get; set; }

        public decimal FinalSalePrice { get; set; }

        public Guid PlateId { get; set; }

        [ForeignKey("PlateId")]
        public Plate Plate { get; set; }

        public DateTime SaleDate { get; set; }
    }
}
