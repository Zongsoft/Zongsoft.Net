﻿/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Net.
 *
 * Zongsoft.Net is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Net; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    internal static class FtpMlstFileFormater
    {
        private const string NEWLINE = "\r\n";
        private const char DELIM = ' ';

        public static string Format(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var output = new StringBuilder();
            Format(context, fileInfo, output);
            return output.ToString();
        }

        public static void Format(FtpCommandContext context, FileSystemInfo fileInfo, StringBuilder output)
        {
            var isFile = fileInfo is FileInfo;

            //Size
            output.AppendFormat("size={0};", isFile ? ((FileInfo)fileInfo).Length : 0);

            //Permission
            output.AppendFormat("perm={0}{1};",
                                /* Can read */ isFile ? "r" : "el",
                                /* Can write */ isFile ? "adfw" : "fpcm");
            
            //Type
            output.AppendFormat("type={0};", isFile ? "file" : "dir");

            //Create
            output.AppendFormat("create={0};", FtpDateUtils.FormatFtpDate(fileInfo.CreationTimeUtc));

            //Modify
            output.AppendFormat("modify={0};", FtpDateUtils.FormatFtpDate(fileInfo.LastWriteTimeUtc));

            //File name
            output.Append(DELIM);
            output.Append(fileInfo.Name);

            output.Append(NEWLINE);
        }
    }
}
