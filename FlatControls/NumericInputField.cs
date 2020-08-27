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
    public class NumericInputField : InputField
    {
        [Browsable(true), Category("Data")]
        public decimal Value
        {
            get { return value; }
            set
            {
                this.value = GetClampedValue(Math.Round(value));
                this.Text = this.Value.ToString();
                UpdateLayout();
                OnValueChanged(EventArgs.Empty);
            }
        }

        [Browsable(true), Category("Data")]
        public decimal Maximum
        {
            get { return maximum; }
            set
            {
                this.maximum = Math.Round(value);
                if (Value > Maximum) Value = Maximum;
            }
        }

        [Browsable(true), Category("Data")]
        public decimal Minimum
        {
            get { return minimum; }
            set
            {
                this.minimum = Math.Round(value);
                if (Value < Minimum) Value = Minimum;
            }
        }

        [Browsable(true), Category("Data")]
        public decimal Increment
        {
            get { return increment; }
            set { this.increment = Math.Round(value); }
        }

        [Browsable(false)]
        public event EventHandler<EventArgs> ValueChanged;

        public enum NumericInputFieldStyle
        {
            Right = 0,
            Both = 1,
            Left = 2
        }

        [Browsable(true), Category("Appearance")]
        public virtual NumericInputFieldStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                UpdateLayout();
            }
        }

        protected virtual NumericInputFieldStyle DefaultStyle { get { return NumericInputFieldStyle.Both; } }

        [Browsable(true), Category("Appearance")]
        public virtual int ButtonSize
        {
            get { return buttonSize; }
            set
            {
                buttonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int ButtonOffset
        {
            get { return buttonOffset; }
            set
            {
                buttonOffset = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text { get => base.Text; set => base.Text = value; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ArrowButton AddButton
        {
            get
            {
                if (addButton == null) addButton = new ArrowButton
                {
                    ArrowDirection = ArrowDirection.Down,
                    ArrowSize = 8,
                    Padding = new Padding(1),
                    Text = ""
                };

                return addButton;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ArrowButton SubtractButton
        {
            get
            {
                if (subtractButton == null) subtractButton = new ArrowButton
                {
                    ArrowDirection = ArrowDirection.Down,
                    ArrowSize = 8,
                    Padding = new Padding(1),
                    Text = ""
                };

                return subtractButton;
            }
        }

        protected override Padding DefaultPadding { get { return new Padding(0, 0, 0, 0); } }

        protected override Rectangle TextBoxRect
        {
            get
            {
                Rectangle textBoxRect = base.TextBoxRect;
                int offset = ButtonSize + ButtonOffset;

                switch (Style)
                {
                    case NumericInputFieldStyle.Right:
                        textBoxRect.Width -= offset;
                        break;
                    case NumericInputFieldStyle.Both:
                        textBoxRect.Width -= offset * 2;
                        textBoxRect.X += offset;
                        break;
                    case NumericInputFieldStyle.Left:
                        textBoxRect.Width -= offset;
                        textBoxRect.X += offset;
                        break;
                }

                return textBoxRect;
            }
        }

        protected ArrowButton addButton;
        protected ArrowButton subtractButton;

        private decimal value = 0;
        private NumericInputFieldStyle style;
        private int buttonSize = 18;
        private int buttonOffset = 3;
        private decimal maximum = 100;
        private decimal minimum = 0;
        private decimal increment = 1;

        public NumericInputField()
        {
            this.Text = Value.ToString();
            this.TextBox.Multiline = false;
            this.TextBox.Leave += TextBoxLeave;

            this.AddButton.Click += AddButtonClick;
            this.Controls.Add(AddButton);

            this.SubtractButton.Click += SubtractButtonClick;
            this.Controls.Add(SubtractButton);

            this.Style = this.DefaultStyle;
        }

        protected override void UpdateLayout()
        {
            base.UpdateLayout();

            int lineSize = BorderSize;
            int buttonWidth = ButtonSize;
            Padding viewPadding = this.Padding;
            Rectangle viewRect = this.ClientRectangle;

            Rectangle contentRect = new Rectangle(
                viewRect.X + viewPadding.Left,
                viewRect.Y + viewPadding.Top,
                viewRect.Width - viewPadding.Horizontal,
                viewRect.Height - viewPadding.Vertical - lineSize);

            Rectangle addButtonRect = new Rectangle();
            Rectangle subtractButtonRect = new Rectangle();

            switch (Style)
            {
                case NumericInputFieldStyle.Right:
                    addButtonRect = new Rectangle(
                         contentRect.Right - buttonWidth,
                         contentRect.Y,
                         buttonWidth,
                         contentRect.Height / 2);

                    subtractButtonRect = new Rectangle(
                        addButtonRect.X,
                        addButtonRect.Bottom,
                        buttonWidth,
                        addButtonRect.Height);

                    this.SubtractButton.ArrowDirection = ArrowDirection.Down;
                    this.AddButton.ArrowDirection = ArrowDirection.Up;
                    this.TextBox.TextAlign = HorizontalAlignment.Left;
                    this.SubtractButton.TabIndex = 2;
                    this.AddButton.TabIndex = 1;
                    this.TextBox.TabIndex = 0;
                    break;
                case NumericInputFieldStyle.Both:
                    subtractButtonRect = new Rectangle(
                        contentRect.X,
                        contentRect.Y,
                        buttonWidth,
                        contentRect.Height);

                    addButtonRect = new Rectangle(
                        contentRect.Right - buttonWidth,
                        contentRect.Y,
                        buttonWidth,
                        contentRect.Height);

                    this.SubtractButton.ArrowDirection = ArrowDirection.Left;
                    this.AddButton.ArrowDirection = ArrowDirection.Right;
                    this.TextBox.TextAlign = HorizontalAlignment.Center;
                    this.SubtractButton.TabIndex = 0;
                    this.AddButton.TabIndex = 2;
                    this.TextBox.TabIndex = 1;
                    break;
                case NumericInputFieldStyle.Left:
                    addButtonRect = new Rectangle(
                        contentRect.X,
                        contentRect.Y,
                        buttonWidth,
                        contentRect.Height / 2);

                    subtractButtonRect = new Rectangle(
                        contentRect.X,
                        addButtonRect.Bottom,
                        buttonWidth,
                        addButtonRect.Height);

                    this.SubtractButton.ArrowDirection = ArrowDirection.Down;
                    this.AddButton.ArrowDirection = ArrowDirection.Up;
                    this.TextBox.TextAlign = HorizontalAlignment.Left;
                    this.SubtractButton.TabIndex = 1;
                    this.AddButton.TabIndex = 0;
                    this.TextBox.TabIndex = 2;
                    break;
            }

            this.AddButton.Bounds = addButtonRect;
            this.SubtractButton.Bounds = subtractButtonRect;
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public virtual void SetValueSilently(decimal value)
        {
            this.value = GetClampedValue(Math.Round(value));
            base.SetValueSilently(this.Value.ToString());
        }

        public override void SetValueSilently(string value)
        {
            SetValueSilently(StringToDecimal(value, this.value));
        }

        private decimal StringToDecimal(string text, decimal defaultValue)
        {
            try
            {
                return Convert.ToDecimal(text);
            }
            catch
            {
                System.Media.SystemSounds.Exclamation.Play();
                return defaultValue;
            }
        }

        private decimal GetClampedValue(decimal value)
        {
            if (value < Minimum) return Minimum;
            else if (value > Maximum) return Maximum;
            return value;
        }

        private void TextBoxLeave(object sender, EventArgs e)
        {
            this.Value = StringToDecimal(TextBox.Text, this.Value);
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            this.Value += Increment;
        }

        private void SubtractButtonClick(object sender, EventArgs e)
        {
            this.Value -= Increment;
        }
    }
}