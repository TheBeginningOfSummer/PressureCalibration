using CommunityToolkit.Mvvm.Input;
using CSharpKit.FileManagement;
using Module;
using System.ComponentModel;
using System.Reflection;
using WinformKit;

namespace PressureCalibration.View
{
    public partial class Setting : Form
    {
        public Point IniPoint = new(30, 30);
        public Point Interval = new(400, 30);
        public int RowCount = 12;
        public int Offset = 80;

        private List<ParameterManager> settingPara = [];
        //参数设置的所有控件
        readonly Dictionary<string, Control> settingBoxesDic = [];
        //绑定的列表数组
        readonly Dictionary<string, BindingList<decimal>> bindingListDic = [];

        public Setting()
        {
            InitializeComponent();

            settingPara.Add(ParameterManager.Get<CalibrationParameter>());
            settingPara.Add(ParameterManager.Get<Acquisition>());
            settingPara.Add(ParameterManager.Get<PressController>());
            settingPara.Add(ParameterManager.Get<TECController>());
            for (int i = 0; i < settingPara.Count; i++)
                Initialize(settingPara[i], HTP设置.TabPages[i]);
        }

        public void Initialize(ParameterManager parameter, Control parent)
        {
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            int j = 0;
            PropertyInfo[] properties;
            #region 通信属性设置
            if (parameter.SocketPara != null)
            {
                properties = parameter.SocketPara.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (property.Name == "Parity" || property.Name == "DataBits" || property.Name == "StopBits") continue;
                    TextBox tb = FormKit.AddSettingBox(parent,
                             FormKit.ControlFactory<Label>(new Point(x, y), $"[LB]({j}){property.Name}", parameter.SocketPara!.Translate(property.Name)),
                             FormKit.ControlFactory<TextBox>(new Point(x, y), $"[TB]({j}){property.Name}", property.GetValue(parameter.SocketPara)!.ToString()!, new Size(50, 25)), Offset);
                    if (property.Name == "Ip") tb.Width = 100;
                    settingBoxesDic.TryAdd(property.Name, tb);

                    y += Interval.Y; j++;
                    if (j % RowCount == 0)
                    {
                        x += Interval.X;
                        y = IniPoint.Y;
                    }
                }
            }
            if (parameter.SerialPara != null)
            {
                properties = parameter.SerialPara.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (property.Name == "Parity" || property.Name == "DataBits" || property.Name == "StopBits") continue;
                    TextBox tb = FormKit.AddSettingBox(parent,
                             FormKit.ControlFactory<Label>(new Point(x, y), $"[LB]({j}){property.Name}", parameter.SerialPara!.Translate(property.Name)),
                             FormKit.ControlFactory<TextBox>(new Point(x, y), $"[TB]({j}){property.Name}", property.GetValue(parameter.SerialPara)!.ToString()!, new Size(50, 25)), Offset);
                    if (property.Name == "Ip") tb.Width = 100;
                    settingBoxesDic.TryAdd(property.Name, tb);

                    y += Interval.Y; j++;
                    if (j % RowCount == 0)
                    {
                        x += Interval.X;
                        y = IniPoint.Y;
                    }
                }
            }
            #endregion

