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
using Zongsoft.Communication.Net;

namespace Zongsoft.Communication.Commands
{
	[DisplayName("${Text.Communication.ClientCommand.Title}")]
	[Description("${Text.Communication.ClientCommand.Description}")]
	[CommandOption("address", Type = typeof(string), Description = "${Text.Communication.Client.Options.Address}")]
	public class TcpClientCommand : CommandBase<CommandContext>, ISenderHost
	{
		#region 成员变量
		private TcpClient _client;
		#endregion

		#region 构造函数
		public TcpClientCommand() : base("Client")
		{
		}

		public TcpClientCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		public TcpClient Client
		{
			get
			{
				return _client;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_client = value;
			}
		}

		ISender ISenderHost.Sender
		{
			get
			{
				return _client;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Options.Count < 1)
			{
				context.Executor.Execute("/Help " + context.Expression.FullPath);
				return this.Client;
			}

			var address = context.Expression.Options.GetValue<string>("address");

			if(!string.IsNullOrWhiteSpace(address))
			{
				address = address.Replace('#', ':');
				_client.RemoteAddress = Zongsoft.Common.Convert.ConvertValue<IPEndPoint>(address);
				context.Output.WriteLine(ResourceUtility.GetString("Text.CommandExecuteSucceed"));
			}

			return this.Client;
		}
		#endregion

		#region 静态方法
		internal static TcpClient GetClient(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as TcpClientCommand;

			if(command != null)
				return command.Client;

			return GetClient(node.Parent);
		}
		#endregion

	}
}
