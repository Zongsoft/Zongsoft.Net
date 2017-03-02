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
using System.Collections.Generic;

using Zongsoft.Services;

namespace Zongsoft.Communication.Commands
{
	public abstract class ServerCommandBase : CommandBase<CommandContext>
	{
		#region 成员字段
		private IListener _server;
		#endregion

		#region 构造函数
		protected ServerCommandBase(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		public IListener Server
		{
			get
			{
				return _server;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_server = value;
			}
		}
		#endregion

		#region 重写方法
		protected override bool CanExecute(object parameter)
		{
			return _server != null && base.CanExecute(parameter);
		}
		#endregion

		#region 静态方法
		internal static IListener GetServer(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as ServerCommandBase;

			if(command != null)
				return command.Server;

			return GetServer(node.Parent);
		}
		#endregion
	}
}
