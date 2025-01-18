using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvoicePayment
    {
        public Guid PaymentID { get; set; }
        public Payment Payment { get; set; }

        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }
    }
}
