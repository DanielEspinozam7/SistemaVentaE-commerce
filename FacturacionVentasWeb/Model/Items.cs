namespace FacturacionVentasWeb.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Items
    {
        [Key]
        public int line_no { get; set; }

        [Required]
        public string itemcode { get; set; }

        public int qty { get; set; }

        public double OriginalPriceLine { get; set; }

        public double PriceLine { get; set; }

        [Required]
        public string descripcion { get; set; }

        [StringLength(128)]
        public string VentaDTO_id { get; set; }

        public virtual VentaDTOs VentaDTOs { get; set; }
    }
}
