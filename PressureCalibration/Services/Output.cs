using System.Collections.Concurrent;
using System.Reflection;
using Calibration.Data;
using OfficeOpenXml;

namespace Calibration.Services
{
    public class ExcelOutput
    {
        public static bool Output<T>(T data, string path = "Data\\TempTest\\", string name = "")
        {
            if (data == null) return false;
            string dataDirectory = Application.StartupPath + path;
            if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);
            string fileName = $"{dataDirectory}[{DateTime.Now:yyyy-MM-dd HHmmss}]{name}.xlsx";
            if (typeof(T) == typeof(SensorCalibration))
                SaveExcel((SensorCalibration)(object)data, fileName);
            else if (typeof(T) == typeof(ConcurrentDictionary<int, GroupCalibration>))
                SaveExcel((ConcurrentDictionary<int, GroupCalibration>)(object)data, fileName);
            else if (typeof(T) == typeof(List<TempTest>))
                SaveExcel((List<TempTest>)(object)data, fileName);
            else if (typeof(T) == typeof(List<PressureTest>))
                SaveExcel((List<PressureTest>)(object)data, fileName);
            else
                return false;
            return true;
        }

        public static void AddData<T>(T data, ExcelWorksheet worksheet, ref int number)
        {
            if (data == null) return;
            if (number < 1) return;
            PropertyInfo[] properties = data.GetType().GetProperties();
            if (number == 1)
            {
                worksheet.Cells[number, 1].Value = "序号";
                for (int j = 0; j < properties.Length; j++)
                    worksheet.Cells[number, j + 2].Value = properties[j].Name;
            }
            for (int j = 0; j < properties.Length; j++)
            {
                worksheet.Cells[number + 1, 1].Value = number;
                worksheet.Cells[number + 1, j + 2].Value = properties[j].GetValue(data);
            }
            number++;
        }

        public static void AddData<T>(List<T> data, ExcelWorksheet worksheet, ref int number)
        {
            if (data == null) return;
            if (number < 1) return;
            for (int i = 0; i < data.Count; i++)
            {
                PropertyInfo[] properties = data[i]!.GetType().GetProperties();
                if (number == 1)
                {
                    worksheet.Cells[number, 1].Value = "序号";
                    for (int j = 0; j < properties.Length; j++)
                        worksheet.Cells[number, j + 2].Value = properties[j].Name;
                }
                for (int j = 0; j < properties.Length; j++)
                {
                    worksheet.Cells[number + 1, 1].Value = number;
                    worksheet.Cells[number + 1, j + 2].Value = properties[j].GetValue(data[i]);
                }
                number++;
            }
        }

