﻿// SPDX-FileCopyrightText: 2024 Kuriimu2 contributers
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
    public abstract class BaseHash<T> : IHash
    {
        /// <inheritdoc cref="Compute(string)"/>
        public byte[] Compute(string input) => Compute(input, Encoding.ASCII);

        /// <inheritdoc cref="Compute(string,Encoding)"/>
        public byte[] Compute(string input, Encoding enc) => Compute(enc.GetBytes(input));

        /// <inheritdoc cref="Compute(Span{byte})"/>
        public byte[] Compute(Span<byte> input) => ConvertResult(ComputeValue(input));

        /// <inheritdoc cref="Compute(Stream)"/>
        public byte[] Compute(Stream input) => ConvertResult(ComputeValue(input));

        /// <summary>
        /// Computes a hash from a string encoded in ASCII.
        /// </summary>
        /// <param name="input">The string to compute the hash to.</param>
        /// <returns>The computed hash.</returns>
        public T ComputeValue(string input) => ComputeValue(input, Encoding.ASCII);

        /// <summary>
        /// Computes a hash from a string.
        /// </summary>
        /// <param name="input">The string to compute the hash to.</param>
        /// <param name="enc">The encoding the string should be encoded in.</param>
        /// <returns>The computed hash.</returns>
        public T ComputeValue(string input, Encoding enc) => ComputeValue(enc.GetBytes(input));

        /// <summary>
        /// Computes a hash over a stream of data.
        /// </summary>
        /// <param name="input">The stream of data to hash.</param>
        /// <returns>The computed hash.</returns>
        public T ComputeValue(Span<byte> input)
        {
            T result = CreateInitialValue();
            ComputeInternal(input, ref result);

            FinalizeResult(ref result);
            return result;
        }

        /// <summary>
        /// Computes a hash over an array of bytes.
        /// </summary>
        /// <param name="input">The array of bytes to hash.</param>
        /// <returns>The computed hash.</returns>
        public T ComputeValue(Stream input)
        {
            T result = CreateInitialValue();

            byte[] buffer = new byte[4096];
            int readSize;
            do
            {
                readSize = input.Read(buffer, 0, 4096);
                ComputeInternal(buffer, ref result);
            } while (readSize > 0);

            FinalizeResult(ref result);

            return result;
        }

        /// <summary>
        /// Creates the start value of the hash computation of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The initial value for the hash computation.</returns>
        protected abstract T CreateInitialValue();

        /// <summary>
        /// Applies operations on the computed hash after all input data was consumed.
        /// </summary>
        /// <param name="result">The computed hash after all data was consumed.</param>
        protected abstract void FinalizeResult(ref T result);

        /// <summary>
        /// Computes the hash on a given span of data. This method may be called multiple times and may be handled as an accumulative operation.
        /// </summary>
        /// <param name="input">The data to consume.</param>
        /// <param name="result">The value to hold the computed hash.</param>
        protected abstract void ComputeInternal(Span<byte> input, ref T result);

        /// <summary>
        /// Converts the value of type <typeparamref name="T"/> to a byte array.
        /// </summary>
        /// <param name="result">The finalized computed hash.</param>
        /// <returns>The byte array representing the finalized computed hash.</returns>
        protected abstract byte[] ConvertResult(T result);
    }
}
