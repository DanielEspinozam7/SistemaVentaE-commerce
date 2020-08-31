namespace FacturacionVentasWeb.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AppDbContex : DbContext
    {
        public AppDbContex()
            : base("name=AppDbContex")
        {
        }

        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Pagos> Pagos { get; set; }
        public virtual DbSet<VentaDTOs> VentaDTOs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VentaDTOs>()
                .HasMany(e => e.Items)
                .WithOptional(e => e.VentaDTOs)
                .HasForeignKey(e => e.VentaDTO_id);

            modelBuilder.Entity<VentaDTOs>()
                .HasMany(e => e.Pagos)
                .WithOptional(e => e.VentaDTOs)
                .HasForeignKey(e => e.VentaDTO_id);
        }
    }
}
