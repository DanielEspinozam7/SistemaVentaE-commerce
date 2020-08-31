using Newtonsoft.Json;
using FacturacionVentasWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;

namespace FacturacionVentasWeb.Clases
{
    public class FacturacionVyV
    {

        public string ruta = "";
        public bool finalizar = false;



        public void CreaDocumentosElectronicos()

        {
            AppDbContex dbContex = new AppDbContex();

            string LogName = "Facturacion.log";
            string errores = "";


            try
            {

                dbContex = new AppDbContex();


                //buscar cabeceras sin folio y sin procesar

                List<VentaDTOs> pedidos = dbContex.VentaDTOs.Where(c => ((c.folio == null) || (c.pdf == null))).ToList<VentaDTOs>();

                Log("Pendientes: " + (Convert.ToString(pedidos.Count)), ruta + @"\Pendientes.txt");


                foreach (VentaDTOs pedidoReferencia in pedidos)
                {
                    if (finalizar) break;

                    try
                    {
                        VentaDTOs pedido = dbContex.VentaDTOs.Find(pedidoReferencia.id);

                        var cfg = Properties.Settings.Default;

                        pedido.FechaDocele = DateTime.Now;

                        if (pedido.rutEmpresa != cfg.RutEmisor)
                        {
                            pedido.ErrorDocElec = "Rut empresa no coincide con el configurado";
                            try
                            {
                                dbContex.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                errores += e.Message;
                            }
                            continue;
                        }


                        string mensajeValidacion = "";

                        if (!Validaciones(pedido, ref mensajeValidacion))
                        {

                            pedido.ErrorDocElec = mensajeValidacion;
                            try
                            {
                                dbContex.SaveChanges();
                                Log(mensajeValidacion, ruta + @"\ErrorValidacion.txt");
                            }
                            catch (Exception e)
                            {
                                errores += e.Message;
                            }
                            continue;

                            


                        }


                        try
                        {
                            dbContex.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            errores += e.Message;
                            continue;
                        }



                        if (string.IsNullOrEmpty(ruta)) return;



                        try
                        {
                            string test = ruta + @"\Pdfs";

                            File.WriteAllText(test + @"\test.pdf", "test");

                        }
                        catch (Exception)
                        {
                            return;
                        }

                        SettingAttribute st = new SettingAttribute();

                        string rutEmisor = cfg.RutEmisor;


                        ServiceFacturacionBES.SolicitarFolio ResponseFolio = new ServiceFacturacionBES.SolicitarFolio();



                        ServiceFacturacionBES.DTELocal WS = new ServiceFacturacionBES.DTELocal();//  ("DTELocalSoap", tienda.Facturador.HostWebService);
                        //WS.Url = tienda.Facturador.HostWebService;



                        int folio = 0;

                        if (pedido.folio != null)
                        {
                            folio = (int)pedido.folio;

                        }

                        //Si es cero se solicita un nuevo Folio y lo asigna al registro
                        if (folio == 0)
                        {
                            ResponseFolio = WS.Solicitar_Folio(rutEmisor, 39);
                            pedido.folio = ResponseFolio.Folio;

                            try
                            {
                                dbContex.SaveChanges();
                                Log("SaveChanges: OK ", "debug.txt");
                            }
                            catch (Exception e)
                            {
                                errores += e.Message;
                            }

                        }
                        else
                        {
                            //lleno fake response :)
                            ResponseFolio.Folio = folio;
                            ResponseFolio.Estatus = 0;
                            ResponseFolio.MsgEstatus = "folio Bkp: ";
                        }


                        
                        if (ResponseFolio.Estatus == 0)
                        {

                            try
                            {
                                string idVisual = "";


                                string email = "";
                                Log("Estatus 0 : " + ResponseFolio.MsgEstatus + Convert.ToString(ResponseFolio.Folio), ruta + @"\LastResponseFolio.txt");

                                //Entry entry = new Entry();

                                //entry = (Entry)JsonConvert.DeserializeObject<Entry>(entry.json, settings);


                                EnvioBOLETA txtBoleta = new EnvioBOLETA();
                                txtBoleta.version = "1.0";
                                //CARATULA
                                txtBoleta.SetDTE = new EnvioBOLETASetDTE();


                                txtBoleta.SetDTE.Caratula = new EnvioBOLETASetDTECaratula();
                                txtBoleta.SetDTE.Caratula.version = "1.0";

                                txtBoleta.SetDTE.Caratula.RutEmisor = rutEmisor;
                                txtBoleta.SetDTE.Caratula.RutEnvia = rutEmisor;

                                txtBoleta.SetDTE.Caratula.FchResol = DateTime.Now;//2018-09-13
                                txtBoleta.SetDTE.Caratula.NroResol = 80;
                                txtBoleta.SetDTE.Caratula.TmstFirmaEnv = DateTime.Now;
                                txtBoleta.SetDTE.Caratula.SubTotDTE = new EnvioBOLETASetDTECaratulaSubTotDTE();
                                txtBoleta.SetDTE.Caratula.SubTotDTE.TpoDTE = 39;
                                txtBoleta.SetDTE.Caratula.SubTotDTE.NroDTE = 1;

                                txtBoleta.SetDTE.DTE = new EnvioBOLETASetDTEDTE();
                                txtBoleta.SetDTE.DTE.version = "1.0";
                                txtBoleta.SetDTE.DTE.Documento = new EnvioBOLETASetDTEDTEDocumento();
                                //CABECERA
                                txtBoleta.SetDTE.DTE.Documento.Encabezado = new EnvioBOLETASetDTEDTEDocumentoEncabezado();

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc = new EnvioBOLETASetDTEDTEDocumentoEncabezadoIdDoc();

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.TipoDTE = 39;
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.Folio = Convert.ToInt32(ResponseFolio.Folio);

                                //se creara con fecha del pedido de Internet
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.FchEmis = Convert.ToDateTime(pedido.created_date);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.IndServicio = 3;

                                //EMISOR
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor = new EnvioBOLETASetDTEDTEDocumentoEncabezadoEmisor();

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.RUTEmisor = rutEmisor;
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.RznSocEmisor = Norm(cfg.RznSocEmisor);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.GiroEmisor = Norm(cfg.GiroEmisor);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.CdgSIISucur = cfg.CdgSIISucur;
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.DirOrigen = Norm(cfg.DirOrigen);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.CmnaOrigen = Norm(cfg.CmnaOrigen);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.CiudadOrigen = Norm(cfg.CiudadOrigen);

                                //RECEPTOR
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor = new EnvioBOLETASetDTEDTEDocumentoEncabezadoReceptor();

                                email = pedido.email_addr;

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.RUTRecep = validarRut(pedido.cust_rut);
                                //CARATULA
                                txtBoleta.SetDTE.Caratula.RutReceptor = txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.RUTRecep;

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.CdgIntRecep = "";
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.RznSocRecep = Norm(pedido.first_name + " " + pedido.last_name);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.Contacto = "";


                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.DirRecep = Norm(pedido.direccion1);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.CmnaRecep = Norm(pedido.comuna);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.CiudadRecep = Norm(pedido.ciudad);

                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.DirPostal = Norm(pedido.direccion1);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.CmnaPostal = Norm(pedido.comuna);
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Receptor.CiudadPostal = Norm(pedido.ciudad);




                                if (string.IsNullOrEmpty(email)) email = "SinMail";

                                //FIN ENCABEZADO

                                //DETALLE

                                //Despacho
                                //double dif = 0;
                                //dif = Math.Round(entry.gross) - Math.Round(entry.totalWithDiscount);

                                //if (dif > 0)
                                //{
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle = new EnvioBOLETASetDTEDTEDocumentoDetalle[entry.CheckoutItems.Count() + 1];
                                //}
                                //else
                                //{
                                txtBoleta.SetDTE.DTE.Documento.Detalle = new EnvioBOLETASetDTEDTEDocumentoDetalle[pedido.Items.Count()];
                                //}

                                int index = 0;


                                double ItemUnitario = 0;

                                foreach (var linea in pedido.Items)
                                {
                                    ItemUnitario = 0;
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index] = new EnvioBOLETASetDTEDTEDocumentoDetalle();

                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].NroLinDet = index + 1;

                                    ItemUnitario = linea.PriceLine / linea.qty;


                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem = new EnvioBOLETASetDTEDTEDocumentoDetalleCdgItem();
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem.TpoCodigo = "ALU";

                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem.VlrCodigo = Norm(linea.itemcode);
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].DscItem = "";
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].NmbItem = Norm(linea.descripcion);
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].QtyItem = linea.qty;
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].PrcItem = Convert.ToInt32(Math.Round(ItemUnitario));
                                    txtBoleta.SetDTE.DTE.Documento.Detalle[index].MontoItem = Convert.ToInt32(Math.Truncate(linea.PriceLine));

                                    index = index + 1;
                                }