        public static void SaveExcel(SensorCalibration sensorData, string flieName = @"d:\myExcel.xlsx")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo file = new(flieName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(flieName);
            }
            using (ExcelPackage myExcelPackage = new(file))
            {
                int index1 = 1;
                //创建ExcelWorkSheet对象，这个对象就是面对表的，是工作簿中单个表
                ExcelWorksheet worksheet1 = myExcelPackage.Workbook.Worksheets.Add("Sheet1");
                AddData(sensorData.CurrentRawData, worksheet1, ref index1);
                int index2 = 1;
                ExcelWorksheet worksheet2 = myExcelPackage.Workbook.Worksheets.Add("Sheet2");
                AddData(sensorData.CurrentValidationData, worksheet2, ref index2);
                ExcelWorksheet worksheet3 = myExcelPackage.Workbook.Worksheets.Add("Sheet3");
                int index3 = 1;
                AddData(sensorData.CurrentCalibrationData, worksheet3, ref index3);
                worksheet3.Cells[1, 18].Value = "Register";
                worksheet3.Cells[index3, 18].Value = sensorData.CurrentCalibrationData.RegisterString;//index3已在AddData方法+1此处不用+1
                //save方法就保存我们这个对象，他就会去执行我们刚刚赋值的那些东西
                myExcelPackage.Save();
                Thread.Sleep(500);
            }
        }

        public static void SaveExcel(ConcurrentDictionary<int, GroupCalibration> allSensorData, string flieName = @"d:\myExcel.xlsx")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo file = new(flieName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(flieName);
            }
            using (ExcelPackage myExcelPackage = new(file))
            {
                int index1 = 1;
                //创建ExcelWorkSheet对象，这个对象就是面对表的，是工作簿中单个表
                ExcelWorksheet worksheet1 = myExcelPackage.Workbook.Worksheets.Add("Sheet1");
                int index2 = 1;
                ExcelWorksheet worksheet2 = myExcelPackage.Workbook.Worksheets.Add("Sheet2");
                ExcelWorksheet worksheet3 = myExcelPackage.Workbook.Worksheets.Add("Sheet3");
                int index3 = 1;
                worksheet3.Cells[1, 18].Value = "Register";
                foreach (var group in allSensorData.Values)
                {
                    for (int i = 0; i < group.SensorDataGroup.Count; i++)
                    {
                        SensorCalibration sensorData = group.SensorDataGroup[i];
                        AddData(sensorData.CurrentRawData, worksheet1, ref index1);
                        AddData(sensorData.CurrentValidationData, worksheet2, ref index2);
                        worksheet3.Cells[index3 + 1, 18].Value = sensorData.CurrentCalibrationData.RegisterString;
                        AddData(sensorData.CurrentCalibrationData, worksheet3, ref index3);
                    }
                }
                myExcelPackage.Save();
                Thread.Sleep(500);
            }
        }

        public static void SaveExcel(List<TempTest> testData, string flieName = @"d:\myExcel.xlsx")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo file = new(flieName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(flieName);
            }
            Dictionary<string, ExcelWorksheet> tempDic = [];
            using (ExcelPackage myExcelPackage = new(file))
            {
                var sheet = myExcelPackage.Workbook.Worksheets.Add("All");
                for (int i = 0; i < testData.Count; i++)
                {
                    string date = testData[i].Date;
                    sheet.Cells[i + 2, 1].Value = date;

                    for (int j = 0; j < testData[i].TempList.Count; j++)
                    {
                        if (i == 0)
                        {
                            sheet.Cells[1, 1].Value = "时间";
                            sheet.Cells[1, 2 + 4 * j].Value = $"D{j + 1}(T1℃)";
                            sheet.Cells[1, 3 + 4 * j].Value = $"D{j + 1}(T2℃)";
                            sheet.Cells[1, 4 + 4 * j].Value = $"D{j + 1}(T3℃)";
                            sheet.Cells[1, 5 + 4 * j].Value = $"D{j + 1}(T4℃)";
                        }
                        for (int k = 0; k < testData[i].TempList[j].Length; k++)
                        {
                            sheet.Cells[i + 2, 1].Value = date;
                            sheet.Cells[i + 2, k + 2 + 4 * j].Value = testData[i].TempList[j][k];
                        }
                        if (!tempDic.ContainsKey($"Device{j}"))
                        {
                            //创建ExcelWorkSheet对象，这个对象就是面对表的，是工作簿中单个表
                            tempDic.Add($"Device{j}", myExcelPackage.Workbook.Worksheets.Add($"Device{j + 1}"));
                            if (i == 0)
                            {
                                tempDic[$"Device{j}"].Cells[1, 1].Value = "时间";
                                tempDic[$"Device{j}"].Cells[1, 2].Value = "T1℃";
                                tempDic[$"Device{j}"].Cells[1, 3].Value = "T2℃";
                                tempDic[$"Device{j}"].Cells[1, 4].Value = "T3℃";
                                tempDic[$"Device{j}"].Cells[1, 5].Value = "T4℃";
                            }
                            for (int k = 0; k < testData[i].TempList[j].Length; k++)
                            {
                                tempDic[$"Device{j}"].Cells[i + 2, 1].Value = date;
                                tempDic[$"Device{j}"].Cells[i + 2, k + 2].Value = testData[i].TempList[j][k];
                            }
                        }
                        else
                        {
                            for (int k = 0; k < testData[i].TempList[j].Length; k++)
                            {
                                tempDic[$"Device{j}"].Cells[i + 2, 1].Value = date;
                                tempDic[$"Device{j}"].Cells[i + 2, k + 2].Value = testData[i].TempList[j][k];
                            }
                        }
                    }
                }
                myExcelPackage.Save();
            }
        }

        public static void SaveExcel(List<PressureTest> testData, string flieName = @"d:\myExcel.xlsx")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo file = new(flieName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(flieName);
            }
            using (ExcelPackage myExcelPackage = new(file))
            {
                ExcelWorksheet sheet = myExcelPackage.Workbook.Worksheets.Add("Sheet1");
                for (int i = 0; i < testData.Count; i++)
                {
                    if (i == 0)
                    {
                        sheet.Cells[1, 1].Value = "时间";
                        sheet.Cells[1, 2].Value = $"压力(Pa)";
                    }
                    sheet.Cells[i + 2, 1].Value = testData[i].Date;
                    sheet.Cells[i + 2, 2].Value = testData[i].Pressure;
                }
                myExcelPackage.Save();
            }
        }

    }
}
