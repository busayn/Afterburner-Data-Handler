using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{
    public class MASM_FormattingItemsList : IList<MASM_FormattingItem>
    {
        public event EventHandler<EventArgs> ListChanged;

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;

                if (value == true)
                    OnListChanged(EventArgs.Empty);
            }
        }

        private List<MASM_FormattingItem> items = new List<MASM_FormattingItem>();
        private bool isDirty = false;

        public MASM_FormattingItem this[int index]
        {
            get { return items[index]; }
            set
            {
                bool isNewValue = items[index] != value;
                items[index] = value;

                if (isNewValue == true)
                    IsDirty = true;
            }
        }

        public int Count { get { return items.Count; } }

        public bool IsReadOnly { get { return ((IList<MASM_FormattingItem>)items).IsReadOnly; } }

        public void Add(MASM_FormattingItem item)
        {
            items.Add(item);
            IsDirty = true;
        }

        public void AddRange(IEnumerable<MASM_FormattingItem> collection)
        {
            items.AddRange(collection);
            IsDirty = true;
        }

        public void Insert(int index, MASM_FormattingItem item)
        {
            items.Insert(index, item);
            IsDirty = true;
        }

        public bool Remove(MASM_FormattingItem item)
        {
            bool removeResult = items.Remove(item);
            IsDirty = true;
            return removeResult;
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
            IsDirty = true;
        }

        public void Clear()
        {
            items.Clear();
            IsDirty = true;
        }

        public bool Contains(MASM_FormattingItem item)
        {
            return items.Contains(item);
        }
        public int IndexOf(MASM_FormattingItem item)
        {
            return items.IndexOf(item);
        }

        public void CopyTo(MASM_FormattingItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IList<MASM_FormattingItem> list)
        {
            if (list == null || list.IsReadOnly == true) return;

            foreach (MASM_FormattingItem item in items)
            {
                list.Add(item);
            }
        }

        public IEnumerator<MASM_FormattingItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        protected virtual void OnListChanged(EventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }
    }
}