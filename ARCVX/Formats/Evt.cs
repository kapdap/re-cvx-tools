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
    public class Evt : Base<EvtHeader>
    {
        public override int MAGIC { get; } = 0;
        public override int MAGIC_LE { get; } = 0;

        public Evt(FileInfo file) : base(file) { }
        public Evt(FileInfo file, Stream stream) : base(file, stream) { }

        public override int GetMagic() =>
            0;

        public override EvtHeader GetHeader() =>
            new();

        public FileInfo Export() =>
            throw new NotImplementedException("Export has not been implemented");
    }

    public struct EvtHeader { }
}