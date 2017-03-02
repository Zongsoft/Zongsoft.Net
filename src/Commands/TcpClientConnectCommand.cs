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

using Zongsoft.Services;
using Zongsoft.Resources;
using Zongsoft.Communication.Net;

namespace Zongsoft.Communication.Commands
{
	[CommandOption("async", Description = "${Text.TcpClientConnectCommand.Options.Async}")]
	public class TcpClientConnectCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public TcpClientConnectCommand() : base("Connect")
		{
		}

		public TcpClientConnectCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var client = TcpClientCommand.GetClient(context.CommandNode);

			if(client == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Client"));

			if(context.Expression.Options.Contains("async"))
			{
				client.Connected += Client_Connected;
				client.Failed += Client_Failed;

				client.ConnectAsync(context);
			}
			else
			{
				if(client.Connect())
					context.Output.WriteLine(CommandOutletColor.Green, ResourceUtility.GetString("Text.CommandExecuteSucceed"));
				else
					context.Output.WriteLine(CommandOutletColor.Red, ResourceUtility.GetString("Text.CommandExecuteFailed"));
			}

			return null;
		}
		#endregion

		#region 事件处理
		private void Client_Failed(object sender, ChannelFailureEventArgs e)
		{
			((TcpClient)sender).Failed -= Client_Failed;

			var context = (CommandContext)e.AsyncState;
			context.Output.WriteLine(CommandOutletColor.Green, ResourceUtility.GetString("Text.CommandExecuteSucceed"));
		}

		private void Client_Connected(object sender, ChannelAsyncEventArgs e)
		{
			((TcpClient)sender).Connected -= Client_Connected;

			var context = (CommandContext)e.AsyncState;
			context.Output.WriteLine(CommandOutletColor.Red, ResourceUtility.GetString("Text.CommandExecuteFailed"));
		}
		#endregion
	}
}
