﻿namespace Server.Data.Entities;

public class Cart {
    public int Id { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = [];
}
