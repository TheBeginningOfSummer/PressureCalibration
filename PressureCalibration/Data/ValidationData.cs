using Calibration.Services;
using SQLite;

namespace Calibration.Data
{
    public class ValidationData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public ValidationData() { }

        public ValidationData(int uid, decimal setP, decimal setT, decimal paceRef, decimal tProbe, int raw_C, int uncalTempCodes)
        {
            Uid = uid;
            SetP = setP;
            SetT = setT;
            PACERef = paceRef;
            TProbe = tProbe;
            RAW_C = raw_C;
            UNCALTempCodes = uncalTempCodes;
        }

        public int Uid { get; set; }
        public decimal SetP { get; set; }
        public decimal SetT { get; set; }
        public decimal PACERef { get; set; }
        public decimal TProbe { get; set; }
        public Int32 RAW_C { get; set; }
        public Int32 UNCALTempCodes { get; set; }

        public double PCal { get; set; }
        public double TCal { get; set; }

        public double PResidual { get; set; } = 999;
        public double TResidual { get; set; } = 999;

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{Uid}  setP:{SetP}  setT:{SetT}  PACERef:{PACERef:N2}  TProbe:{TProbe:N2}  RAW_C:{RAW_C}  UNCALTempCodes:{UNCALTempCodes}  TCal:{TCal:N2}  PCal:{PCal:N2}  PResidual:{PResidual:N2}  TResidual:{TResidual:N2}]";
        }

        public void Verify(CalibrationData calibrationData)
        {
            Calculation.StartValidation(calibrationData, RAW_C, UNCALTempCodes, out double pCal, out double tCal);
            PCal = pCal;
            TCal = tCal;
            PResidual = PCal - (double)PACERef;
            TResidual = TCal - (double)TProbe;
        }

    }
}
