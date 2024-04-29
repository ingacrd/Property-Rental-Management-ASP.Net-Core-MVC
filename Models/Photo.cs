using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRentals.Models;

public partial class Photo
{
    public int PhotoId { get; set; }

    [NotMapped] // This property is not mapped to the database
    public IFormFile PhotoFile { get; set; }
    public string PhotoLink { get; set; } = null!;

    public int? ApartmentId { get; set; }

    public virtual Apartment? Apartment { get; set; }
}
