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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
	/// <summary>
	/// 查询当前支持的扩展命令
	/// </summary>
	internal class FtpFeatCommand : FtpCommand
	{
		public FtpFeatCommand() : base("FEAT")
		{
		}

		protected override object OnExecute(FtpCommandContext context)
		{
			const string MESSAGE = @"211-Features:
 MDTM
 SIZE
 PASV
 UTF8
 HELP
 MFMT
 MLST size*;type*;perm*;create*;modify*;
 MLSD
 REST
 OPTS
 NOOP
211 End";
			context.Channel.Send(MESSAGE);

			return MESSAGE;
		}
	}
}