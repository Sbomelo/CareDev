﻿using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        // Navigation Properties
        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();
        public virtual ICollection<Bed> Beds { get; set; } = new List<Bed>();

    }
}
