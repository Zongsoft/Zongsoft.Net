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
using System.Net;
using System.ComponentModel;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Communication.Commands
{
	[DisplayName("${Text.Communication.ClientStatusCommand.Title}")]
	[Description("${Text.Communication.ClientStatusCommand.Description}")]
	public class TcpClientStatusCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public TcpClientStatusCommand() : base("Status")
		{
		}

		public TcpClientStatusCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var client = TcpClientCommand.GetClient(context.CommandNode);

			if(client == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Client"));

			bool isConnected = client.IsConnected(1000);

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.IsConnected}") + ": ");
			context.Output.WriteLine(isConnected ? ResourceUtility.GetString("${Text.Yes}") : ResourceUtility.GetString("${Text.No}"));

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.RemoteEndPoint}") + ": ");
			context.Output.WriteLine(client.RemoteAddress);

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.LocalEndPoint}") + ": ");
			context.Output.WriteLine(client.LocalAddress);

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.LastConnectTime}") + ": ");
			context.Output.WriteLine(client.Channel.LastConnectTime);

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.LastSendTime}") + ": ");
			context.Output.WriteLine(client.Channel.LastSendTime);

			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.LastReceivedTime}") + ": ");
			context.Output.WriteLine(client.Channel.LastReceivedTime);

			return new
			{
				IsConnected = isConnected,
				RemoteAddress = client.RemoteAddress,
				LocalAddress = client.LocalAddress,
				LastConnectTime = client.Channel.LastConnectTime,
				LastSendTime = client.Channel.LastSendTime,
				LastReceivedTime = client.Channel.LastReceivedTime,
			};
		}
		#endregion
	}
}
