using CarInsuranceMVC.Models;
using CarInsuranceMVC.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CarInsuranceMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {

            using (InsuranceEntities db = new InsuranceEntities())

            {

                var insurees = db.Insurees;
                var insureeVMs = new List<InsureeVM>();
                foreach (var insuree in insurees)
                {
                    var insureeVM = new InsureeVM();
                    insureeVM.FirstName = insuree.FirstName;
                    insureeVM.LastName = insuree.LastName;
                    insureeVM.EmailAddress = insuree.EmailAddress;
                    insureeVM.QuoteMonthly = insuree.QuoteMonthly;
                    insureeVM.QuoteYearly = insuree.QuoteYearly;
                    insureeVMs.Add(insureeVM);
                }

                return View(insureeVMs);
            }
        }
    }
}