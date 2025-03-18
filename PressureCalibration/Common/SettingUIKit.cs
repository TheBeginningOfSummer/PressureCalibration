using CommunityToolkit.Mvvm.Input;
using CSharpKit;
using CSharpKit.Communication;
using CSharpKit.FileManagement;
using Microsoft.VisualBasic;
using Services;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using WinformKit;

namespace UIKit
{
    public class SettingUIKit
    {

    }

    public class SettingInfo
    {
        //需要设置的类
        public Loader? SettingPara { get; set; }
        public Action<object>? CommandAction;

        public Point IniPoint = new(30, 30);
        public Point Interval = new(400, 30);
        public int MaxRowCount = 12;
        public int Offset = 80;

        public SettingInfo(Loader settingPara)
        {
            SettingPara = settingPara;
        }

        public SettingInfo()
        {

        }

        public void Initialize(Control parent)
        {
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            int rowCount = 0;

            #region 通信属性设置
            SetChildParameter(parent, SettingPara?.SocketPort, ref x, ref y, ref rowCount);
            SetChildParameter(parent, SettingPara?.SerialPort, ref x, ref y, ref rowCount, "Parity", "DataBits", "StopBits", "Name");
            SetParameter(parent, SettingPara, ref x, ref y, ref rowCount, "CurrentState", "Instance", "Axes");
            #endregion
        }

