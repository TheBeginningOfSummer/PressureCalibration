using CSharpKit.DataManagement;
using SQLite;

namespace Calibration.Data
{
    public class CalibrationData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public CalibrationData() { }

        public Int32 uid { get; set; }
        public int b40 {  get; set; }
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
        public int A {  get; set; }
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
}
