//Original author: zgke@sina.com qq:116149
//Update after 11 years by RC
//Temporarily remove anything about write a PSD file
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace LocalVersionControlSystem.Helper
{
    public class PSDHelper
    {
        //PSD file part 1: Head
        private class PSDHead
        {
            //Length of head is 26
            private byte[] _headBytes = new byte[26];

            //Get _headBytes
            public byte[] GetBytes()
            {
                return _headBytes;
            }

            //Version
            public byte _version
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
            public ushort _channels
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
            public uint _height
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
            public uint _width
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
            public ushort _bitsPerPixel
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
            public ushort _colorMode
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

            public PSDHead(byte[] p_Data)
            {
                _headBytes = p_Data;
            }
        }

        //PSD file part 2: ColorMode
        private class PSDColorMode
        {
            //The size of color data in bytes
            private byte[] _sizeBytes = new byte[4];
            //Only color data in bytes
            private byte[] _colorModeBytes;

            //The size of color data in uint
            public uint _bIMSize
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
            public ColorPalette _colorData
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

            public PSDColorMode(FileStream fs)
            {
                byte[] countByte = new byte[4];
                fs.Read(countByte, 0, 4);
                _sizeBytes = countByte;
                Array.Reverse(countByte);
                int count = BitConverter.ToInt32(countByte, 0);
                _colorModeBytes = new byte[count];
                if (count != 0)
                    fs.Read(_colorModeBytes, 0, count);
            }
        }

        //PSD file part 3: Image resources
        private class BIM
        {
            private byte[] _data = new byte[] { 0x38, 0x42, 0x49, 0x4D };
            private byte[] _typeID = new byte[2];
            private byte[] _name = Array.Empty<byte>();

            public ushort TypeID
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { _typeID[1], _typeID[0] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    _typeID[0] = _Value[1];
                    _typeID[1] = _Value[0];
                }
            }

            public byte[] m_Value;

            public BIM(FileStream p_FileStream)
            {
                byte[] _Type = new byte[4];
                p_FileStream.Read(_Type, 0, 4);
                if (_data[0] == _Type[0] && _data[1] == _Type[1] && _data[2] == _Type[2] && _data[3] == _Type[3])
                {
                    p_FileStream.Read(_typeID, 0, 2);
                    int _SizeOfName = p_FileStream.ReadByte();
                    int _nSizeOfName = (int) _SizeOfName;
                    if (_nSizeOfName > 0)
                    {
                        if ((_nSizeOfName % 2) != 0) { _SizeOfName = p_FileStream.ReadByte(); }
                        _name = new byte[_nSizeOfName];
                        p_FileStream.Read(_name, 0, _nSizeOfName);
                    }
                    _SizeOfName = p_FileStream.ReadByte();
                    byte[] _CountByte = new byte[4];
                    p_FileStream.Read(_CountByte, 0, 4);
                    Array.Reverse(_CountByte);
                    int _DataCount = BitConverter.ToInt32(_CountByte, 0);
                    if (_DataCount % 2 != 0) _DataCount++;
                    m_Value = new byte[_DataCount];
                    p_FileStream.Read(m_Value, 0, _DataCount);
                    m_Read = true;
                }
            }

            private bool m_Read = false;

            public bool Read
            {
                get { return m_Read; }
                set { m_Read = value; }
            }

            #region Type=1005
            public ushort hRes
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { m_Value[1], m_Value[0] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[0] = _Value[1];
                    m_Value[1] = _Value[0];
                }
            }
            public uint hResUnit
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { m_Value[5], m_Value[4], m_Value[3], m_Value[2] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[2] = _Value[3];
                    m_Value[3] = _Value[2];
                    m_Value[4] = _Value[1];
                    m_Value[5] = _Value[0];
                }
            }
            public ushort widthUnit
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { m_Value[7], m_Value[6] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[6] = _Value[1];
                    m_Value[7] = _Value[0];
                }
            }
            public ushort vRes
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { m_Value[9], m_Value[8] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[8] = _Value[1];
                    m_Value[9] = _Value[0];
                }
            }
            public uint vResUnit
            {
                get
                {
                    return BitConverter.ToUInt32(new byte[] { m_Value[13], m_Value[12], m_Value[11], m_Value[10] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[10] = _Value[3];
                    m_Value[11] = _Value[2];
                    m_Value[12] = _Value[1];
                    m_Value[13] = _Value[0];
                }
            }
            public ushort heightUnit
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[] { m_Value[15], m_Value[14] }, 0);
                }
                set
                {
                    byte[] _Value = BitConverter.GetBytes(value);
                    m_Value[14] = _Value[1];
                    m_Value[15] = _Value[0];
                }
            }
            #endregion

        }
        private class LayerMaskInfo
        {

            public byte[] m_Data = new byte[0];

            public LayerMaskInfo(FileStream p_Stream)
            {
                byte[] _Count = new byte[4];
                p_Stream.Read(_Count, 0, 4);
                Array.Reverse(_Count);

                int _ReadCount = BitConverter.ToInt32(_Count, 0);

                m_Data = new byte[_ReadCount];
                if (_ReadCount != 0) p_Stream.Read(m_Data, 0, _ReadCount);
            }

            public LayerMaskInfo()
            {
            }

            public byte[] GetBytes()
            {
                MemoryStream _Memory = new MemoryStream();
                byte[] _Value = BitConverter.GetBytes(m_Data.Length);
                Array.Reverse(_Value);
                _Memory.Write(_Value, 0, _Value.Length);
                if (m_Data.Length != 0) _Memory.Write(m_Data, 0, m_Data.Length);
                return _Memory.ToArray();
            }
        }
        private class ImageData
        {
            private ushort p_Type = 0;

            private PSDHead m_HeaderInfo;

            public ImageData()
            {
            }
            public ImageData(FileStream p_FileStream, PSDHead p_HeaderInfo)
            {
                m_HeaderInfo = p_HeaderInfo;
                byte[] _ShortBytes = new byte[2];
                p_FileStream.Read(_ShortBytes, 0, 2);
                Array.Reverse(_ShortBytes);
                p_Type = BitConverter.ToUInt16(_ShortBytes, 0);
                switch (p_Type)
                {
                    case 0: //RAW DATA
                        RawData(p_FileStream);
                        break;
                    case 1:
                        RleData(p_FileStream);
                        break;
                    default:
                        throw new Exception("Type =" + p_Type.ToString());
                }
            }

            #region RLE数据
            private void RleData(FileStream p_Stream)
            {
                switch (m_HeaderInfo._colorMode)
                {
                    case 3:  //RGB
                        LoadRLERGB(p_Stream);
                        break;
                    case 4:  //CMYK
                        LoadRLECMYK(p_Stream);
                        break;
                    default:
                        throw new Exception("RLE ColorMode =" + m_HeaderInfo._colorMode.ToString());
                }
            }

            private void LoadRLERGB(FileStream p_Stream)
            {
                int _Width = (int) m_HeaderInfo._width;
                int _Height = (int) m_HeaderInfo._height;
                m_PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format24bppRgb);
                BitmapData _PSDImageData = m_PSDImage.LockBits(new Rectangle(0, 0, m_PSDImage.Width, m_PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] _ImageBytes = new byte[_PSDImageData.Stride * _PSDImageData.Height];
                int _WriteIndex = 0;
                int _EndIndex = _PSDImageData.Stride * _PSDImageData.Height;
                p_Stream.Position += _Height * m_HeaderInfo._channels * 2;

                int _Count = _Width * _Height;
                int _WrtieType = 0;
                int _HeightIndex = 0;
                int _WidthIndex = 0;
                int _Index = 0;

                while (true)
                {
                    if (_WriteIndex > _EndIndex - 1) break;
                    byte _Read = (byte) p_Stream.ReadByte();
                    if (_Read == 128) continue; //Erroe
                    if (_Read > 128)
                    {
                        _Read ^= 0x0FF;
                        _Read += 2;
                        byte _ByteValue = (byte) p_Stream.ReadByte();

                        for (byte i = 0; i != _Read; i++)
                        {
                            _WrtieType = _WriteIndex / _Count;
                            switch (_WrtieType)
                            {
                                case 0: //Red
                                    _HeightIndex = _WriteIndex / _Width;
                                    _WidthIndex = _WriteIndex % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3) + 2;
                                    _ImageBytes[_Index] = _ByteValue;
                                    break;
                                case 1: //Green
                                    _HeightIndex = (_WriteIndex - _Count) / _Width;
                                    _WidthIndex = (_WriteIndex - _Count) % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3) + 1;
                                    _ImageBytes[_Index] = _ByteValue;
                                    break;
                                case 2:
                                    _HeightIndex = (_WriteIndex - _Count - _Count) / _Width;
                                    _WidthIndex = (_WriteIndex - _Count - _Count) % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3);
                                    _ImageBytes[_Index] = _ByteValue;
                                    break;
                            }
                            //_ImageBytes[_WriteIndex] = _ByteValue;
                            _WriteIndex++;
                        }
                    }
                    else
                    {
                        _Read++;
                        for (byte i = 0; i != _Read; i++)
                        {
                            _WrtieType = _WriteIndex / _Count;
                            switch (_WrtieType)
                            {
                                case 0: //Red
                                    _HeightIndex = _WriteIndex / _Width;
                                    _WidthIndex = _WriteIndex % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3) + 2;
                                    _ImageBytes[_Index] = (byte) p_Stream.ReadByte();
                                    break;
                                case 1: //Green
                                    _HeightIndex = (_WriteIndex - _Count) / _Width;
                                    _WidthIndex = (_WriteIndex - _Count) % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3) + 1;
                                    _ImageBytes[_Index] = (byte) p_Stream.ReadByte();
                                    break;
                                case 2:
                                    _HeightIndex = (_WriteIndex - _Count - _Count) / _Width;
                                    _WidthIndex = (_WriteIndex - _Count - _Count) % _Width;
                                    _Index = (_PSDImageData.Stride * _HeightIndex) + (_WidthIndex * 3);
                                    _ImageBytes[_Index] = (byte) p_Stream.ReadByte();
                                    break;
                            }
                            //_ImageBytes[_WriteIndex] = (byte)p_Stream.ReadByte();
                            _WriteIndex++;
                        }
                    }
                }
                Marshal.Copy(_ImageBytes, 0, _PSDImageData.Scan0, _ImageBytes.Length);
                m_PSDImage.UnlockBits(_PSDImageData);
            }

            private void LoadRLECMYK(FileStream p_Stream)
            {

                int _Width = (int) m_HeaderInfo._width;
                int _Height = (int) m_HeaderInfo._height;

                int _Count = _Width * _Height * (m_HeaderInfo._bitsPerPixel / 8) * m_HeaderInfo._channels;
                p_Stream.Position += _Height * m_HeaderInfo._channels * 2;
                byte[] _ImageBytes = new byte[_Count];

                int _WriteIndex = 0;
                while (true)
                {
                    if (_WriteIndex > _Count - 1) break;
                    byte _Read = (byte) p_Stream.ReadByte();
                    if (_Read == 128) continue; //Erroe
                    if (_Read > 128)
                    {
                        _Read ^= 0x0FF;
                        _Read += 2;
                        byte _ByteValue = (byte) p_Stream.ReadByte();

                        for (byte i = 0; i != _Read; i++)
                        {
                            _ImageBytes[_WriteIndex] = _ByteValue;
                            _WriteIndex++;
                        }
                    }
                    else
                    {
                        _Read++;
                        for (byte i = 0; i != _Read; i++)
                        {
                            _ImageBytes[_WriteIndex] = (byte) p_Stream.ReadByte();
                            _WriteIndex++;
                        }
                    }
                }

                m_PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format24bppRgb);
                BitmapData _PSDImageData = m_PSDImage.LockBits(new Rectangle(0, 0, m_PSDImage.Width, m_PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] _WriteBytes = new byte[_PSDImageData.Stride * _PSDImageData.Height];

                int _StarIndex = 0;
                int _Index = 0;
                int _Size = _Width * _Height;
                double C;
                double M;
                double Y;
                double K;
                double _MaxColours = Math.Pow(2, m_HeaderInfo._bitsPerPixel);
                int _Size2 = _Size * 2;
                int _Size3 = _Size * 3;
                for (int i = 0; i != _PSDImageData.Height; i++)
                {
                    _StarIndex = _PSDImageData.Stride * i;
                    _Index = i * _Width;
                    for (int z = 0; z != _PSDImageData.Width; z++)
                    {
                        C = 1.0 - (double) _ImageBytes[_Index + z] / _MaxColours;
                        M = 1.0 - (double) _ImageBytes[_Index + z + _Size] / _MaxColours;
                        Y = 1.0 - (double) _ImageBytes[_Index + z + _Size2] / _MaxColours;
                        K = 1.0 - (double) _ImageBytes[_Index + z + _Size3] / _MaxColours;
                        ConvertCMYKToRGB(C, M, Y, K, _WriteBytes, _StarIndex + z * 3);
                    }
                }

                Marshal.Copy(_WriteBytes, 0, _PSDImageData.Scan0, _WriteBytes.Length);
                m_PSDImage.UnlockBits(_PSDImageData);
            }
            #endregion

            #region RAW数据
            private void RawData(FileStream p_Stream)
            {
                switch (m_HeaderInfo._colorMode)
                {
                    case 2: //Index
                        LoadRAWIndex(p_Stream);
                        return;
                    case 3:  //RGB   
                        LoadRAWRGB(p_Stream);
                        return;
                    case 4: //CMYK
                        LoadRAWCMYK(p_Stream);
                        return;
                    default:
                        throw new Exception("RAW ColorMode =" + m_HeaderInfo._colorMode.ToString());
                }

            }

            private void LoadRAWCMYK(FileStream p_Stream)
            {
                int _Width = (int) m_HeaderInfo._width;
                int _Height = (int) m_HeaderInfo._height;
                m_PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format24bppRgb);
                BitmapData _PSDImageData = m_PSDImage.LockBits(new Rectangle(0, 0, m_PSDImage.Width, m_PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] _WriteBytes = new byte[_PSDImageData.Stride * _PSDImageData.Height];
                int _PerPixel = m_HeaderInfo._bitsPerPixel / 8;
                int _PixelsCount = _Width * _Height;
                int _BytesCount = _PixelsCount * 4 * _PerPixel;
                byte[] _ImageBytes = new byte[_BytesCount];
                p_Stream.Read(_ImageBytes, 0, _BytesCount);

                int _StarIndex = 0;
                int _Index = 0;
                int _Size = _Width * _Height;
                double C;
                double M;
                double Y;
                double K;
                double _MaxColours = Math.Pow(2, m_HeaderInfo._bitsPerPixel);
                int _Size2 = _Size * 2;
                int _Size3 = _Size * 3;

                if (_PerPixel == 2)
                {
                    _Size *= 2;
                    _Size2 *= 2;
                    _Size3 *= 2;
                }
                for (int i = 0; i != _PSDImageData.Height; i++)
                {
                    _StarIndex = _PSDImageData.Stride * i;

                    _Index = i * _Width;
                    if (_PerPixel == 2) _Index *= 2;
                    for (int z = 0; z != _PSDImageData.Width; z++)
                    {
                        switch (_PerPixel)
                        {
                            case 1:
                                C = 1.0 - (double) _ImageBytes[_Index + z] / _MaxColours;
                                M = 1.0 - (double) _ImageBytes[_Index + z + _Size] / _MaxColours;
                                Y = 1.0 - (double) _ImageBytes[_Index + z + _Size2] / _MaxColours;
                                K = 1.0 - (double) _ImageBytes[_Index + z + _Size3] / _MaxColours;
                                ConvertCMYKToRGB(C, M, Y, K, _WriteBytes, _StarIndex + z * 3);
                                break;
                            case 2:
                                C = 1.0 - (double) BitConverter.ToUInt16(_ImageBytes, _Index + z * 2) / _MaxColours;
                                M = 1.0 - (double) BitConverter.ToUInt16(_ImageBytes, _Index + z * 2 + _Size) / _MaxColours;
                                Y = 1.0 - (double) BitConverter.ToUInt16(_ImageBytes, _Index + z * 2 + _Size2) / _MaxColours;
                                K = 1.0 - (double) BitConverter.ToUInt16(_ImageBytes, _Index + z * 2 + _Size3) / _MaxColours;
                                ConvertCMYKToRGB(C, M, Y, K, _WriteBytes, _StarIndex + z * 3);
                                break;
                        }


                    }
                }
                Marshal.Copy(_WriteBytes, 0, _PSDImageData.Scan0, _WriteBytes.Length);
                m_PSDImage.UnlockBits(_PSDImageData);
            }

            /// <summary>
            /// 直接获取RGB 256色图
            /// </summary>
            /// <param name="p_Stream"></param>
            private void LoadRAWIndex(FileStream p_Stream)
            {
                int _Width = (int) m_HeaderInfo._width;
                int _Height = (int) m_HeaderInfo._height;
                m_PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format8bppIndexed);
                BitmapData _PSDImageData = m_PSDImage.LockBits(new Rectangle(0, 0, m_PSDImage.Width, m_PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                byte[] _ImageBytes = new byte[_PSDImageData.Stride * _PSDImageData.Height];

                int _PixelsCount = _Width * _Height;
                byte[] _Data = new byte[_PixelsCount];
                p_Stream.Read(_Data, 0, _PixelsCount);

                int _ReadIndex = 0;
                int _WriteIndex = 0;
                for (int i = 0; i != _Height; i++)
                {
                    _WriteIndex = i * _PSDImageData.Stride;
                    for (int z = 0; z != _Width; z++)
                    {
                        _ImageBytes[z + _WriteIndex] = _Data[_ReadIndex];
                        _ReadIndex++;
                    }
                }

                Marshal.Copy(_ImageBytes, 0, _PSDImageData.Scan0, _ImageBytes.Length);
                m_PSDImage.UnlockBits(_PSDImageData);
            }

            /// <summary>
            /// 获取图形24B   Photo里对应为
            /// </summary>
            /// <param name="p_Stream"></param>
            private void LoadRAWRGB(FileStream p_Stream)
            {
                int _Width = (int) m_HeaderInfo._width;
                int _Height = (int) m_HeaderInfo._height;
                m_PSDImage = new Bitmap(_Width, _Height, PixelFormat.Format24bppRgb);
                BitmapData _PSDImageData = m_PSDImage.LockBits(new Rectangle(0, 0, m_PSDImage.Width, m_PSDImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] _ImageBytes = new byte[_PSDImageData.Stride * _PSDImageData.Height];

                int _PixelsCount = _Width * _Height;
                int _BytesCount = _PixelsCount * 3 * (m_HeaderInfo._bitsPerPixel / 8);
                byte[] _Data = new byte[_BytesCount];
                p_Stream.Read(_Data, 0, _BytesCount);

                int _Red = 0;
                int _Green = _PixelsCount;
                int _Blue = _PixelsCount + _PixelsCount;
                int _ReadIndex = 0;
                int _WriteIndex = 0;

                if (m_HeaderInfo._bitsPerPixel == 16)
                {
                    _Green *= m_HeaderInfo._bitsPerPixel / 8;
                    _Blue *= m_HeaderInfo._bitsPerPixel / 8;
                }

                for (int i = 0; i != _Height; i++)
                {
                    _WriteIndex = i * _PSDImageData.Stride;
                    for (int z = 0; z != _Width; z++)
                    {
                        _ImageBytes[(z * 3) + 2 + _WriteIndex] = _Data[_ReadIndex + _Red];
                        _ImageBytes[(z * 3) + 1 + _WriteIndex] = _Data[_ReadIndex + _Green];
                        _ImageBytes[(z * 3) + _WriteIndex] = _Data[_ReadIndex + _Blue];
                        _ReadIndex += m_HeaderInfo._bitsPerPixel / 8;
                    }
                }
                Marshal.Copy(_ImageBytes, 0, _PSDImageData.Scan0, _ImageBytes.Length);
                m_PSDImage.UnlockBits(_PSDImageData);
            }
            #endregion

            private Bitmap m_PSDImage;

            public Bitmap PSDImage
            {
                get { return m_PSDImage; }
                set { m_PSDImage = value; }
            }


            private void ConvertCMYKToRGB(double p_C, double p_M, double p_Y, double p_K, byte[] p_DataBytes, int p_Index)
            {
                int _Red = (int) ((1.0 - (p_C * (1 - p_K) + p_K)) * 255);
                int _Green = (int) ((1.0 - (p_M * (1 - p_K) + p_K)) * 255);
                int _Blue = (int) ((1.0 - (p_Y * (1 - p_K) + p_K)) * 255);

                if (_Red < 0) _Red = 0;
                else if (_Red > 255) _Red = 255;
                if (_Green < 0) _Green = 0;
                else if (_Green > 255) _Green = 255;
                if (_Blue < 0) _Blue = 0;
                else if (_Blue > 255) _Blue = 255;

                p_DataBytes[p_Index] = (byte) _Blue;
                p_DataBytes[p_Index + 1] = (byte) _Green;
                p_DataBytes[p_Index + 2] = (byte) _Red;
            }
        }

        private PSDHead m_Head;
        private PSDColorMode m_ColorModel;
        private IList<BIM> m_8BIMList = new List<BIM>();
        private LayerMaskInfo m_LayerMaskInfo;
        private ImageData m_ImageData;

        public PSDHelper(string p_FileFullPath)
        {
            if (!File.Exists(p_FileFullPath)) return;
            FileStream _PSD = File.Open(p_FileFullPath, FileMode.Open);
            byte[] _HeadByte = new byte[26];
            _PSD.Read(_HeadByte, 0, 26);
            m_Head = new PSDHead(_HeadByte);
            m_ColorModel = new PSDColorMode(_PSD);

            long _ReadCount = _PSD.Position;
            while (true)
            {
                BIM _Bim = new BIM(_PSD);
                if (!_Bim.Read || _PSD.Position - _ReadCount >= m_ColorModel._bIMSize) break;
                m_8BIMList.Add(_Bim);
            }
            m_LayerMaskInfo = new LayerMaskInfo(_PSD);
            m_ImageData = new ImageData(_PSD, m_Head);
            if (m_Head._colorMode == 2) m_ImageData.PSDImage.Palette = m_ColorModel._colorData;
            _PSD.Close();
        }

        //Image
        public Bitmap PSDImage
        {
            get { return m_ImageData.PSDImage; }
            set { m_ImageData.PSDImage = value; }
        }
    }
}