                                // restar total items si total mayor agregar item po recargo despacho

                                ////calculo despacho


                                //if (dif > 0)
                                //{
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index] = new EnvioBOLETASetDTEDTEDocumentoDetalle();

                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].NroLinDet = index + 1;

                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem = new EnvioBOLETASetDTEDTEDocumentoDetalleCdgItem();
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem.TpoCodigo = "ALU";

                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].CdgItem.VlrCodigo = tienda.Facturador.DifDespachoOtrosCodigo;
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].DscItem = "";
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].NmbItem = tienda.Facturador.DifDespachoOtros; ;
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].QtyItem = 1;
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].PrcItem = Convert.ToInt32(Math.Truncate(dif));
                                //    txtBoleta.SetDTE.DTE.Documento.Detalle[index].MontoItem = Convert.ToInt32(Math.Truncate(dif));

                                //}


                                //66666666-6

                                //REF PAra PAGOS y caja vendedor
                                txtBoleta.SetDTE.DTE.Documento.Referencia = new EnvioBOLETASetDTEDTEDocumentoReferencia[3];

                                txtBoleta.SetDTE.DTE.Documento.Referencia[0] = new EnvioBOLETASetDTEDTEDocumentoReferencia();
                                txtBoleta.SetDTE.DTE.Documento.Referencia[0].NroLinRef = 1;
                                txtBoleta.SetDTE.DTE.Documento.Referencia[0].CodRef = "CAJA";
                                txtBoleta.SetDTE.DTE.Documento.Referencia[0].RazonRef = "";
                                txtBoleta.SetDTE.DTE.Documento.Referencia[0].CodVndor = " Venta Web " + pedido.id;

                                //idVisual = pedido.comments;


                                txtBoleta.SetDTE.DTE.Documento.Referencia[0].CodCaja = "Orden N: " + pedido.comments;



                                txtBoleta.SetDTE.DTE.Documento.Referencia[1] = new EnvioBOLETASetDTEDTEDocumentoReferencia();
                                txtBoleta.SetDTE.DTE.Documento.Referencia[1].NroLinRef = 2;
                                txtBoleta.SetDTE.DTE.Documento.Referencia[1].CodRef = "PAGO";

                                txtBoleta.SetDTE.DTE.Documento.Referencia[1].RazonRef = "TOTAL PAGOS: " + pedido.Pagos.Sum(p => p.monto);


                                txtBoleta.SetDTE.DTE.Documento.Referencia[1].CodVndor = "";
                                txtBoleta.SetDTE.DTE.Documento.Referencia[1].CodCaja = "";

                                txtBoleta.SetDTE.DTE.Documento.Referencia[2] = new EnvioBOLETASetDTEDTEDocumentoReferencia();
                                txtBoleta.SetDTE.DTE.Documento.Referencia[2].NroLinRef = 3;
                                txtBoleta.SetDTE.DTE.Documento.Referencia[2].CodRef = "MTO";
                                txtBoleta.SetDTE.DTE.Documento.Referencia[2].RazonRef = email;//cliente.email
                                txtBoleta.SetDTE.DTE.Documento.Referencia[2].CodVndor = "";
                                txtBoleta.SetDTE.DTE.Documento.Referencia[2].CodCaja = "";

                                //Totales
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Totales = new EnvioBOLETASetDTEDTEDocumentoEncabezadoTotales();
                                txtBoleta.SetDTE.DTE.Documento.Encabezado.Totales.MntTotal = Convert.ToInt32(pedido.total_amt);

                                txtBoleta.SetDTE.DTE.Documento.ID = "R" + txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.RUTEmisor + "T" + Convert.ToString(txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.TipoDTE) + "F" + Convert.ToString(ResponseFolio.Folio);
                                txtBoleta.SetDTE.ID = "R" + txtBoleta.SetDTE.DTE.Documento.Encabezado.Emisor.RUTEmisor + "T" + Convert.ToString(txtBoleta.SetDTE.DTE.Documento.Encabezado.IdDoc.TipoDTE) + "F" + Convert.ToString(ResponseFolio.Folio);

                                string temp = txtBoleta.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");



                                ServiceFacturacionBES.ProcesarTXTBoleta ResponseBOL = WS.Carga_TXTBoleta(temp, "XML");

                                if (ResponseBOL.Estatus == 0)
                                {


                                    string rutaPdfBkp = ruta + @"\Pdfs";



                                    pedido.folio = ResponseFolio.Folio;
                                    pedido.pdf = ResponseBOL.PDF;
                                    pedido.ErrorDocElec = "PROCESADO OK";
                                    pedido.FechaDocele = DateTime.Now;

                                    dbContex.SaveChanges();


                                    Log(temp, ruta + @"\LastBolXML.txt");

                                    //escribe PDF

                                    try
                                    {

                                        File.WriteAllBytes(rutaPdfBkp + @"\" + idVisual + "-BO-" + Convert.ToString(ResponseFolio.Folio) + ".pdf", ResponseBOL.PDF);

                                    }
                                    catch (Exception e)
                                    {


                                    }

                                }
                                else
                                {

                                    pedido.ErrorDocElec = "Estatus: " + Convert.ToString(ResponseBOL.Estatus) + " " + ResponseBOL.MsgEstatus;
                                    pedido.FechaDocele = DateTime.Now;
                                    dbContex.SaveChanges();
                                    Log("Estatus: " + Convert.ToString(ResponseBOL.Estatus) + " " + ResponseBOL.MsgEstatus, ruta + @"\LastResponseBOLError.txt");
                                    Log(temp, ruta + @"\BolXMLError.txt");

                                }




                            }

                            catch (Exception ex0)
                            {

                                Log("ex0:" + ex0.Message + ex0.ToString() + " " + ex0.Message + " " + ex0.InnerException, ruta + @"\ErrorTXTBoleta.txt");
                            }

                        }
                        else
                        {

                            Log(Convert.ToString(ResponseFolio.Estatus) + " " + ResponseFolio.MsgEstatus, ruta + @"\LastResponseFolio.txt");
                        }


                    }


                    catch (Exception ex1)
                    {



                        Log("ex1:" + ex1.Message + ex1.ToString(), ruta + @"\LastError.txt");
                    }


                }// Ciclo foreach



            }
            catch (Exception ex)
            {

                Log("ex: " + ex.ToString() + ex.Message, ruta + @"\LastError.txt");


            }

            finally
            {
                dbContex.Dispose();
            }

        }


        private string validarRut(string rut)
        {

            if (rut == null) return "66666666-6";
            if (rut == String.Empty) return "66666666-6";

            bool validacion = false;

            char dv = Convert.ToChar("0");
            string SrutAux = "";
            try
            {
                rut = rut.ToUpper();
                rut = rut.Replace(".", "");
                rut = rut.Replace("-", "");
                int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));
                SrutAux = Convert.ToString(rutAux);

                dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }
                if (dv == (char)(s != 0 ? s + 47 : 75))
                {
                    validacion = true;
                }
            }
            catch (Exception)
            {
                return "66666666-6";
            }

            if (validacion) return Convert.ToInt32(SrutAux) + "-" + Convert.ToString(dv); else return "66666666-6";


        }
        private bool Validaciones(VentaDTOs venta, ref string msg)
        {
            try
            {


                msg = "";

                if (venta.total_amt == 0) msg = msg + "Total no puede ser cero" + "\n\r";
                if (venta.Pagos.Sum(p => p.monto) == 0) msg = msg + "Pagos no puedeser cero" + "\n\r";
                if (venta.Pagos.Sum(p => p.monto) != venta.total_amt) msg = msg + "Pagos y total no coincide" + "\n\r";
                if (venta.Items.Sum(p => p.PriceLine) != venta.total_amt) msg = msg + "Suma de items con descuento y total no coincide" + "\n\r";

                if (msg == "")
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {

                msg = venta.id + "\n\r" + msg + ex.ToString();

                return false;
            }

        }

        private void Log(string sMsg, string archivo, bool append = false)
        {
            try
            {

                if (append == false)
                {
                    EliminaLog(archivo);
                };

                StreamWriter oSW = new StreamWriter((archivo), true);

                string scomando = String.Empty;

                oSW.WriteLine(((DateTime.Now + ("| " + sMsg))));

                oSW.Flush();

                oSW.Close();

            }
            catch (Exception err)
            {

                // MessageBox.Show(err.Message);

            }
        }

        private string Norm(string value)
        {

            if (value == null) return string.Empty;
            if (value == string.Empty) return value;

            try
            {
                return Regex.Replace(value.Normalize(NormalizationForm.FormD), @"[^A-Za-z0-9|.,_\-@ ]", string.Empty);
            }
            catch (Exception)
            {
                return value;
            }



        }
        private void EliminaLog(string archivo)
        {
            try
            {

                File.Delete(archivo);

            }
            catch (Exception err)
            {


            }

        }

    }

}