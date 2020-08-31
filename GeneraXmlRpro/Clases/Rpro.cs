using GeneraXmlRpro.DataSetRproTableAdapters;
using GeneraXmlRpro.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneraXmlRpro.Clases
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            value = value.Norm();

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string Norm(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            value = value.Norm();

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string Norm(this string value)
        {

            if (string.IsNullOrEmpty(value)) return value;

            try
            {
                return Regex.Replace(value.Normalize(NormalizationForm.FormD), @"[^A-Za-z0-9|.,_\-@ ]", string.Empty);
            }
            catch (Exception)
            {
                return value;
            }



        }
    }
    public class Rpro
    {
        string ruta = "";
        public Rpro(string _ruta)
        {
            ruta = _ruta;
        }


        public void GeneraXmlsINVOICE()
        {
            string errores = "";
            AppDbContex appDbContex = new AppDbContex();
            DataSetRproTableAdapters.VYVARTICULOSTableAdapter adapter = new DataSetRproTableAdapters.VYVARTICULOSTableAdapter();



            try
            {
                string xmlDoc = "";
                string sbs = "";
                string store = "";
                string shiptoStoreNo = "";
                string AlusError = "";
                string invoiceSid = "";
                string invoiceNo = "";

                //Cliente
                string CustXml = "";
                string CustID = "";
                string CustSid = "";
                string rut = ""; //normalizado 
                string orderRut = "";
                string ArchivoLog = "";
                //acumula clientes nuevos;
                string xmlCustomer = "";




                DateTime created_date = DateTime.Now;




                DataSetRpro.VYVARTICULOSDataTable ItemTable = new DataSetRpro.VYVARTICULOSDataTable();

                List<VentaDTOs> VentasList = appDbContex.VentaDTOs.Where(e => (e.folio != null
                                                                        && e.InvoiceSid == null
                                                                        && e.pdf != null)).Take(20).ToList<VentaDTOs>();

                Log("Pendientes: " + (Convert.ToString(VentasList.Count)), "Pendientes.txt");
                Log("Pendientes: ", "debug.txt");
                var cfg = Properties.Settings.Default;

                foreach (VentaDTOs pedidoRef in VentasList)
                {

                    VentaDTOs pedido = appDbContex.VentaDTOs.Find(pedidoRef.id);
                    ArchivoLog = pedido.id+"-"+ pedido.folio.ToString()+".txt";
                    EliminaLog(ArchivoLog);
                    pedido.FechaDocele = DateTime.Now;

                    try
                    {
                        appDbContex.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        errores += e.Message;
                        continue;
                    }


                    string createdDate = Convert.ToDateTime(pedido.created_date).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    string xml = InvoiceCAB();

                    try
                    {
                        //parte limpio

                        Log("Verifica archivos pendientes en:  " + cfg.Invoice_CarpetaStation + @"\INVOICE999.xml", ArchivoLog, false);

                        //Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                        //{

                        if (File.Exists(cfg.Invoice_CarpetaStation + @"\INVOICE999.xml"))
                        {
                            File.WriteAllText(ruta + @"\Info.txt", "Existen Xmls pendientes en carpeta " + cfg.Invoice_CarpetaStation + @"\INVOICE999.xml", Encoding.UTF8);

                            Log("Existen Xmls pendientes en carpeta " + cfg.Invoice_CarpetaStation + @"\INVOICE999.xml", ArchivoLog, true);

                            continue;
                        }

                        //});



                        Log("Invoice Cabecera", ArchivoLog, true);

                        int iSbs = Convert.ToInt32(pedido.sbs_no);
                        int iStore = Convert.ToInt32(pedido.store_no);

                        sbs = iSbs.ToString("00#");
                        store = iStore.ToString("00#");


                        //sSID:='1'+'0000'+ AdoQuery.FieldByname('invc_no').AsStng;

                        //  SBS   Store  TIpoDoc  1=Boleta 2=fatura etc... Folio 12 digitos
                        //  001   001     1 


                        invoiceSid = sbs + store + "1" + Convert.ToString(pedido.folio).Trim().PadLeft(12, '0');
                        invoiceNo = Convert.ToString(pedido.folio).Trim().PadLeft(9, '0');// item.folio.Trim();//. code.PadLeft(14, '0');

                        shiptoStoreNo = store;


                        //CLiente
                        try
                        {
                            string dv = "";
                            CustID = validarRut(pedido.cust_rut, out dv);

                            ////Rut Completo 

                            rut = "";

                            if (!string.IsNullOrEmpty(CustID))
                            {
                                rut = CustID + "-" + dv;

                                if (dv.ToUpper() == "K")
                                {
                                    //-3204210179006066692
                                    //      11000140055692
                                    CustSid = "11000" + CustID + "0";
                                }
                                else
                                {
                                    CustSid = "11000" + CustID + dv;
                                }

                                var queriesTableAdapter = new ClientesTableAdapter();

                                var ClientesDb = queriesTableAdapter.GetData(Convert.ToDecimal(sbs), rut);

                                CustXml = "";

                                if (ClientesDb.Count() == 0)
                                {
                                    // Si no existe Sid para ese rut genera xml de creacion de cliente
                                    CustXml = CustomerXML();

                                    CustXml = CustXml.Replace("{cust_sid}", CustSid);
                                    CustXml = CustXml.Replace("{cust_id}", CustID);
                                    CustXml = CustXml.Replace("{sbs_no}", sbs);
                                    CustXml = CustXml.Replace("{store_no}", store);
                                    CustXml = CustXml.Replace("{first_name}", pedido.first_name.Norm(40));
                                    CustXml = CustXml.Replace("{last_name}", pedido.last_name.Norm(40));
                                    CustXml = CustXml.Replace("{info1}", rut);

                                    CustXml = CustXml.Replace("{created_date}", (DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")));
                                    CustXml = CustXml.Replace("{modified_date}", (DateTime.Now.AddSeconds(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")));

                                    CustXml = CustXml.Replace("{address1}", pedido.direccion1.Norm(40));
                                    CustXml = CustXml.Replace("{address2}", pedido.comuna.Norm(40));
                                    CustXml = CustXml.Replace("{address3}", pedido.ciudad.Norm(40));
                                    CustXml = CustXml.Replace("{address4}", "");
                                    CustXml = CustXml.Replace("{address5}", "");
                                    CustXml = CustXml.Replace("{address6}", "");
                                    CustXml = CustXml.Replace("{zip}", "");
                                    CustXml = CustXml.Replace("{phone1}", pedido.phone1.Norm(40));// order.objFactura.DatosCliente.Telefono.Norm(40));
                                    CustXml = CustXml.Replace("{phone2}", pedido.phone2.Norm(40));// order.objFactura.DatosCliente.Celular.Norm(40));
                                    CustXml = CustXml.Replace("{email_addr}", pedido.email_addr);//order.objFactura.DatosCliente.Correo.Norm(40));
                                    CustXml = CustXml.Replace(@"{notes}", "");

                                    //CustXml = CustXml.Replace("{CustUdf1}", "");
                                    //CustXml = CustXml.Replace("{CustUdf2}", "");
                                    //CustXml = CustXml.Replace("{CustUdf3}", "");
                                    //CustXml = CustXml.Replace("{CustUdf4}", "");
                                    //CustXml = CustXml.Replace("{CustUdf5}", "");
                                    //CustXml = CustXml.Replace("{CustUdf6}", "");
                                    //CustXml = CustXml.Replace("{CustUdf7}", "");
                                    //CustXml = CustXml.Replace("{CustUdf8}", "");
                                    //CustXml = CustXml.Replace("{CustUdf9}", "");
                                    //CustXml = CustXml.Replace("{CustUdf10}", "");
                                    //CustXml = CustXml.Replace("{CustUdf11}", "");
                                    //CustXml = CustXml.Replace("{CustUdf12}", "");
                                    //CustXml = CustXml.Replace("{CustUdf13}", "");
                                    //CustXml = CustXml.Replace("{CustUdf14}", "");
                                    //CustXml = CustXml.Replace("{CustUdf15}", "");
                                    //CustXml = CustXml.Replace("{CustUdf16}", "");
                                    //CustXml = CustXml.Replace("{CustUdf17}", "");
                                    //CustXml = CustXml.Replace("{CustUdf18}", "");
                                    //CustXml = CustXml.Replace("{CustUdf19}", "");
                                    //CustXml = CustXml.Replace("{CustUdf20}", "");

                                }
                                else //cliente deberia existir trae custSid y CUstID para el rut 
                                {
                                    DataSetRpro.ClientesRow clieRpro = ClientesDb.First();
                                    //devuelve custsid y cusId
                                    CustSid = clieRpro.CUST_SID;
                                    CustID = clieRpro.CUST_ID;

                                }
                            }

                            string XmlInvcCust = InvoiceCustomer();

                            //incluye cliente
                            if (rut != "")
                            {

                                XmlInvcCust = XmlInvcCust.Replace("{cust_sid}", CustSid);
                                XmlInvcCust = XmlInvcCust.Replace("{cust_id}", CustID);
                                XmlInvcCust = XmlInvcCust.Replace("{store_no}", store);
                                XmlInvcCust = XmlInvcCust.Replace("{first_name}", pedido.first_name.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{last_name}", pedido.last_name.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{info1}", rut);
                                XmlInvcCust = XmlInvcCust.Replace("{modified_date}", DateTime.Now.AddSeconds(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                                XmlInvcCust = XmlInvcCust.Replace("{sbs_no}", sbs);
                                XmlInvcCust = XmlInvcCust.Replace("{address1}", pedido.direccion1.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{address2}", pedido.comuna.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{address3}", pedido.ciudad.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{address4}", "");
                                XmlInvcCust = XmlInvcCust.Replace("{phone1}", pedido.phone1.Norm(40));
                                XmlInvcCust = XmlInvcCust.Replace("{phone2}", pedido.phone2.Norm(40));




                                xml = xml.Replace("<CUSTOMER/>", XmlInvcCust);
                                xml = xml.Replace("<SHIPTO_CUSTOMER/>", "");

                                xml = xml.Replace("{cust_sid}", CustSid);
                                xml = xml.Replace("{addr_no}", "1");
                                //llena la salida

                            }


                            xmlCustomer = xmlCustomer + CustXml;

                        }
                        catch (Exception)
                        {


                        }

                        //Fin CLiente

                        xml = xml.Replace("{invc_sid}", invoiceSid);
                        xml = xml.Replace("{sbs_no}", sbs);
                        xml = xml.Replace("{store_no}", store);
                        xml = xml.Replace("{invc_no}", invoiceNo);
                        xml = xml.Replace("{modified_date}", createdDate);
                        xml = xml.Replace("{created_date}", createdDate);
                        xml = xml.Replace("{tracking_no}", Convert.ToString(pedido.folio).Trim());
                        xml = xml.Replace("{fiscal_doc_id}", Convert.ToString(pedido.folio).Trim());

                        // cust_fld
                        xml = xml.Replace("{cust_fld}", "");

                        // CANAL ORIGEN
                        xml = xml.Replace("{comment_no_1}", "Venta Web");


                        xml = xml.Replace("{comment_no_2}", pedido.id);

                        xml = xml.Replace("{note}", "");


                        //pago

                        try
                        {
                            string cardName = "";
                            xml = xml.Replace("{udf_no1}", cfg.Invoice_FlagName);

                            Log("Pago " + errores, ArchivoLog, true);




                            foreach (var pago in pedido.Pagos)
                            {

                                try
                                {

                                    if (string.IsNullOrEmpty(pago.tender_type))
                                    {
                                        Log("Pago tender_type no especificado: ", ArchivoLog, true);
                                    }
                                }
                                catch (Exception)
                                {

                                }


                                //if ((pago.tender_type.ToUpper() == "EFECTIVO")|| (pago.tender_type.ToUpper() == "EFECIVO"))
                                //{
                                //    xml = xml.Replace("{tender_type}", "0");//0 efectivo 2 credito
                                //    xml = xml.Replace("{montoPago}", pedido.total_amt.ToString());
                                //    xml = xml.Replace("{auth}", "");
                                //    xml = xml.Replace("{crd_name}", "");
                                //    xml = xml.Replace("{eftdata0}", "");
                                //    xml = xml.Replace("{eftdata1}", "");
                                //    xml = xml.Replace("{eftdata2}", "");
                                //    xml = xml.Replace("{eftdata3}", "");
                                //    xml = xml.Replace("{eftdata4}", "");
                                //    xml = xml.Replace("{eftdata5}", "");
                                //    xml = xml.Replace("{eftdata6}", "");
                                //    xml = xml.Replace("{eftdata7}", "");
                                //    xml = xml.Replace("{eftdata8}", "");
                                //    xml = xml.Replace("{eftdata9}", "");
                                //    xml = xml.Replace("{eftdata10}", "");
                                //}
                                //else 
                                string crdName = "";

                                if (pago.tender_type.ToUpper() == "DEBITO")
                                {

                                    crdName = "DWEB";
                                    xml = xml.Replace("{tender_type}", "11");
                                    xml = xml.Replace("{montoPago}", pedido.total_amt.ToString());
                                    xml = xml.Replace("{auth}", pago.auth_no);
                                    xml = xml.Replace("{crd_name}", crdName);
                                    xml = xml.Replace("{eftdata0}", "DB");
                                    xml = xml.Replace("{eftdata1}", "0");
                                    xml = xml.Replace("{eftdata2}", crdName);
                                    xml = xml.Replace("{eftdata3}", pago.op_no);
                                    xml = xml.Replace("{eftdata4}", "0");
                                    xml = xml.Replace("{eftdata5}", pago.comments);
                                    xml = xml.Replace("{eftdata6}", "0");
                                    xml = xml.Replace("{eftdata7}", "0");
                                    xml = xml.Replace("{eftdata8}", "0");
                                    xml = xml.Replace("{eftdata9}", "0");
                                    xml = xml.Replace("{eftdata10}", "0");
                                }
                                else
                                {
                                    crdName = "CWEB";
                                    xml = xml.Replace("{tender_type}", "2"); // 2 credito
                                    xml = xml.Replace("{montoPago}", pedido.total_amt.ToString());
                                    xml = xml.Replace("{auth}", pago.auth_no);
                                    xml = xml.Replace("{crd_name}", crdName);
                                    xml = xml.Replace("{eftdata0}", "CR");
                                    if (string.IsNullOrEmpty(pago.cuotas))
                                    {
                                        xml = xml.Replace("{eftdata1}", "1");
                                    }
                                    else
                                    {
                                        xml = xml.Replace("{eftdata1}", pago.cuotas.ToString());
                                    }
                                    
                                    xml = xml.Replace("{eftdata2}", crdName);
                                    xml = xml.Replace("{eftdata3}", pago.op_no);
                                    xml = xml.Replace("{eftdata4}", "0");
                                    xml = xml.Replace("{eftdata5}", pago.comments);
                                    xml = xml.Replace("{eftdata6}", "0");
                                    xml = xml.Replace("{eftdata7}", "0");
                                    xml = xml.Replace("{eftdata8}", "0");
                                    xml = xml.Replace("{eftdata9}", "0");
                                    xml = xml.Replace("{eftdata10}", "0");
                                }


                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            Log("Pago con error en: " + ex, ArchivoLog, true);
                        }

                        
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'TENDER_TYPE', ttCreditCard);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'AMT', AttrValue.Monto);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'DOC_NO', AttrValue.NumeroTarjeta);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'AUTH', AttrValue.Autorizacion);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'CRD_TYPE', AttrValue.Crd_Type);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA0', AttrValue.TipoTarjeta);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA1', AttrValue.Cuotas);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA2', AttrValue.NombreTarjeta);
                        //      
                        //campos adicionales en pos integrado
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA3', AttrValue.NrOperacion);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA4', AttrValue.MontoCuota);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA5', AttrValue.Cuenta);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA6', AttrValue.FechaOperacion);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA7', AttrValue.HoraOperacion);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA8', AttrValue.CodComercio);
                        //fAdapter.BOSetAttributeValueByName(TenderHnd, 'EFTDATA9', AttrValue.TerminalID);


                        int index = 0;
                        double ItemUnitario = 0;
                        double dif = 0;
                        AlusError = "";

                        Log("Articulos  ", ArchivoLog);
                        foreach (var linea in pedido.Items)
                        {

                            index = index + 1;
                            string tempItem = InvoiceITEM();
                            ItemUnitario = Convert.ToDouble(linea.PriceLine/linea.qty);



                            tempItem = tempItem.Replace("{item_pos}", index.ToString());
                            tempItem = tempItem.Replace("{store_no}", shiptoStoreNo);
                            tempItem = tempItem.Replace("{price}", Convert.ToInt32(Math.Round(ItemUnitario)).ToString());
                            tempItem = tempItem.Replace("{orig_price}", Convert.ToInt32(Math.Round(ItemUnitario)).ToString());

                            //totalItems += checkoutitem.totalWithDiscount;

                            tempItem = tempItem.Replace("{qty}", Convert.ToInt32(linea.qty).ToString());
                            tempItem = tempItem.Replace("{alu}", linea.itemcode);



                            Log("FillByAlu:  " + linea.itemcode , ArchivoLog, true);

                            try
                            {
                                adapter.FillBySbsAlu(ItemTable, linea.itemcode, iSbs.ToString());
                            }
                            catch (Exception oraEx)
                            {

                                Log("verifique vista VyVProductos " + oraEx.Message + " " + oraEx.InnerException, "OracleError.txt");
                            }



                            Log("ItemTable.Count: :  ", ArchivoLog, true);
                            if (ItemTable.Count > 0)
                            {
                                DataSetRpro.VYVARTICULOSRow rowItem = ItemTable.First();

                                tempItem = tempItem.Replace("{item_sid}", rowItem.ITEM_SID);
                                //tempItem = tempItem.Replace("{descripcion1}", rowItem.DESCRIPTION1);
                                tempItem = tempItem.Replace("{tax_code}", rowItem.TAX_CODE.ToString());



                                tempItem = tempItem.Replace("{tax_amt}", (Convert.ToDecimal(ItemUnitario) * (rowItem.TAX_PERC1 / 100)).ToString("#.00")).Replace(",", ".");

                                tempItem = tempItem.Replace("{tax_perc}", Convert.ToString(rowItem.TAX_PERC1).Replace(",", "."));

                                tempItem = tempItem.Replace("{cost}", Math.Round(rowItem.COST).ToString());
                                tempItem = tempItem.Replace("{price_level}", "1");

                            }
                            else
                            {
                                //checkoutitem.ProductVersion.code
                                AlusError += linea.itemcode + "\r\n";
                            }

                            xml += tempItem;
                            Log("xml: " + xml, "debug.txt");
                        }
                    }
                    catch (Exception e3)
                    {
                        errores += pedido.id + " " + e3.Message + " " + xml + "\r\n";
                        xml = "";
                    }


                    if (AlusError != "")
                    {
                        //alus con error no se procesa 

                        //ile.WriteAllText(ruta + @"\logs\AluError_" + entry._id + ".txt", AlusError, Encoding.UTF8);
                        Log("AluError: " + AlusError, ArchivoLog, true);

                        xml = "";

                        try
                        {

                            pedido.InvoiceSid = null;
                            pedido.ErrorInvoice = "articulo no existe " + AlusError;
                            appDbContex.SaveChanges();
                        }
                        catch (Exception e1)
                        {
                            File.WriteAllText(ruta + @"SaveInvoiceError.txt", e1.Message, Encoding.UTF8);
                            Log("SaveChangesInvoiceError " + e1.Message, ArchivoLog, true);
                        }


                    }

                    if (string.IsNullOrEmpty(xml)) continue;

                    xml += @"</INVC_ITEMS><INVC_FEES/><INVC_COMMENTS/></INVOICE>";

                    xmlDoc += xml;

                    //Actualizamos SId

                    try
                    {

                        Log("SaveChanges: ", ArchivoLog);

                        pedido.InvoiceSid = invoiceSid;
                        pedido.ErrorInvoice = "OK";
                        appDbContex.SaveChanges();
                        Log("Save: OK ", ArchivoLog, true);
                    }
                    catch (Exception e)
                    {
                        Log("Save: ERROR " + e.Message, ArchivoLog, true);
                        errores += e.Message;

                    }

                }

                if (string.IsNullOrEmpty(xmlDoc)) return;


                try
                {
                    xmlDoc = InvoiceDOC() + xmlDoc + "</INVOICES></DOCUMENT>";

                    if (!string.IsNullOrEmpty(xmlCustomer))
                    {
                        xmlCustomer = CustDOC() + xmlCustomer + @"</CUSTOMERS></DOCUMENT>";

                        File.WriteAllText(cfg.Invoice_CarpetaStation + @"\CUSTOMER666.xml", xmlCustomer, Encoding.UTF8);
                        File.WriteAllText(ruta + @"\CUSTOMER666.xml", xmlCustomer, Encoding.UTF8);
                    }

                    // var credentials = new UserCredentials("frch", "retail", "SAP.0209");
                    // Impersonation.RunAsUser(credentials, LogonType.NewCredentials

                    //escribe en carpeta de integracion ECM con autentificacion para RUTAS EN RED

                    //Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                    // {
                    File.WriteAllText(cfg.Invoice_CarpetaStation + @"\INVOICE999.xml", xmlDoc, Encoding.UTF8);
                    File.WriteAllText(ruta + @"\INVOICE999.xml", xmlDoc, Encoding.UTF8);

                    //});
                }
                catch (Exception e)
                {

                    errores = errores + e.Message;
                }

                if (errores != "")
                {
                    Log("Errores: " + errores, ArchivoLog, true);
                    File.WriteAllText(ruta + @"\Errores.txt", errores);
                }


                if (errores != "")
                {
                    Log("Errores: " + errores, ArchivoLog, true);
                    File.WriteAllText(ruta + @"\Errores.txt", errores);
                }

            }
            catch (Exception e)
            {
                File.WriteAllText(ruta + @"\GeneraXmlsInvoiceError.txt", "Linea: " + e.StackTrace + " " + e.InnerException + " " + e.Message, Encoding.UTF8);
                if (errores != "")
                {
                    File.WriteAllText(ruta + @"\ErrorInvc.txt", errores);
                }
            }
            finally
            {
                appDbContex.Dispose();
            }

            return;
        }


        private void EliminaLog(string archivo)
        {
            try
            {

                File.Delete(ruta + @"\" + archivo);

            }
            catch (Exception err)
            {


            }

        }

        public void EjecutaEcm()
        {
            var resp = ExecuteCommandSync(ruta, ruta + @"\ejecutaECM.bat", "");
        }

        private string ExecuteCommandSync(string workingDirectory, string fileName, string command)
        {
            string result = "";

            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo(fileName, command);//new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.

                procStartInfo.WorkingDirectory = workingDirectory;
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;


                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                result = proc.StandardOutput.ReadToEnd();
                // Display the command output.

                return result;

                //Console.WriteLine(result);
            }
            catch (Exception ex)
            {

                // Log the exception
            }

            return result;


        }

        private string InvoiceDOC()
        {
            string temp = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            temp += @"<DOCUMENT xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><INVOICES>";
            return temp;

        }
        private string CustDOC()
        {
            string temp = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            temp += @"<DOCUMENT xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><CUSTOMERS>";
            return temp;

        }

        private string CustomerXML()
        {
            string
              temp = @"<CUSTOMER cust_sid=""{cust_sid}"" sbs_no=""{sbs_no}"" cust_id=""{cust_id}"" store_no=""{store_no}"" home_sbs_no=""{sbs_no}""";
            temp += @" home_store_no="""" station="""" status="""" first_name=""{first_name}"" last_name=""{last_name}"" info1=""{info1}""";
            temp += @" info2="""" price_lvl="""" credit_limit="""" credit_used="""" store_credit="""" accept_checks="""" detax=""""";
            temp += @" max_disc_perc="""" active=""1"" mark1="""" mark2="""" udf1_date="""" udf2_date="""" created_date=""{created_date}""";
            temp += @" modified_date=""{modified_date}"" ref_cust_sid="""" email_addr=""{email_addr}"" qb_id="""" ar_flag="""" cms=""0"" household_code=""""";
            temp += @" marketing_flag="""" sec_lvl="""" cust_type=""0"" notes=""{notes}"" cms_post_date=""{created_date}"" merge_cust_sid=""""";
            temp += @" allow_post="""" allow_phone="""" allow_email="""" shipping_priority="""" controller=""0"" orig_controller=""0""";
            temp += @" last_sale_date="""" check_limit="""" fst_sale_date="""" lst_sale_date="""" lst_return_date="""" lst_sale_amt=""""";
            temp += @" disc_perc_upper_limit=""100"" term_type="""" cent_payment_amt="""" lty_opt_in="""" lty_enroll_date=""""";
            temp += @" lty_balance="""" lty_accumulated="""" lty_lvl_sid="""" lty_lvl_locked="""" lty_opt_in_manual=""""";
            temp += @" alternate_id1="""" alternate_id2="""" region_sbs_no=""-1"" region_name="""" sector_name="""" cust_class_name=""""";
            temp += @" company_name="""" title="""" tax_area_name="""" tax_area2_name="""" createdby_sbs_no=""1""  createdby_empl_name=""SYSADMIN""";
            temp += @" modifiedby_sbs_no=""1"" modifiedby_empl_name=""SYSADMIN"" primary_clerk_sbs_no=""1""";
            temp += @" primary_clerk_name=""SYSADMIN"" lty_lvl_name="""">";
            temp += @" <CUST_ADDRESSS>";
            temp += @" <CUST_ADDRESS addr_no=""1"" begin_date="""" end_date="""" addr_name="""" shipping="""" address1=""{address1}""";
            temp += @" address2=""{address2}"" address3=""{address3}"" address4=""{address4}"" address5=""{address5}""";
            temp += @" address6=""{address6}"" zip=""{zip}"" phone1=""{phone1}"" phone2=""{phone2}"" email_addr=""{email_addr}""";
            temp += @" include_phone=""0"" include_email=""0"" include_post=""0"" default_addr=""0"" season_start_date=""""";
            temp += @" season_end_date="""" active=""1"" phone1_type_name="""" phone2_type_name="""" country_name=""""";
            temp += @" tax_area_name="""" tax_area2_name="""" addr_type=""""/>";
            temp += @" </CUST_ADDRESSS>";
            temp += @" <CUST_SUPPLS>";
            //temp += @" <CUST_SUPPL udf_no=""1"" udf_value =""{CustUdf1}"" />";
            //temp += @" <CUST_SUPPL udf_no=""2"" udf_value =""{CustUdf2}"" />";
            //temp += @" <CUST_SUPPL udf_no=""3"" udf_value =""{CustUdf3}"" />";
            //temp += @" <CUST_SUPPL udf_no=""4"" udf_value =""{CustUdf4}"" />";
            //temp += @" <CUST_SUPPL udf_no=""5"" udf_value =""{CustUdf5}"" />";
            //temp += @" <CUST_SUPPL udf_no=""6"" udf_value =""{CustUdf6}"" />";
            //temp += @" <CUST_SUPPL udf_no=""7"" udf_value =""{CustUdf7}"" />";
            //temp += @" <CUST_SUPPL udf_no=""8"" udf_value =""{CustUdf8}"" />";
            //temp += @" <CUST_SUPPL udf_no=""9"" udf_value =""{CustUdf9}"" />";
            //temp += @" <CUST_SUPPL udf_no=""10"" udf_value =""{CustUdf10}"" />";
            //temp += @" <CUST_SUPPL udf_no=""11"" udf_value =""{CustUdf11}"" />";
            //temp += @" <CUST_SUPPL udf_no=""12"" udf_value =""{CustUdf12}"" />";
            //temp += @" <CUST_SUPPL udf_no=""13"" udf_value =""{CustUdf13}"" />";
            //temp += @" <CUST_SUPPL udf_no=""14"" udf_value =""{CustUdf14}"" />";
            //temp += @" <CUST_SUPPL udf_no=""15"" udf_value =""{CustUdf15}"" />";
            //temp += @" <CUST_SUPPL udf_no=""16"" udf_value =""{CustUdf16}"" />";
            //temp += @" <CUST_SUPPL udf_no=""17"" udf_value =""{CustUdf17}"" />";
            //temp += @" <CUST_SUPPL udf_no=""18"" udf_value =""{CustUdf18}"" />";
            //temp += @" <CUST_SUPPL udf_no=""19"" udf_value =""{CustUdf19}"" />";
            //temp += @" <CUST_SUPPL udf_no=""20"" udf_value =""{CustUdf20}"" />";
            temp += @" </CUST_SUPPLS>";
            temp += @" <CUST_CRDS>";
            temp += @" </CUST_CRDS>";
            temp += @" <CUST_TERMS>";
            temp += @" </CUST_TERMS>";
            temp += @" </CUSTOMER>";


            return temp;
        }
        private string InvoiceCustomer()
        {

            string temp = @"<CUSTOMER cust_sid=""{cust_sid}"" cust_id=""{cust_id}"" store_no=""{store_no}"" station="""" first_name=""{first_name}"" last_name=""{last_name}"" price_lvl=""""";
            temp += @" detax="""" info1=""{info1}"" info2="""" modified_date=""{modified_date}"" sbs_no=""{sbs_no}"" cms=""0"" company_name="""" title=""""";
            temp += @" tax_area_name="""" shipping="""" address1=""{address1}"" address2=""{address2}"" address3=""{address3}"" address4=""{address4}""";
            temp += @" address5="""" address6="""" zip="""" phone1="""" phone2="""" country_name="""" alternate_id1="""" alternate_id2="""" />""";
            temp += @"<SHIPTO_CUSTOMER cust_sid=""{cust_sid}"" cust_id=""{cust_id}"" store_no=""{store_no}"" station="""" first_name=""{first_name}""";
            temp += @" last_name=""{last_name}"" price_lvl="""" detax="""" info1=""{info1}"" info2="""" modified_date=""{modified_date}""";
            temp += @" sbs_no=""{sbs_no}"" cms=""0"" company_name="""" title="""" tax_area_name="""" shipping="""" address1=""{address1}""";
            temp += @" address2=""{address2}"" address3=""{address3}"" address4=""{address4}"" address5="""" address6="""" zip="""" phone1=""{phone1}"" phone2=""{phone2}""";
            temp += @" country_name="""" alternate_id1="""" alternate_id2=""""/>";
            return temp;
        }
        private string InvoiceCAB()
        {
            string temp = @"<INVOICE invc_sid=""{invc_sid}"" sbs_no=""{sbs_no}"" store_no=""{store_no}"" invc_no=""{invc_no}"" invc_type=""0""";
            temp += @" hisec_type = """" status = ""0"" proc_status = ""1"" workstation = ""1"" orig_store_no = ""{store_no}""";
            temp += @" use_vat=""1"" vat_options=""0"" disc_perc="""" disc_amt="""" note=""{note}"" over_tax_perc=""""";
            temp += @" over_tax_perc2 = """" tax_reb_perc = """" tax_reb_amt = """" rounding_offset = """"";
            temp += @" created_date=""{created_date}"" modified_date=""{created_date}"" post_date=""{created_date}""";
            temp += @" tracking_no = ""{tracking_no}"" audited = ""0"" cms_post_date = ""{created_date}""";
            temp += @" cust_fld=""{cust_fld}"" held=""0"" drawer_no=""1"" controller=""1""";
            temp += @" orig_controller = ""1"" elapsed_time = ""0"" activity_perc = ""100"" activity_perc2 = ""0""";
            temp += @" activity_perc3 = ""0"" activity_perc4 = ""0""";
            temp += @" activity_perc5=""0"" eft_invc_no="""" detax=""0"" fiscal_doc_id=""{fiscal_doc_id}""  ship_perc=""""";
            temp += @" trans_disc_amt = """"  empl_sbs_no=""1"" empl_name=""SYSADMIN"" tax_area_name = ""IVA""";
            temp += @" createdby_sbs_no = ""{sbs_no}"" modifiedby_sbs_no = ""{sbs_no}""";
            temp += @" clerk_sbs_no=""{sbs_no}"" clerk_name=""SYSADMIN"">";
            temp += @"<CUSTOMER/>";
            temp += @"<SHIPTO_CUSTOMER/>";
            temp += @"<INVC_EXTRAS/>";
            temp += @"<INVC_SUPPLS>";
            temp += @" <INVC_SUPPL udf_no=""1"" udf_value=""{udf_no1}""/>";
            //temp += @" <INVC_SUPPL udf_no =""2"" udf_value = ""CONTADO"" />";
            temp += @" </INVC_SUPPLS>";
            temp += @"<INVC_COMMENTS>";
            temp += @"<INVC_COMMENT comment_no=""1"" comments=""{comment_no_1}""/>";
            temp += @"<INVC_COMMENT comment_no=""2"" comments=""{comment_no_2}""/>";
            temp += @"</INVC_COMMENTS>";
            temp += @"<INVC_TENDERS>";
            temp += @"<INVC_TENDER tender_type = ""{tender_type}"" tender_no = ""1"" taken = ""{montoPago}"" given = ""0"" amt = ""{montoPago}""";
            temp += @" reference = """" auth=""{auth}"" eftdata0=""{eftdata0}"" crd_name=""{crd_name}"" eftdata1=""{eftdata1}"" eftdata2=""{eftdata2}"" crd_normal_sale = ""1""";
            temp += @" tender_state=""0""  eftdata3=""{eftdata3}"" eftdata4=""{eftdata4}""";
            temp += @" eftdata5 =""{eftdata3}"" eftdata6=""{eftdata3}"" eftdata7=""{eftdata3}""";
            temp += @" eftdata8 = ""{eftdata3}"" eftdata9 = ""{eftdata3}"" balance_remaining = ""0""/>";
            temp += @"</INVC_TENDERS>";
            temp += @" <INVC_ITEMS>";


            return temp;

        }
        private string InvoiceITEM()
        {
            string temp = @"<INVC_ITEM item_pos=""{item_pos}"" item_sid=""{item_sid}"" qty=""{qty}"" orig_price=""{orig_price}"" price=""{price}"" tax_code=""{tax_code}""";
            temp += @" tax_perc = ""{tax_perc}"" tax_amt = ""{tax_amt}"" tax_code2 = """" tax_perc2 = ""0"" tax_amt2 = ""0"" cost = """" price_lvl = ""1"" spif = ""0""";
            temp += @" sched_no = """" comm_code = """" comm_amt = ""0"" cust_fld = """" scan_upc = """" serial_no = """" lot_number = """" kit_flag = ""0""";
            temp += @" pkg_item_sid = """" pkg_seq_no = """" orig_cmpnt_item_sid = """" detax = ""0"" usr_disc_perc = ""0"" udf_value1 = """"";
            temp += @" udf_value2 = """" udf_value3 = """" udf_value4 = """" activity_perc = ""100"" activity_perc2 = ""0"" activity_perc3 = ""0""";
            temp += @" activity_perc4 = ""0"" activity_perc5 = ""0"" comm_amt2 = ""0"" comm_amt3 = ""0"" comm_amt4 = ""0"" comm_amt5 = ""0""";
            temp += @" so_sid = """" so_orig_item_pos = """" item_origin = """" pkg_no = """" shipto_cust_sid = """" shipto_addr_no = """"";
            temp += @" orig_cost = """" item_note1 = """" item_note2 = """" item_note3 = """" item_note4 = """" item_note5 = """"";
            temp += @" item_note6 = """" item_note7 = """" item_note8 = """" item_note9 = """" item_note10 = """" promo_flag = ""0""";
            temp += @" gift_add_value = ""0"" ref_invc_no = """"";
            temp += @" alt_upc = """" alt_alu = """" alt_cost = """" alt_vend_code = """" orig_prc_bdt = """" prc_bdt = """"";
            temp += @" subloc_code = """" subloc_id = """" tender_state = ""0"" failure_msg = """" proc_date = """"";
            temp += @" cent_commit_txn = ""0"" price_flag = ""0"" force_orig_tax = """" sn_qty = """" sn_active = """" sn_received = """"";
            temp += @" sn_sold = """" sn_transferred = """" sn_so_reserved = """" sn_returned = """" sn_returned_to_vnd = """" sn_adjusted = """"";
            temp += @" tax_area2_name = """" empl_sbs_no = ""1"" empl_name = ""SYSADMIN"" >";
            temp += @" <INVN_BASE_ITEM item_sid = ""{item_sid}"" upc = """" alu = ""{alu}"" style_sid = """"/>";
            temp += @" </INVC_ITEM >";


            return temp;

        }



        private void Log(string sMsg, string archivo, bool append = false)
        {
            try
            {

                StreamWriter oSW = new StreamWriter((ruta + @"\" + archivo), append);

                string scomando = String.Empty;

                oSW.WriteLine(((DateTime.Now + ("| " + sMsg))));

                oSW.Flush();

                oSW.Close();

            }
            catch (Exception err)
            {

                //MessageBox.Show(err.Message);

            }
        }

        private string validarRut(string rut, out string d)
        {
            d = "";

            if (rut == null) return "";
            if (rut == String.Empty) return "";

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
                return "";
            }

            if (validacion) d = dv.ToString();

            if (validacion)
                return Convert.ToInt32(SrutAux).ToString();
            else
                return "";


        }


    }
}
