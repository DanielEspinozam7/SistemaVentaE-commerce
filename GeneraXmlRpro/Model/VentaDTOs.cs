namespace GeneraXmlRpro.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VentaDTOs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VentaDTOs()
        {
            Items = new HashSet<Items>();
            Pagos = new HashSet<Pagos>();
        }

        public string id { get; set; }

        public string rutEmpresa { get; set; }

        public int sbs_no { get; set; }

        public int store_no { get; set; }

        public int invc_type { get; set; }

        public DateTime created_date { get; set; }

        public int total_amt { get; set; }

        public string comments { get; set; }

        public string cust_rut { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email_addr { get; set; }

        public string phone1 { get; set; }

        public string phone2 { get; set; }

        public string direccion1 { get; set; }

        public string direccion2 { get; set; }

        public string comuna { get; set; }

        public string ciudad { get; set; }

        public string region { get; set; }

        public string zip { get; set; }

        public int? folio { get; set; }

        public byte[] pdf { get; set; }

        public string pdfLink { get; set; }

        public string ErrorDocElec { get; set; }

        public string ErrorInvoice { get; set; }

        [StringLength(20)]
        public string InvoiceSid { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public DateTime? FechaDocele { get; set; }

        public DateTime? FechaProcRpro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Items> Items { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pagos> Pagos { get; set; }
    }
}
