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

using ARCVX.Formats;
using ARCVX.Reader;
using System.CommandLine.Binding;
using System.CommandLine;
using System.IO;

namespace ARCVX
{
    public class Config
    {
        public string Path { get; set; }
        public DirectoryInfo Folder { get; set; } = null;
        public bool Overwrite { get; set; } = false;
        public Region Language { get; set; } = Region.US;
        public FileInfo LanguageFile { get; set; } = null;
        public ByteOrder ByteOrder { get; set; } = ByteOrder.BigEndian;
    }

    public class ConfigBinder : BinderBase<Config>
    {
        private readonly Option<string> _pathOption;
        private readonly Option<DirectoryInfo> _folderOption;
        private readonly Option<bool> _overwriteOption;
        private readonly Option<Region> _languageOption;
        private readonly Option<FileInfo> _languageFileOption;
        private readonly Option<ByteOrder> _byteOrderOption;

        public ConfigBinder(
            Option<string> pathOption,
            Option<DirectoryInfo> folderOption,
            Option<bool> overwriteOption,
            Option<Region> languageOption,
            Option<FileInfo> languageFileOption,
            Option<ByteOrder> byteOrderOption)
        {
            _pathOption = pathOption;
            _folderOption = folderOption;
            _overwriteOption = overwriteOption;
            _languageOption = languageOption;
            _languageFileOption = languageFileOption;
            _byteOrderOption = byteOrderOption;
        }

        protected override Config GetBoundValue(BindingContext bindingContext) =>
            new Config
            {
                Path = bindingContext.ParseResult.GetValueForOption(_pathOption),
                Folder = bindingContext.ParseResult.GetValueForOption(_folderOption),
                Overwrite = bindingContext.ParseResult.GetValueForOption(_overwriteOption),
                Language = bindingContext.ParseResult.GetValueForOption(_languageOption),
                LanguageFile = bindingContext.ParseResult.GetValueForOption(_languageFileOption),
                ByteOrder = bindingContext.ParseResult.GetValueForOption(_byteOrderOption)
            };
    }
}
