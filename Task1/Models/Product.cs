using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Task1.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:(75))]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public int ProductionCost { get; set; }

        
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }


        public int Earning { get; set; }
    }
}
