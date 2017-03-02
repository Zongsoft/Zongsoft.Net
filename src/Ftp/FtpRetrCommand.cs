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
    /// <summary>
    /// 从服务器上复制（下载）文件
    /// </summary>
    internal class FtpRetrCommand : FtpCommand
    {
        public FtpRetrCommand() : base("RETR")
        {
        }

		protected override object OnExecute(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            if (string.IsNullOrEmpty(context.Statement.Argument))
                throw new SyntaxException();

            context.Channel.CheckDataChannel();

			try
			{
				//context.Channel.Status = FtpSessionStatus.Download;

				var path = context.Statement.Argument;
				string localPath = context.Channel.MapVirtualPathToLocalPath(path);
				context.Statement.Result = localPath;

				var fileInfo = new FileInfo(localPath);

				if(!fileInfo.Exists)
					throw new FileNotFoundException(path);

				var message = "150 Open data connection for file transfer.";

				if(context.Channel.DataChannel.SendFile(fileInfo, context.Channel.FileOffset))
					message = "226 Transfer complete.";
				else
					message = "426 Connection closed; transfer aborted.";

				context.Channel.Send(message);
				context.Channel.FileOffset = 0;

				return message;
			}
			catch(FtpException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new InternalException(e.Message);
			}
			finally
			{
				context.Channel.CloseDataChannel();
				//context.Channel.Status = FtpSessionStatus.Wait;
			}
        }
    }
}