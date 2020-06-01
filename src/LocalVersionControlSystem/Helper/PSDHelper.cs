//Original author: zgke@sina.com qq:116149
//Update after 11 years by RC
//Temporarily remove anything about write a PSD file
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;

namespace LocalVersionControlSystem.Helper
{
    public class PSDHelper
    {
        //PSD file part 1: Head
        private class Head
        {
            //Length of head is 26
            private byte[] _headBytes = new byte[26];

            //Get _headBytes
            public byte[] GetBytes()
            {
                return _headBytes;
            }

            //Version
            public byte Version
            {
                get
                {
                    return _headBytes[5];
                }
                set
                {
                    _headBytes[5] = value;
                }
            }

            //Number of channels
            public ushort Channels
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _headBytes[13], _headBytes[12] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _headBytes[13] = _Value[1];
                    _headBytes[12] = _Value[0];
                }
            }

            //Height
            public uint Height
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { _headBytes[17], _headBytes[16], _headBytes[15], _headBytes[14] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _headBytes[17] = _Value[0];
                    _headBytes[16] = _Value[1];
                    _headBytes[15] = _Value[2];
                    _headBytes[14] = _Value[3];
                }
            }

            //Width
            public uint Width
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { _headBytes[21], _headBytes[20], _headBytes[19], _headBytes[18] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _headBytes[21] = _Value[0];
                    _headBytes[20] = _Value[1];
                    _headBytes[19] = _Value[2];
                    _headBytes[18] = _Value[3];
                }
            }

            //Number of bits per channel
            public ushort BitsPerPixel
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _headBytes[23], _headBytes[22] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _headBytes[23] = _Value[1];
                    _headBytes[22] = _Value[0];
                }

            }

            //Color mode
            public ushort ColorMode
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _headBytes[25], _headBytes[24] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _headBytes[25] = _Value[1];
                    _headBytes[24] = _Value[0];
                }
            }

            public Head(FileStream fs)
            {
                fs.Read(_headBytes, 0, 26);
            }

            public Head()
            {

            }
        }

        //PSD file part 2: ColorMode
        private class ColorMode
        {
            //The size of color data in bytes
            private byte[] _sizeBytes = new byte[4];
            //Only color data in bytes
            private byte[] _colorModeBytes = Array.Empty<byte>();

            //The size of color data in uint
            public uint BIMSize
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { _sizeBytes[3], _sizeBytes[2], _sizeBytes[1], _sizeBytes[0] }, 0);
                }
                set
                {
                    byte[] _valueBytes = BitConverter.GetBytes(value);
                    _sizeBytes[0] = _valueBytes[3];
                    _sizeBytes[1] = _valueBytes[2];
                    _sizeBytes[2] = _valueBytes[1];
                    _sizeBytes[3] = _valueBytes[0];
                }
            }

            //Color data
            public ColorPalette ColorData
            {
                get
                {
                    using Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                    ColorPalette _ColorPalette = bitmap.Palette;
                    if (_colorModeBytes.Length == 0) return _ColorPalette;
                    for (int i = 0; i != 256; i++)
                    {
                        _ColorPalette.Entries[i] = Color.FromArgb(_colorModeBytes[i], _colorModeBytes[i + 256], _colorModeBytes[i + 512]);
                    }
                    return _ColorPalette;
                }
                set
                {
                    _colorModeBytes = new byte[768];
                    for (int i = 0; i != 256; i++)
                    {
                        _colorModeBytes[i] = value.Entries[i].R;
                        _colorModeBytes[i + 256] = value.Entries[i].G;
                        _colorModeBytes[i + 512] = value.Entries[i].B;
                    }
                }
            }

            public ColorMode(FileStream fs)
            {
                byte[] countByte = new byte[4];
                fs.Read(countByte, 0, 4);
                Array.Reverse(countByte);
                int count = BitConverter.ToInt32(countByte, 0);
                _colorModeBytes = new byte[count];
                if (count != 0)
                    fs.Read(_colorModeBytes, 0, count);
                fs.Read(_sizeBytes, 0, 4);
            }
        }

        //PSD file part 3: Image resources
        private class BIM
        {
            //8BIM
            private byte[] _signature = new byte[] { 0x38, 0x42, 0x49, 0x4D };
            private byte[] _typeID = new byte[2];
            private byte[] _name = Array.Empty<byte>();
            private byte[] _dataLength = Array.Empty<byte>();
            private byte[] _data = Array.Empty<byte>();

            public bool _isRead { get; set; } = false;

            //Get Type ID
            public ushort TypeID
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _typeID[1], _typeID[0] }, 0);
                }
                set
                {
                    byte[] valueBytes = BitConverter.GetBytes(value);
                    _typeID[0] = valueBytes[1];
                    _typeID[1] = valueBytes[0];
                }
            }

            public BIM(FileStream fs)
            {
                byte[] signature = new byte[4];
                fs.Read(signature, 0, 4);
                if (_signature[0] == signature[0] && _signature[1] == signature[1] && _signature[2] == signature[2] && _signature[3] == signature[3])
                {
                    fs.Read(_typeID, 0, 2);

                    int size = fs.ReadByte();
                    int nameLength = (int) size;
                    if (nameLength > 0)
                    {
                        if ((nameLength % 2) != 0)
                            size = fs.ReadByte();
                        _name = new byte[nameLength];
                        fs.Read(_name, 0, nameLength);
                    }
                    size = fs.ReadByte();

                    _dataLength = new byte[4];
                    fs.Read(_dataLength, 0, 4);
                    byte[] temp = _dataLength;
                    Array.Reverse(temp);
                    int dataLength = BitConverter.ToInt32(temp, 0);
                    if (dataLength % 2 != 0)
                        dataLength++;

                    _data = new byte[dataLength];
                    fs.Read(_data, 0, dataLength);
                    _isRead = true;
                }
            }

            public BIM()
            {

            }

            //For BIM with type = 1005, load ResolutionInfo
            #region Type=1005
            public ushort hRes
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _data[1], _data[0] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[0] = _Value[1];
                    _data[1] = _Value[0];
                }
            }
            public uint hResUnit
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { _data[5], _data[4], _data[3], _data[2] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[2] = _Value[3];
                    _data[3] = _Value[2];
                    _data[4] = _Value[1];
                    _data[5] = _Value[0];
                }
            }
            public ushort widthUnit
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _data[7], _data[6] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[6] = _Value[1];
                    _data[7] = _Value[0];
                }
            }
            public ushort vRes
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _data[9], _data[8] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[8] = _Value[1];
                    _data[9] = _Value[0];
                }
            }
            public uint vResUnit
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { _data[13], _data[12], _data[11], _data[10] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[10] = _Value[3];
                    _data[11] = _Value[2];
                    _data[12] = _Value[1];
                    _data[13] = _Value[0];
                }
            }
            public ushort heightUnit
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _data[15], _data[14] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _data[14] = _Value[1];
                    _data[15] = _Value[0];
                }
            }
            #endregion

        }

        //PSD file part 4: Layer and mask info
        private class LayerMaskInfo
        {

            public byte[] _layerMaskInfoBytes = Array.Empty<byte>();

            public LayerMaskInfo(FileStream p_Stream)
            {
                byte[] lengthBytes = new byte[4];
                p_Stream.Read(lengthBytes, 0, 4);
                Array.Reverse(lengthBytes);

                int length = BitConverter.ToInt32(lengthBytes, 0);

                _layerMaskInfoBytes = new byte[length];
                if (length != 0) p_Stream.Read(_layerMaskInfoBytes, 0, length);
            }

            public LayerMaskInfo()
            {

            }

            public byte[] GetBytes()
            {
                using MemoryStream _Memory = new MemoryStream();
                byte[] lengthBytes = BitConverter.GetBytes(_layerMaskInfoBytes.Length);
                Array.Reverse(lengthBytes);
                _Memory.Write(lengthBytes, 0, lengthBytes.Length);
                if (_layerMaskInfoBytes.Length != 0) _Memory.Write(_layerMaskInfoBytes, 0, _layerMaskInfoBytes.Length);
                return _Memory.ToArray();
            }
        }

        //PSD file part 5: Image data
        private class ImageData
        {
            private ushort _type = 0;

            private Head _head;

            public Bitmap PSDImage { get; set; }

            public ImageData(FileStream fs, Head head)
            {
                _head = head;
                PSDImage = new Bitmap(1, 1);
                byte[] typeBytes = new byte[2];
                fs.Read(typeBytes, 0, 2);
                Array.Reverse(typeBytes);
                _type = BitConverter.ToUInt16(typeBytes, 0);
                switch (_type)
                {
                    case 0: //RAW DATA
                        RawData(fs);
                        break;
                    case 1:
                        RleData(fs);
                        break;
                    default:
                        throw new Exception("Type =" + _type);
                }
            }

            #region RAW data
            private void RawData(FileStream fs)
            {
                switch (_head.ColorMode)
                {
                    case 2: //Index
                        LoadRAWIndex(fs);
                        return;
                    case 3:  //RGB   
                        LoadRAWRGB(fs);
                        return;
                    case 4: //CMYK
                        LoadRAWCMYK(fs);
                        return;
                    default:
                        throw new Exception("RAW ColorMode =" + _head.ColorMode);
                }
            }

            private void LoadRAWCMYK(FileStream fs)
            {
                int width = (int) _head.Width;
                int height = (int) _head.Height;
                PSDImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData pSDImageData = PSDImage.LockBits(new Rectangle(0, 0, PSDImage.Width, PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] writeBytes = new byte[pSDImageData.Stride * pSDImageData.Height];
                int perPixel = _head.BitsPerPixel / 8;
                int pixelsCount = width * height;
                int bytesCount = pixelsCount * 4 * perPixel;
                byte[] imageBytes = new byte[bytesCount];
                fs.Read(imageBytes, 0, bytesCount);

                int starIndex = 0;
                int index = 0;
                int size = width * height;
                double c;
                double m;
                double y;
                double k;
                double maxColours = Math.Pow(2, _head.BitsPerPixel);
                int size2 = size * 2;
                int size3 = size * 3;

                if (perPixel == 2)
                {
                    size *= 2;
                    size2 *= 2;
                    size3 *= 2;
                }
                for (int i = 0; i != pSDImageData.Height; i++)
                {
                    starIndex = pSDImageData.Stride * i;

                    index = i * width;
                    if (perPixel == 2) index *= 2;
                    for (int z = 0; z != pSDImageData.Width; z++)
                    {
                        switch (perPixel)
                        {
                            case 1:
                                c = 1.0 - (double) imageBytes[index + z] / maxColours;
                                m = 1.0 - (double) imageBytes[index + z + size] / maxColours;
                                y = 1.0 - (double) imageBytes[index + z + size2] / maxColours;
                                k = 1.0 - (double) imageBytes[index + z + size3] / maxColours;
                                ConvertCMYKToRGB(c, m, y, k, writeBytes, starIndex + z * 3);
                                break;
                            case 2:
                                c = 1.0 - (double) BitConverter.ToUInt16(imageBytes, index + z * 2) / maxColours;
                                m = 1.0 - (double) BitConverter.ToUInt16(imageBytes, index + z * 2 + size) / maxColours;
                                y = 1.0 - (double) BitConverter.ToUInt16(imageBytes, index + z * 2 + size2) / maxColours;
                                k = 1.0 - (double) BitConverter.ToUInt16(imageBytes, index + z * 2 + size3) / maxColours;
                                ConvertCMYKToRGB(c, m, y, k, writeBytes, starIndex + z * 3);
                                break;
                        }


                    }
                }
                Marshal.Copy(writeBytes, 0, pSDImageData.Scan0, writeBytes.Length);
                PSDImage.UnlockBits(pSDImageData);
            }

            private void LoadRAWIndex(FileStream fs)
            {
                int width = (int) _head.Width;
                int height = (int) _head.Height;
                PSDImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                BitmapData pSDImageData = PSDImage.LockBits(new Rectangle(0, 0, PSDImage.Width, PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                byte[] imageBytes = new byte[pSDImageData.Stride * pSDImageData.Height];

                int pixelsCount = width * height;
                byte[] data = new byte[pixelsCount];
                fs.Read(data, 0, pixelsCount);

                int readIndex = 0;
                int writeIndex = 0;
                for (int i = 0; i != height; i++)
                {
                    writeIndex = i * pSDImageData.Stride;
                    for (int z = 0; z != width; z++)
                    {
                        imageBytes[z + writeIndex] = data[readIndex];
                        readIndex++;
                    }
                }

                Marshal.Copy(imageBytes, 0, pSDImageData.Scan0, imageBytes.Length);
                PSDImage.UnlockBits(pSDImageData);
            }

            private void LoadRAWRGB(FileStream fs)
            {
                int _Width = (int) _head.Width;
                int _Height = (int) _head.Height;
                PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format24bppRgb);
                BitmapData pSDImageData = PSDImage.LockBits(new Rectangle(0, 0, PSDImage.Width, PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] imageBytes = new byte[pSDImageData.Stride * pSDImageData.Height];

                int pixelsCount = _Width * _Height;
                int bytesCount = pixelsCount * 3 * (_head.BitsPerPixel / 8);
                byte[] _Data = new byte[bytesCount];
                fs.Read(_Data, 0, bytesCount);

                int red = 0;
                int green = pixelsCount;
                int blue = pixelsCount + pixelsCount;
                int readIndex = 0;
                int writeIndex = 0;

                if (_head.BitsPerPixel == 16)
                {
                    green *= _head.BitsPerPixel / 8;
                    blue *= _head.BitsPerPixel / 8;
                }

                for (int i = 0; i != _Height; i++)
                {
                    writeIndex = i * pSDImageData.Stride;
                    for (int z = 0; z != _Width; z++)
                    {
                        imageBytes[(z * 3) + 2 + writeIndex] = _Data[readIndex + red];
                        imageBytes[(z * 3) + 1 + writeIndex] = _Data[readIndex + green];
                        imageBytes[(z * 3) + writeIndex] = _Data[readIndex + blue];
                        readIndex += _head.BitsPerPixel / 8;
                    }
                }
                Marshal.Copy(imageBytes, 0, pSDImageData.Scan0, imageBytes.Length);
                PSDImage.UnlockBits(pSDImageData);
            }
            #endregion

            #region RLE Data
            private void RleData(FileStream fs)
            {
                switch (_head.ColorMode)
                {
                    case 3:  //RGB
                        LoadRLERGB(fs);
                        break;
                    case 4:  //CMYK
                        LoadRLECMYK(fs);
                        break;
                    default:
                        throw new Exception("RLE ColorMode =" + _head.ColorMode);
                }
            }

            private void LoadRLERGB(FileStream fs)
            {
                int width = (int) _head.Width;
                int height = (int) _head.Height;
                PSDImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData pSDImageData = PSDImage.LockBits(new Rectangle(0, 0, PSDImage.Width, PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] imageBytes = new byte[pSDImageData.Stride * pSDImageData.Height];
                int writeIndex = 0;
                int endIndex = pSDImageData.Stride * pSDImageData.Height;
                fs.Position += height * _head.Channels * 2;

                int count = width * height;
                int wrtieType = 0;
                int heightIndex = 0;
                int widthIndex = 0;
                int index = 0;

                while (true)
                {
                    if (writeIndex > endIndex - 1) break;
                    byte read = (byte) fs.ReadByte();
                    if (read == 128) continue; //Erroe
                    if (read > 128)
                    {
                        read ^= 0x0FF;
                        read += 2;
                        byte _ByteValue = (byte) fs.ReadByte();

                        for (byte i = 0; i != read; i++)
                        {
                            wrtieType = writeIndex / count;
                            switch (wrtieType)
                            {
                                case 0: //Red
                                    heightIndex = writeIndex / width;
                                    widthIndex = writeIndex % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3) + 2;
                                    imageBytes[index] = _ByteValue;
                                    break;
                                case 1: //Green
                                    heightIndex = (writeIndex - count) / width;
                                    widthIndex = (writeIndex - count) % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3) + 1;
                                    imageBytes[index] = _ByteValue;
                                    break;
                                case 2:
                                    heightIndex = (writeIndex - count - count) / width;
                                    widthIndex = (writeIndex - count - count) % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3);
                                    imageBytes[index] = _ByteValue;
                                    break;
                            }
                            //_ImageBytes[_WriteIndex] = _ByteValue;
                            writeIndex++;
                        }
                    }
                    else
                    {
                        read++;
                        for (byte i = 0; i != read; i++)
                        {
                            wrtieType = writeIndex / count;
                            switch (wrtieType)
                            {
                                case 0: //Red
                                    heightIndex = writeIndex / width;
                                    widthIndex = writeIndex % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3) + 2;
                                    imageBytes[index] = (byte) fs.ReadByte();
                                    break;
                                case 1: //Green
                                    heightIndex = (writeIndex - count) / width;
                                    widthIndex = (writeIndex - count) % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3) + 1;
                                    imageBytes[index] = (byte) fs.ReadByte();
                                    break;
                                case 2:
                                    heightIndex = (writeIndex - count - count) / width;
                                    widthIndex = (writeIndex - count - count) % width;
                                    index = (pSDImageData.Stride * heightIndex) + (widthIndex * 3);
                                    imageBytes[index] = (byte) fs.ReadByte();
                                    break;
                            }
                            //_ImageBytes[_WriteIndex] = (byte)p_Stream.ReadByte();
                            writeIndex++;
                        }
                    }
                }
                Marshal.Copy(imageBytes, 0, pSDImageData.Scan0, imageBytes.Length);
                PSDImage.UnlockBits(pSDImageData);
            }

            private void LoadRLECMYK(FileStream fs)
            {

                int width = (int) _head.Width;
                int height = (int) _head.Height;

                int count = width * height * (_head.BitsPerPixel / 8) * _head.Channels;
                fs.Position += height * _head.Channels * 2;
                byte[] imageBytes = new byte[count];

                int writeIndex = 0;
                while (true)
                {
                    if (writeIndex > count - 1) break;
                    byte read = (byte) fs.ReadByte();
                    if (read == 128) continue; //Erroe
                    if (read > 128)
                    {
                        read ^= 0x0FF;
                        read += 2;
                        byte byteValue = (byte) fs.ReadByte();

                        for (byte i = 0; i != read; i++)
                        {
                            imageBytes[writeIndex] = byteValue;
                            writeIndex++;
                        }
                    }
                    else
                    {
                        read++;
                        for (byte i = 0; i != read; i++)
                        {
                            imageBytes[writeIndex] = (byte) fs.ReadByte();
                            writeIndex++;
                        }
                    }
                }

                PSDImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData pSDImageData = PSDImage.LockBits(new Rectangle(0, 0, PSDImage.Width, PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] writeBytes = new byte[pSDImageData.Stride * pSDImageData.Height];

                int starIndex = 0;
                int index = 0;
                int size = width * height;
                double c;
                double m;
                double y;
                double k;
                double maxColours = Math.Pow(2, _head.BitsPerPixel);
                int size2 = size * 2;
                int size3 = size * 3;
                for (int i = 0; i != pSDImageData.Height; i++)
                {
                    starIndex = pSDImageData.Stride * i;
                    index = i * width;
                    for (int z = 0; z != pSDImageData.Width; z++)
                    {
                        c = 1.0 - (double) imageBytes[index + z] / maxColours;
                        m = 1.0 - (double) imageBytes[index + z + size] / maxColours;
                        y = 1.0 - (double) imageBytes[index + z + size2] / maxColours;
                        k = 1.0 - (double) imageBytes[index + z + size3] / maxColours;
                        ConvertCMYKToRGB(c, m, y, k, writeBytes, starIndex + z * 3);
                    }
                }

                Marshal.Copy(writeBytes, 0, pSDImageData.Scan0, writeBytes.Length);
                PSDImage.UnlockBits(pSDImageData);
            }
            #endregion

            private void ConvertCMYKToRGB(double c, double m, double y, double k, byte[] dataBytes, int index)
            {
                int red = (int) ((1.0 - (c * (1 - k) + k)) * 255);
                int green = (int) ((1.0 - (m * (1 - k) + k)) * 255);
                int blue = (int) ((1.0 - (y * (1 - k) + k)) * 255);

                if (red < 0) red = 0;
                else if (red > 255) red = 255;
                if (green < 0) green = 0;
                else if (green > 255) green = 255;
                if (blue < 0) blue = 0;
                else if (blue > 255) blue = 255;

                dataBytes[index] = (byte) blue;
                dataBytes[index + 1] = (byte) green;
                dataBytes[index + 2] = (byte) red;
            }
        }

        private Head _head;
        private ColorMode _colorMode;
        private IList<BIM> _8BIMList = new List<BIM>();
        private LayerMaskInfo _layerMaskInfo;
        private ImageData _imageData;

        public PSDHelper(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception();
            }
            FileStream fs = File.Open(filePath, FileMode.Open);

            _head = new Head(fs);
            _colorMode = new ColorMode(fs);

            long readCount = fs.Position;
            while (true)
            {
                BIM _Bim = new BIM(fs);
                if (!_Bim._isRead || fs.Position - readCount >= _colorMode.BIMSize)
                    break;
                _8BIMList.Add(_Bim);
            }
            _layerMaskInfo = new LayerMaskInfo(fs);
            _imageData = new ImageData(fs, _head);
            if (_head.ColorMode == 2)
                _imageData.PSDImage.Palette = _colorMode.ColorData;
            fs.Close();
        }

        //Image
        public Bitmap PSDImage
        {
            get
            {
                return _imageData.PSDImage;
            }
            set
            {
                _imageData.PSDImage = value;
            }
        }
    }
}
