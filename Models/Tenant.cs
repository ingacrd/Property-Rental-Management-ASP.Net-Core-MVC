using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Tenant
{
    public int TenantId { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual User User { get; set; } = null!;
}
