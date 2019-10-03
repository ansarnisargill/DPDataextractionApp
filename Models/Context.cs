
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DPSQLDumpApp
{
    public class MainContext : DbContext
    {
        private const string connectionString = "Data Source=.;Initial Catalog=duo;User ID=sa;Password=StrongAdmin";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<House> Houses { get; set; }
    }
    public class House
    {
        [Key]
        public int ID { get; set; } = 0;
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string OriginalURL { get; set; }
        public string PicAddress { get; set; }
        public string PicAlt { get; set; }
        public string Price { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Tags { get; set; }
        public int Bedrooms { get; set; } = 0;
        public int Washrooms { get; set; } = 0;
        public string LivingSpaceArea { get; set; }
        public string LotDimensions { get; set; }
        public int OriginalId { get; set; }
        public DateTime PostingDate { get; set; }
        public decimal? PriceD { get; set; }

        public House()
        {

        }
    }
    public class HouseDTO
    {

        public string OriginalURL { get; set; }

        public string Price { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Bedrooms { get; set; } = 0;
        public int Washrooms { get; set; } = 0;
        public string LivingSpaceArea { get; set; }
        public string LotDimensions { get; set; }
        public DateTime PostingDate { get; set; }
    }

}