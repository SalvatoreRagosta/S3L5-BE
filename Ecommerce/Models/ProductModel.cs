using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        [AllowHtml]
        public string content { get; set; }
        public string excerpt { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH':'mm':'ss}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public int status { get; set; }
        public string author { get; set; }
        public string images { get; set; }
        public string featured_image { get; set; }
    }

    public class ProductDBContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}