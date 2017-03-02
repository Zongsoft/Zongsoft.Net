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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Communication.Commands
{
	[DisplayName("${Text.Communication.SendCommand.Title}")]
	[Description("${Text.Communication.SendCommand.Description}")]
	[CommandOption("type", Type = typeof(ContentType), DefaultValue = ContentType.Text, Description = "${Text.Communication.SendCommand.Options.Type}")]
	[CommandOption("encoding", Type = typeof(Encoding), DefaultValue = "UTF8", Description = "${Text.Command.Options.Encoding}")]
	public class SendCommand : CommandBase<CommandContext>
	{
		#region 同步变量
		private readonly object _syncRoot;
		#endregion

		#region 私有变量
		private int _totalCount;
		private int _failedCount;
		#endregion

		#region 构造函数
		public SendCommand() : this("Send")
		{
		}

		public SendCommand(string name) : base(name)
		{
			_syncRoot = new object();
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			_totalCount = 0;
			_failedCount = 0;

			var sender = this.GetSender(context.CommandNode);

			if(sender == null)
				throw new CommandException(ResourceUtility.GetString("${Text.CannotObtainCommandTarget}", "Sender"));

			var waitHandles = new List<WaitHandle>();

			try
			{
				sender.Failed += Sender_Failed;
				sender.Sent += Sender_Sent;

				if(context.Expression.Options.GetValue<ContentType>("type") == ContentType.File)
				{
					this.SendFiles(sender, context, waitHandles);
				}
				else
				{
					var encoding = context.Expression.Options.GetValue<Encoding>("encoding");

					foreach(string arg in context.Expression.Arguments)
					{
						var token = new SendToken(context, string.Format(ResourceUtility.GetString("${Text.ThisText}"), arg), new EventWaitHandle(false, EventResetMode.ManualReset));
						waitHandles.Add(token.WaitHandle);
						sender.Send(encoding.GetBytes(arg), token);
					}
				}

				this.OnSendCompleted(context, waitHandles);
			}
			catch(Exception ex)
			{
				context.Output.WriteLine(CommandOutletColor.Magenta, ResourceUtility.GetString("${Text.AnExceptionOccurredOnRun}"));
				context.Output.WriteLine(CommandOutletColor.Red, string.Format("{0} [{1}]", ex.Message, ex.Source));
				context.Output.WriteLine(CommandOutletColor.DarkYellow, ex.StackTrace);

				this.OnSendCompleted(context, waitHandles);
			}
			finally
			{
				sender.Failed -= Sender_Failed;
				sender.Sent -= Sender_Sent;
			}

			return _totalCount - _failedCount;
		}
		#endregion

		#region 事件处理
		private void Sender_Failed(object sender, ChannelFailureEventArgs e)
		{
			var token = (SendToken)e.AsyncState;
			int index = Interlocked.Increment(ref _totalCount);
			Interlocked.Increment(ref _failedCount);

			try
			{
				//同步锁定，以防止异步并发中导致显示信息错位
				Monitor.Enter(_syncRoot);

				token.Context.Output.Write(CommandOutletColor.DarkYellow, "[{0}] ", index);
				token.Context.Output.Write(CommandOutletColor.DarkRed, ResourceUtility.GetString("${Text.Communication.SendCommand.SendFailed}"), token.Message);
				token.Context.Output.WriteLine(CommandOutletColor.DarkGray, ResourceUtility.GetString("${Text.Communication.SendCommand.Channel}"), e.Channel.ChannelId, e.Channel.LastSendTime);
			}
			finally
			{
				//退出同步锁定
				Monitor.Exit(_syncRoot);

				//设置信号量为非堵塞
				token.WaitHandle.Set();
			}
		}

		private void Sender_Sent(object sender, SentEventArgs e)
		{
			var token = (SendToken)e.AsyncState;
			int index = Interlocked.Increment(ref _totalCount);

			try
			{
				//同步锁定，以防止异步并发中导致显示信息错位
				Monitor.Enter(_syncRoot);

				token.Context.Output.Write(CommandOutletColor.DarkYellow, "[{0}] ", index);
				token.Context.Output.Write(CommandOutletColor.Green, ResourceUtility.GetString("${Text.Communication.SendCommand.SendSucceed}"), token.Message);
				token.Context.Output.WriteLine(CommandOutletColor.DarkGray, ResourceUtility.GetString("${Text.Communication.SendCommand.Channel}"), e.Channel.ChannelId, e.Channel.LastSendTime);
			}
			finally
			{
				//退出同步锁定
				Monitor.Exit(_syncRoot);

				//设置信号量为非堵塞
				token.WaitHandle.Set();
			}
		}
		#endregion

		#region 私有方法
		private void OnSendCompleted(CommandContext context, List<WaitHandle> waitHandles)
		{
			//等待所有的发送方法全部返回
			if(waitHandles.Count > 0)
				EventWaitHandle.WaitAll(waitHandles.ToArray());

			//清空信号量列表
			waitHandles.Clear();

			context.Output.WriteLine();
			context.Output.Write("***** ");
			context.Output.Write(CommandOutletColor.DarkYellow, ResourceUtility.GetString("${Text.Communication.SendCommand.SendCompleted}"), _totalCount, _failedCount);
			context.Output.WriteLine(" *****");
		}

		private void SendFiles(ISender sender, CommandContext context, IList<WaitHandle> waitHandles)
		{
			foreach(string arg in context.Expression.Arguments)
			{
				if(string.IsNullOrWhiteSpace(arg))
					continue;

				if(File.Exists(arg))
				{
					var waitHandle = this.SendFile(sender, context, arg);

					if(waitHandle != null)
						waitHandles.Add(waitHandle);
				}
				else if(Directory.Exists(arg))
				{
					var fileNames = Directory.GetFiles(arg);

					if(fileNames != null && fileNames.Length > 1)
						Array.Sort(fileNames, StringComparer.CurrentCulture);

					context.Output.WriteLine(ResourceUtility.GetString("${Text.Communication.SendCommand.PrepareSendFiles}"), arg, fileNames.Length);

					foreach(string fileName in fileNames)
					{
						var waitHandle = this.SendFile(sender, context, fileName);

						if(waitHandle != null)
							waitHandles.Add(waitHandle);
					}
				}
				else
				{
					context.Output.WriteLine(ResourceUtility.GetString("${Text.FileOrDirectoryNotExists}"), arg);
				}
			}
		}

		private WaitHandle SendFile(ISender sender, CommandContext context, string fileName)
		{
			if(string.IsNullOrWhiteSpace(fileName))
				return null;

			if(!File.Exists(fileName))
			{
				context.Output.WriteLine(ResourceUtility.GetString("${Text.FileOrDirectoryNotExists}"), fileName);
				return null;
			}

			var fileInfo = new FileInfo(fileName);
			var token = new SendToken(context, string.Format(ResourceUtility.GetString("${Text.ThisFile}"), fileName, GetFileSize(fileInfo.Length)), new EventWaitHandle(false, EventResetMode.ManualReset));

			using(var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.SequentialScan))
			{
				sender.Send(stream, token);
			}

			return token.WaitHandle;
		}

		private string GetFileSize(long length)
		{
			if(length < 1024)
				return length.ToString();

			if(length < 1024 * 1024)
				return (length / (double)1024).ToString("0.00") + "KB";

			if(length < 1024 * 1024 * 1024)
				return (length / (double)(1024 * 1024)).ToString("0.00") + "MB";

			return (length / (double)(1024 * 1024 * 1024)).ToString("0.00") + "GB";
		}

		private ISender GetSender(CommandTreeNode node)
		{
			if(node == null)
				return null;

			if(node.Command != null)
			{
				if(node.Command is ISender)
					return (ISender)node.Command;

				if(node.Command is ISenderHost)
					return ((ISenderHost)node.Command).Sender;
			}

			return GetSender(node.Parent);
		}
		#endregion

		#region 嵌套子类
		private class SendToken
		{
			#region 公共字段
			public readonly string Message;
			public readonly CommandContext Context;
			public readonly EventWaitHandle WaitHandle;
			public readonly DateTime StartTime;
			#endregion

			#region 构造函数
			internal SendToken(CommandContext context, string message, EventWaitHandle waitHandle)
			{
				this.Context = context;
				this.Message = message;
				this.WaitHandle = waitHandle;
				this.StartTime = DateTime.Now;
			}
			#endregion
		}
		#endregion

		public enum ContentType
		{
			[Description("${Text.ContentType.Text}")]
			Text,

			[Description("${Text.ContentType.File}")]
			File,
		}
	}
}
