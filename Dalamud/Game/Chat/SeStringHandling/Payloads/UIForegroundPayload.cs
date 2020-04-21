using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dalamud.Game.Chat.SeStringHandling.Payloads
{
    public class UIForegroundPayload : Payload
    {
        public override PayloadType Type => PayloadType.UIForeground;

        private UIColor color;
        public UIColor UIColor
        {
            get
            {
                this.color ??= this.dataResolver.GetExcelSheet<UIColor>().GetRow(this.colorKey);
                return this.color;
            }
        }

        public uint RGB
        {
            get
            {
                return UIColor.UIForeground;
            }
        }

        private ushort colorKey;

        public override byte[] Encode()
        {
            var colorBytes = MakeInteger(this.colorKey);
            var chunkLen = colorBytes.Length + 1;

            var bytes = new List<byte>(new byte[]
            {
                START_BYTE, (byte)SeStringChunkType.UIForeground, (byte)chunkLen
            });

            bytes.AddRange(colorBytes);
            bytes.Add(END_BYTE);

            return bytes.ToArray();
        }

        public override string ToString()
        {
            return $"{Type} - UIColor: {colorKey}";
        }

        protected override void ProcessChunkImpl(BinaryReader reader, long endOfStream)
        {
            this.colorKey = (ushort)GetInteger(reader);
        }

        protected override byte GetMarkerForIntegerBytes(byte[] bytes)
        {
            return bytes.Length switch
            {
                // a single byte of 0x01 is used to 'disable' color, and has no marker
                1 => (byte)IntegerType.None,
                _ => base.GetMarkerForIntegerBytes(bytes)
            };
        }
    }
}
