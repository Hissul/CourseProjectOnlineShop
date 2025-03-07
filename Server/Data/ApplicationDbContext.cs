﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) {
    }
}
