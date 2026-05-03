using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Nyerguds.Util.Ui
{
    /// <summary>
    /// Offers the ability to list user controls, which can send updates of their child controls back to a controller.
    /// </summary>
    /// <typeparam name="TControl">Type of the user controls with which to populate the list.</typeparam>
    /// <typeparam name="TInfoObject">Type of the information objects that contain all information to create/manage a listed control.</typeparam>
    public abstract partial class ControlsList<TControl,TInfoObject> : UserControl where TControl : Control
    {
        protected List<TControl> m_Contents = new List<TControl>();

        protected ControlsList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populate the list with controls.
        /// </summary>
        /// <param name="customControlInfo">Contains a list of information objects with which to create the custom controls.</param>
        /// <param name="controller">The controller to assign to the created custom controls.</param>
        public void Populate(CustomControlInfo<TControl, TInfoObject> customControlInfo, ListedControlController<TInfoObject> controller)
        {
            this.Reset();
            if (customControlInfo == null)
                return;
            this.SuspendLayout();
            this.lblTypeName.Text = customControlInfo.Name;
            foreach (TInfoObject controlInfo in customControlInfo.Properties)
            {
                try
                {
                    TControl control = customControlInfo.MakeControl(controlInfo, controller);
                    this.AddControl(control, false);
                }
                catch (NotImplementedException) { /* ignore */ }
            }
            this.PerformLayout();
        }

        /// <summary>
        /// Focus the first listed item.
        /// </summary>
        public void FocusFirst()
        {
            if (m_Contents.Count == 0)
                return;
            this.FocusItem(this.m_Contents[0]);
        }

        /// <summary>
        /// Focus the item. Can be overridden to focus a specific sub-control on the item.
        /// </summary>
        /// <param name="control">The control to focus.</param>
        protected virtual void FocusItem(TControl control)
        {
            control.Focus();
        }

        protected void AddControl(TControl control, Boolean refresh)
        {
            if (refresh)
                this.SuspendLayout();
            Int32 YPos;
            if (m_Contents.Count == 0)
                YPos = this.lblTypeName.Location.Y * 2 + this.lblTypeName.Size.Height;
            else
            {
                TControl lastControl = m_Contents[m_Contents.Count - 1];
                YPos = lastControl.Location.Y + lastControl.Size.Height;
            }
            control.Location = new Point(0, YPos);
            this.m_Contents.Add(control);
            this.Controls.Add(control);
            control.TabIndex = this.Controls.Count;
            control.Size = new Size(this.DisplayRectangle.Width, control.Size.Height);
            this.Size = new Size(this.Size.Width, YPos + control.Size.Height);
            if (refresh)
                this.PerformLayout();
        }

        public void Reset()
        {
            this.SuspendLayout();
            this.lblTypeName.Text = String.Empty;
            foreach (TControl c in m_Contents)
            {
                this.Controls.Remove(c);
                c.Dispose();
            }
            this.m_Contents.Clear();
            this.PerformLayout();
        }

        protected void EffectBarList_Resize(object sender, EventArgs e)
        {
            this.SuspendLayout();
            foreach (TControl c in m_Contents)
                c.Size = new Size(this.DisplayRectangle.Width, c.Size.Height);
            this.PerformLayout();
        }
    }
}
