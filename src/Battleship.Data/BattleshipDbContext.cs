using Battleship.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Data
{
    public class BattleshipDbContext : DbContext
    {
        public BattleshipDbContext(DbContextOptions<BattleshipDbContext> options) : base(options) { }

        public DbSet<CompletedGameSummary> CompletedGameSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompletedGameSummary>(entity =>
                entity.HasKey(x => x.GameId)
            );
        }
    }
}
