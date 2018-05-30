using Emeci.App_Code;
using Emeci.Models;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Linq;
using static System.Configuration.ConfigurationManager;
using static Emeci.App_Code._Transactions;
using System;
using static Emeci.Models.PayUModel;

namespace Emeci.Controllers
{
    public class PayUController : Controller
    {
        public string ApiKey { get { return AppSettings["apikey_payu"]; } }
        public string MerchantId { get { return AppSettings["merchantId_payu"]; } }


        public ActionResult PayUReq()
        {
            if (TempData["Membership"] == null)
                return null;

            MembershipModel Membership = (MembershipModel)TempData["Membership"];
            PayUModel Model = new PayUModel(Membership.Email, Membership.Type, Membership.EMECI);
            PayUT PayU = new PayUT();
            PayU.AddTransaction(Membership, Model.referenceCode);
            TempData.Remove("Membership");

            return PartialView(Model);
        }


        [HttpGet]
        public ActionResult GetConfirmation()
        {
            ServiceTracer.Log("Inicia HttpGet confirmacion PayU");
            bool IsReserved = false;

            NameValueCollection q = Request.QueryString;
            if (q.AllKeys.Length > 0)
            {
                ServiceTracer.Log($"Respuesta: {q.ToString()}");
                string currency = q["currency"] ?? "";
                string polTransactionState = q["polTransactionState"] ?? "";
                string polResponseCode = q["polResponseCode"] ?? "";
                string referenceCode = q["referenceCode"] ?? "";
                string reference_pol = q["reference_pol"] ?? "";
                string lapTransactionState = q["lapTransactionState"] ?? "";
                string polPaymentMethodType = q["polPaymentMethodType"] ?? "";
                string polPaymentMethod = q["polPaymentMethod"] ?? "";
                string TX_VALUE = q["TX_VALUE"] ?? "";
                string transactionState = q["transactionState"] ?? "";
                string signature = q["signature"] ?? "";

                string PaymentMethod = string.Empty;
                if (polPaymentMethodType == "7")
                    PaymentMethod = "Oxxo||7Eleven";
                else PaymentMethod = "Pago con tarjeta";

                TX_VALUE = Math.Round(double.Parse(TX_VALUE), 1).ToString("N1").Replace(",", "");

                string str = $"{ApiKey}~{MerchantId}~{referenceCode}~{TX_VALUE}~{currency}~{transactionState}";
                if (signature == new PayUModel().GetAsignature(str))
                {
                    ServiceTracer.Log("Signatura validada");
                    switch (lapTransactionState.ToLower())
                    {
                        case "approved":
                            if (polTransactionState == "4" && polResponseCode == "1")
                            {
                                IsReserved = CheckStatusTransaction(referenceCode);
                                if (!IsReserved)
                                {
                                    PayUT PayU = new PayUT();
                                    PayU.UpdateTransaction(StatusT.Confirmado, referenceCode, reference_pol, PaymentMethod);
                                    IsReserved = true;
                                    SendEmail(referenceCode);
                                }
                            }
                            break;
                        case "declined":
                            ServiceTracer.Log("Declinado!");
                            break;
                        case "error":
                            ServiceTracer.Log("Error!");
                            break;
                        case "expired":
                            ServiceTracer.Log("Expirado!");
                            break;
                        case "pending":
                            /* fueron por pago en sucursales,
                             * oxxo, sevel eleven
                             */
                            if (polTransactionState == "7" || polTransactionState == "14" || polTransactionState == "15")
                            {
                                if (polResponseCode == "15" || polResponseCode == "25" || polResponseCode == "26")
                                {
                                    new PayUT().UpdateTransaction(StatusT.Proceso, referenceCode, reference_pol, PaymentMethod);
                                    IsReserved = true;
                                    SendEmail(referenceCode);
                                }
                            }
                            break;
                    }
                }
                else ServiceTracer.Log($"La signatura no es identica: *string signature: {str}");
            }
            else return Redirect("https://www.emeci.com");


            TempData["PaidOut"] = IsReserved ? StatusT.Confirmado : StatusT.Rechazado;
            return RedirectToAction("Membership", "Home");
        }


        [HttpPost]
        public void Confirmation()
        {
            ServiceTracer.Log("Inicia HttpPost confirmacion PayU");
            NameValueCollection q = Request.Form;
            if (q.AllKeys.Length > 0)
            {
                ServiceTracer.Log($"Respuesta: {q.ToString()}");
                string currency = q["currency"] ?? "";
                string reference_sale = q["reference_sale"] ?? "";
                string reference_pol = q["reference_pol"] ?? "";
                string merchant_id = q["merchant_id"] ?? "";
                string value = q["value"] ?? "";
                string state_pol = q["state_pol"] ?? "";
                string payment_method = q["payment_method"] ?? "";
                string payment_method_type = q["payment_method_type"] ?? "";
                string response_code_pol = q["response_code_pol"] ?? "";
                string sign = q["sign"] ?? "";

                string str = $"{ApiKey}~{merchant_id}~{reference_sale}~{value}~{currency}~{state_pol}";
                if (sign == new PayUModel().GetAsignature(str))
                {
                    ServiceTracer.Log("Signatura validada");
                    if (state_pol == "4" && response_code_pol == "1")
                    {
                        if (!CheckStatusTransaction(reference_sale))
                        {
                            string PaymentMethod = string.Empty;
                            if (payment_method_type == "7")
                                PaymentMethod = "Oxxo||7Eleven";
                            else PaymentMethod = "Pago con tarjeta";

                            new PayUT().UpdateTransaction(StatusT.Confirmado, reference_sale, reference_pol, PaymentMethod);
                            SendEmail(reference_sale);
                        }
                    }
                    else ServiceTracer.Log("No fue aprobado");
                }
                else ServiceTracer.Log($"La signatura no es identica: *string signature: {str}");
            }
        }



        bool CheckStatusTransaction(string Reference)
        {
            using (EmeciEntities DB = new EmeciEntities())
            {
                var Row = (from x in DB.NewMembership
                           where x.Reference == Reference
                           select new { x.Status }).FirstOrDefault();
                if (Row != null)
                    return (int)StatusT.Confirmado == Row.Status.Value;
            }

            return false;
        }


        void SendEmail(string Reference)
        {
            using(EmeciEntities DB = new EmeciEntities())
            {
                var x = (from n in DB.NewMembership
                         where n.Reference == Reference
                         select n).FirstOrDefault();
                if (x != null)
                {
                    MembershipModel.MembershipType MembershipT = MembershipModel.MembershipType.NewFile;
                    string Subject = string.Empty;
                    if (!string.IsNullOrEmpty(x.EMECI))
                    {
                        Subject = "Renovación expediente";
                        MembershipT = MembershipModel.MembershipType.Renewal;
                    }
                    else Subject = "Nuevo expediente";

                    var Email = new EmailMembership
                    {
                        To = x.Email,
                        Subject = Subject,
                        Name = x.Name,
                        EMECI = x.EMECI,
                        MembershipT = MembershipT
                    };

                    Email.Send();
                }
            }
        }
    }
}