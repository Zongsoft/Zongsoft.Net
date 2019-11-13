# Zongsoft.Net

![license](https://img.shields.io/github/license/Zongsoft/Zongsoft.Net) ![download](https://img.shields.io/nuget/dt/Zongsoft.Net) ![version](https://img.shields.io/github/v/release/Zongsoft/Zongsoft.Net?include_prereleases) ![github stars](https://img.shields.io/github/stars/Zongsoft/Zongsoft.Net?style=social)

README: [English](https://github.com/Zongsoft/Zongsoft.Net/blob/master/README.md) | [简体中文](https://github.com/Zongsoft/Zongsoft.Net/blob/master/README-zh_CN.md)

-----

该项目是关于高性能TCP通讯处理的类库，实现采用的是 .NET Socket 提供的异步事件处理模型机制，在 Windows 平台中由底层 WinSocket 提供IOCP支持。

本项目还包括一套 FTP 服务器类库，支持 FTP 数据传输的“主动”和“被动”两种模式的完整实现。
> 为什么我们要重新实现一个 FTP 服务器？因为我们需要在FTP服务器收到任何命令时执行业务系统中的某些事务，而市面上的第三方FTP服务器似乎都没有提供这种灵活而高效的扩展机制。

当然，如果您需要利用这些扩展机制的话，那么请先参考 [Zongsoft.CoreLibrary](https://github.com/Zongsoft/Zongsoft.CoreLibrary) 项目中 `Services.Composition` 命令空间的那些类，我们称这套扩展机制为“执行管道(ExecutionPipelines)”模型。
