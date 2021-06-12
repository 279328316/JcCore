using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Linq.Expressions;
using System.Collections;

namespace Jc
{
    /// <summary>
    /// 异常处理Helper
    /// </summary>
    internal class DbExHelper
    {
        /// <summary>
        /// 抛出异常
        /// <param name="msg">异常消息</param>
        /// <param name="innerException">内部异常</param>
        /// </summary>
        internal static void Throw(string msg = "异常", Exception innerException = null)
        {
            throw new Exception(msg, innerException);
        }

        /// <summary>
        /// 如果为true,则抛出异常
        /// </summary>
        /// <param name="throwIf">抛出异常条件</param>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        internal static void ThrowIf(bool throwIf, string msg = "异常")
        {
            if (throwIf)
            {
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 如果为null,则抛出异常
        /// 默认严格模式下string.Empty,Guid.Empty,int 0,ICollection.Count 0 都会被作为false
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="msg">异常消息</param>
        /// <param name="isStrict">严格模式 true</param>
        /// <returns></returns>
        internal static void ThrowIfNull(object obj,string msg = "异常", bool isStrict = true)
        {
            if (IsNullOrEmpty(obj,isStrict))
            {
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 如果不为null,则抛出异常
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="msg">异常消息</param>
        /// <param name="isStrict">默认严格模式下string.Empty,Guid.Empty,int 0,ICollection.Count 0 都会被作为false</param>
        /// <returns></returns>
        internal static void ThrowIfNotNull(object obj, string msg = "异常", bool isStrict = true)
        {
            if (!IsNullOrEmpty(obj,isStrict))
            {
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Obj IsNullOrEmpty
        /// 默认严格模式下string.Empty,Guid.Empty,int 0,ICollection.Count 0 都会被作为false
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isStrict">严格模式</param>
        /// <returns></returns>
        internal static bool IsNullOrEmpty(object obj, bool isStrict = true)
        {
            bool result = false;
            if (obj == null)
            {
                result = true;
            }
            else if (isStrict)
            {
                Type type = obj.GetType();
                if (type == typeof(string))
                {
                    if (string.IsNullOrEmpty(obj.ToString()))
                    {
                        result = true;
                    }
                }
                else if (type == typeof(Guid) || type == typeof(Guid?))
                {
                    if (((Guid)obj) == Guid.Empty)
                    {
                        result = true;
                    }
                }
                else if (type == typeof(int) || type == typeof(int?))
                {
                    if (((int)obj) == 0)
                    {
                        result = true;
                    }
                }
                else if (type == typeof(long) || type == typeof(long?))
                {
                    if (((long)obj) == 0)
                    {
                        result = true;
                    }
                }
                else if (obj is ICollection)
                {
                    ICollection collection = obj as ICollection;
                    if (collection.Count <= 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Exception详细异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="withStackTrace"></param>
        /// <returns></returns>
        internal static string GetExceptionMsg(Exception ex,bool withStackTrace = true)
        {
            if(ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            string msg = ex.Message;
            
            if (withStackTrace)
            {
                msg += $"\r\n{ex.StackTrace}";
            }
            Exception innerEx = ex.InnerException;
            int innerExCount = 0;
            while (innerEx != null && innerExCount < 5)
            {
                msg += $"\r\n{innerEx.Message}";
                if (withStackTrace)
                {
                    msg += $"\r\n{innerEx.StackTrace}";
                }
                innerEx = innerEx.InnerException;
                innerExCount++;
            }
            return msg;
        }
    
    }
}
