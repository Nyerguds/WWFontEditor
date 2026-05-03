using System;
using System.Collections.Generic;
using System.Linq;
using Nyerguds.FileData.EmotionalPictures;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileEmo : FontFile
    {
        const Int32 ImgWidth = 0x11;
        const Int32 ImgHeight = 0x11;
        const Int32 ImgSize = ImgWidth * ImgHeight;
        public override Int32 SymbolsTypeMin { get { return 0xFF; } }
        public override Int32 SymbolsTypeMax { get { return 0xFF; } }
        public override Int32 FontWidthTypeMin { get { return 0x00; } }
        public override Int32 FontWidthTypeMax { get { return ImgWidth; } }
        public override Int32 FontHeightTypeMin { get { return ImgHeight; } }
        public override Int32 FontHeightTypeMax { get { return ImgHeight; } }
        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "ppp" }; } }
        public override String ShortTypeName { get { return "EmoFnt (CGSP)"; } }
        public override String ShortTypeDescription { get { return "Emotional Pictures font"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with max. 255 characters, an internal symbol size of 17x17, and variable widths per symbol. The space is saved as width, but the game ignores it and just uses 4. The font can be optimised by saving duplicate symbols only one time, and is compressed."; } }
        public override String[] GamesListForType { get { return new String[] { "Cover Girl Strip Poker" }; } }
        

        public override void LoadFont(Byte[] fileData)
        {
            // Basic premise: after the 0x200 byte header is an array of 17x17 images.
            // For each index for 0 to 254, check in the first block of 0x100 which image to use,
            // then check in the second block of 0x100 how much width to take from that image.
            // Index 255 is unused, but the width data seems to holds the width of the space character.
            Byte[] decompressedData;
            try
            {
                decompressedData = PppCompression.DecompressPppRle(fileData);
            }
            catch (ArgumentException)
            {
                throw new FileTypeLoadException(ERR_DECOMPRESS);
            }

            if (decompressedData.Length < 0x200)
                throw new FileTypeLoadException(ERR_NOHEADER);
            this.m_FontHeight = this.FontHeightTypeMax;
            // Wlll be increased to the max found in the file.
            this.m_FontWidth = 0;
            Int32 nrOfImages = decompressedData.Length - 0x200;
            if (nrOfImages % ImgSize != 0)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            nrOfImages /= ImgSize;

            FontFileSymbol[] imageDataList = new FontFileSymbol[0xFF];
            for (Int32 i = 0; i < 0xFF; ++i)
            {
                Int32 symbWidth = decompressedData[0x100 + i];
                // Easy check: symbol widths cannot exceed 17. If any do, consider the file to be not of this type.
                if (symbWidth > ImgWidth)
                    throw new FileTypeLoadException(ERR_MAXWIDTH);
                if (symbWidth == 0)
                    imageDataList[i] = new FontFileSymbol(this);
                else
                {
                    Byte imageNr = decompressedData[i];
                    if (imageNr >= nrOfImages)
                        throw new FileTypeLoadException("Referenced image index does not exist in file!");
                    Int32 offset = 0x200 + ImgSize * imageNr;
                    Byte[] symbol = new Byte[ImgSize];
                    Array.Copy(decompressedData, offset, symbol, 0, ImgSize);
                    FontFileSymbol ffs = new FontFileSymbol(symbol, 0x11, 0x11, 0, this.BitsPerPixel, this.TransparencyColor);
                    if (symbWidth > this.m_FontWidth)
                        this.m_FontWidth = symbWidth;
                    ffs.ChangeWidth(symbWidth, this.TransparencyColor);
                    imageDataList[i] = ffs;
                }
            }
            // Set space character width (though I think it's not used by the game).
            imageDataList[0x20].ChangeWidth(0, this.TransparencyColor);
            imageDataList[0x20].ChangeWidth(decompressedData[0x100 + 0xFF], this.TransparencyColor);
            this.m_ImageDataList = new List<FontFileSymbol>(imageDataList);
        }

        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            return new SaveOption[] { new SaveOption("OPT", SaveOptionType.Boolean, "Optimise to remove duplicate symbols", "1") };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 imagesCount = this.m_ImageDataList.Count;
            Byte[] imageNrs = new Byte[0x100];
            Byte[] imageWidths = new Byte[0x100];
            List<Byte[]> images = new List<Byte[]>();
            Int32 imageNr = 0;

            Boolean optimise = GeneralUtils.IsTrueValue(SaveOption.GetSaveOptionValue(saveOptions, "OPT"));

            for (Int32 i = 0; i < imagesCount; ++i)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                Int32 foundIndex = -1;
                if (i == 0x20)
                {
                    // The fact this is hijacked means the space character is never saved as image.
                    imageWidths[0xFF] = (Byte) ffs.Width;
                }
                else if (ffs.Width > 0)
                {
                    Byte[] currentImage = ImageUtils.ChangeStride(ffs.ByteData, ffs.Width, ffs.Height, 17, false, this.TransparencyColor);
                    if (optimise)
                    {
                        for (Int32 j = 0; j < images.Count; ++j)
                        {
                            foundIndex = -1;
                            if (currentImage.SequenceEqual(images[j]))
                            {
                                foundIndex = j;
                                imageNrs[i] = (Byte)foundIndex;
                                break;
                            }
                        }
                    }
                    if (!optimise || foundIndex == -1)
                    {
                        imageNrs[i] = (Byte)imageNr++;
                        images.Add(currentImage);
                    }
                    imageWidths[i] = (Byte)ffs.Width;
                }
            }
            Byte[] fullData = new Byte[0x200 + imageNr * ImgSize];
            Array.Copy(imageNrs, 0, fullData, 0, 0x100);
            Array.Copy(imageWidths, 0, fullData, 0x100, 0x100);
            for (Int32 i = 0; i < imageNr; i++)
                Array.Copy(images[i], 0, fullData, 0x200 + i * ImgSize, ImgSize);
            return PppCompression.CompressPppRle(fullData);
        }
    }
}