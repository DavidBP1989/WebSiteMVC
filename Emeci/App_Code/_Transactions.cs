using System;
using Emeci.Models;
using System.Linq;
using static Emeci.Models.PayUModel;

namespace Emeci.App_Code
{
    public class _Transactions
    {
        public class PayUT
        {
            public void AddTransaction(MembershipModel Model, string Reference)
            {
                using (EmeciEntities DB = new EmeciEntities())
                {
                    DB.NewMembership.Add(new NewMembership()
                    {
                        Name = Model.Name,
                        Phone = Model.Phone,
                        Email = Model.Email,
                        C_Date = DateTime.Now,
                        Reference = Reference,
                        Status = (int)StatusT.Proceso,
                        EMECI = Model.EMECI
                    });

                    try
                    {
                        DB.SaveChanges();
                    }
                    catch (Exception) { }
                }
            }

            public void UpdateTransaction(StatusT StatusTransaction, string Reference, 
                string ReferenceBank, string PaymentMethod)
            {
                using(EmeciEntities DB = new EmeciEntities())
                {
                    NewMembership Membership = (from x in DB.NewMembership
                                                where x.Reference == Reference
                                                select x).First();
                    if (Membership != null)
                    {
                        Membership.ReferenceBank = ReferenceBank;
                        Membership.Status = (int)StatusTransaction;
                        Membership.PaymentMethod = PaymentMethod;
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
    }
}