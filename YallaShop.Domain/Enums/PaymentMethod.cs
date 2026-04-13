using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Entites;

namespace YallaShop.Domain.Enums
{
    public enum PaymentMethod
    {
        None = 0,
        Card = 1,
        PayPal = 2,
        CashOnDelivery = 3
    }
}
