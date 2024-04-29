using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public int TenantId { get; set; }

    public int ApartmentId { get; set; }

    public DateOnly RentalDate { get; set; }

    public DateOnly EndContractDate { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
