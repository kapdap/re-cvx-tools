// SPDX-FileCopyrightText: 2024 Kuriimu2 contributers
//
// SPDX-License-Identifier: GPL-3.0-or-later
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
using System.IO;
using System.Text;

namespace ARCVX.Hash
{
    /// <summary>
    /// Exposes methods to compute a hash over given input data.
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// Computes a hash from a string encoded in ASCII.
        /// </summary>
        /// <param name="input">The string to compute the hash to.</param>
        /// <returns>The computed hash.</returns>
        byte[] Compute(string input);

        /// <summary>
        /// Computes a hash from a string.
        /// </summary>
        /// <param name="input">The string to compute the hash to.</param>
        /// <param name="enc">The encoding the string should be encoded in.</param>
        /// <returns>The computed hash.</returns>
        byte[] Compute(string input, Encoding enc);

        /// <summary>
        /// Computes a hash over a stream of data.
        /// </summary>
        /// <param name="input">The stream of data to hash.</param>
        /// <returns>The computed hash.</returns>
        byte[] Compute(Stream input);

        /// <summary>
        /// Computes a hash over an array of bytes.
        /// </summary>
        /// <param name="input">The array of bytes to hash.</param>
        /// <returns>The computed hash.</returns>
        byte[] Compute(Span<byte> input);
    }
}
