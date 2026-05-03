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
        private const String INI_KEY_ENABLEPREVIEWWRAP = "EnablePreviewWrap";

        private const String INI_SECTION_PALETTES = "Palettes";
        private const String INI_KEY_GENERATE1BITBR = "1BitBR";
        private const String INI_KEY_GENERATE1BITBW = "1BitBW";
        private const String INI_KEY_GENERATE1BITWB = "1BitWB";
        private const String INI_KEY_GENERATE4BITRAINBOW = "4BitRainbow";
        private const String INI_KEY_GENERATE4BITBW = "4BitBW";
        private const String INI_KEY_GENERATE4BITWB = "4BitWB";
        private const String INI_KEY_GENERATE4BITWINDOWS = "4BitWindows";
        private const String INI_KEY_LIMIT8BITPALETTES = "Limit8BitPalettes";
        private const String INI_KEY_GENERATE8BITRAINBOW = "8BitRainbow";
        private const String INI_KEY_GENERATE8BITWINDOWS = "8BitWindows";
        private const String INI_KEY_GENERATE8BITBW = "8BitBW";
        private const String INI_KEY_GENERATE8BITWB = "8BitWB";

        private const String INI_SECTION_IO = "IO";
        private const String INI_KEY_DISABLECOMPRESSION = "DisableCompression";


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
        public const Boolean DefEnablePreviewWrap = false;

        public const Boolean DefGenerate1BitBR = true;
        public const Boolean DefGenerate1BitBW = true;
        public const Boolean DefGenerate1BitWB = true;

        public const Boolean DefGenerate4BitRainbow = true;
        public const Boolean DefGenerate4BitBW = true;
        public const Boolean DefGenerate4BitWB = true;
        public const Boolean DefGenerate4BitWindows = true;

        public const Boolean DefLimit8BitPalettes = true;
        public const Boolean DefGenerate8BitRainbow = true;
        public const Boolean DefGenerate8BitWindows = true;
        public const Boolean DefGenerate8BitBW = true;
        public const Boolean DefGenerate8BitWB = true;

        public const Boolean DefDisableCompression = false;
        
        
        public Color EditAreaGrid { get; set; }
        public Color EditAreaFrame { get; set; }
        public Color BackgroundGrid { get; set; }
        public Color BackgroundFrame { get; set; }
        public Color Background { get; set; }
        public Boolean UsePaletteBG { get; set; }

        public Int32 Zoom { get; set; }
        public Int32 SelectedSymbol { get; set; }
        public Boolean EnableGrid { get; set; }
        public Boolean EnableArea { get; set; }
        public Boolean EnablePixelWrap { get; set; }
        public Boolean EnablePreviewWrap { get; set; }

        public Boolean Generate1BitBR { get; set; }
        public Boolean Generate1BitBW { get; set; }
        public Boolean Generate1BitWB { get; set; }
        
        public Boolean Generate4BitRainbow { get; set; }
        public Boolean Generate4BitBW { get; set; }
        public Boolean Generate4BitWB { get; set; }
        public Boolean Generate4BitWindows { get; set; }

        public Boolean Limit8BitPalettes { get; set; }
        public Boolean Generate8BitRainbow { get; set; }
        public Boolean Generate8BitWindows { get; set; }
        public Boolean Generate8BitBW { get; set; }
        public Boolean Generate8BitWB { get; set; }

        public Boolean DisableCompression { get; set; }
        
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
            this.EditAreaGrid = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAGRID, null), DefEditAreaGrid);
            this.EditAreaFrame = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAFRAME, null), DefEditAreaFrame);
            this.BackgroundGrid = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDGRID, null), DefBackgroundGrid);
            this.BackgroundFrame = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDFRAME, null), DefBackgroundFrame);
            this.Background = ColorFromString(settings.GetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUND, null), DefBackground);
            this.UsePaletteBG = settings.GetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_USEPALETTEBG, DefUsePaletteBG);
            
            this.Zoom = settings.GetIntValue(INI_SECTION_DEFAULTS, INI_KEY_ZOOM, DefZoom);
            this.SelectedSymbol = settings.GetIntValue(INI_SECTION_DEFAULTS, INI_KEY_SELECTEDSYMBOL, DefSelectedSymbol);
            this.EnableGrid = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEGRID, DefEnableGrid);
            this.EnableArea = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEAREA, DefEnableArea);
            this.EnablePixelWrap = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPIXELWRAP, DefEnablePixelWrap);
            this.EnablePreviewWrap = settings.GetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPREVIEWWRAP, DefEnablePreviewWrap);

            this.Generate1BitBR = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBR, DefGenerate1BitBR);
            this.Generate1BitBW = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBW, DefGenerate1BitBW);
            this.Generate1BitWB = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITWB, DefGenerate1BitWB);
            // Don't allow no defaults at all.
            if (!this.Generate1BitBR && !this.Generate1BitBW && !this.Generate1BitWB)
                this.Generate1BitBR = true;
            this.Generate4BitRainbow = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITRAINBOW, DefGenerate4BitRainbow);
            this.Generate4BitWindows = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWINDOWS, DefGenerate4BitWindows);
            this.Generate4BitBW = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITBW, DefGenerate4BitBW);
            this.Generate4BitWB = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWB, DefGenerate4BitWB);
            // Don't allow no defaults at all.
            if (!this.Generate4BitRainbow && !this.Generate4BitBW && !this.Generate4BitWB && !this.Generate4BitWindows)
                this.Generate4BitRainbow = true;

            this.Limit8BitPalettes = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_LIMIT8BITPALETTES, DefLimit8BitPalettes);
            this.Generate8BitRainbow = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITRAINBOW, DefGenerate8BitRainbow);
            this.Generate8BitWindows = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWINDOWS, DefGenerate8BitWindows);
            this.Generate8BitBW = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITBW, DefGenerate8BitBW);
            this.Generate8BitWB = settings.GetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWB, DefGenerate8BitWB);
            // Don't allow no defaults at all.
            if (!this.Generate8BitRainbow && !this.Generate8BitBW && !this.Generate8BitWB && !this.Generate8BitWindows)
                this.Generate8BitRainbow = true;
            
            this.DisableCompression = settings.GetBoolValue(INI_SECTION_IO, INI_KEY_DISABLECOMPRESSION, DefDisableCompression);
        }
        
        public Boolean SaveSettings()
        {
            IniFile settings = GetSettingsFile();
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAGRID, ColorToString(this.EditAreaGrid));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_EDITAREAFRAME, ColorToString(this.EditAreaFrame));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDGRID, ColorToString(this.BackgroundGrid));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUNDFRAME, ColorToString(this.BackgroundFrame));
            settings.SetStringValue(INI_SECTION_USERINTERFACE, INI_KEY_BACKGROUND, ColorToString(this.Background));
            settings.SetBoolValue(INI_SECTION_USERINTERFACE, INI_KEY_USEPALETTEBG, this.UsePaletteBG);

            settings.SetIntValue(INI_SECTION_DEFAULTS, INI_KEY_ZOOM, this.Zoom);
            settings.SetIntValue(INI_SECTION_DEFAULTS, INI_KEY_SELECTEDSYMBOL, this.SelectedSymbol);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEGRID, this.EnableGrid);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEAREA, this.EnableArea);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPIXELWRAP, this.EnablePixelWrap);
            settings.SetBoolValue(INI_SECTION_DEFAULTS, INI_KEY_ENABLEPREVIEWWRAP, this.EnablePreviewWrap);
            
            if (!this.Generate1BitBR && !this.Generate1BitBW && !this.Generate1BitWB)
                this.Generate1BitBR = true;
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBR, this.Generate1BitBR);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITBW, this.Generate1BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE1BITWB, this.Generate1BitWB);

            if (!this.Generate4BitRainbow && !this.Generate4BitWindows && !this.Generate4BitBW && !this.Generate4BitWB)
                this.Generate4BitRainbow = true;
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITRAINBOW, this.Generate4BitRainbow);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWINDOWS, this.Generate4BitWindows);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITBW, this.Generate4BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE4BITWB, this.Generate4BitWB);

            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_LIMIT8BITPALETTES, this.Limit8BitPalettes);
            if (!this.Generate8BitRainbow && !this.Generate8BitWindows && !this.Generate8BitBW && !this.Generate8BitWB)
                this.Generate8BitRainbow = true;
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITRAINBOW, this.Generate8BitRainbow);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWINDOWS, this.Generate8BitWindows);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITBW, this.Generate8BitBW);
            settings.SetBoolValue(INI_SECTION_PALETTES, INI_KEY_GENERATE8BITWB, this.Generate8BitWB);

            settings.SetBoolValue(INI_SECTION_IO, INI_KEY_DISABLECOMPRESSION, this.DisableCompression);

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
