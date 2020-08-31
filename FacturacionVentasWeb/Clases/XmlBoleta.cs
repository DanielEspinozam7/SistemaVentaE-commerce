using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FacturacionVentasWeb.Clases
{
    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sii.cl/SiiDte", IsNullable = false)]
    public partial class EnvioBOLETA
    {

        private EnvioBOLETASetDTE setDTEField;

        private string versionField;

        /// <comentarios/>
        public EnvioBOLETASetDTE SetDTE
        {
            get
            {
                return this.setDTEField;
            }
            set
            {
                this.setDTEField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTE
    {

        private EnvioBOLETASetDTECaratula caratulaField;

        private EnvioBOLETASetDTEDTE dTEField;

        private string idField;

        /// <comentarios/>
        public EnvioBOLETASetDTECaratula Caratula
        {
            get
            {
                return this.caratulaField;
            }
            set
            {
                this.caratulaField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTEDTE DTE
        {
            get
            {
                return this.dTEField;
            }
            set
            {
                this.dTEField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTECaratula
    {

        private string rutEmisorField;

        private string rutEnviaField;

        private string rutReceptorField;

        private System.DateTime fchResolField;

        private int nroResolField;

        private System.DateTime tmstFirmaEnvField;

        private EnvioBOLETASetDTECaratulaSubTotDTE subTotDTEField;

        private string versionField;

        /// <comentarios/>
        public string RutEmisor
        {
            get
            {
                return this.rutEmisorField;
            }
            set
            {
                this.rutEmisorField = value;
            }
        }

        /// <comentarios/>
        public string RutEnvia
        {
            get
            {
                return this.rutEnviaField;
            }
            set
            {
                this.rutEnviaField = value;
            }
        }

        /// <comentarios/>
        public string RutReceptor
        {
            get
            {
                return this.rutReceptorField;
            }
            set
            {
                this.rutReceptorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime FchResol
        {
            get
            {
                return this.fchResolField;
            }
            set
            {
                this.fchResolField = value;
            }
        }

        /// <comentarios/>
        public int NroResol
        {
            get
            {
                return this.nroResolField;
            }
            set
            {
                this.nroResolField = value;
            }
        }

        /// <comentarios/>
        public System.DateTime TmstFirmaEnv
        {
            get
            {
                return this.tmstFirmaEnvField;
            }
            set
            {
                this.tmstFirmaEnvField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTECaratulaSubTotDTE SubTotDTE
        {
            get
            {
                return this.subTotDTEField;
            }
            set
            {
                this.subTotDTEField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTECaratulaSubTotDTE
    {

        private int tpoDTEField;

        private int nroDTEField;

        /// <comentarios/>
        public int TpoDTE
        {
            get
            {
                return this.tpoDTEField;
            }
            set
            {
                this.tpoDTEField = value;
            }
        }

        /// <comentarios/>
        public int NroDTE
        {
            get
            {
                return this.nroDTEField;
            }
            set
            {
                this.nroDTEField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTE
    {

        private EnvioBOLETASetDTEDTEDocumento documentoField;

        private string versionField;

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumento Documento
        {
            get
            {
                return this.documentoField;
            }
            set
            {
                this.documentoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumento
    {

        private EnvioBOLETASetDTEDTEDocumentoEncabezado encabezadoField;

        private EnvioBOLETASetDTEDTEDocumentoDetalle[] detalleField;

        private EnvioBOLETASetDTEDTEDocumentoReferencia[] referenciaField;

        private string idField;

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoEncabezado Encabezado
        {
            get
            {
                return this.encabezadoField;
            }
            set
            {
                this.encabezadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("Detalle")]
        public EnvioBOLETASetDTEDTEDocumentoDetalle[] Detalle
        {
            get
            {
                return this.detalleField;
            }
            set
            {
                this.detalleField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("Referencia")]
        public EnvioBOLETASetDTEDTEDocumentoReferencia[] Referencia
        {
            get
            {
                return this.referenciaField;
            }
            set
            {
                this.referenciaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoEncabezado
    {

        private EnvioBOLETASetDTEDTEDocumentoEncabezadoIdDoc idDocField;

        private EnvioBOLETASetDTEDTEDocumentoEncabezadoEmisor emisorField;

        private EnvioBOLETASetDTEDTEDocumentoEncabezadoReceptor receptorField;

        private EnvioBOLETASetDTEDTEDocumentoEncabezadoTotales totalesField;

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoEncabezadoIdDoc IdDoc
        {
            get
            {
                return this.idDocField;
            }
            set
            {
                this.idDocField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoEncabezadoEmisor Emisor
        {
            get
            {
                return this.emisorField;
            }
            set
            {
                this.emisorField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoEncabezadoReceptor Receptor
        {
            get
            {
                return this.receptorField;
            }
            set
            {
                this.receptorField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoEncabezadoTotales Totales
        {
            get
            {
                return this.totalesField;
            }
            set
            {
                this.totalesField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoEncabezadoIdDoc
    {

        private int tipoDTEField;

        private int folioField;

        private System.DateTime fchEmisField;

        private int indServicioField;

        /// <comentarios/>
        public int TipoDTE
        {
            get
            {
                return this.tipoDTEField;
            }
            set
            {
                this.tipoDTEField = value;
            }
        }

        /// <comentarios/>
        public int Folio
        {
            get
            {
                return this.folioField;
            }
            set
            {
                this.folioField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime FchEmis
        {
            get
            {
                return this.fchEmisField;
            }
            set
            {
                this.fchEmisField = value;
            }
        }

        /// <comentarios/>
        public int IndServicio
        {
            get
            {
                return this.indServicioField;
            }
            set
            {
                this.indServicioField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoEncabezadoEmisor
    {

        private string rUTEmisorField;

        private string rznSocEmisorField;

        private string giroEmisorField;

        private int cdgSIISucurField;

        private string dirOrigenField;

        private string cmnaOrigenField;

        private string ciudadOrigenField;

        /// <comentarios/>
        public string RUTEmisor
        {
            get
            {
                return this.rUTEmisorField;
            }
            set
            {
                this.rUTEmisorField = value;
            }
        }

        /// <comentarios/>
        public string RznSocEmisor
        {
            get
            {
                return this.rznSocEmisorField;
            }
            set
            {
                this.rznSocEmisorField = value;
            }
        }

        /// <comentarios/>
        public string GiroEmisor
        {
            get
            {
                return this.giroEmisorField;
            }
            set
            {
                this.giroEmisorField = value;
            }
        }

        /// <comentarios/>
        public int CdgSIISucur
        {
            get
            {
                return this.cdgSIISucurField;
            }
            set
            {
                this.cdgSIISucurField = value;
            }
        }

        /// <comentarios/>
        public string DirOrigen
        {
            get
            {
                return this.dirOrigenField;
            }
            set
            {
                this.dirOrigenField = value;
            }
        }

        /// <comentarios/>
        public string CmnaOrigen
        {
            get
            {
                return this.cmnaOrigenField;
            }
            set
            {
                this.cmnaOrigenField = value;
            }
        }

        /// <comentarios/>
        public string CiudadOrigen
        {
            get
            {
                return this.ciudadOrigenField;
            }
            set
            {
                this.ciudadOrigenField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoEncabezadoReceptor
    {

        private string rUTRecepField;

        private string cdgIntRecepField;

        private string rznSocRecepField;

        private string contactoField;

        private string dirRecepField;

        private string cmnaRecepField;

        private string ciudadRecepField;

        private string dirPostalField;

        private string cmnaPostalField;

        private string ciudadPostalField;

        /// <comentarios/>
        public string RUTRecep
        {
            get
            {
                return this.rUTRecepField;
            }
            set
            {
                this.rUTRecepField = value;
            }
        }

        /// <comentarios/>
        public string CdgIntRecep
        {
            get
            {
                return this.cdgIntRecepField;
            }
            set
            {
                this.cdgIntRecepField = value;
            }
        }

        /// <comentarios/>
        public string RznSocRecep
        {
            get
            {
                return this.rznSocRecepField;
            }
            set
            {
                this.rznSocRecepField = value;
            }
        }

        /// <comentarios/>
        public string Contacto
        {
            get
            {
                return this.contactoField;
            }
            set
            {
                this.contactoField = value;
            }
        }

        /// <comentarios/>
        public string DirRecep
        {
            get
            {
                return this.dirRecepField;
            }
            set
            {
                this.dirRecepField = value;
            }
        }

        /// <comentarios/>
        public string CmnaRecep
        {
            get
            {
                return this.cmnaRecepField;
            }
            set
            {
                this.cmnaRecepField = value;
            }
        }

        /// <comentarios/>
        public string CiudadRecep
        {
            get
            {
                return this.ciudadRecepField;
            }
            set
            {
                this.ciudadRecepField = value;
            }
        }

        /// <comentarios/>
        public string DirPostal
        {
            get
            {
                return this.dirPostalField;
            }
            set
            {
                this.dirPostalField = value;
            }
        }

        /// <comentarios/>
        public string CmnaPostal
        {
            get
            {
                return this.cmnaPostalField;
            }
            set
            {
                this.cmnaPostalField = value;
            }
        }

        /// <comentarios/>
        public string CiudadPostal
        {
            get
            {
                return this.ciudadPostalField;
            }
            set
            {
                this.ciudadPostalField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoEncabezadoTotales
    {

        private int mntTotalField;

        /// <comentarios/>
        public int MntTotal
        {
            get
            {
                return this.mntTotalField;
            }
            set
            {
                this.mntTotalField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoDetalle
    {

        private int nroLinDetField;

        private EnvioBOLETASetDTEDTEDocumentoDetalleCdgItem cdgItemField;

        private string nmbItemField;

        private string dscItemField;

        private int qtyItemField;

        private int prcItemField;

        private int montoItemField;

        /// <comentarios/>
        public int NroLinDet
        {
            get
            {
                return this.nroLinDetField;
            }
            set
            {
                this.nroLinDetField = value;
            }
        }

        /// <comentarios/>
        public EnvioBOLETASetDTEDTEDocumentoDetalleCdgItem CdgItem
        {
            get
            {
                return this.cdgItemField;
            }
            set
            {
                this.cdgItemField = value;
            }
        }

        /// <comentarios/>
        public string NmbItem
        {
            get
            {
                return this.nmbItemField;
            }
            set
            {
                this.nmbItemField = value;
            }
        }

        /// <comentarios/>
        public string DscItem
        {
            get
            {
                return this.dscItemField;
            }
            set
            {
                this.dscItemField = value;
            }
        }

        /// <comentarios/>
        public int QtyItem
        {
            get
            {
                return this.qtyItemField;
            }
            set
            {
                this.qtyItemField = value;
            }
        }

        /// <comentarios/>
        public int PrcItem
        {
            get
            {
                return this.prcItemField;
            }
            set
            {
                this.prcItemField = value;
            }
        }

        /// <comentarios/>
        public int MontoItem
        {
            get
            {
                return this.montoItemField;
            }
            set
            {
                this.montoItemField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoDetalleCdgItem
    {

        private string tpoCodigoField;

        private string vlrCodigoField;

        /// <comentarios/>
        public string TpoCodigo
        {
            get
            {
                return this.tpoCodigoField;
            }
            set
            {
                this.tpoCodigoField = value;
            }
        }

        /// <comentarios/>
        public string VlrCodigo
        {
            get
            {
                return this.vlrCodigoField;
            }
            set
            {
                this.vlrCodigoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sii.cl/SiiDte")]
    public partial class EnvioBOLETASetDTEDTEDocumentoReferencia
    {

        private int nroLinRefField;

        private string codRefField;

        private string razonRefField;

        private string codVndorField;

        private string codCajaField;

        /// <comentarios/>
        public int NroLinRef
        {
            get
            {
                return this.nroLinRefField;
            }
            set
            {
                this.nroLinRefField = value;
            }
        }

        /// <comentarios/>
        public string CodRef
        {
            get
            {
                return this.codRefField;
            }
            set
            {
                this.codRefField = value;
            }
        }

        /// <comentarios/>
        public string RazonRef
        {
            get
            {
                return this.razonRefField;
            }
            set
            {
                this.razonRefField = value;
            }
        }

        /// <comentarios/>
        public string CodVndor
        {
            get
            {
                return this.codVndorField;
            }
            set
            {
                this.codVndorField = value;
            }
        }

        /// <comentarios/>
        public string CodCaja
        {
            get
            {
                return this.codCajaField;
            }
            set
            {
                this.codCajaField = value;
            }
        }
    }
}
