using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AfterburnerDataHandler.FlatControls
{
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    class TabViewDesigner : ParentControlDesigner
    {
        private bool isSelected = true;
        
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));

            if (selectionService != null)
                selectionService.SelectionChanged += this.OnSelectionChanged;

            //MessageBox.Show(this.Component.ToString());

            if (this.Control is TabView)
            {
                TabView view = this.Control as TabView;
                
                view.ControlAdded += TabViewControlAdded;
            }

            this.Verbs.Add(new DesignerVerb("+ Add Tab", (object sender, EventArgs e) =>
            {
                AddTab();
            }));

            this.Verbs.Add(new DesignerVerb("- Remove Tab", (object sender, EventArgs e) =>
            {
                RemoveTab();
            }));

            this.Verbs.Add(new DesignerVerb("\x25B2 Move Back", (object sender, EventArgs e) =>
            {
                TabPanel currentTab = this.Control is TabView ? (Control as TabView).SelectedTab : null;
                MoveTab(currentTab, GetNearbyTab(currentTab, true));

            }));

            this.Verbs.Add(new DesignerVerb("\x25BC Move Forward", (object sender, EventArgs e) =>
            {
                TabPanel currentTab = this.Control is TabView ? (Control as TabView).SelectedTab : null;
                MoveTab(currentTab, GetNearbyTab(currentTab, false));
            }));
        }

        private void TabViewControlAdded(object sender, ControlEventArgs e)
        {
            //MessageBox.Show(e.Control.IsHandleCreated.ToString());
        }

        protected virtual TabPanel GetNearbyTab(TabPanel tab, bool searchPrevious)
        {
            if (!(this.Control is TabView) || tab == null || !this.Control.Controls.Contains(tab)) return null;

            bool tabFound = false;
            TabPanel nearbyTab = null;

            foreach (Control c in this.Control.Controls)
            {
                if (!(c is TabPanel)) continue;

                if (c == tab)
                {
                    tabFound = true;
                }

                if (searchPrevious)
                {
                    if (tabFound) break;
                    nearbyTab = c as TabPanel;
                }
                else if (tabFound && c != tab)
                {
                    nearbyTab = c as TabPanel;
                    break;
                }

            }

            return nearbyTab;
        }

        protected virtual void MoveTab(TabPanel from, TabPanel to)
        {
            if (!(this.Control is TabView)) return;

            if (from == null || to == null ||
                !this.Control.Controls.Contains(from) ||
                !this.Control.Controls.Contains(to)) return;

            IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            TabView tabView = this.Control as TabView;

            if (designerHost == null) return;

            using (DesignerTransaction transaction = designerHost.CreateTransaction($"Add tab to \"{tabView.Name}\""))
            {
                try
                {
                    PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(tabView)[nameof(tabView.Controls)];

                    this.RaiseComponentChanging(controlsProperty);
                    this.Control.Controls.SetChildIndex(from, Control.Controls.GetChildIndex(to));
                    this.RaiseComponentChanged(controlsProperty, null, null);

                    tabView.UpdateLayout();
                    tabView.Refresh();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
            }
        }

        protected virtual void AddTab()
        {
            if (!(this.Control is TabView)) return;

            IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            TabView tabView = this.Control as TabView;

            if (designerHost == null) return;
            
            using (DesignerTransaction transaction = designerHost.CreateTransaction($"Add tab to \"{tabView.Name}\""))
            {
                try
                {
                    TabPanel tab = (TabPanel)designerHost.CreateComponent(typeof(TabPanel));
                    PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(tabView)[nameof(tabView.Controls)];
                    PropertyDescriptor tabTextProperty = TypeDescriptor.GetProperties(tab)[nameof(tab.Text)];

                    this.RaiseComponentChanging(controlsProperty);
                    tabView.Controls.Add(tab);
                    this.RaiseComponentChanged(controlsProperty, null, null);

                    tabTextProperty.SetValue(tab, tab.Name);
                    tabView.ShowTab(tab);

                    tabView.UpdateLayout();
                    tabView.Refresh();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
            }
        }

        protected virtual void RemoveTab()
        {
            if (!(this.Control is TabView)) return;

            IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            TabView tabView = this.Control as TabView;
            TabPanel tabPanel = null;

            if (tabView.SelectedTab != null)
                tabPanel = tabView.SelectedTab;
            
            if (designerHost == null || tabPanel == null) return;

            using (DesignerTransaction transaction = designerHost.CreateTransaction($"Remove tab from \"{tabView.Name}\""))
            {
                try
                {
                    PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(tabView)[nameof(tabView.Controls)];

                    this.RaiseComponentChanging(controlsProperty);
                    designerHost.DestroyComponent(tabPanel);
                    this.RaiseComponentChanged(controlsProperty, null, null);

                    tabView.UpdateLayout();
                    tabView.Refresh();
                    transaction.Commit();
                }
                catch 
                {
                    transaction.Cancel();
                }
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
            ICollection selectedComponents = selectionService?.GetSelectedComponents();

            isSelected = false;
            
            if (selectedComponents != null)
            {
                foreach (object component in selectedComponents)
                {
                    if (component == this.Component)
                        isSelected = true;
                }
            }
        }

        public override bool CanParent(Control control)
        {
            return false;
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            de.Effect = DragDropEffects.None;
        }

        protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y,
            int width, int height, bool hasLocation, bool hasSize)
        {
            return null;
        }

        

        protected override void WndProc(ref Message m)
        {
            if (isSelected == true && (m.Msg == 0x201 || m.Msg == 0x0200 || m.Msg == 0x02A3))
            {
                DefWndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
