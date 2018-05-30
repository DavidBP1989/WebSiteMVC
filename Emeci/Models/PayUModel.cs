using System;
using System.Security.Cryptography;
using System.Text;
using static Emeci.Models.MembershipModel;
using static System.Configuration.ConfigurationManager;

namespace Emeci.Models
{
    public class PayUModel
    {
        public enum StatusT
        {
            Proceso = 0,
            Confirmado = 1,
            Rechazado = 2,
            Ninguno = 3
        }

        public string apiKey { get; set; }
        public bool AllowTest { get; set; }

        public string url { get; set; }
        public string merchantId { get; set; }
        public string referenceCode { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string tax { get; set; }
        public string taxReturnBase { get; set; }
        public string signature { get; set; }
        public string accountId { get; set; }
        public string currency { get; set; }
        public string buyerEmail { get; set; }
        public string lng { get; set; }
        public string responseUrl { get; set; }
        public string confirmationUrl { get; set; }
        public string test { get; set; }

        public PayUModel() { }

        public PayUModel(string CustomerEmail, MembershipType Type, string EMECI)
        {
            apiKey = AppSettings["apikey_payu"];

            url = AppSettings["url_payu"];
            merchantId = AppSettings["merchantId_payu"];
            accountId = AppSettings["accountId_payu"];

            long Ticks = DateTime.Now.Ticks;
            byte[] Bytes = BitConverter.GetBytes(Ticks);
            referenceCode = Convert.ToBase64String(Bytes).Replace("+", "").Replace("/", "").TrimEnd('=');

            amount = AppSettings["membershipPrice"];
            switch (Type)
            {
                case MembershipType.NewFile:
                    description = "Nuevo expediente EMECI S.C.";
                    break;
                case MembershipType.Renewal:
                    description = $"Renovación de expediente EMECI S.C. ({EMECI})";
                    break;
            }
            
            tax = "0";
            taxReturnBase = "0";
            currency = "MXN";
            buyerEmail = CustomerEmail;
            responseUrl = AppSettings["urlresponse_payu"];
            confirmationUrl = AppSettings["urlconfirmation_payu"];

            string str = $"{apiKey}~{merchantId}~{referenceCode}~{amount}~{currency}";
            signature = GetAsignature(str);
            lng = "es";
            test = "1";
            AllowTest = url.ToLower().Contains("sandbox");   
        }


        public string GetAsignature(string str)
        {
            StringBuilder sb;
            using (MD5 Hash = MD5.Create())
            {
                byte[] Data = Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
                sb = new StringBuilder();
                for (int i = 0; i < Data.Length; i++)
                    sb.Append(Data[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
    }
}