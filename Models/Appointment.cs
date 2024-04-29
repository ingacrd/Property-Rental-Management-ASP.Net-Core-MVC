using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int TenantId { get; set; }

    public int ManagerId { get; set; }

    public int ApartmentId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Manager Manager { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
