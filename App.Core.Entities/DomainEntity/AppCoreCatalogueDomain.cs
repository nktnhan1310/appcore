using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Entities.DomainEntity
{
    public class AppCoreCatalogueDomain : AppCoreDomain
    {
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
