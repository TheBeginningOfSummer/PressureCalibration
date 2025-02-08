using CommunityToolkit.Mvvm.Input;
using CSharpKit.FileManagement;
using Module;
using Services;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using WinformKit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace PressureCalibration.View
{
    public partial class Setting : Form
    {
        readonly List<SettingInfo> settingInfos = [];

        public Setting()
        {
            InitializeComponent();

            settingInfos.Add(new SettingInfo(ParameterManager.Get<CalibrationParameter>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<Acquisition>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<PressController>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<TECController>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<ZmotionMotionControl>()));
            for (int i = 0; i < settingInfos.Count; i++)
                settingInfos[i].Initialize(HTP设置.TabPages[i]);
        }

        private void BTN保存_Click(object sender, EventArgs e)
        {
            settingInfos[HTP设置.SelectedIndex].Save();
        }

    }

    public class SettingInfo
    {
        //需要设置的类
        public ParameterManager SettingPara { get; set; }
        //参数设置的所有控件
        public Dictionary<string, Control> SettingBoxes { get; set; } = [];
        //绑定的集合
        public Dictionary<string, BindingList<decimal>> BindingListDic { get; set; } = [];

        public Point IniPoint = new(30, 30);
        public Point Interval = new(400, 30);
        public int RowCount = 12;
        public int Offset = 80;

        public SettingInfo(ParameterManager settingPara)
        {
            SettingPara = settingPara;
        }

        public void Initialize(Control parent)
        {
            ParameterManager parameter = SettingPara;
            BindingSource source = [];
            source.DataSource = SettingPara;
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            int j = 0;
            
            #region 通信属性设置
            SetParameter(parent, parameter, parameter.SocketPara, ref x, ref y, ref j);
            SetParameter(parent, parameter, parameter.SerialPara, ref x, ref y, ref j);
            #endregion

            #region 参数属性设置
            PropertyInfo[] properties = parameter.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "FilePath" || property.Name == "FileName" || property.Name == "SerialPara" || property.Name == "SocketPara") continue;
                if (property.Name == "CurrentState" || property.Name == "Instance") continue;
                if (property.GetValue(parameter) is List<decimal> list)
                {
                    BindingList<decimal> listData = [];
                    foreach (var item in list) listData.Add(item);
                    BindingListDic.TryAdd(property.Name, listData);

                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"[CB]{property.Name}", parameter.Translate(property.Name), "", listData, 60, Offset);
                    cBox.SelectedIndexChanged += CBox_SelectedIndexChanged;
                    cBox.Tag = 0;
                    SettingBoxes.TryAdd(property.Name, cBox);

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
                    checkBox.DataBindings.Add("Checked", source, property.Name);
                    SettingBoxes.TryAdd(property.Name, checkBox);
                }
                else if (property.GetValue(parameter) is Dictionary<string, BaseAxis> axes)
                {
                    BindingList<string> listData = [];
                    foreach (var item in axes) listData.Add(item.Key);

                    ComboBox cBox = FormKit.AddSettingBox<ComboBox>(parent, new Point(x, y), $"[CB]{property.Name}", parameter.Translate(property.Name), "", listData, 60, Offset);

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
                else
                {
                    var textBox = FormKit.AddSettingBox<TextBox>(parent, new Point(x, y), $"[TB]{property.Name}", parameter.Translate(property.Name), property.GetValue(parameter)!.ToString()!, xOffset: Offset);
                    textBox.DataBindings.Add("Text", source, property.Name);
                    SettingBoxes.TryAdd(property.Name, textBox);
                    if (property.Name == "IP") textBox.Width = 100;
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

        private void SetParameter<T>(Control parent, ParameterManager parameter, T instance, ref int x, ref int y, ref int j)
        {
            if (instance != null)
            {
                BindingSource source = [];
                source.DataSource = instance;
                PropertyInfo[] properties = instance.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (property.Name == "FilePath" || property.Name == "FileName" || property.Name == "SerialPara" || property.Name == "SocketPara") continue;
                    if (property.Name == "CurrentState" || property.Name == "Instance") continue;
                    if (property.Name == "Parity" || property.Name == "DataBits" || property.Name == "StopBits") continue;
                    var tb = FormKit.AddSettingBox<TextBox>(parent, new Point(x, y), $"[TB]({j}){property.Name}", 
                        parameter.Translate(property.Name), 
                        property.GetValue(instance)!.ToString()!, 
                        xOffset: Offset);
                    tb.DataBindings.Add("Text", source, property.Name);
                    SettingBoxes.TryAdd(property.Name, tb);
                    if (property.Name == "Ip") tb.Width = 100;

                    y += Interval.Y; j++;
                    if (j % RowCount == 0)
                    {
                        x += Interval.X;
                        y = IniPoint.Y;
                    }
                }
            }
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
                ComboBox settingBoxe = (ComboBox)SettingBoxes[propertyName];
                BindingList<decimal> bindingData = BindingListDic[propertyName];
                switch (command)
                {
                    case "Add":
                        if (decimal.TryParse(settingBoxe.Text, out decimal addValue))
                        {
                            BindingListDic[propertyName].Add(addValue);
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

        public void Save()
        {
            //if (SettingPara is null) return;
            //Dictionary<string, object> saveDic = [];
            //foreach (var item in BindingListDic)
            //    saveDic.Add(item.Key, item.Value.ToList());
            //foreach (var item in SettingBoxes)
            //{
            //    if (item.Value is CheckBox checkBox)
            //        saveDic.TryAdd(item.Key, checkBox.Checked);
            //    else if (item.Value is TextBox textBox)
            //        saveDic.TryAdd(item.Key, textBox.Text);
            //}
            if (SettingPara.Save())
                MessageBox.Show("保存成功。", "提示");
        }

    }
}
