using Nyerguds.Ini;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWFontEditor.Domain
{
    public class FontEditSettings
    {
        private const String INI_SECTION_USERINTERFACE = "UserInterface";
        private const String INI_KEY_EDITAREAGRID = "EditAreaGrid";
        private const String INI_KEY_EDITAREAFRAME = "EditAreaFrame";
        private const String INI_KEY_BACKGROUNDGRID = "BackgroundGrid";
        private const String INI_KEY_BACKGROUNDFRAME = "BackgroundFrame";
        private const String INI_KEY_BACKGROUND = "Background";
        private const String INI_KEY_USEPALETTEBG = "UsePaletteBG";

        private const String INI_SECTION_DEFAULTS = "Defaults";
        private const String INI_KEY_ZOOM = "Zoom";
        private const String INI_KEY_SELECTEDSYMBOL = "SelectedSymbol";
        private const String INI_KEY_ENABLEGRID = "EnableGrid";
        private const String INI_KEY_ENABLEAREA = "EnableArea";
        private const String INI_KEY_ENABLEPIXELWRAP = "EnablePixelWrap";

        private const String INI_SECTION_PALETTES = "Palettes";
        private const String INI_KEY_GENERATE1BITBR = "1BitBR";
        private const String INI_KEY_GENERATE1BITBW = "1BitBW";
        private const String INI_KEY_GENERATE1BITWB = "1BitWB";
        private const String INI_KEY_GENERATE4BITRAINBOW = "4BitRainbow";
        private const String INI_KEY_GENERATE4BITBW = "4BitBW";
        private const String INI_KEY_GENERATE4BITWB = "4BitWB";
        private const String INI_KEY_GENERATE4BITWINDOWS = "4BitWindows";
        private const String INI_KEY_GENERATE8BITRAINBOW = "8BitRainbow";
        private const String INI_KEY_GENERATE8BITWINDOWS = "8BitWindows";
        private const String INI_KEY_GENERATE8BITBW = "8BitBW";
        private const String INI_KEY_GENERATE8BITWB = "8BitWB";

        public static readonly Color DefEditAreaGrid = Color.Blue;
        public static readonly Color DefEditAreaFrame = Color.Red;
        public static readonly Color DefBackgroundGrid = Color.White;
        public static readonly Color DefBackgroundFrame = Color.Black;
        public static readonly Color DefBackground = Color.LightGray;
        public const Boolean DefUsePaletteBG = false;

        public const Int32   DefZoom = 20;
        public const Int32   DefSelectedSymbol = 32;
        public const Boolean DefEnableGrid = true;
        public const Boolean DefEnableArea = true;
        public const Boolean DefEnablePixelWrap = false;

        public const Boolean DefGenerate1BitBR = true;
        public const Boolean DefGenerate1BitBW = true;
        public const Boolean DefGenerate1BitWB = true;
        public const Boolean DefGenerate4BitRainbow = true;
        public const Boolean DefGenerate4BitBW = true;
        public const Boolean DefGenerate4BitWB = true;
        public const Boolean DefGenerate4BitWindows = true;
        public const Boolean DefGenerate8BitRainbow = true;
        public const Boolean DefGenerate8BitWindows = true;
        public const Boolean DefGenerate8BitBW = true;
        public const Boolean DefGenerate8BitWB = true;
        
        public Color EditAreaGrid { get { return m_EditAreaGrid; } set { m_EditAreaGrid = value; } }
        public Color EditAreaFrame { get { return m_EditAreaFrame; } set { m_EditAreaFrame = value; } }
        public Color BackgroundGrid { get { return m_BackgroundGrid; } set { m_BackgroundGrid = value; } }
        public Color BackgroundFrame { get { return m_BackgroundFrame; } set { m_BackgroundFrame = value; } }
        public Color Background { get { return m_Background; } set { m_Background = value; } }
        public Boolean UsePaletteBG { get { return m_UsePaletteBG; } set { m_UsePaletteBG = value; } }

        public Int32 Zoom { get { return m_Zoom; } set { m_Zoom = value; } }
        public Int32 SelectedSymbol { get { return m_SelectedSymbol; } set { m_SelectedSymbol = value; } }
        public Boolean EnableGrid { get { return m_EnableGrid; } set { m_EnableGrid = value; } }
        public Boolean EnableArea { get { return m_EnableArea; } set { m_EnableArea = value; } }
        public Boolean EnablePixelWrap { get { return this.m_EnablePixelWrap; } set { this.m_EnablePixelWrap = value; } }

        public Boolean Generate1BitBR { get { return m_Generate1BitBR; } set { m_Generate1BitBR = value; } }
        public Boolean Generate1BitBW { get { return m_Generate1BitBW; } set { m_Generate1BitBW = value; } }
        public Boolean Generate1BitWB { get { return m_Generate1BitWB; } set { m_Generate1BitWB = value; } }
        public Boolean Generate4BitRainbow { get { return m_Generate4BitRainbow; } set { m_Generate4BitRainbow = value; } }
        public Boolean Generate4BitBW { get { return m_Generate4BitBW; } set { m_Generate4BitBW = value; } }
        public Boolean Generate4BitWB { get { return m_Generate4BitWB; } set { m_Generate4BitWB = value; } }
        public Boolean Generate4BitWindows { get { return m_Generate4BitWindows; } set { m_Generate4BitWindows = value; } }
        public Boolean Generate8BitRainbow { get { return m_Generate8BitRainbow; } set { m_Generate8BitRainbow = value; } }
        public Boolean Generate8BitWindows { get { return m_Generate8BitWindows; } set { m_Generate8BitWindows = value; } }
        public Boolean Generate8BitBW { get { return m_Generate8BitBW; } set { m_Generate8BitBW = value; } }
        public Boolean Generate8BitWB { get { return m_Generate8BitWB; } set { m_Generate8BitWB = value; } }

        private Color m_EditAreaGrid;
        private Color m_EditAreaFrame;
        private Color m_BackgroundGrid;
        private Color m_BackgroundFrame;
        private Color m_Background;
        private Boolean m_UsePaletteBG;
        
        private Int32 m_Zoom;
        private Int32 m_SelectedSymbol;
        private Boolean m_EnableGrid;
        private Boolean m_EnableArea;
        private Boolean m_EnablePixelWrap;

        private Boolean m_Generate1BitBR;
        private Boolean m_Generate1BitBW;
        private Boolean m_Generate1BitWB;
        private Boolean m_Generate4BitRainbow;
        private Boolean m_Generate4BitBW;
        private Boolean m_Generate4BitWB;
        private Boolean m_Generate4BitWindows;
        private Boolean m_Generate8BitRainbow;
        private Boolean m_Generate8BitWindows;
        private Boolean m_Generate8BitBW;
        private Boolean m_Generate8BitWB;

        public FontEditSettings()
        {
            ReadSettings();
        }

        protected IniFile GetSettingsFile()
        {
            String iniPath = Application.ExecutablePath;
            if (iniPath.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                iniPath = iniPath.Substring(0, iniPath.Length - 4);
            iniPath += ".ini";
            return new IniFile(iniPath) { BooleanWriteMode = BooleanMode.TRUE_FALSE };
        }

        protected void ReadSettings()
        {
            IniFile settings = GetSettingsFile();
            this.m_EditAreaGrid = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAGRID, null), DefEditAreaGrid);
            this.m_EditAreaFrame = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAFRAME, null), DefEditAreaFrame);
            this.m_BackgroundGrid = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDGRID, null), DefBackgroundGrid);
            this.m_BackgroundFrame = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDFRAME, null), DefBackgroundFrame);
            this.m_Background = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUND, null), DefBackground);
            this.m_UsePaletteBG = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_USEPALETTEBG, DefUsePaletteBG);
            
            this.m_Zoom = settings.GetIntValue(INI_SECTION_DEFAULTS, INI_KEY_ZOOM, DefZoom);
            this.m_SelectedSymbol = settings.GetIntValue(INI_SECTION_DEFAULTS, INI_KEY_SELECTEDSYMBOL, DefSelectedSymbol);
            this.m_EnableGrid = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEGRID, DefEnableGrid);
            this.m_EnableArea = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEAREA, DefEnableArea);
            this.m_EnablePixelWrap = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPIXELWRAP, DefEnablePixelWrap);
            
            this.m_Generate1BitBR = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE1BITBR, DefGenerate1BitBR);
            this.m_Generate1BitBW = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE1BITBW, DefGenerate1BitBW);
            this.m_Generate1BitWB = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE1BITWB, DefGenerate1BitWB);
            // Don't allow no defaults at all.
            if (!m_Generate1BitBR && !m_Generate1BitBW && !m_Generate1BitWB)
                m_Generate1BitBR = true;
            this.m_Generate4BitRainbow = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE4BITRAINBOW, DefGenerate4BitRainbow);
            this.m_Generate4BitWindows = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE4BITWINDOWS, DefGenerate4BitWindows);
            this.m_Generate4BitBW = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE4BITBW, DefGenerate4BitBW);
            this.m_Generate4BitWB = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE4BITWB, DefGenerate4BitWB);
            // Don't allow no defaults at all.
            if (!m_Generate4BitRainbow && !m_Generate4BitBW && !m_Generate4BitWB && !m_Generate4BitWindows)
                m_Generate4BitRainbow = true;
            this.m_Generate8BitRainbow = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE8BITRAINBOW, DefGenerate8BitRainbow);
            this.m_Generate8BitWindows = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE8BITWINDOWS, DefGenerate8BitWindows);
            this.m_Generate8BitBW = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE8BITBW, DefGenerate8BitBW);
            this.m_Generate8BitWB = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_GENERATE8BITWB, DefGenerate8BitWB);
            // Don't allow no defaults at all.
            if (!m_Generate8BitRainbow && !m_Generate8BitBW && !m_Generate8BitWB && !m_Generate8BitWindows)
                m_Generate8BitRainbow = true;
        }
        
        public Boolean SaveSettings()
        {
            IniFile settings = GetSettingsFile();
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAGRID, ColorToString(m_EditAreaGrid));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAFRAME, ColorToString(m_EditAreaFrame));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDGRID, ColorToString(m_BackgroundGrid));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDFRAME, ColorToString(m_BackgroundFrame));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUND, ColorToString(m_Background));
            settings.SetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_USEPALETTEBG, m_UsePaletteBG);

            settings.SetIntValue(INI_SECTION_DEFAULTS, INI_KEY_ZOOM, m_Zoom);
            settings.SetIntValue(INI_SECTION_DEFAULTS, INI_KEY_SELECTEDSYMBOL, m_SelectedSymbol);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEGRID, m_EnableGrid);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEAREA, m_EnableArea);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPIXELWRAP, this.m_EnablePixelWrap);

            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBR, m_Generate1BitBR);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBW, m_Generate1BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITWB, m_Generate1BitWB);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITRAINBOW, m_Generate4BitRainbow);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWINDOWS, m_Generate4BitWindows);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITBW, m_Generate4BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWB, m_Generate4BitWB);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITRAINBOW, m_Generate8BitRainbow);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWINDOWS, m_Generate8BitWindows);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITBW, m_Generate8BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWB, m_Generate8BitWB);
            return settings.WriteIni();
        }

        private static String ColorToString(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private static Color ColorFromString(String colorString, Color defaultCol)
        {
            if (String.IsNullOrEmpty(colorString))
                return defaultCol;
            try { return ColorTranslator.FromHtml(colorString); }
            catch { return defaultCol; }
        }

    }
}
