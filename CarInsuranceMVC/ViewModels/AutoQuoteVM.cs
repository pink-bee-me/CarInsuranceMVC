using System;

namespace CarInsuranceMVC.ViewModels
{
    public class AutoQuoteVM
    {
        public int InsureeId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CarYear { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public bool InsureeIsPorsche { get; set; }
        public bool InsureeIsCarrera { get; set; }
        public bool DUI { get; set; }
        public int InsureeSpeedingTickets { get; set; }
        public bool CoverageType { get; set; }

        public decimal BaseRate { get; set; }
        public decimal AgeUnder18 { get; set; }
        public decimal AgeBtw19and25 { get; set; }
        public decimal Age26andUp { get; set; }
        public decimal AutoYearBefore2000 { get; set; }
        public decimal AutoYearAfter2015 { get; set; }
        public decimal IsPorsche { get; set; }
        public decimal IsCarrera { get; set; }
        public decimal SubTotalBeforeDuiCalc { get; set; }
        public decimal DuiRateUp25Percent { get; set; }
        public decimal SubTotalAfterDuiCalc { get; set; }
        public decimal SpeedingTickets { get; set; }
        public decimal SubTotalBeforeCoverageCalc { get; set; }
        public decimal FullCoverageRateUp50Percent { get; set; }
        public decimal SubTotalAfterCoverageCalc { get; set; }
        public decimal QuoteMonthly { get; set; }
        public decimal QuoteYearly { get; set; }
    }
}