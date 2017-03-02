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
using Zongsoft.Communication.Net;

namespace Zongsoft.Communication.Commands
{
	[DisplayName("${Text.Communication.ServerStatusCommand.Title}")]
	[Description("${Text.Communication.ServerStatusCommand.Description}")]
	public class ServerStatusCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public ServerStatusCommand() : base("Status")
		{
		}

		public ServerStatusCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var server = ServerCommandBase.GetServer(context.CommandNode);

			if(server == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Server"));

			if(server is TcpServer)
				this.DisplayTcpServerInfo((TcpServer)server, context.Output, this.GetActiveChannel(context.CommandNode));
			else if(server is Net.FtpServer)
				this.DisplayFtpServerInfo((FtpServer)server, context.Output);
			else
				this.DisplayListenerInfo(server, context.Output);

			return server.IsListening;
		}
		#endregion

		#region 私有方法
		private void DisplayTcpServerInfo(TcpServer server, ICommandOutlet output, IChannel activeChannel)
		{
			this.DisplayListenerInfo(server, output);

			if(!server.IsListening)
				return;

			output.WriteLine();
			output.Write(CommandOutletColor.DarkMagenta, "ID\t");
			output.Write(CommandOutletColor.DarkMagenta, ResourceUtility.GetString("${LastReceivedTime}") + "\t\t");
			output.Write(CommandOutletColor.DarkMagenta, ResourceUtility.GetString("${LastSendTime}") + "\t\t");
			output.Write(CommandOutletColor.DarkMagenta, ResourceUtility.GetString("${LocalEndPoint}") + "\t\t");
			output.WriteLine(CommandOutletColor.DarkMagenta, ResourceUtility.GetString("${RemoteEndPoint}") + "\t");

			var channels = server.ChannelManager.GetActivedChannels();

			foreach(TcpServerChannel channel in channels)
			{
				if(channel == activeChannel)
					output.Write(CommandOutletColor.Magenta, "* ");

				output.WriteLine(
					"{0}\t{1:yyyy-MM-dd HH:mm:ss}\t{2:yyyy-MM-dd HH:mm:ss}\t{3}\t\t{4}",
					channel.ChannelId,
					channel.LastReceivedTime,
					channel.LastSendTime,
					channel.LocalEndPoint,
					channel.RemoteEndPoint);
			}

			output.WriteLine();
		}

		private void DisplayFtpServerInfo(FtpServer server, ICommandOutlet output)
		{
			this.DisplayListenerInfo(server, output);
		}

		private void DisplayListenerInfo(IListener listener, ICommandOutlet output)
		{
			if(listener is TcpServer)
			{
				output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${ListenAddress}") + ": ");
				output.Write("{0}", ((Zongsoft.Communication.Net.TcpServer)listener).Address);
			}

			output.Write(CommandOutletColor.DarkGray, " [");
			if(listener.IsListening)
				output.Write(CommandOutletColor.Green, ResourceUtility.GetString("${Listening}"));
			else
				output.Write(CommandOutletColor.Red, ResourceUtility.GetString("${Stopped}"));
			output.Write(CommandOutletColor.DarkGray, "]");

			if(listener is IWorker && ((IWorker)listener).Disabled)
				output.WriteLine(CommandOutletColor.DarkMagenta, "({0})", ResourceUtility.GetString("${Disabled}"));
			else
				output.WriteLine();
		}

		private IChannel GetActiveChannel(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as ServerCommand;

			if(command != null)
				return command.Channel;

			return GetActiveChannel(node.Parent);
		}
		#endregion
	}
}
