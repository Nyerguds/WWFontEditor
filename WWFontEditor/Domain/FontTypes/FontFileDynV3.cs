using Nyerguds.Util;
using System;
using System.Linq;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>
    public class FontFileDynV3 : FontFileDynV2
    {
        public override String ShortTypeName { get { return "DYN v3"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v3 (DieH/CaveOl/F-14/Mech/A10)"; } }
        public override String LongTypeDescription { get { return "A 1 BPP font with the file header specifying the global width and height for all symbols and the amount of symbols. It is optimized by only saving the used range of symbols."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Die Hard", "Caveman Ugh-Lympics", "F-14 Tomcat", "Suzuki's RM250 Motocross", "MechWarrior", "A-10 Tank Killer", "A-10 Tank Killer v1.5", "Ghostbusters II", "DeathTrack", "David Wolf: Secret Agent" }; }
        }
        
        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x0C)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte[] sectionId = new Byte[4];
            Array.Copy(fileData, 0, sectionId, 0, 4);
            if (!sectionId.SequenceEqual(Encoding.ASCII.GetBytes("FNT:")))
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 fileSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            if (fileSize != fileData.Length - 8)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            this.LoadFont(fileData, 0x08);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return this.SaveFont(saveOptions, true);
        }

    }
}