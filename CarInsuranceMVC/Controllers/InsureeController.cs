using CarInsuranceMVC.Models;
using CarInsuranceMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CarInsuranceMVC.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,QuoteMonthly,QuoteYearly")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("QuoteCalculator");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,QuoteMonthly,QuoteYearly")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult QuoteCalculator()
        {

            using (InsuranceEntities db = new InsuranceEntities())
            {
                var insurees = db.Insurees;
                var autoQuotes = db.AutoQuotes;

                var autoQuoteVMs = new List<AutoQuoteVM>();
                var insureeVMs = new List<InsureeVM>();

                {
                    foreach (Insuree insuree in insurees)
                    {

                        var autoQuote = new AutoQuote();
                        var autoQuoteVM = new AutoQuoteVM();
                        var insureeVM = new InsureeVM();


                        var baseRate = 50.00;


                        var age = DateTime.Now.Year - insuree.DateOfBirth.Year;



                        if (insuree.DateOfBirth.Month > DateTime.Now.Month
                         || insuree.DateOfBirth.Month == DateTime.Now.Month
                          && insuree.DateOfBirth.Day > DateTime.Now.Day)
                            age--;

                        var insureeAge = Convert.ToInt32(age);


                        double under18 = (insureeAge < 18) ? 100.00 : 0.00;
                        double btw19and25 = ((insureeAge > 18) && (age <= 25)) ? 50.00 : 0.00;
                        double over25 = (insureeAge > 25) ? 25.00 : 0.00;


                        double autoYearPrior2000 = (insuree.CarYear < 2000) ? 25.00 : 0.00;
                        double autoYearAfter2015 = (insuree.CarYear > 2015) ? 25.00 : 0.00;


                        double yesIsPorsche = (insuree.CarMake == "Porsche") ? 25.00 : 0.00;
                        double yesIsCarrera = (insuree.CarModel == "Carrera") ? 25.00 : 0.00;

                        double subtotalBeforeDUI = baseRate + under18 + btw19and25 + over25 +
                                                   autoYearPrior2000 + autoYearAfter2015 + yesIsPorsche +
                                                   yesIsCarrera;

                        int isTrueDUI = (insuree.DUI == true) ? 1 : 0;
                        double yesDUI = subtotalBeforeDUI * 0.25;//Calculating the rate of increase if DUI is true
                        double duiRate = (isTrueDUI == 1) ? yesDUI : 0.00;// value that will be placed in Quote DUIRateUP25Percent
                        double subtotalAfterDUI = duiRate + subtotalBeforeDUI;// value that will be placed in Quote SubTotalAfterDUICalc

                        int speedingTickets = insuree.SpeedingTickets;
                        double speedingTicketsRate = speedingTickets * 10.00;

                        double subtotalBeforeCoverageCalc = subtotalAfterDUI + speedingTicketsRate;
                        int coverageTypeFull = (insuree.CoverageType == true) ? 1 : 0;
                        double fullCoverageRate = subtotalBeforeCoverageCalc * 0.50;// calculating the rate of increase if FullCoverage is true
                        double yesFullCoverage = (coverageTypeFull == 1) ? fullCoverageRate : 0.00;//value that will be placed in FullCovRateUP50Percent  
                        double subtotalAfterCoverageCalc = subtotalBeforeCoverageCalc + yesFullCoverage;



                        double monthlyQuote = subtotalAfterCoverageCalc;
                        double yearlyQuote = subtotalAfterCoverageCalc * 12;

                        insuree.QuoteMonthly = Convert.ToDecimal(monthlyQuote);
                        insuree.QuoteYearly = Convert.ToDecimal(yearlyQuote);
                        autoQuote.InsureeId = Convert.ToInt32(insuree.Id);
                        autoQuote.BaseRate = Convert.ToDecimal(baseRate);
                        autoQuote.AgeUnder18 = Convert.ToDecimal(under18);
                        autoQuote.AgeBtw19and25 = Convert.ToDecimal(btw19and25);
                        autoQuote.Age26andUp = Convert.ToDecimal(over25);
                        autoQuote.AutoYearBefore2000 = Convert.ToDecimal(autoYearPrior2000);
                        autoQuote.AutoYearAfter2015 = Convert.ToDecimal(autoYearAfter2015);
                        autoQuote.IsPorsche = Convert.ToDecimal(yesIsPorsche);
                        autoQuote.IsCarrera = Convert.ToDecimal(yesIsCarrera);
                        autoQuote.SubTotalBeforeDuiCalc = Convert.ToDecimal(subtotalBeforeDUI);
                        autoQuote.DuiRateUp25Percent = Convert.ToDecimal(duiRate);
                        autoQuote.SubTotalAfterDuiCalc = Convert.ToDecimal(subtotalAfterDUI);
                        autoQuote.SpeedingTickets = Convert.ToDecimal(speedingTicketsRate);
                        autoQuote.SubTotalBeforeCoverageCalc = Convert.ToDecimal(subtotalBeforeCoverageCalc);
                        autoQuote.FullCoverageRateUp50Percent = Convert.ToDecimal(fullCoverageRate);
                        autoQuote.SubTotalAfterCoverageCalc = Convert.ToDecimal(subtotalAfterCoverageCalc);


                        insureeVM.QuoteMonthly = insuree.QuoteMonthly;
                        insureeVM.QuoteYearly = insuree.QuoteYearly;
                        autoQuoteVM.InsureeId = autoQuote.InsureeId;
                        autoQuoteVM.BaseRate = autoQuote.BaseRate;
                        autoQuoteVM.AgeUnder18 = autoQuote.AgeUnder18;
                        autoQuoteVM.AgeBtw19and25 = autoQuote.AgeBtw19and25;
                        autoQuoteVM.Age26andUp = autoQuote.Age26andUp;
                        autoQuoteVM.AutoYearBefore2000 = autoQuote.AutoYearBefore2000;
                        autoQuoteVM.AutoYearAfter2015 = autoQuote.AutoYearAfter2015;
                        autoQuoteVM.IsPorsche = autoQuote.IsPorsche;
                        autoQuoteVM.IsCarrera = autoQuote.IsCarrera;
                        autoQuoteVM.SubTotalBeforeDuiCalc = autoQuote.SubTotalBeforeDuiCalc;
                        autoQuoteVM.DuiRateUp25Percent = autoQuote.DuiRateUp25Percent;
                        autoQuoteVM.SubTotalAfterDuiCalc = autoQuote.SubTotalAfterDuiCalc;
                        autoQuoteVM.SpeedingTickets = autoQuote.SpeedingTickets;
                        autoQuoteVM.SubTotalBeforeCoverageCalc = autoQuote.SubTotalBeforeCoverageCalc;
                        autoQuoteVM.FullCoverageRateUp50Percent = autoQuote.FullCoverageRateUp50Percent;
                        autoQuoteVM.SubTotalAfterCoverageCalc = autoQuote.SubTotalAfterCoverageCalc;
                        autoQuoteVM.QuoteMonthly = autoQuote.QuoteMonthly;
                        autoQuoteVM.QuoteYearly = autoQuote.QuoteYearly;












                        db.AutoQuotes.Add(autoQuote);
                        db.Insurees.Add(insuree);
                        db.SaveChanges();
                    }
                    return View(autoQuoteVMs);
                }
            }
        }
    }
}
