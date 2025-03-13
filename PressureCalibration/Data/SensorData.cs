using CSharpKit.DataManagement;
using Module;
using SQLite;

namespace Data
{
    public class RawDataBOE2520
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public RawDataBOE2520() { }

        public Int32 uid { get; set; }

        public int T_idx { get; set; }
        public int P_idx { get; set; }
        public decimal SetP { get; set; }
        public decimal SetT { get; set; }
        /// <summary>
        /// 检测的压力值
        /// </summary>
        public decimal PACERef { get; set; }
        /// <summary>
        /// 检测的温度值
        /// </summary>
        public decimal TProbe { get; set; }
        /// <summary>
        /// 校准前压力原始值
        /// </summary>
        public Int32 RAW_C { get; set; }
        /// <summary>
        /// 校准前温度原始值
        /// </summary>
        public Int32 UNCALTempCodes { get; set; }

        private decimal tens;
        /// <summary>
        /// 校准前摄氏度值
        /// </summary>
        public decimal Tens
        {
            get
            {
                tens = (decimal)UNCALTempCodes / 128 - 273.15m;
                return tens;
            }
            set { tens = value; }
        }

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{uid}  setP:{SetP}  setT:{SetT}  PACERef:{PACERef:N2}  TProbe:{TProbe:N2}  RAW_C:{RAW_C}  UNCALTempCodes:{UNCALTempCodes}  Tens:{Tens:N2}]";
        }

    }
    public class CalibrationBOE2520
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public CalibrationBOE2520() { }

        public Int32 uid { get; set; }
        public int b40 { get; set; }
        public int b31 { get; set; }
        public int b30 { get; set; }
        public int b22 { get; set; }
        public int b21 { get; set; }
        public int b20 { get; set; }
        public int b12 { get; set; }
        public int b11 { get; set; }
        public int b10 { get; set; }
        public int b02 { get; set; }
        public int b01 { get; set; }
        public int b00 { get; set; }
        public int A { get; set; }
        public int alpha { get; set; }

        private string registerString = "";
        public string RegisterString
        {
            get
            {
                registerString = DataConverter.BytesToHexString(registerData);
                return registerString;
            }
            set
            {
                registerString = value;
                registerData = DataConverter.HexStringToBytes(registerString);
            }
        }

        public byte[] registerData = new byte[28];

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{uid}] b40:{b40}  b31:{b31}  b30:{b30}  b22:{b22}  b21:{b21}  b20:{b20}  b12:{b12}   b11:{b11}  b10:{b10}  b02:{b02}  b01:{b01}  b00:{b00}" +
                $"  A:{A}  alpha:{alpha}{Environment.NewLine}register:{DataConverter.BytesToHexString(registerData)}";
        }
    }
    public class ValidationBOE2520
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public ValidationBOE2520() { }

        public ValidationBOE2520(int uid, decimal setP, decimal setT, decimal paceRef, decimal tProbe, int raw_C, int uncalTempCodes)
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

        public void Verify(CalibrationBOE2520 calibrationData)
        {
            Calculation.StartValidation(calibrationData, RAW_C, UNCALTempCodes, out double pCal, out double tCal);
            PCal = pCal;
            TCal = tCal;
            PResidual = PCal - (double)PACERef;
            TResidual = TCal - (double)TProbe;
        }

    }

    public class RawDataZXC6862
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public RawDataZXC6862() { }

        public Int32 uid { get; set; }

        public int T_idx { get; set; }
        public int P_idx { get; set; }
        public decimal SetP { get; set; }
        public decimal SetT { get; set; }
        /// <summary>
        /// 检测的压力值
        /// </summary>
        public decimal PRef { get; set; }
        /// <summary>
        /// 检测的温度值
        /// </summary>
        public decimal TRef { get; set; }
        /// <summary>
        /// 校准前压力原始值
        /// </summary>
        public Int32 PRaw { get; set; }
        /// <summary>
        /// 校准前温度原始值
        /// </summary>
        public Int32 TRaw { get; set; }

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{uid}  setP:{SetP}  setT:{SetT}  PRef:{PRef:N2}  TRef:{TRef:N2}  PRaw:{PRaw}  TRaw:{TRaw}]";
        }

    }
    public class CalibrationZXC6862
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public CalibrationZXC6862() { }

        public Int32 uid { get; set; }
        public int C00 { get; set; }
        public int C01 { get; set; }
        public int C10 { get; set; }
        public int C11 { get; set; }
        public int C20 { get; set; }
        public int C21 { get; set; }
        public int C30 { get; set; }

        public int C0 { get; set; }
        public int C1 { get; set; }

        private string registerString = "";
        public string RegisterString
        {
            get
            {
                registerString = DataConverter.BytesToHexString(registerData);
                return registerString;
            }
            set
            {
                registerString = value;
                registerData = DataConverter.HexStringToBytes(registerString);
            }
        }

        public byte[] registerData = new byte[36];

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{uid}] C00:{C00}  C01:{C01}  C10:{C10}  C11:{C11}  C20:{C20}  C21:{C21}  C30:{C30}" +
                $"  C0:{C0}  C1:{C1}{Environment.NewLine}register:{DataConverter.BytesToHexString(registerData)}";
        }
    }
    public class ValidationZXC6862
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public ValidationZXC6862() { }

        public ValidationZXC6862(int uid, decimal setP, decimal setT, decimal paceRef, decimal tProbe, int raw_C, int uncalTempCodes)
        {
            Uid = uid;
            SetP = setP;
            SetT = setT;
            PRef = paceRef;
            TRef = tProbe;
            PRaw = raw_C;
            TRaw = uncalTempCodes;
        }

        public int Uid { get; set; }
        public decimal SetP { get; set; }
        public decimal SetT { get; set; }
        public decimal PRef { get; set; }
        public decimal TRef { get; set; }
        public Int32 PRaw { get; set; }
        public Int32 TRaw { get; set; }

        public double PCal { get; set; }
        public double TCal { get; set; }

        public double PResidual { get; set; } = 999;
        public double TResidual { get; set; } = 999;

        public string Date { get; set; } = DateTime.Now.ToString("G");

        public string Display()
        {
            return $"[uid:{Uid}  setP:{SetP}  setT:{SetT}  PACERef:{PRef:N2}  TProbe:{TRef:N2}  RAW_C:{PRaw}  UNCALTempCodes:{TRaw}  TCal:{TCal:N2}  PCal:{PCal:N2}  PResidual:{PResidual:N2}  TResidual:{TResidual:N2}]";
        }

        public void Verify(CalibrationZXC6862 calibrationData)
        {
            //Calculation.StartValidation(calibrationData, RAW_C, UNCALTempCodes, out double pCal, out double tCal);
            //PCal = pCal;
            //TCal = tCal;
            //PResidual = PCal - (double)PACERef;
            //TResidual = TCal - (double)TProbe;
        }

    }

    public class RawDataZXW7570
    {

    }
    public class CalibrationZXW7570
    {

    }
    public class ValidationZXW7570
    {

    }
}
