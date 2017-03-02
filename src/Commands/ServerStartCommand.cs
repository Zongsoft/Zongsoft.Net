/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
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
using System.ComponentModel;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Communication.Commands
{
	[DisplayName("${Text.Communication.ServerStartCommand.Title}")]
	[Description("${Text.Communication.ServerStartCommand.Description}")]
	public class ServerStartCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public ServerStartCommand() : base("Start")
		{
		}

		public ServerStartCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var server = ServerCommandBase.GetServer(context.CommandNode);

			if(server == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Server"));

			if(server.IsListening)
			{
				context.Output.WriteLine(ResourceUtility.GetString("Text.ServerHasBeenStarted"));
				return false;
			}

			server.Start();

			if(server.IsListening)
				context.Output.WriteLine(CommandOutletColor.Green, ResourceUtility.GetString("Text.CommandExecuteSucceed"));
			else
				context.Output.WriteLine(CommandOutletColor.Red, ResourceUtility.GetString("Text.CommandExecuteFailed"));

			return server.IsListening;
		}
		#endregion
	}
}
