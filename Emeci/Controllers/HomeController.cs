using Emeci.Models;
using System.Web.Mvc;
using static Emeci.Models.PayUModel;
using System.Linq;

namespace Emeci.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Membership()
        {
            var Model = new MembershipModel();
            if (TempData["PaidOut"] != null)
            {
                Model.Status = (StatusT)TempData["PaidOut"];
                TempData.Remove("PaidOut");
            }

            return View(Model);
        }

        public ActionResult xy()
        {
            EmailMembership m = new EmailMembership();
            return View("~/Views/Emails/EmailMembership.cshtml", m);
        }

        [HttpPost]
        public ActionResult Membership(MembershipModel Model)
        {
            switch (Model.Type)
            {
                case MembershipModel.MembershipType.NewFile:
                    if (!string.IsNullOrEmpty(Model.Name) && !string.IsNullOrEmpty(Model.Phone) &&
                        !string.IsNullOrEmpty(Model.Email))
                    {
                        TempData["Membership"] = Model;
                        return RedirectToAction("PayUReq", "PayU");
                    }
                    break;
                case MembershipModel.MembershipType.Renewal:
                    if (!string.IsNullOrEmpty(Model.EMECI))
                    {
                        using(EmeciEntities DB = new EmeciEntities())
                        {
                            var x = (from r in DB.Registro
                                     where r.Emeci == Model.EMECI
                                     select new { r.Nombre, r.Apellido, r.Emails, r.Telefono }).FirstOrDefault();
                            if (x != null)
                            {
                                Model.Name = $"{x.Nombre} {x.Apellido}";
                                Model.Phone = x.Telefono;
                                Model.Email = x.Emails.Split(',')[0];
                                TempData["Membership"] = Model;
                                return RedirectToAction("PayUReq", "PayU");
                            }
                            else Model.Error = "Número de tarjeta no encontrado";
                        }
                    }
                    break;
            }

            return View(Model);
        }
    }
}