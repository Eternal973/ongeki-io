﻿using System;
using System.Runtime.InteropServices;

namespace MU3Input
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public struct OutputData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U1)]
        public byte[] Buttons;

        public short Lever;
        [MarshalAs(UnmanagedType.U1)] public byte Scan;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] AimiId;
        public OptButtons OptButton;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public unsafe struct SetLedInput
    {
        public byte Type;
        public byte LedBrightness;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public fixed byte LedColors[3 * 10];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public unsafe struct SetOptionInput
    {
        public byte Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public fixed byte AimiId[10];
    }
    public abstract class IO
    {
        public abstract OutputData Data { get; }

        private byte[] leftButtonsCache = new byte[5];
        private byte[] rightButtonsCache = new byte[5];
        public byte LeftButton
        {
            get
            {
                byte result = 0;
                for (int i = 4; i >= 0; i--)
                {
                    result <<= 1;
                    // 按钮触点数量不为0时
                    if (Data.Buttons[i] > 0)
                    {
                        // 当已被按下并增加触点数量时自动松开一帧
                        if (leftButtonsCache[i] > 0 && Data.Buttons[i] > leftButtonsCache[i])
                        {
                            result += 0;
                        }
                        else
                        {
                            result += 1;
                        }
                    }
                }
                Array.Copy(Data.Buttons, 0, leftButtonsCache, 0, 5);
                return result;
            }
        }

        public byte RightButton
        {
            get
            {
                byte result = 0;
                for (int i = 4; i >= 0; i--)
                {
                    result <<= 1;
                    if (Data.Buttons[i + 5] > 0)
                    {
                        if (rightButtonsCache[i] > 0 && Data.Buttons[i + 5] > rightButtonsCache[i])
                        {
                            result += 0;
                        }
                        else
                        {
                            result += 1;
                        }
                    }
                }
                Array.Copy(Data.Buttons, 5, rightButtonsCache, 0, 5);
                return result;
            }
        }

        public short Lever
        {
            get
            {
                return Data.Lever;
            }
        }

        public byte Scan => Data.Scan;

        public byte[] AimiId => Data.AimiId;
        public OptButtons OptButtonsStatus => Data.OptButton;

        public abstract bool IsConnected { get; }
        public abstract void Reconnect();
        public abstract void SetLed(uint data);
    }
    [Flags]
    public enum OptButtons : byte
    {
        Test = 0b01,
        Service = 0b10
    }
}
