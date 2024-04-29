using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public string ApartmentCode { get; set; } = null!;

    public string PropertyCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Rent { get; set; }

    public int StatusId { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }
    public int FloorArea { get; set; }
    public int ParkingSpots { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public virtual Property PropertyCodeNavigation { get; set; } = null!;

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual Status Status { get; set; } = null!;
}
