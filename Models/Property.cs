using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Property
{
    public string PropertyCode { get; set; } = null!;

    public int OwnerId { get; set; }

    public int ManagerId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual Manager Manager { get; set; } = null!;

    public virtual Owner Owner { get; set; } = null!;
}
