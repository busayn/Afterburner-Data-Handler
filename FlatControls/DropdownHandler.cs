using System;
using System.Collections;
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
    public partial class DropdownHandler
    {
        public event EventHandler<ItemEventArgs> ItemSelected;
        public event EventHandler<ItemEventArgs> SelectedValueChanged;
        public virtual ObjectCollection Items { get { return items; } }

        public virtual object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                selectedIndex = -1;

                for (int i = 0; i < Items.Count; i++)
                {
                    if (selectedItem is IComparable && Items[i] is IComparable)
                    {
                        if (((IComparable)Items[i]).CompareTo(selectedItem) == 0)
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                    else if (Items[i] == selectedItem)
                    {
                        selectedIndex = i;
                        break;
                    }
                    else if (Items[i].ToString() == selectedItem.ToString())
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                OnSelectedValueChanged(new ItemEventArgs
                {
                    data = selectedItem,
                    index = selectedIndex
                });
            }
        }

        public virtual int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;

                if (value < 0 || value >= Items.Count)
                {
                    selectedIndex = -1;
                    selectedItem = null;
                }
                else
                {
                    selectedIndex = value;
                    selectedItem = Items[value];
                }

                OnSelectedValueChanged(new ItemEventArgs
                {
                    data = selectedItem,
                    index = selectedIndex
                });
            }
        }

        public virtual bool AutoSelectFirstItem { get; set; } = true;

        public virtual Font Font { get { return font; } set { font = value; } }
        public virtual Point Location { get { return location; } set { location = value; } }
        public virtual Size Size { get { return size; } set { size = value; } }

        public virtual int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value > 0 ? value : 0;

                if (menu != null && menu.IsDisposed == false)
                    menu.ItemHeight = itemHeight;
            }
        }

        public virtual int MaxVisibleItems
        {
            get { return maxVisibleItems; }
            set
            {
                maxVisibleItems = value > 0 ? value : 0;

                if (menu != null && menu.IsDisposed == false)
                    menu.MaxVisibleItems = maxVisibleItems;
            }
        }

        private ObjectCollection items;
        private DropdownMenu menu;
        private Font font;
        private Point location = Point.Empty;
        private Size size = new Size(200, 400);
        private int itemHeight = 24;
        private int maxVisibleItems = 10;
        private int selectedIndex;
        private object selectedItem;

        public DropdownHandler()
        {
            items = new ObjectCollection(this);
        }

        public virtual DropdownMenu Show(Control control)
        {
            Close();

            menu = CreateNewMenu();
            menu.Size = new Size(control.Width + menu.Padding.Horizontal, 0);
            menu.Items.Clear();

            foreach (object obj in this.Items)
            {
                menu.Items.Add(obj);
            }

            Rectangle controlRect = control.RectangleToScreen(control.ClientRectangle);
            Rectangle screenBounds = Screen.FromControl(control).WorkingArea;
            int menuHeight = menu.Height;

            if (controlRect.Bottom + menuHeight < screenBounds.Bottom)
                menu.Location = new Point(controlRect.Left - menu.Padding.Left, controlRect.Bottom);
            else
                menu.Location = new Point(controlRect.Left - menu.Padding.Left, controlRect.Left- menuHeight);

            menu.Show();
            menu.FocusItem(SelectedIndex);

            return menu;
        }

        public virtual DropdownMenu Show(Rectangle bounds)
        {
            Close();

            menu = CreateNewMenu();
            menu.Size = bounds.Size;
            menu.Location = bounds.Location;
            menu.Items.Clear();

            foreach (object obj in this.Items)
            {
                menu.Items.Add(obj);
            }

            menu.Show();
            menu.FocusItem(SelectedIndex);

            return menu;
        }

        public virtual void Close()
        {
            if (menu != null && menu.IsDisposed == false)
            {
                menu.Close();
                menu.Dispose();
            }
        }

        public virtual TEnum ToEnum<TEnum>(TEnum defaultValue) where TEnum : struct, Enum
        {
            if (typeof(TEnum).IsEnum == false)
                return defaultValue;

            if (SelectedItem is TEnum)
                return (TEnum)SelectedItem;

            TEnum parsingResult;

            if (Enum.TryParse<TEnum>(SelectedItem?.ToString() ?? "", out parsingResult) == false)
            {
                parsingResult = defaultValue;
            }

            return parsingResult;
        }

        public virtual void FromEnum<TEnum>(TEnum selectedValue)
        {
            if (typeof(TEnum).IsEnum == false)
                return;

            Array enums = Enum.GetValues(typeof(TEnum));
            this.Items.Clear();

            foreach (TEnum item in enums)
            {
                this.Items.Add(item);
            }

            this.SelectedItem = selectedValue;
        }

        protected virtual void OnItemSelected(ItemEventArgs e)
        {
            ItemSelected?.Invoke(this, e);
        }

        protected virtual void OnSelectedValueChanged(ItemEventArgs e)
        {
            SelectedValueChanged?.Invoke(this, e);
        }

        protected virtual void MenuItemSelected(object sender, ItemEventArgs e)
        {
            selectedIndex = e.index;
            selectedItem = e.data;

            OnSelectedValueChanged(e);
            OnItemSelected(e);
        }

        private DropdownMenu CreateNewMenu()
        {
            DropdownMenu menu = new DropdownMenu
            {
                Font = Font,
                MaxVisibleItems = this.MaxVisibleItems,
                ItemHeight = this.ItemHeight,
                StartPosition = FormStartPosition.Manual
            };
            menu.ItemSelected += MenuItemSelected;

            return menu;
        }


        public class ObjectCollection : IList
        {
            private DropdownHandler handler;
            private List<object> items = new List<object>();

            public ObjectCollection(DropdownHandler handler)
            {
                this.handler = handler;
            }

            public object this[int index]
            {
                get { return items[index]; }
                set
                {
                    items[index] = value;

                    if (handler.menu != null && handler.menu.IsDisposed == false)
                    {
                        handler.menu.Items.RemoveAt(index);
                        handler.menu.Items.Insert(index, value);
                    }
                }
            }

            public int Add(object item)
            {
                try
                {
                    items.Add(item);

                    if (handler.menu != null && handler.menu.IsDisposed == false)
                    {
                        handler.menu.Items.Add(item);
                    }

                    if (handler.AutoSelectFirstItem == true && this.Count == 1)
                        handler.SelectedItem = item;

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

                if (handler.menu != null && handler.menu.IsDisposed == false)
                {
                    handler.menu.Items.RemoveAt(index);
                }
            }

            public void Insert(int index, object item)
            {
                items.Insert(index, item);

                if (handler.menu != null && handler.menu.IsDisposed == false)
                {
                    handler.menu.Items.Insert(index, item);
                }

                if (handler.AutoSelectFirstItem == true && this.Count == 1)
                    handler.SelectedItem = item;
            }

            public void Clear()
            {
                items.Clear();

                if (handler.menu != null && handler.menu.IsDisposed == false)
                {
                    handler.menu.Items.Clear();
                }
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