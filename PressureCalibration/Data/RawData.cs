using SQLite;

namespace Calibration.Data
{
    public class RawData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public RawData() { }

        public Int32 uid { get; set; }

        public int T_idx { get; set; }  
        public int P_idx { get; set; }
        public decimal SetP { get; set; }
        public decimal SetT {  get; set; }
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
        public Int32 RAW_C {  get; set; }
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
}
