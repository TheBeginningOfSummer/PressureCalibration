using Calibration.Data;
using CSharpKit.DataManagement;
using MathWorks.MATLAB.NET.Arrays;

namespace Module
{
    public class Calculation
    {
        private readonly static Calibration.Method calculator = new();
        private static void B12mapper(int b12Val, ref int b40prog, ref int b12prog)
        {
            if (b12Val > -513 && b12Val < 510)
            {
                b40prog = (int)Math.Floor((decimal)b12Val / 2);
                if (b40prog == b12Val / 2)
                    b12prog = 0;
                else
                    b12prog = 1;
            }
            else if (b12Val < -512)
            {
                b40prog = -256;
                b12prog = b12Val - 2 * b40prog;
            }
            else
            {
                b40prog = 255;
                b12prog = b12Val - 2 * b40prog;
            }

        }
        public static CalibrationData? StartCalibration12(List<RawData> oriData)
        {
            if (oriData == null || oriData.Count == 0) return null;

            int[] ROMData = [0, -75, 3657, 18, -181, -4508, -129, 3025, -207093, 135, -2246, 77723];
            int[] bits = [9, 10, 15, 7, 11, 18, 9, 13, 19, 11, 14, 20];

            CalibrationData data = new() { uid = oriData[0].uid };

            //开始温度拟合
            {
                double[] aT = new double[oriData.Count];
                double[] mT = new double[oriData.Count];

                int index = 0;
                foreach (RawData ori in oriData)
                {
                    aT[index] = Convert.ToDouble(ori.Tens);
                    mT[index] = Convert.ToDouble(ori.TProbe);
                    index++;
                }

                MWNumericArray tens = aT;
                MWNumericArray tProbe = mT;

                MWArray tResult = calculator.TemperatureCalibrationV02(tens, tProbe);
                data.alpha = Convert.ToInt32(tResult.ToArray().GetValue(0, 0)!);
                data.A = Convert.ToInt32(tResult.ToArray().GetValue(0, 1)!);
            }
            //结束温度拟合

            //压力拟合
            {
                double[] P_ref = new double[oriData.Count];
                double[] digT = new double[oriData.Count];
                double[] digC = new double[oriData.Count];

                int index = 0;
                foreach (RawData ori in oriData)
                {
                    P_ref[index] = Convert.ToDouble(ori.PACERef / 100);
                    digT[index] = ori.UNCALTempCodes;
                    digC[index] = ori.RAW_C;
                    index++;
                }

                MWNumericArray para11 = new(15, 1, digT);
                MWArray T = calculator.Raw2Temp(calculator.Temp2Raw(para11, 0, 0), data.alpha, data.A);

                MWNumericArray P = new(15, 1, P_ref);
                
                MWNumericArray C = new(15, 1, digC);

                MWArray pResult = calculator.PresCalCode_V0(P, T, C);

                data.b40 = Convert.ToInt32(pResult.ToArray().GetValue(0, 0)!);
                data.b31 = Convert.ToInt32(pResult.ToArray().GetValue(0, 1)!);
                data.b30 = Convert.ToInt32(pResult.ToArray().GetValue(0, 2)!);
                data.b22 = Convert.ToInt32(pResult.ToArray().GetValue(0, 3)!);
                data.b21 = Convert.ToInt32(pResult.ToArray().GetValue(0, 4)!);
                data.b20 = Convert.ToInt32(pResult.ToArray().GetValue(0, 5)!);
                data.b12 = Convert.ToInt32(pResult.ToArray().GetValue(0, 6)!);
                data.b11 = Convert.ToInt32(pResult.ToArray().GetValue(0, 7)!);
                data.b10 = Convert.ToInt32(pResult.ToArray().GetValue(0, 8)!);
                data.b02 = Convert.ToInt32(pResult.ToArray().GetValue(0, 9)!);
                data.b01 = Convert.ToInt32(pResult.ToArray().GetValue(0, 10)!);
                data.b00 = Convert.ToInt32(pResult.ToArray().GetValue(0, 11)!);
            }

            //计算寄存器的值
            {
                int b31 = data.b31 - ROMData[1];
                int b30 = data.b30 - ROMData[2];
                int b22 = data.b22 - ROMData[3];
                int b21 = data.b21 - ROMData[4];
                int b20 = data.b20 - ROMData[5];
                int b11 = data.b11 - ROMData[7];
                int b10 = data.b10 - ROMData[8];
                int b02 = data.b02 - ROMData[9];
                int b01 = data.b01 - ROMData[10];
                int b00 = data.b00 - ROMData[11];

                int b40 = 0;
                int b12 = 0;
                B12mapper(data.b12, ref b40, ref b12);
                b40 = b40 - ROMData[0];
                b12 = b12 - ROMData[6];

                //计算补码
                //b40 = b40 >= 0 ? b40 : (int)(Math.Pow(2, bits[0]) + b40);
                //b31 = b31 >= 0 ? b31 : (int)(Math.Pow(2, bits[1]) + b31);
                //b30 = b30 >= 0 ? b30 : (int)(Math.Pow(2, bits[2]) + b30);
                //b22 = b22 >= 0 ? b22 : (int)(Math.Pow(2, bits[3]) + b22);
                //b21 = b21 >= 0 ? b21 : (int)(Math.Pow(2, bits[4]) + b21);
                //b20 = b20 >= 0 ? b20 : (int)(Math.Pow(2, bits[5]) + b20);
                //b12 = b12 >= 0 ? b12 : (int)(Math.Pow(2, bits[6]) + b12);
                //b11 = b11 >= 0 ? b11 : (int)(Math.Pow(2, bits[7]) + b11);
                //b10 = b10 >= 0 ? b10 : (int)(Math.Pow(2, bits[8]) + b10);
                //b02 = b02 >= 0 ? b02 : (int)(Math.Pow(2, bits[9]) + b02);
                //b01 = b01 >= 0 ? b01 : (int)(Math.Pow(2, bits[10]) + b01);
                //b00 = b00 >= 0 ? b00 : (int)(Math.Pow(2, bits[11]) + b00);

                b40 = b40 >= 0 ? b40 : BytesTool.GetValueByBit(b40, bits[0]);
                b31 = b31 >= 0 ? b31 : BytesTool.GetValueByBit(b31, bits[1]);
                b30 = b30 >= 0 ? b30 : BytesTool.GetValueByBit(b30, bits[2]);
                b22 = b22 >= 0 ? b22 : BytesTool.GetValueByBit(b22, bits[3]);
                b21 = b21 >= 0 ? b21 : BytesTool.GetValueByBit(b21, bits[4]);
                b20 = b20 >= 0 ? b20 : BytesTool.GetValueByBit(b20, bits[5]);
                b12 = b12 >= 0 ? b12 : BytesTool.GetValueByBit(b12, bits[6]);
                b11 = b11 >= 0 ? b11 : BytesTool.GetValueByBit(b11, bits[7]);
                b10 = b10 >= 0 ? b10 : BytesTool.GetValueByBit(b10, bits[8]);
                b02 = b02 >= 0 ? b02 : BytesTool.GetValueByBit(b02, bits[9]);
                b01 = b01 >= 0 ? b01 : BytesTool.GetValueByBit(b01, bits[10]);
                b00 = b00 >= 0 ? b00 : BytesTool.GetValueByBit(b00, bits[11]);

                //alpha_OTP A_OTP b40 b31 b30 b22 b21 b20 b12 b11 b10 b02 b01 b00
                MWNumericArray nbs = new int[] { 13, 14, 9, 10, 15, 7, 11, 18, 9, 13, 19, 11, 14, 20 };
                MWNumericArray ab = new int[] { data.alpha, data.A, b40, b31, b30, b22, b21, b20, b12, b11, b10, b02, b01, b00 };
                MWArray rr = calculator.ResisterCode(ab, nbs);

                string[] ss = rr.ToString().Split('\n');

                int index = 0;
                foreach (string s in ss)
                {
                    data.registerData[index++] = Convert.ToByte(s, 2);
                }
            }
            //结束计算寄存器的值
            return data;
        }
        public static CalibrationData? StartCalibration9(List<RawData> oriData)
        {
            if (oriData == null || oriData.Count == 0) return null;

            int[] ROMData = [0, -75, 3657, 18, -181, -4508, -129, 3025, -207093, 135, -2246, 77723];
            int[] bits = [9, 10, 15, 7, 11, 18, 9, 13, 19, 11, 14, 20];

            CalibrationData data = new() { uid = oriData[0].uid };

            //开始温度拟合
            {
                double[] aT = new double[oriData.Count];
                double[] mT = new double[oriData.Count];

                int index = 0;
                foreach (RawData ori in oriData)
                {
                    aT[index] = Convert.ToDouble(ori.Tens);
                    mT[index] = Convert.ToDouble(ori.TProbe);
                    index++;
                }

                MWNumericArray tens = aT;
                MWNumericArray tProbe = mT;

                MWArray tResult = calculator.TemperatureCalibrationV02(tens, tProbe);
                data.alpha = Convert.ToInt32(tResult.ToArray().GetValue(0, 0)!);
                data.A = Convert.ToInt32(tResult.ToArray().GetValue(0, 1)!);
            }
            //结束温度拟合

            //压力拟合
            {
                double[] P_ref = new double[oriData.Count];
                double[] digT = new double[oriData.Count];
                double[] digC = new double[oriData.Count];

                int index = 0;
                foreach (RawData ori in oriData)
                {
                    P_ref[index] = Convert.ToDouble(ori.PACERef / 100);
                    digT[index] = ori.UNCALTempCodes;
                    digC[index] = ori.RAW_C;
                    index++;
                }

                MWNumericArray para11 = new(15, 1, digT);
                MWArray T = calculator.Raw2Temp(calculator.Temp2Raw(para11, 0, 0), data.alpha, data.A);

                MWNumericArray P = new(15, 1, P_ref);

                MWNumericArray C = new(15, 1, digC);

                MWArray pResult = calculator.PresCalCode_V1(P, T, C);

                data.b40 = Convert.ToInt32(pResult.ToArray().GetValue(0, 0)!);
                data.b31 = Convert.ToInt32(pResult.ToArray().GetValue(0, 1)!);
                data.b30 = Convert.ToInt32(pResult.ToArray().GetValue(0, 2)!);
                data.b22 = Convert.ToInt32(pResult.ToArray().GetValue(0, 3)!);
                data.b21 = Convert.ToInt32(pResult.ToArray().GetValue(0, 4)!);
                data.b20 = Convert.ToInt32(pResult.ToArray().GetValue(0, 5)!);
                data.b12 = Convert.ToInt32(pResult.ToArray().GetValue(0, 6)!);
                data.b11 = Convert.ToInt32(pResult.ToArray().GetValue(0, 7)!);
                data.b10 = Convert.ToInt32(pResult.ToArray().GetValue(0, 8)!);
                data.b02 = Convert.ToInt32(pResult.ToArray().GetValue(0, 9)!);
                data.b01 = Convert.ToInt32(pResult.ToArray().GetValue(0, 10)!);
                data.b00 = Convert.ToInt32(pResult.ToArray().GetValue(0, 11)!);
            }

            //计算寄存器的值
            {
                int b31 = data.b31 - ROMData[1];
                int b30 = data.b30 - ROMData[2];
                int b22 = data.b22 - ROMData[3];
                int b21 = data.b21 - ROMData[4];
                int b20 = data.b20 - ROMData[5];
                int b11 = data.b11 - ROMData[7];
                int b10 = data.b10 - ROMData[8];
                int b02 = data.b02 - ROMData[9];
                int b01 = data.b01 - ROMData[10];
                int b00 = data.b00 - ROMData[11];

                int b40 = 0;
                int b12 = 0;
                B12mapper(data.b12, ref b40, ref b12);
                b40 = b40 - ROMData[0];
                b12 = b12 - ROMData[6];

                //计算补码
                //b40 = b40 >= 0 ? b40 : (int)(Math.Pow(2, bits[0]) + b40);
                //b31 = b31 >= 0 ? b31 : (int)(Math.Pow(2, bits[1]) + b31);
                //b30 = b30 >= 0 ? b30 : (int)(Math.Pow(2, bits[2]) + b30);
                //b22 = b22 >= 0 ? b22 : (int)(Math.Pow(2, bits[3]) + b22);
                //b21 = b21 >= 0 ? b21 : (int)(Math.Pow(2, bits[4]) + b21);
                //b20 = b20 >= 0 ? b20 : (int)(Math.Pow(2, bits[5]) + b20);
                //b12 = b12 >= 0 ? b12 : (int)(Math.Pow(2, bits[6]) + b12);
                //b11 = b11 >= 0 ? b11 : (int)(Math.Pow(2, bits[7]) + b11);
                //b10 = b10 >= 0 ? b10 : (int)(Math.Pow(2, bits[8]) + b10);
                //b02 = b02 >= 0 ? b02 : (int)(Math.Pow(2, bits[9]) + b02);
                //b01 = b01 >= 0 ? b01 : (int)(Math.Pow(2, bits[10]) + b01);
                //b00 = b00 >= 0 ? b00 : (int)(Math.Pow(2, bits[11]) + b00);

                b40 = b40 >= 0 ? b40 : BytesTool.GetValueByBit(b40, bits[0]);
                b31 = b31 >= 0 ? b31 : BytesTool.GetValueByBit(b31, bits[1]);
                b30 = b30 >= 0 ? b30 : BytesTool.GetValueByBit(b30, bits[2]);
                b22 = b22 >= 0 ? b22 : BytesTool.GetValueByBit(b22, bits[3]);
                b21 = b21 >= 0 ? b21 : BytesTool.GetValueByBit(b21, bits[4]);
                b20 = b20 >= 0 ? b20 : BytesTool.GetValueByBit(b20, bits[5]);
                b12 = b12 >= 0 ? b12 : BytesTool.GetValueByBit(b12, bits[6]);
                b11 = b11 >= 0 ? b11 : BytesTool.GetValueByBit(b11, bits[7]);
                b10 = b10 >= 0 ? b10 : BytesTool.GetValueByBit(b10, bits[8]);
                b02 = b02 >= 0 ? b02 : BytesTool.GetValueByBit(b02, bits[9]);
                b01 = b01 >= 0 ? b01 : BytesTool.GetValueByBit(b01, bits[10]);
                b00 = b00 >= 0 ? b00 : BytesTool.GetValueByBit(b00, bits[11]);

                //alpha_OTP A_OTP b40 b31 b30 b22 b21 b20 b12 b11 b10 b02 b01 b00
                MWNumericArray nbs = new int[] { 13, 14, 9, 10, 15, 7, 11, 18, 9, 13, 19, 11, 14, 20 };
                MWNumericArray ab = new int[] { data.alpha, data.A, b40, b31, b30, b22, b21, b20, b12, b11, b10, b02, b01, b00 };
                MWArray rr = calculator.ResisterCode(ab, nbs);

                string[] ss = rr.ToString().Split('\n');

                int index = 0;
                foreach (string s in ss)
                {
                    data.registerData[index++] = Convert.ToByte(s, 2);
                }
            }
            //结束计算寄存器的值
            return data;
        }
        /// <summary>
        /// 校验标定系数
        /// </summary>
        /// <param name="calibPara">标定系数</param>
        /// <param name="y">传感器读取压力值</param>
        /// <param name="t">传感器读取温度值</param>
        /// <param name="p">实际压力值</param>
        /// <param name="Pcal">传感器读取值经系数校正的压力值</param>
        /// <param name="residual">传感器压力值与压控实际压力值的差值</param>
        public static void StartValidation(CalibrationData calibPara, double y, double t, double p, ref double Pcal, ref double residual)
        {
            double[] digT = [t];
            MWNumericArray para11 = new MWNumericArray(1, 1, digT);
            MWArray digT1 = calculator.Raw2Temp(calculator.Temp2Raw(para11, 0, 0), calibPara.alpha, calibPara.A);

            y = (-4682800 + Math.Pow(2, 43) / ((y - 349526) / 2)) / Math.Pow(2, 21);
            t = ((double)digT1.ToArray().GetValue(0, 0)! - 30145) / Math.Pow(2, 12);

            double P_poly = calibPara.b40 * Math.Pow(y, 4) + calibPara.b31 * Math.Pow(y, 3) * t + calibPara.b30 * Math.Pow(y, 3) + calibPara.b22 * Math.Pow(y, 2) *
                Math.Pow(t, 2) + calibPara.b21 * Math.Pow(y, 2) * t + calibPara.b20 * Math.Pow(y, 2) +
                calibPara.b12 * y * Math.Pow(t, 2) + calibPara.b11 * y * t + calibPara.b10 * y +
                calibPara.b02 * Math.Pow(t, 2) + calibPara.b01 * t + calibPara.b00;

            P_poly = P_poly / Math.Pow(2, 18);

            Pcal = (Math.Pow(2, 16) * P_poly + 75000) / 100;
            residual = Pcal - p;
        }
        /// <summary>
        /// 校验标定系数
        /// </summary>
        /// <param name="calibPara">标定系数</param>
        /// <param name="raw_C">传感器读取压力值</param>
        /// <param name="uncalTempCodes">传感器读取温度值</param>
        /// <param name="paceRef">实际压力值</param>
        /// <param name="Pcal">传感器读取值经系数校正的压力值</param>
        /// <param name="residual">传感器压力值与压控实际压力值的差值</param>
        /// <param name="tCal">系数校正后的温度值</param>
        public static void StartValidation(CalibrationData calibPara, double raw_C, double uncalTempCodes, out double pCal, out double tCal)
        {
            MWNumericArray para11 = new(1, 1, [uncalTempCodes]);
            MWArray digT = calculator.Raw2Temp(calculator.Temp2Raw(para11, 0, 0), calibPara.alpha, calibPara.A);

            double C_poly = (-4682800 + Math.Pow(2, 43) / ((raw_C - 349526) / 2)) / Math.Pow(2, 21);
            double T_poly = ((double)digT.ToArray().GetValue(0, 0)! - 30145) / Math.Pow(2, 12);
            double P_poly = calibPara.b40 * Math.Pow(C_poly, 4) + calibPara.b31 * Math.Pow(C_poly, 3) * T_poly + calibPara.b30 * Math.Pow(C_poly, 3) + calibPara.b22 * Math.Pow(C_poly, 2) *
                Math.Pow(T_poly, 2) + calibPara.b21 * Math.Pow(C_poly, 2) * T_poly + calibPara.b20 * Math.Pow(C_poly, 2) +
                calibPara.b12 * C_poly * Math.Pow(T_poly, 2) + calibPara.b11 * C_poly * T_poly + calibPara.b10 * C_poly +
                calibPara.b02 * Math.Pow(T_poly, 2) + calibPara.b01 * T_poly + calibPara.b00;

            P_poly /= Math.Pow(2, 18);

            pCal = (Math.Pow(2, 16) * P_poly + 75000);
            tCal = (double)digT.ToArray().GetValue(0, 0)! / 128 - 273.15;
        }

        public static double GetPCal(double p)
        {
            p /= Math.Pow(2, 18);
            var PCal = (Math.Pow(2, 16) * p + 75000) / 100;
            return PCal;
        }

        public static double GetTCal(double t, int alpha, int A)
        {
            double[] digT = [t];
            MWNumericArray para11 = new MWNumericArray(1, 1, digT);
            MWArray digT1 = calculator.Raw2Temp(calculator.Temp2Raw(para11, 0, 0), alpha, A);
            double T = (double)digT1.ToArray().GetValue(0, 0)!;
            return T / 128 - 273.15;
        }

    }
}