            #region 参数属性设置
            properties = parameter.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "FilePath" || property.Name == "FileName" || property.Name == "SerialPara" || property.Name == "SocketPara") continue;
                if (property.Name == "CurrentState" || property.Name == "Instance") continue;
                if (property.GetValue(parameter) is List<decimal> list)
                {
                    BindingList<decimal> listData = [];
                    foreach (var item in list) listData.Add(item);
                    bindingListDic.TryAdd(property.Name, listData);

                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"[CB]{property.Name}", parameter.Translate(property.Name), "", listData, 60, Offset);
                    cBox.SelectedIndexChanged += CBox_SelectedIndexChanged;
                    cBox.Tag = 0;
                    settingBoxesDic.TryAdd(property.Name, cBox);

                    var b1 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 150, y - 5), $"[BN]{property.Name}添加", "添加", new Size(40, 25), Color.DarkGray, Color.White, cmdPara: new Dictionary<string, string>() { [property.Name] = "Add" }));
                    var b2 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 190, y - 5), $"[BN]{property.Name}更改", "更改", new Size(40, 25), Color.DarkGray, Color.White, cmdPara: new Dictionary<string, string>() { [property.Name] = "Change" }));
                    var b3 = FormKit.AddControl(parent, FormKit.ControlFactory<Button>
                        (new Point(x + 230, y - 5), $"[BN]{property.Name}删除", "删除", new Size(40, 25), Color.DarkGray, Color.White, cmdPara: new Dictionary<string, string>() { [property.Name] = "Delete" }));
                    b1.Command = new RelayCommand<object>(ChangeItem);
                    b2.Command = new RelayCommand<object>(ChangeItem);
                    b3.Command = new RelayCommand<object>(ChangeItem);
                    b1.Font = new Font("Segoe UI", 7f);
                    b2.Font = new Font("Segoe UI", 7f);
                    b3.Font = new Font("Segoe UI", 7f);
                }
                else if (property.GetValue(parameter) is bool onOff)
                {
                    var checkBox = FormKit.AddSettingBox<CheckBox>(parent, new Point(x, y), $"[CHB]{property.Name}", parameter.Translate(property.Name), "", xOffset: Offset);
                    checkBox.Checked = onOff;
                    settingBoxesDic.TryAdd(property.Name, checkBox);
                }
                else
                {
                    var textBox = FormKit.AddSettingBox<TextBox>(parent, new Point(x, y), $"[TB]{property.Name}", parameter.Translate(property.Name), property.GetValue(parameter)!.ToString()!, xOffset: Offset);
                    settingBoxesDic.TryAdd(property.Name, textBox);
                }

                y += Interval.Y; j++;
                if (j % RowCount == 0)
                {
                    x += Interval.X;
                    y = IniPoint.Y;
                }
            }
            #endregion
        }

        private void CBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
                comboBox.Tag = comboBox.SelectedIndex;
        }

        private void ChangeItem(object? argument)
        {
            if (argument is Dictionary<string, string> parameter)
            {
                string propertyName = parameter.Keys.First();
                string command = parameter.Values.First();
                ComboBox settingBoxe = (ComboBox)settingBoxesDic[propertyName];
                BindingList<decimal> bindingData = bindingListDic[propertyName];
                switch (command)
                {
                    case "Add":
                        if (decimal.TryParse(settingBoxe.Text, out decimal addValue))
                        {
                            bindingListDic[propertyName].Add(addValue);
                            FormKit.ShowInfoBox("已添加");
                        }
                        break;
                    case "Change":
                        if (bindingData.Count <= 0) return;
                        if (decimal.TryParse(settingBoxe.Text, out decimal changeValue))
                        {
                            bindingData[(int)settingBoxe.Tag!] = changeValue;
                            FormKit.ShowInfoBox("已修改");
                        }
                        break;
                    case "Delete":
                        if (bindingData.Count <= 0) return;
                        bindingData.RemoveAt((int)settingBoxe.Tag!);
                        FormKit.ShowInfoBox($"已删除");
                        break;
                }
            }
        }

        private void BTN保存_Click(object sender, EventArgs e)
        {
            if (settingPara is null) return;
            Dictionary<string, object> saveDic = [];
            foreach (var item in bindingListDic)
                saveDic.Add(item.Key, item.Value.ToList());
            foreach (var item in settingBoxesDic)
            {
                if (item.Value is CheckBox checkBox)
                    saveDic.TryAdd(item.Key, checkBox.Checked);
                else
                    saveDic.TryAdd(item.Key, item.Value.Text);
            }
            if (settingPara[HTP设置.SelectedIndex].Save(saveDic))
                MessageBox.Show("保存成功。", "提示");
        }

    }
}
