using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class DropdownMenu : Form
    {
        public virtual DropdownCollection Items { get { return items; } }
        public event EventHandler<ItemEventArgs> ItemSelected;

        public virtual int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value > 0 ? value : 0;

                foreach (Control c in this.Controls)
                {
                    c.Height = itemHeight;
                }

                UpdateMenuHeight();
            }
        }
        
        public virtual int MaxVisibleItems
        {
            get { return maxVisibleItems; }
            set
            {
                maxVisibleItems = value > 0 ? value : 0;
                UpdateMenuHeight();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = Theme.Current.WindowBackgroundColor; } }


        protected virtual VerticalListContainer View
        {
            get
            {
                if (view == null)
                {
                    view = new VerticalListContainer { Dock = DockStyle.Fill, AutoScroll = true };
                    this.Controls.Add(view);
                }

                return view;
            }
        }

        protected override Size DefaultMinimumSize { get { return new Size(0, ItemHeight); } }
        protected override Size DefaultMaximumSize { get { return new Size(0, 0); } }
        protected override Padding DefaultPadding { get { return new Padding(1); } }
        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.Style |= NativeMethods.WS_CHILD;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        private VerticalListContainer view;
        private DropdownCollection items;
        private int itemHeight = 24;
        private int maxVisibleItems = 10;
        private Color backgroundColor = Color.FromArgb(55, 60, 70);

        private NativeMethods.HookProc mouseHookCallback;
        private NativeMethods.HookProc keyboardHookCallback;
        private IntPtr mouseHook = IntPtr.Zero;
        private IntPtr keyboardHook = IntPtr.Zero;
        private bool useScrollHook = false;

        public DropdownMenu()
        {
            this.items = new DropdownCollection(this);
            this.Text = "";
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScroll = true;

            useScrollHook = Environment.OSVersion.Version.Major < 10;

            Theme.GlobalThemeChanged += GlobalThemeChanged;
            UpdateTheme();
        }

        public virtual void FocusItem(int itemIndex)
        {
            if (itemIndex > -1 && itemIndex < this.View.Controls.Count)
                this.View.Controls[itemIndex].Select();
        }

        public virtual void UpdateMenuHeight()
        {
            int itemsCount = this.View.Controls.Count;

            if (itemsCount > this.MaxVisibleItems)
                itemsCount = this.MaxVisibleItems;

            this.Height = itemsCount * this.ItemHeight + this.Padding.Vertical;
        }

        protected virtual void UpdateTheme()
        {
            Theme theme = Theme.Current;
            this.BackColor = theme.ControlBackgroundColor;
            this.Invalidate();
        }

        private void GlobalThemeChanged(object sender, EventArgs e)
        {
            UpdateTheme();
        }

        protected virtual DropdownMenuItem AddItemControl(object item, int index)
        {
            string itemName = item.ToString();

            if (item != null)
            {
                MemberInfo[] memberInfo = item.GetType().GetMember(item.ToString());

                if (memberInfo != null && memberInfo.Length > 0)
                {
                    DescriptionAttribute[] descriptions = memberInfo[0].GetCustomAttributes(
                        typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                    if (descriptions != null && descriptions.Length > 0)
                        itemName = descriptions[0].Description;
                }
            }

            DropdownMenuItem itemControl = new DropdownMenuItem()
            {
                Text = itemName,
                Tag = item,
                Height = ItemHeight
            };

            itemControl.MouseDown += ItemMouseDown;
            itemControl.KeyDown += ItemKeyDown;
            this.View.Controls.Add(itemControl);
            this.View.Controls.SetChildIndex(itemControl, index);

            return itemControl;
        }

        protected virtual void SelectItem(Control item)
        {
            if (item is DropdownMenuItem && this.View.Controls.Contains(item))
            {
                OnItemSelected(new ItemEventArgs
                {
                    data = item.Tag,
                    index = this.View.Controls.GetChildIndex(item)
                });

                this.Close();
            }
        }

        protected virtual void OnItemSelected(ItemEventArgs e)
        {
            ItemSelected?.Invoke(this, e);
        }

        protected virtual void ItemKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is Control && 
                e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Return ||
                e.KeyCode == Keys.Space)
            {
                SelectItem(sender as Control);
            }
        }

        protected virtual void ItemMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control && e.Button == MouseButtons.Left)
            {
                SelectItem(sender as Control);
            }
        }

        protected virtual void FocusItem(Control item)
        {
            if(item != null)
            {
                this.View.ScrollControlIntoView(item);
                item.Select();
            }
        }

        protected virtual void FocusNextItem(bool forward)
        {
            int controlsCount = this.View.Controls.Count;

            if (controlsCount < 1) return;

            Control activeControl = this.ActiveControl;

            if (activeControl == null) FocusItem(this.View.Controls[0]);
            else
            {
                int activeIndex = this.View.Controls.IndexOf(activeControl);
                int offset = forward == true ? 1 : -1;
                int nextIndex = activeIndex + offset;

                if (nextIndex < 0) nextIndex = controlsCount - 1;
                else if (nextIndex >= controlsCount) nextIndex = 0;

                FocusItem(this.View.Controls[nextIndex]);
            }
        }

        protected virtual void AddHooks()
        {
            mouseHookCallback -= MouseHook;
            mouseHookCallback += MouseHook;

            mouseHook = NativeMethods.SetWindowsHookEx(
                NativeMethods.HookType.WH_MOUSE_LL,
                mouseHookCallback,
                NativeMethods.GetModuleHandle("user32"),
                0);

            keyboardHookCallback -= KeyboardHook;
            keyboardHookCallback += KeyboardHook;

            keyboardHook = NativeMethods.SetWindowsHookEx(
                NativeMethods.HookType.WH_KEYBOARD_LL,
                keyboardHookCallback,
                NativeMethods.GetModuleHandle("user32"),
                0);

            if (mouseHook == IntPtr.Zero || keyboardHook == IntPtr.Zero)
            {
                this.Close();
            }
        }

        protected virtual void RemoveHooks()
        {
            if (mouseHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;
            }

            if (keyboardHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(keyboardHook);
                keyboardHook = IntPtr.Zero;
            }
        }

        protected virtual IntPtr MouseHook(int code, IntPtr wParam, IntPtr lParam)
        {
            NativeMethods.MouseMessageType messageType = (NativeMethods.MouseMessageType)wParam;
            NativeMethods.MSLLHOOKSTRUCT messageData = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);

            if (code >= 0 && this.IsHandleCreated)
            {
                switch (messageType)
                {
                    case NativeMethods.MouseMessageType.WM_LBUTTONDOWN:
                    case NativeMethods.MouseMessageType.WM_RBUTTONDOWN:
                        if (!this.Bounds.Contains(messageData.pt))
                        {
                            this.Close();
                        }
                        break;
                    case NativeMethods.MouseMessageType.WM_MOUSEMOVE:
                        if (this.Bounds.Contains(messageData.pt))
                        {
                            Control controlToSelect = this.View.GetChildAtPoint(this.View.PointToClient(messageData.pt));
                            if (controlToSelect != this.ActiveControl) controlToSelect?.Select();
                        }
                        break;
                    case NativeMethods.MouseMessageType.WM_MOUSEWHEEL:
                        if (useScrollHook == true && this.Bounds.Contains(messageData.pt) && this.IsHandleCreated == true)
                        {
                            IntPtr positionPtr = (IntPtr)((messageData.pt.Y << 16) | (messageData.pt.X & 0xffff));

                            Message msg = Message.Create(
                                this.Handle,
                                (int)NativeMethods.MouseMessageType.WM_MOUSEWHEEL,
                                (IntPtr)messageData.mouseData,
                                positionPtr);

                            this.WndProc(ref msg);
                        }
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(mouseHook, code, wParam, lParam);
        }

        protected virtual IntPtr KeyboardHook(int code, IntPtr wParam, IntPtr lParam)
        {
            NativeMethods.KeyboardMessageType messageType = (NativeMethods.KeyboardMessageType)wParam;
            NativeMethods.KBDLLHOOKSTRUCT messageData = Marshal.PtrToStructure<NativeMethods.KBDLLHOOKSTRUCT>(lParam);

            if (code >= 0 &&
                messageType == NativeMethods.KeyboardMessageType.WM_KEYDOWN ||
                messageType == NativeMethods.KeyboardMessageType.WM_SYSKEYDOWN)
            {
                IntPtr cancelMassege = new IntPtr(-1);

                if (Controls.Count > 0)
                {
                    switch ((Keys)messageData.vkCode)
                    {
                        case Keys.Up:
                            FocusNextItem(false);
                            return cancelMassege;
                        case Keys.Down:
                            FocusNextItem(true);
                            return cancelMassege;
                    }
                }

                switch ((Keys)messageData.vkCode)
                {
                    case Keys.Return:
                    case Keys.Space:
                        SelectItem(this.ActiveControl);
                        return cancelMassege;
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(mouseHook, code, wParam, lParam);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedControl != null && AutoScroll)
            {
                base.OnLayout(levent);
            }

            AdjustFormScrollbars(AutoScroll);
            base.OnLayout(levent);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AddHooks();
            UpdateMenuHeight();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            RemoveHooks();
        }

        protected override void Dispose(bool disposing)
        {
            RemoveHooks();
            lock (this) { this.ItemSelected = null; }
            base.Dispose(disposing);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            UpdateMenuHeight();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            UpdateMenuHeight();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == NativeMethods.WM_ACTIVATEAPP)
            {
                this.Close();
            }
        }

        public class DropdownCollection : IList
        {
            private DropdownMenu menu;
            private List<object> items = new List<object>();

            public DropdownCollection(DropdownMenu menu)
            {
                this.menu = menu;
            }

            public object this[int index]
            {
                get { return items[index]; }
                set
                {
                    items[index] = value;
                    menu.View.Controls.RemoveAt(index);
                    menu.AddItemControl(value, index);
                }
            }

            public int Add(object item)
            {
                try
                {
                    items.Add(item);
                    menu.AddItemControl(item, menu.View.Controls.Count);
                    return items.Count - 1;
                }
                catch
                {
                    return -1;
                }
            }

            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
                menu.View.Controls.RemoveAt(index);
            }

            public void Insert(int index, object item)
            {
                items.Insert(index, item);
                menu.AddItemControl(item, index);
            }

            public void Clear()
            {
                items.Clear();
                menu.View.Controls.Clear();
            }

            public void Move(int oldIndex, int newIndex)
            {
                object removedItem = this[oldIndex];
                this.RemoveAt(oldIndex);
                this.Insert(newIndex, removedItem);
            }

            public void AddRange(object[] items)
            {
                foreach (object item in items)
                {
                    this.Add(item);
                }
            }

            public void Remove(object item)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] == item)
                    {
                        this.RemoveAt(i);
                        return;
                    }
                }
            }

            public int Count => items.Count;
            public bool IsReadOnly => ((IList)items).IsReadOnly;
            public bool IsFixedSize => ((IList)items).IsFixedSize;
            public object SyncRoot => ((IList)items).SyncRoot;
            public bool IsSynchronized => ((IList)items).IsSynchronized;
            public bool Contains(object value) { return items.Contains(value); }
            public void CopyTo(Array array, int index) { items.CopyTo((object[])array, index); }
            public IEnumerator GetEnumerator() { return items.GetEnumerator(); }
            public int IndexOf(object value) { return items.IndexOf(value); }
        }
    }
}