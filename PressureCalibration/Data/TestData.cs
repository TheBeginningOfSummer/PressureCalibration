namespace Data
{
    public class TempTest
    {
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
        public List<decimal[]> Pressure { get; set; } = [];
        public List<decimal[]> Temperature { get; set; } = [];
    }
}
