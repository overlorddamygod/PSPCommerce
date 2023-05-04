using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSPCommerce.Models
{
	public class BaseModel
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

