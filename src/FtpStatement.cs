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

namespace Zongsoft.Communication.Net
{
    public class FtpStatement
    {
        #region 成员字段
        private string _name;
        private string _argument;
        private object _result;
        #endregion

        #region 构造函数
        internal FtpStatement(string name, string argument)
        {
            _name = name;
            _argument = argument;
        }
        #endregion

        #region 公共属性
        public string Name
        {
            get { return _name; }
        }

        public string Argument
        {
            get { return _argument; }
        }

        public object Result
        {
            get { return _result; }
            internal set { _result = value; }
        }
        #endregion
    }
}