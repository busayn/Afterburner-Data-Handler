using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using AfterburnerDataHandler.SharedMemory.Afterburner;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public class MASMFormattingEditor : VerticalListContainer
    {
        [Browsable(false)]
        public StringFormattingCollection Items { get; protected set; }

        [Browsable(false)]
        public ControlCollection AdditionalProperties
        {
            get
            {
                return AdditionalPropertiesContainer?.Controls;
            }
        }

        public VerticalListContainer HeaderContainer { get; protected set; }
        public PropertyContainer Header { get; protected set; }
        public FlatButton ApplyButton { get; protected set; }
        public FlatButton CancelButton { get; protected set; }
        public FlexColumnContainer AdditionalPropertiesContainer { get; protected set; }
        public FlatButton TopAddItemButton { get; protected set; }
        public FlatButton BottomAddItemButton { get; protected set; }
        public VerticalListContainer ItemsView { get; protected set; }

        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;

                if (this.Header != null)
                    this.Header.Text = value;
            }
        }

        public event EventHandler<EventArgs> Apply;
        public event EventHandler<EventArgs> Cancel;
        public Func<List<string>> AvailableProperties;


        public MASMFormattingEditor()
        {
            InitializeGUI();
            InitializeHandles();
        }

        protected virtual void InitializeGUI()
        {
            this.AutoScroll = true;
            this.BackgroundSource = Theme.BackgroundSource.Inherit;
            this.Margin = new Padding(0);
            this.Padding = new Padding(16);

            this.HeaderContainer = new VerticalListContainer
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 0, 6),
            };
            this.Controls.Add(HeaderContainer);

            this.Header = new PropertyContainer
            {
                Height = 40,
                AutoScroll = false,
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                Padding = new Padding(0),
                Margin = new Padding(6),
                Font = MainForm.HeaderFont
            };
            this.HeaderContainer.Controls.Add(Header);

            this.ApplyButton = new FlatButton
            {
                Text = "Apply",
                IconOffset = 0,
                Padding = new Padding(6, 0, 6, 0),
                Font = MainForm.MainFont
            };
            this.Header.Controls.Add(ApplyButton);

            this.CancelButton = new FlatButton
            {
                Text = "Cancel",
                IconOffset = 0,
                Padding = new Padding(6, 0, 6, 0),
                Font = MainForm.MainFont
            };
            this.Header.Controls.Add(CancelButton);

            this.AdditionalPropertiesContainer = new FlexColumnContainer
            {
                AutoScroll = false,
                ColumnsLayout = FlexColumnsLayoutEngine.ColumnsLayout.VerticalGrid,
                Height = 600,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(6, 0, 6, 6),
                Margin = new Padding(0),
                MinColumnCount = 1,
                MaxColumnCount = 4,
                MinColumnSize = 240,
                MaxColumnSize = 0,
            };
            this.HeaderContainer.Controls.Add(AdditionalPropertiesContainer);

            this.TopAddItemButton = new FlatButton
            {
                Text = "Add Item",
                Margin = new Padding(0, 6, 0, 6),
                UseButtonBorder = false
            };
            this.Controls.Add(TopAddItemButton);

            this.ItemsView = new VerticalListContainer
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackgroundSource = Theme.BackgroundSource.Inherit,
                Margin = new Padding(0),
                Padding = new Padding(0),
            };
            this.Controls.Add(ItemsView);

            this.BottomAddItemButton = new FlatButton
            {
                Text = "Add Item",
                Margin = new Padding(0, 6, 0, 16),
                UseButtonBorder = false,
                Visible = false
            };
            this.Controls.Add(BottomAddItemButton);

            this.Items = new StringFormattingCollection(this, this.ItemsView);
        }

        protected virtual void InitializeHandles()
        {
            this.ItemsView.ControlAdded += (object sender, ControlEventArgs e) =>
            {
                this.BottomAddItemButton.Visible = true;
            };

            this.ItemsView.ControlRemoved += (object sender, ControlEventArgs e) =>
            {
                this.BottomAddItemButton.Visible = this.ItemsView.Controls.Count > 0;
            };

            this.TopAddItemButton.Click += (object sender, EventArgs e) =>
            {
                this.Items.Insert(0, new MASM_FormattingItem(MASM_FormattingItem.ItemMode.Property));
            };

            this.BottomAddItemButton.Click += (object sender, EventArgs e) =>
            {
                this.Items.Add(new MASM_FormattingItem(MASM_FormattingItem.ItemMode.Property));
            };

            this.ApplyButton.Click += (object sender, EventArgs e) =>
            {
                OnApply(e);
            };

            this.CancelButton.Click += (object sender, EventArgs e) =>
            {
                if (MessageBox.Show(this,
                    "Are you sure? All unsaved data will be lost.",
                    "Confirm cancellation",
                    MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    OnCancel(e);
                }
            };
        }

        protected void OnApply(EventArgs e)
        {
            Apply?.Invoke(this, e);
        }

        protected void OnCancel(EventArgs e)
        {
            Cancel?.Invoke(this, e);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.Update();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.Select();
        }

        public class StringFormattingCollection : IList<MASM_FormattingItem>
        {
            private MASMFormattingEditor editor;
            private Control view;
            private List<MASM_FormattingItem> items = new List<MASM_FormattingItem>();

            public StringFormattingCollection(MASMFormattingEditor editor, Control view)
            {
                this.editor = editor;
                this.view = view;
            }

            public MASM_FormattingItem this[int index]
            {
                get { return items[index]; }
                set
                {
                    items[index] = value;

                    if (view?.Controls?[index] is MASMFormattingItemEditor)
                        (view?.Controls?[index] as MASMFormattingItemEditor).UpdateValues();
                }
            }

            public void Add(MASM_FormattingItem item)
            {
                items.Add(item);

                if (view != null)
                {
                    view.Controls.Add(new MASMFormattingItemEditor
                    {
                        TargetCollection = this,
                        Index = this.Count - 1,
                        AvailableProperties = editor.AvailableProperties,
                    });
                }
            }

            public void AddRange(IEnumerable<MASM_FormattingItem> collection)
            {
                foreach (MASM_FormattingItem item in collection)
                {
                    this.Add(item);
                }
            }

            public void RemoveAt(int index)
            {
                items.RemoveAt(index);

                if (view != null && view.Controls.Count > index)
                {
                    view.Controls.RemoveAt(index);
                    view.PerformLayout();
                }

                UpdateControlsIndex(index);
            }

            public void Insert(int index, MASM_FormattingItem item)
            {
                items.Insert(index, item);

                if (view != null)
                {
                    MASMFormattingItemEditor itemEditor = new MASMFormattingItemEditor
                    {
                        TargetCollection = this,
                        Index = index,
                        AvailableProperties = editor.AvailableProperties,
                    };

                    view.Controls.Add(itemEditor);
                    view.Controls.SetChildIndex(itemEditor, index);
                    UpdateControlsIndex(index);
                }
            }

            public void Clear()
            {
                items.Clear();
                view?.Controls.Clear();
            }

            public void Move(int oldIndex, int newIndex)
            {
                MASM_FormattingItem removedItem = this[oldIndex];
                items.RemoveAt(oldIndex);
                items.Insert(newIndex, removedItem);
                view.Controls[newIndex].Select();
                UpdateControlsIndex(oldIndex, newIndex);
            }

            public bool Remove(MASM_FormattingItem item)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] == item)
                    {
                        this.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            public void UpdateControlsIndex(int startIndex)
            {
                if (view == null) return;
                UpdateControlsIndex(startIndex, view.Controls.Count - 1);
            }

            public void UpdateControlsIndex(int startIndex, int endIndex)
            {
                if (view == null) return;

                int start = Math.Min(startIndex, endIndex);
                int end = Math.Max(startIndex, endIndex);

                if (start < 0)
                    start = 0;

                if (end >= view.Controls.Count)
                    end = view.Controls.Count - 1;

                for (int i = start; i <= end; i++)
                {
                    if (view.Controls[i] is MASMFormattingItemEditor)
                        (view.Controls[i] as MASMFormattingItemEditor).Index = i;
                }
            }

            public int Count => items.Count;

            public bool IsReadOnly => ((IList<MASM_FormattingItem>)items).IsReadOnly;

            public bool Contains(MASM_FormattingItem item)
            {
                return items.Contains(item);
            }

            public void CopyTo(MASM_FormattingItem[] array, int arrayIndex)
            {
                items.CopyTo(array, arrayIndex);
            }

            public int IndexOf(MASM_FormattingItem item)
            {
                return items.IndexOf(item);
            }

            public IEnumerator<MASM_FormattingItem> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return items.GetEnumerator();
            }
        }
    }
}