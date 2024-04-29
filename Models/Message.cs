using System;
using System.Collections.Generic;

namespace PropertyRentals.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderUserId { get; set; }

    public int ReceiverUserId { get; set; }

    public int ApartmentId { get; set; }

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime MessageDateTime { get; set; }

    public int StatusId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual User ReceiverUser { get; set; } = null!;

    public virtual User SenderUser { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
