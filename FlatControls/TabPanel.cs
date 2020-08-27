using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace AfterburnerDataHandler.FlatControls
{
    [Designer(typeof(TabPanelDesigner))]
    public class TabPanel : Panel
    {
        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text { get { return base.Text; } set { base.Text = value; } }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }

        public virtual Image Icon
        {
            get { return icon; }
            set
            {
                icon = value;

                if (icon == null)
                {

                }

                if (Parent is TabView)
                {
                    (Parent as TabView).UpdateLayout();
                }
            }
        }

        private Image icon = SystemIcons.Application.ToBitmap();

        public virtual bool ShowTab()
        {
            bool tabViewFound = this.Parent is TabView;

            if (tabViewFound == true)
            {
                tabViewFound = (this.Parent as TabView).ShowTab(this);
            }

            return tabViewFound;
        }

        public virtual void UpdateViewLayout()
        {
            if (Parent is TabView) (this.Parent as TabView).UpdateLayout();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Parent is TabView)
            {
                (this.Parent as TabView).UpdateLayout();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            UpdateViewLayout();
        }
    }
}
