using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public partial class Dropdown : FlatButton
    {
        [Browsable(false), Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text { get { return base.Text; } set { base.Text = value; } }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true), Browsable(true), Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
            typeof(System.Drawing.Design.UITypeEditor))]
        public virtual DropdownHandler.ObjectCollection Items
        {
            get { return DropdownHandler.Items; }
        }

        [Browsable(false)]
        public event EventHandler<ItemEventArgs> ItemSelected;

        [Browsable(false)]
        public event EventHandler<ItemEventArgs> SelectedValueChanged;

        [Browsable(false), Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object SelectedItem
        {
            get { return DropdownHandler.SelectedItem; }
            set { DropdownHandler.SelectedItem = value; }
        }

        [Browsable(false), Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int SelectedIndex
        {
            get { return DropdownHandler.SelectedIndex; }
            set { DropdownHandler.SelectedIndex = value; }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int DropIconSize
        {
            get { return dropIconSize; }
            set
            {
                dropIconSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int DropButtonSize
        {
            get { return dropButtonSize; }
            set
            {
                dropButtonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        protected virtual Rectangle DropIconRect
        {
            get
            {
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewRect.Right - viewPadding.Right - DropButtonSize,
                    viewPadding.Top,
                    DropButtonSize,
                    viewRect.Height - viewPadding.Vertical);
            }
        }

        protected override Rectangle TextRect
        {
            get
            {
                Rectangle textRect = base.TextRect;
                textRect.Width -= DropButtonSize;
                return textRect;
            }
        }

        protected DropdownHandler DropdownHandler { get { return dropdownHandler; } set { dropdownHandler = value; } }
        protected Point[] Arrow { get { return arrow; } set { arrow = value; } }
        protected override ContentAlignment DefaulTextAlignment { get { return ContentAlignment.MiddleLeft; } }
        
        private int dropIconSize = 8;
        private int dropButtonSize = 20;
        private Point[] arrow = new Point[0];
        private DropdownHandler dropdownHandler = new DropdownHandler();

        public Dropdown()
        {
            DropdownHandler.ItemSelected += MenuItemSelected;
            DropdownHandler.SelectedValueChanged += MenuValueChanged;
        }

        public virtual TEnum ToEnum<TEnum>(TEnum defaultValue) where TEnum : struct, Enum
        {
            return DropdownHandler.ToEnum(defaultValue);
        }

        public virtual void FromEnum<TEnum>(TEnum selectedValue)
        {
            DropdownHandler.FromEnum<TEnum>(selectedValue);
        }

        protected virtual void MenuItemSelected(object sender, ItemEventArgs e)
        {
            OnItemSelected(e);
        }
        
        protected virtual void MenuValueChanged(object sender, ItemEventArgs e)
        {
            this.Text = e.data.ToString();
            OnSelectedValueChanged(e);
        }

        protected virtual void OnItemSelected(ItemEventArgs e)
        {
            this.ItemSelected?.Invoke(this, e);
        }

        protected virtual void OnSelectedValueChanged(ItemEventArgs e)
        {
            this.SelectedValueChanged?.Invoke(this, e);
        }

        protected override void UpdateLayout()
        {
            base.UpdateLayout();
            Arrow = VectorIcons.DownArrow(DropIconRect, DropIconSize);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            SmoothingMode lastSmoothingMode = pe.Graphics.SmoothingMode;
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush brush = new SolidBrush(this.Enabled == true ? this.TextColor : this.DisabledTextColor))
            {
                pe.Graphics.FillPolygon(brush, Arrow);
            }

            pe.Graphics.SmoothingMode = lastSmoothingMode;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            DropdownHandler.Show(this);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            DropdownHandler.Close();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            DropdownHandler.Close();
        }
    }
}