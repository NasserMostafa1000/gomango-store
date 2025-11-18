using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataLayer.Entities
{
    public class ShippingDiscountCodes
    {
        public string promoCodeNumber { get; set; } = null!;
        public bool IsActive { get; set; }

    }
}
