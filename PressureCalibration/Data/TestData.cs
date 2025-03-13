namespace Data
{
    public class TempTest
    {
        public string Date { get; set; } = DateTime.Now.ToString("HH:mm:ss");
        public List<decimal[]> TempList { get; set; } = [];
    }

    public class PressureTest
    {
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public decimal Pressure { get; set; }

        public PressureTest(string date, decimal pressure)
        {
            Date = date;
            Pressure = pressure;
        }

    }

    public class SensorTest
    {
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string GUID { get; set; } = "";
        public int UID { get; set; }
        public int Raw_C { get; set; }
        public int TempCodes { get; set; }
        public double Pressure { get; set; }
        public double Temp { get; set; }

        public string Show()
        {
            return $"{Date} {GUID} {UID} {Raw_C} {TempCodes} {Pressure}";
        }
    }
}
