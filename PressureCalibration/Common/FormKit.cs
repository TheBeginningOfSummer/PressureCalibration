using System.ComponentModel;

namespace WinformKit
{
    public class FormKit
    {
        public static void OnThread(Control control, Action method)
        {
            if (control.IsHandleCreated)
                control.Invoke(method);
            else
                method();
        }
        /// <summary>
        /// 得到一个矩形阵列的坐标
        /// </summary>
        /// <param name="x">阵列起始X坐标</param>
        /// <param name="y">阵列起始Y坐标</param>
        /// <param name="count">阵列元素个数</param>
        /// <param name="length">每行的元素个数</param>
        /// <param name="xInterval">阵列坐标x方向间距</param>
        /// <param name="yInterval">阵列坐标y方向间距</param>
        /// <returns>阵列坐标列表</returns>
        public static List<Point> GetLocation(int x, int y, int count, int length, int xInterval, int yInterval)
        {
            int o = x;
            List<Point> locationList = [];
            for (int i = 0; i < count; i++)
            {
                locationList.Add(new Point(x, y));
                x += xInterval;
                if ((i + 1) % length == 0)
                {
                    x = o;
                    y += yInterval;
                }
            }
            return locationList;
        }

        public static void GetRowPosition1(ref int x, ref int y, int xInitial, int xInterval, int yInterval, int i, int row)
        {
            x += xInterval;
            if ((i + 1) % row == 0)
            {
                x = xInitial;
                y += yInterval;
            }
        }

        public static void GetRowPosition2(ref int x, ref int y, int yInitial, int xInterval, int yInterval, int i, int row, bool isSwitch = false)
        {
            if (isSwitch)
                y += yInterval;
            else
                y -= yInterval;
            if ((i + 1) % row == 0)
            {
                if (isSwitch)
                    x -= xInterval;
                else
                    x += xInterval;
                y = yInitial;
            }
        }

        #region 控件添加
        /// <summary>
        /// 生成一个控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="point">位置</param>
        /// <param name="name">名称</param>
        /// <param name="text">text属性</param>
        /// <param name="size">大小</param>
        /// <param name="color">label背景颜色</param>
        /// <param name="foreColor">label字体颜色</param>
        /// <param name="tag">tag</param>
        /// <param name="cmdPara">参数</param>
        /// <param name="data">ComboBox数据</param>
        /// <returns></returns>
        public static T ControlFactory<T>(Point point, string name = "备注", string text = "", 
            Size? size = null, Color? color = null, Color? foreColor = null, 
            object? tag = null, object? cmdPara = null, object? data = null) where T : Control, new()
        {
            T control = new()
            {
                Location = point,
                Name = name,
                Text = text,
            };
            if (size != null) control.Size = (Size)size;
            if (tag != null) control.Tag = tag;
            if (control is Label label)
            {
                label.TextAlign = ContentAlignment.MiddleCenter;
                if (size != null)
                    label.AutoSize = false;
                else
                    label.AutoSize = true;
                if (color != null)
                    label.BackColor = (Color)color;
                if (foreColor != null)
                    label.ForeColor = (Color)foreColor;
            }
            if (control is TextBox textBox)
            {
                if (size != null)
                {
                    //textBox.AutoSize = false;
                }
            }
            if (control is ComboBox comboBox)
            {
                comboBox.DataSource = data;
            }
            if (control is Button button)
            {
                if (cmdPara != null) button.CommandParameter = cmdPara;
                else button.CommandParameter = button;
                if (color != null)
                    button.BackColor = (Color)color;
                if (foreColor != null)
                    button.ForeColor = (Color)foreColor;
                button.FlatStyle = FlatStyle.Flat;
            }
            return control;
        }

        public static T AddControl<T>(Control parent, T child) where T : Control
        {
            parent.Controls.Add(child);
            return child;
        }
        
        public static T AddSettingBox<T>(Control parent, Label label, T setting, int xOffset = 80, int yOffset = -4) where T : Control
        {
            AddControl(parent, label);
            Point settingLocation = new(label.Location.X + xOffset, label.Location.Y + yOffset);
            setting.Location = settingLocation;
            return AddControl(parent, setting);
        }
        /// <summary>
        /// 添加设置控件
        /// </summary>
        /// <typeparam name="T">设置控件类型</typeparam>
        /// <param name="parent">父控件</param>
        /// <param name="position">位置</param>
        /// <param name="name">名称</param>
        /// <param name="tip">提示信息</param>
        /// <param name="value">text信息</param>
        /// <param name="data">绑定的数据（ComboBox）</param>
        /// <param name="width">控件宽</param>
        /// <param name="xOffset">水平偏移</param>
        /// <param name="yOffset">竖直偏移</param>
        /// <returns></returns>
        public static T AddSettingBox<T>(Control parent, Point position, string name, string tip, string value, object? data = null, int width = 50, int xOffset = 80, int yOffset = -4) where T : Control, new()
        {
            Point settingLocation = new(position.X + xOffset, position.Y + yOffset);
            Label label = ControlFactory<Label>(position, name, tip);
            T setting = ControlFactory<T>(settingLocation, name, value, new Size(width, 25));
            if (setting is TextBox)
            {
                setting.Name = name;
            }
            if (setting is ComboBox comboBox)
            {
                comboBox.Name = name;
                comboBox.DataSource = data;
            }
            return AddSettingBox(parent, label, setting, xOffset, yOffset);
        }
        #endregion

        #region 消息框
        public static void ShowInfoBox(string message, string caption = "提示")
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowErrorBox(string message, string caption = "错误")
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowQuestionBox(string message, string caption = "提示")
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        #endregion

        public static void UpdateMessage(Control control, string text, bool isAddDate = true, bool isCover = false)
        {
            string message = text;
            if (isAddDate)
                message = $"[{DateTime.Now}] {text}{Environment.NewLine}";
            if (control.IsDisposed) return;
            if (control.IsHandleCreated)
            {
                if (isCover)
                    control.Invoke(() => control.Text = message);
                else
                    control.Invoke(() => control.Text += message);
            }
            else
            {
                if (isCover)
                    control.Text = message;
                else
                    control.Text += message;
            }
        }

        public static void UpdateListBox(ListBox list, List<string> data)
        {
            list.Items.Clear();
            foreach (var item in data) list.Items.Add(item);
        }

        #region 数据绑定
        public static void TextBinding<T>(Control control, T viewModel, string propertyName)
        {
            control.DataBindings.Add(new Binding("Text", viewModel, propertyName));
        }

        public static void TextBinding<T>(List<Control> controls, T viewModel)
        {
            foreach (var control in controls)
            {
                if (control.Tag == null) continue;
                TextBinding(control, viewModel, (string)control.Tag);
            }
        }

        public static void ListBinding<T>(ListBox lb, BindingList<T> list, string display = "", string value = "")
        {
            lb.DataSource = list;
            lb.DisplayMember = display;
            lb.ValueMember = value;
        }

        public static void ListBinding<T>(ComboBox cb, BindingList<T> list, string display = "", string value = "")
        {
            cb.DataSource = list;
            cb.DisplayMember = display;
            cb.ValueMember = value;
        }
        #endregion
    }

}