        public void SetParameter<T>(Control parent, T? instance, ref int x, ref int y, ref int rowCount, params string[] ignore) where T : Loader
        {
            if (instance == null) return;
            BindingSource source = [];
            source.DataSource = instance;
            PropertyInfo[] properties = instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                //跳过的属性
                if (ignore.Contains(property.Name)) continue;
                if (property.Name == "SerialPort" || property.Name == "SocketPort") continue;
                //属性分类
                if (property.GetValue(instance) is bool)
                {
                    var checkBox = FormKit.AddSettingBox<CheckBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", xOffset: Offset);
                    checkBox.DataBindings.Add("Checked", instance, property.Name, false, DataSourceUpdateMode.OnPropertyChanged);
                }
                else if (property.GetValue(instance) is BindingList<decimal> decimalList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", decimalList, 60, Offset);
                    cBox.Tag = property.Name;
                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]添加", "添加", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));
                    var b2 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 190, y - 5), $"BN[{property.Name}]更改", "更改", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));
                    var b3 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 230, y - 5), $"BN[{property.Name}]删除", "删除", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));

                    b1.Command = new RelayCommand<object>(AddItem);
                    b2.Command = new RelayCommand<object>(ChangeItem);
                    b3.Command = new RelayCommand<object>(RemoveItem);

                    b1.Font = new Font("Segoe UI", 7f);
                    b2.Font = new Font("Segoe UI", 7f);
                    b3.Font = new Font("Segoe UI", 7f);
                }
                else if (property.GetValue(instance) is BindingList<string> stringList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", stringList, 60, Offset);
                    
                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]添加", "添加", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    var b2 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 190, y - 5), $"BN[{property.Name}]设置", "设置", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    var b3 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 230, y - 5), $"BN[{property.Name}]删除", "删除", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    b1.Command = new RelayCommand<object>(AddItem);
                    b2.Command = new RelayCommand<object>(SetItem);
                    b3.Command = new RelayCommand<object>(RemoveItem);
                    b1.Font = new Font("Segoe UI", 7f);
                    b2.Font = new Font("Segoe UI", 7f);
                    b3.Font = new Font("Segoe UI", 7f);
                }
                else if (property.GetValue(instance) is BindingList<SerialPortTool> comList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", comList, 60, Offset);
                    cBox.DisplayMember = "Name";
                    cBox.ValueMember = "Name";
                    cBox.Tag = property.Name;

                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]设置", "设置", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    b1.Command = new RelayCommand<object>(SetItem);
                    b1.Font = new Font("Segoe UI", 7f);
                }
                else
                {
                    var propertyValue = property.GetValue(instance); string value = "";
                    if (propertyValue != null) value = propertyValue.ToString()!;
                    var textBox = FormKit.AddSettingBox<TextBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), value, xOffset: Offset);
                    textBox.DataBindings.Add("Text", instance, property.Name);
                    if (property.Name == "Ip") textBox.Width = 100;
                }
                //列变化
                y += Interval.Y; rowCount++;
                if (rowCount % MaxRowCount == 0)
                {
                    x += Interval.X;
                    y = IniPoint.Y;
                }
            }
        }

        public void SetChildParameter<T>(Control parent, T? instance, ref int x, ref int y, ref int rowCount, params string[] ignore) where T : ISetting
        {
            if (instance == null) return;
            BindingSource source = [];
            source.DataSource = instance;
            PropertyInfo[] properties = instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                //跳过的属性
                if (ignore.Contains(property.Name)) continue;
                if (property.Name == "SerialPort" || property.Name == "SocketPort") continue;
                //属性分类
                if (property.GetValue(instance) is bool)
                {
                    var checkBox = FormKit.AddSettingBox<CheckBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", xOffset: Offset);
                    checkBox.DataBindings.Add("Checked", source, property.Name);
                }
                else if (property.GetValue(instance) is BindingList<decimal> decimalList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", decimalList, 60, Offset);
                    cBox.Tag = property.Name;
                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]添加", "添加", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));
                    var b2 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 190, y - 5), $"BN[{property.Name}]更改", "更改", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));
                    var b3 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 230, y - 5), $"BN[{property.Name}]删除", "删除", new Size(40, 25), Color.DodgerBlue, Color.White,
                        cmdPara: cBox));

                    b1.Command = new RelayCommand<object>(AddItem);
                    b2.Command = new RelayCommand<object>(ChangeItem);
                    b3.Command = new RelayCommand<object>(RemoveItem);

                    b1.Font = new Font("Segoe UI", 7f);
                    b2.Font = new Font("Segoe UI", 7f);
                    b3.Font = new Font("Segoe UI", 7f);
                }
                else if (property.GetValue(instance) is BindingList<string> stringList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", stringList, 60, Offset);

                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]添加", "添加", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    var b2 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 190, y - 5), $"BN[{property.Name}]设置", "设置", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    var b3 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 230, y - 5), $"BN[{property.Name}]删除", "删除", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    b1.Command = new RelayCommand<object>(AddItem);
                    b2.Command = new RelayCommand<object>(SetItem);
                    b3.Command = new RelayCommand<object>(RemoveItem);
                    b1.Font = new Font("Segoe UI", 7f);
                    b2.Font = new Font("Segoe UI", 7f);
                    b3.Font = new Font("Segoe UI", 7f);
                }
                else if (property.GetValue(instance) is BindingList<SerialPortTool> comList)
                {
                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), "", comList, 60, Offset);
                    cBox.DisplayMember = "Name";
                    cBox.ValueMember = "Name";
                    cBox.Tag = property.Name;
                    cBox.DropDownStyle = ComboBoxStyle.DropDownList;

                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"BN[{property.Name}]设置", "设置", new Size(40, 25), Color.DodgerBlue, Color.White, cmdPara: cBox));
                    b1.Command = new RelayCommand<object>(SetItem);
                    b1.Font = new Font("Segoe UI", 7f);
                }
                else
                {
                    var propertyValue = property.GetValue(instance); string value = "";
                    if (propertyValue != null) value = propertyValue.ToString()!;
                    var textBox = FormKit.AddSettingBox<TextBox>(parent, new Point(x, y), $"{property.Name}",
                        instance.Translate($"{property.Name}"), value, xOffset: Offset);
                    textBox.DataBindings.Add("Text", source, property.Name);
                    if (property.Name == "Ip") textBox.Width = 100;
                }
                //列变化
                y += Interval.Y; rowCount++;
                if (rowCount % MaxRowCount == 0)
                {
                    x += Interval.X;
                    y = IniPoint.Y;
                }
            }
        }

        private static string GetPropName(string name)
        {
            //return Regex.Match(name, @"(?<=\[)([^\[\]]*)(?=\])").Value;
            return Regex.Match(name, @"\[([^\[\]]*)\]").Groups[1].Value;
        }

        #region 集合属性操作
        private void AddItem(object? argument)
        {
            if (argument is ComboBox cBox)
            {
                //得到输入的数据
                string input = Interaction.InputBox($"请输入要添加的值：", "提示", "");
                if (input == "") return;
                //得到要修改数据
                string propName = GetPropName(cBox.Name);
                var propInfo = SettingPara?.GetType().GetProperty(propName);
                if (propInfo == null) return;
                if (propInfo.GetValue(SettingPara) is BindingList<decimal> list)
                {
                    if (!decimal.TryParse(input, out var decimalValue)) return;
                    list.Add(decimalValue);
                    CommandAction?.Invoke("Add");
                    FormKit.ShowInfoBox("已添加");
                }
                else if (propInfo.GetValue(SettingPara) is BindingList<string> stringList)
                {
                    stringList.Add(input);
                    CommandAction?.Invoke("Add");
                    FormKit.ShowInfoBox("已添加");
                }
            }
        }

        private void ChangeItem(object? argument)
        {
            if (argument is ComboBox cBox)
            {
                //得到输入的数据
                string input = Interaction.InputBox($"请输入修改值：", "提示", "");
                if (input == "") return;
                //得到要修改数据
                string propName = GetPropName(cBox.Name);
                var propInfo = SettingPara?.GetType().GetProperty(propName);
                if (propInfo == null) return;
                if (propInfo.GetValue(SettingPara) is BindingList<decimal> list)
                {
                    if (list.Count == 0) return;
                    if (!decimal.TryParse(input, out var decimalValue)) return;
                    list[cBox.SelectedIndex] = decimalValue;
                    FormKit.ShowInfoBox("已修改");
                }
            }
        }

        private void RemoveItem(object? argument)
        {
            if (argument is ComboBox cBox)
            {
                string propName = GetPropName(cBox.Name);
                var propInfo = SettingPara?.GetType().GetProperty(propName);
                if (propInfo == null) return;
                if (propInfo.GetValue(SettingPara) is BindingList<decimal> list)
                {
                    if (list.Count == 0) return;
                    list.RemoveAt(cBox.SelectedIndex);
                    CommandAction?.Invoke("Remove");
                    FormKit.ShowInfoBox($"已删除");
                }
                else if (propInfo.GetValue(SettingPara) is BindingList<string> stringList)
                {
                    if (stringList.Count == 0) return;
                    stringList.RemoveAt(cBox.SelectedIndex);
                    CommandAction?.Invoke("Remove");
                    FormKit.ShowInfoBox($"已删除");
                }
            }
        }

        private void SetItem(object? argument)
        {
            if (argument is ComboBox cBox)
            {
                if (SettingPara is ZmotionMotionControl zmotion)
                {
                    if (zmotion.Axes.TryGetValue(cBox.Text, out ZmotionAxis? axis))
                        CommandAction?.Invoke(axis);
                }
                else
                {
                    if(cBox.Tag is string name)
                    {
                        var info = SettingPara?.GetType().GetProperty(name);
                        if (info == null) return;
                        if (info.GetValue(SettingPara) is BindingList<SerialPortTool> comList)
                            CommandAction?.Invoke(comList[cBox.SelectedIndex]);
                    }
                }
            }
        }
        #endregion

        public void Save()
        {
            if (SettingPara == null) return;
            if (SettingPara.Save())
                MessageBox.Show("保存成功。", "提示");
        }

    }

}
