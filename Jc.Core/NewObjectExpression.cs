
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// New Object Expression
    /// 合并NewExpression使用.
    /// </summary>
    public class NewObjectExpression : Expression, IArgumentProvider
    {
        private IList<Expression> arguments;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="arguments"></param>
        /// <param name="members"></param>
        internal NewObjectExpression(ConstructorInfo constructor, IList<Expression> arguments, List<MemberInfo> members)
        {
            this.Constructor = constructor;
            this.arguments = arguments;
            this.Members = members;

            if (members != null)
            {
                List<string> nameList = members.Select(member => member.Name).ToList();
                for (int i = 0; i < nameList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ExpressionString))
                    {
                        ExpressionString += "," + nameList[i];
                    }
                    else
                    {
                        ExpressionString = nameList[i];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type
        {
            get { return Constructor.DeclaringType; }
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.New; }
        }

        /// <summary>
        /// Gets the called constructor.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        /// Gets the arguments to the constructor.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments
        {
            get { return (ReadOnlyCollection<Expression>)arguments; }
        }

        Expression IArgumentProvider.GetArgument(int index)
        {
            return arguments[index];
        }

        int IArgumentProvider.ArgumentCount
        {
            get
            {
                return arguments.Count;
            }
        }
        
        /// <summary>
        /// ExpressionString
        /// </summary>
        public string ExpressionString { get; private set; } = "";

        public ConstructorInfo Constructor1 => Constructor;

        public List<MemberInfo> Members { get; set; }

        /// <summary>
        /// 更新members
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public NewObjectExpression Update(IList<Expression> arguments, List<MemberInfo> members)
        {
            if (arguments != null)
            {
                this.arguments = arguments;
            }
            if (Members != null)
            {
                this.Members = members;
                ExpressionString = "";
                List<string> nameList = members.Select(member => member.Name).ToList();
                for (int i = 0; i < nameList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ExpressionString))
                    {
                        ExpressionString += "," + nameList[i];
                    }
                    else
                    {
                        ExpressionString = nameList[i];
                    }
                }                
            }
            return this;
        }
    }
}