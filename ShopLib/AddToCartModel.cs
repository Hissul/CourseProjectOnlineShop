﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopLib;

public class AddToCartModel {
    public string UserId { get; set; } = null!;
    public int ProductId { get; set; }
}
