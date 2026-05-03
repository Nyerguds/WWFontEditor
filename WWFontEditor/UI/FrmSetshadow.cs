using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WWFontEditor.UI
{
    public partial class FrmSetshadow : Form
    {
        private Regex coordsPattern = new Regex("\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\]");
        
        public Int32[] CustomColors { get; set; }

        private Point[] m_ShadowCoords;
        public Point[] ShadowCoords
        {
            get {return this.m_ShadowCoords;}
            set
            {
                this.m_ShadowCoords = value;
                this.SetCoordsText(this.m_ShadowCoords);
            }
        }
        
        private Color m_ShadowColor = Color.Black;
        public Color ShadowColor
        {
            get {return this.m_ShadowColor;}
            set
            {
                this.m_ShadowColor = value;
                this.lblValShadowColor.BackColor = value;
            }
        }

        public FrmSetshadow()
        {
            this.InitializeComponent();
        }

        private void SetCoordsText(Point[] coords)
        {
            if (coords == null || coords.Length == 0)
            {
                this.txtCoords.Text = String.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
            Boolean first = true;
            foreach (Point p in coords.Distinct())
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");
                sb.Append('[').Append(p.X).Append(',').Append(p.Y).Append(']');
            }
            this.txtCoords.Text = sb.ToString();
        }

        private void ColorLabel_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ' || e.KeyChar == '\r' || e.KeyChar == '\n')
                this.ColorLabel_Click(sender, e);
        }
        
        private void ColorLabel_Click(Object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = label.BackColor;
            cdl.FullOpen = true;
            cdl.CustomColors = this.CustomColors;
            DialogResult res = cdl.ShowDialog();
            this.CustomColors = cdl.CustomColors;
            if (res == DialogResult.OK)
            {
                label.BackColor = cdl.Color;
                label.ForeColor = cdl.Color;
            }
        }

        private void btnOk_Click(Object sender, EventArgs e)
        {
            String coords = this.txtCoords.Text;
            List<Point> newPoints = new List<Point>();
            Match match = this.coordsPattern.Match(coords);
            while (match.Success)
            {
                Int32 x = Int32.Parse(match.Groups[1].Value);
                Int32 y = Int32.Parse(match.Groups[2].Value);
                newPoints.Add(new Point(x,y));
                match = match.NextMatch();
            }
            this.m_ShadowCoords = newPoints.Distinct().ToArray();
            this.m_ShadowColor = this.lblValShadowColor.BackColor;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TextBoxSelectAll(Object sender, KeyEventArgs e)
        {
            if (!e.Control || e.Alt || e.Shift || e.KeyCode != Keys.A)
                return;
            TextBox tb = sender as TextBox;
            if (tb == null)
                return;
            tb.SelectAll();
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

    }
}
