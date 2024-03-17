// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  ARCVX
 *  
 *  Copyright 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using System;
using System.IO;

namespace ARCVX.Formats
{
    public class Mes : Base<MesHeader>
    {
        public override int MAGIC { get; } = 0;
        public override int MAGIC_LE { get; } = 0;

        public Mes(FileInfo file) : base(file) { }
        public Mes(FileInfo file, Stream stream) : base(file, stream) { }

        public override int GetMagic() =>
            0;

        public override MesHeader GetHeader() =>
            new();

        public FileInfo Export() =>
            throw new NotImplementedException("Export has not been implemented");
    }

    public struct MesHeader { }
}