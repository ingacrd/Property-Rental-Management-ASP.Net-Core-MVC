using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRentals.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; } 

    public string Phone { get; set; }
    public string Photo { get; set; }

    [NotMapped] // This property is not mapped to the database
    public IFormFile PhotoFile { get; set; }
    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual ICollection<Message> MessageReceiverUsers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenderUsers { get; set; } = new List<Message>();

    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();

    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
