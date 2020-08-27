using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public class DropdownInputField : InputField
    {
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

        [Browsable(false)]
        public event EventHandler<EventArgs> DropDown;

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

        [Browsable(true), Category("Layout")]
        public virtual int ButtonSize
        {
            get { return buttonSize; }
            set
            {
                buttonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Layout")]
        public virtual int ButtonOffset
        {
            get { return buttonOffset; }
            set
            {
                buttonOffset = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        protected override Padding DefaultPadding { get { return new Padding(2, 0, 0, 0); } }

        protected override Rectangle TextBoxRect
        { 
            get
            {
                Rectangle textBoxRect = base.TextBoxRect;
                textBoxRect.Width -= ButtonSize + ButtonOffset;
                return textBoxRect; 
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual ArrowButton DropButton
        {
            get
            {
                if (dropButton == null) dropButton = new ArrowButton
                {
                    ArrowDirection = ArrowDirection.Down,
                    ArrowSize = 8,
                    Text = ""
                };

                return dropButton;
            }
        }

        protected DropdownHandler DropdownHandler { get { return dropdownHandler; } set { dropdownHandler = value; } }
        protected ArrowButton dropButton;

        private int buttonSize = 22;
        private int buttonOffset = 3;
        private DropdownHandler dropdownHandler = new DropdownHandler();

        public DropdownInputField()
        {
            DropdownHandler.AutoSelectFirstItem = false;
            DropdownHandler.ItemSelected += MenuItemSelected;
            DropdownHandler.SelectedValueChanged += MenuValueChanged;
            this.DropButton.Click += DropButtonClick;
            this.Controls.Add(DropButton);
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

        protected virtual void OnDropDown(EventArgs e)
        {
            this.DropDown?.Invoke(this, e);
        }

        public override void UpdateTheme()
        {
            base.UpdateTheme();

            DropButton.UseGlobalTheme = false;
            DropButton.Theme = this.Theme;
            DropButton.UpdateTheme();
        }

        protected override void UpdateLayout()
        {
            base.UpdateLayout();

            Rectangle viewRect = this.ClientRectangle;
            Rectangle textBoxRect = TextBoxRect;
            Padding viewPadding = this.Padding;
            int lineSize = BorderSize;
            int buttonWidth = ButtonSize;

            if (DropButton != null)
            {
                DropButton.Size = new Size(
                    buttonWidth,
                    viewRect.Height - viewPadding.Vertical - lineSize);

                DropButton.Location = new Point(
                    textBoxRect.Right + ButtonOffset,
                    viewPadding.Top);
            }
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

        private void DropButtonClick(object sender, EventArgs e)
        {
            DropdownHandler.Show(this);
            OnDropDown(EventArgs.Empty);
        }
    }
}