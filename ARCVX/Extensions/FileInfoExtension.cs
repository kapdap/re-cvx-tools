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

using System.IO;

namespace ARCVX.Extensions
{
    public static class FileInfoExtension
    {
        public static FileStream OpenReadShared(this FileInfo file) =>
            file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
}
