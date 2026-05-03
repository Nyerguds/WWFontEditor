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
        private Regex parse = new Regex("\\s*\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\],?\\s*");
        
        public Int32[] CustomColors { get; set; }

        private Point[] m_ShadowCoords;
        public Point[] ShadowCoords
        {
            get {return m_ShadowCoords;}
            set
            {
                m_ShadowCoords = value;
                SetCoordsText(m_ShadowCoords);
            }
        }
        
        private Color m_ShadowColor = Color.Black;
        public Color ShadowColor
        {
            get {return m_ShadowColor;}
            set
            {
                m_ShadowColor = value;
                this.lblValShadowColor.BackColor = value;
            }
        }

        public FrmSetshadow()
        {
            InitializeComponent();
        }

        private void SetCoordsText(Point[] coords)
        {
            if (coords == null || coords.Length == 0)
            {                
                txtCoords.Text = String.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
            Boolean first = true;
            foreach (Point p in coords.Distinct())
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append('[').Append(p.X).Append(',').Append(p.Y).Append(']');
            }
            txtCoords.Text = sb.ToString();
        }
        
        private void ColorLabel_Click(object sender, EventArgs e)
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            String coords = txtCoords.Text;
            List<Point> newPoints = new List<Point>();
            Match match = parse.Match(coords);
            while (match.Success)
            {
                Int32 x = Int32.Parse(match.Groups[1].Value);
                Int32 y = Int32.Parse(match.Groups[2].Value);
                newPoints.Add(new Point(x,y));
                match = match.NextMatch();
            }
            m_ShadowCoords = newPoints.Distinct().ToArray();
            m_ShadowColor = lblValShadowColor.BackColor;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
