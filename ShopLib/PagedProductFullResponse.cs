﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopLib;
public class PagedProductFullResponse {
    public List<ProductFullModel> Items { get; set; } = new ();
    public int TotalCount { get; set; }
}
