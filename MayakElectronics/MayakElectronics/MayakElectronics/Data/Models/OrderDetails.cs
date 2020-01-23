using MayakElectronics.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Models
{
    public class OrderDetailCourier
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderCourierId { get; set; }

        public int ProductId { get; set; }

        public int Amount { get; set; }

        public double Price { get; set; }

        public virtual Product Product { get; set; }

        public virtual OrderCourier Order { get; set; }
    }

    public class OrderDetailPickUp
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderPickUpId { get; set; }

        public int ProductId { get; set; }

        public int Amount { get; set; }

        public double Price { get; set; }

        public virtual Product Product { get; set; }

        public virtual OrderPickUp Order { get; set; }
    }

    public class OrderDetailPost
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderPostId { get; set; }

        public int ProductId { get; set; }

        public int Amount { get; set; }

        public double Price { get; set; }

        public virtual Product Product { get; set; }

        public virtual OrderPost Order { get; set; }
    }
}
