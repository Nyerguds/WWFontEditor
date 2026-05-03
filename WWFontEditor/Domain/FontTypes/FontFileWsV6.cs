using System;
using Nyerguds.Util;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Westwood Studios RA2/Nox font format.
    /// </summary>
    public class FontFileWsV6 : FontFile
    {
        public override int SymbolsTypeMax { get { return 0x100; } }
        public override int FontWidthTypeMax { get { return 0xFF; } }
        public override int FontHeightTypeMax { get { return 0xFF; } }
        public override int YOffsetTypeMax { get { return 0; } }
        public override int BitsPerPixel { get { return 1; } }

        public override String ShortTypeName { get { return "WWFont v6"; } }
        public override String ShortTypeDescription { get { return "WWFont v6 (RA2, NoX)"; } }
        public override String LongTypeDescription { get { return "A 1-bpp font which supports unicode."; } }
        public override String[] GamesListForType { get { return new String[] { "Command & Conquer Red Alert 2", "Nox", }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // BIG FAT TODO - this is not done yet at all.
            if (fileData.Length < 0x1C)
                throw new FileTypeLoadException(ERR_NOHEADER);
            String format = Encoding.ASCII.GetString(fileData, 0, 4);
            if (!String.Equals(format, "FoNt", StringComparison.InvariantCulture))
                throw new FileTypeLoadException(ERR_BADHEADER);
            //UInt32 dataStart? = (UInt32) ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            Int32 stride = (Int32) ArrayUtils.ReadIntFromByteArray(fileData, 0x08, 4, true);
            this.m_FontHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 4, true); // non-U: ok 
            this.m_FontWidth = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x10, 4, true); // non-U: ok 
            // should always be "1".
            Int32 count = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 4, 0x14, true); //  // non-U: set to '1'
            Int32 symbolSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x18, 4, true); // non-U: ok
            
            //UInt32 dword1C = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0x1C, 4, true);
            //UInt32 pointer = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0x20, 4, true);
            //UInt32 dword24 = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0x24, 4, true);
            //UInt32 startSymbol = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0x28, 4, true);
            //UInt32 endSymbol = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0x2C, 4, true);

        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return null;
        }

    }
}
