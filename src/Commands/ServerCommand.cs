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
	[DisplayName("${Text.Communication.ServerCommand.Title}")]
	[Description("${Text.Communication.ServerCommand.Description}")]
	[CommandOption("channel", Type = typeof(int), DefaultValue = 0, Description = "${Text.Communication.ServerCommand.Options.Channel}")]
	public class ServerCommand : ServerCommandBase, ISenderHost
	{
		#region 事件定义
		public event EventHandler ChannelChanged;
		#endregion

		#region 成员变量
		private IChannel _channel;
		#endregion

		#region 构造函数
		public ServerCommand() : base("Server")
		{
		}

		public ServerCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		public IChannel Channel
		{
			get
			{
				//如果当前通道为空或者状态是空闲均返回空。
				//因为当通达为空闲状态时，是不能进行任何网络操作的，
				//所以返回给其他命令会导致不可预知的效果。
				if(_channel == null || _channel.IsIdled)
					return null;

				return _channel;
			}
			set
			{
				if(object.ReferenceEquals(_channel, value))
					return;

				_channel = value;

				this.OnChannelChanged(EventArgs.Empty);
			}
		}

		ISender ISenderHost.Sender
		{
			get
			{
				return this.Channel;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var server = ServerCommandBase.GetServer(context.CommandNode) as TcpServer;

			if(server == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Server"));

			int channelId = -1;

			if(context.Expression.Options.TryGetValue("channel", out channelId))
			{
				if(channelId < 0)
					throw new CommandOptionValueException("channel", channelId.ToString());

				//获取指定编号的通道对象
				var channel = server.ChannelManager.GetActivedChannel(channelId);

				//如果获取指定的通道失败或者获取到的通道为空闲状态，则抛出命令选项值异常
				if(channel == null || channel.IsIdled)
					throw new CommandException(string.Format("Can not obtain the actived channel by '#{0}' channel-id.", channelId));

				//设置服务器的活动通道
				_channel = channel;

				//显示命令执行成功信息
				context.Output.WriteLine(ResourceUtility.GetString("${Text.CommandExecuteSucceed}"));
			}

			return channelId;
		}
		#endregion

		#region 激发事件
		protected virtual void OnChannelChanged(EventArgs args)
		{
			this.ChannelChanged?.Invoke(this, args);
		}
		#endregion
	}
}
