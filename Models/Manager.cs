using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual User User { get; set; } = null!;
}
