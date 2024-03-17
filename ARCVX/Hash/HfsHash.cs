/*   ARCVX
 * 
 *   Copyright 2024 Kuriimu2 contributers (https://github.com/FanTranslatorsInternational/Kuriimu2)
 *   Copyright 2024 Kapdap <kapdap@pm.me> (https://github.com/kapdap)
 * 
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;

namespace ARCVX.Hash
{
    class HfsHash : BaseHash<byte[]>
    {
        private static readonly byte[] InitValues = { 0x87, 0x55, 0x07, 0xB5, 0x4B, 0x04, 0xA5, 0xAE, 0xC7, 0x67, 0xBE, 0xCB, 0x01, 0x50, 0x58, 0x44 };
        private static readonly int[] RotValues = { 1, 6, 3, 4, 2, 5, 7, 4, 6, 2, 1, 5, 3, 1, 7, 3 };

        protected override byte[] CreateInitialValue()
        {
            var buffer = new byte[16];
            Array.Copy(InitValues, buffer, 16);

            return buffer;
        }

        protected override void FinalizeResult(ref byte[] result)
        {
            for (var i = 0; i < 16; i++)
                result[i] = Rot(result[i], RotValues[i]);
        }

        protected override void ComputeInternal(Span<byte> input, ref byte[] result)
        {
            for (var i = 0; i < input.Length; i++)
                result[i % 16] += input[i];
        }

        protected override byte[] ConvertResult(byte[] result)
        {
            return result;
        }

        private static byte Rot(byte value, int rot) => (byte)((value >> rot) | (value << (8 - rot)));
    }
}
