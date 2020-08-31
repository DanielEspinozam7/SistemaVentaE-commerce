namespace GeneraXmlRpro.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Pagos
    {
        [Key]
        public int line_no { get; set; }

        [Required]
        public string tender_type { get; set; }

        public string crd_Name { get; set; }

        public string cuotas { get; set; }

        public string op_no { get; set; }

        public string auth_no { get; set; }

        public string comments { get; set; }

        public int monto { get; set; }

        [StringLength(128)]
        public string VentaDTO_id { get; set; }

        public virtual VentaDTOs VentaDTOs { get; set; }
    }
}
