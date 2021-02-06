/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// Type Guess
    /// </summary>
    public class TypeGuess
    {
        Dictionary<string, Func<ITag, CompileContext, Type>> dict;
        /// <summary>
        /// ctor
        /// </summary>
        public TypeGuess()
        {
            dict = new Dictionary<string, Func<ITag, CompileContext, Type>>(StringComparer.OrdinalIgnoreCase);
            Initialize();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            this.Register<NullTag>((tag, ctx) =>
            {
                return typeof(object);
            });
            this.Register<StringTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<TextTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<BooleanTag>((tag, ctx) =>
            {
                return typeof(Boolean);
            });
            this.Register<LayoutTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<NumberTag>((tag, ctx) =>
            {
                return ((NumberTag)tag).Value.GetType();
            });
            this.Register<VariableTag>((tag, ctx) =>
            {
                var t = tag as VariableTag;
                if (t.Parent == null)
                {
                    return ctx.Data.GetType(t.Name);
                }
                var parentType = GetType(t.Parent, ctx);
                var p = DynamicHelpers.GetPropertyInfo(parentType, t.Name);
                if (p != null)
                {
                    return p.PropertyType;
                }
                var f = DynamicHelpers.GetFieldInfo(parentType, t.Name);
                if (f != null)
                {
                    return f.FieldType;
                }
                throw new Exception.CompileException($"[VariableTag]: \"{t.Name}\" is not defined");
            });
            this.Register<FunctaionTag>((tag, ctx) =>
            {
                var t = tag as FunctaionTag;
                if (t.Parent == null)
                {
                    var bodyType = ctx.Data.GetType(t.Name);
                    if (bodyType.BaseType.FullName != "System.MulticastDelegate")
                    {
                        throw new Exception.CompileException($"[FunctaionTag]: \"{bodyType.BaseType}\" is not supported.");
                    }
                    var invokeMethod = bodyType.GetMethod("Invoke");
                    return invokeMethod.ReturnType;

                }
                var parentType = GetType(t.Parent, ctx);
                Type[] types = new Type[t.Children.Count];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = GetType(t.Children[i], ctx);
                }
                var method = DynamicHelpers.GetMethod(parentType, t.Name, types);
                if (method != null)
                {
                    return method.ReturnType;
                }
                throw new Exception.CompileException($"[FunctaionTag]: \"{t.Name}\" is not defined");
            });
            this.Register<ReferenceTag>((tag, ctx) =>
            {
                var t = tag as ReferenceTag;
                return GetType(t.Child, ctx);
            });
            this.Register<LogicTag>((tag, ctx) =>
            {
                return typeof(bool);
            });
            this.Register<ArithmeticTag>((tag, ctx) =>
            {
                var t = tag as ArithmeticTag;
                var types = new List<Type>();
                var opts = new List<Operator>();
                for (var i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt == null)
                    {
                        types.Add(GetType(t.Children[i] as ITag, ctx));
                    }
                }
                if (types.Count == 1)
                {
                    return types[0];
                }
                if (types.Contains(typeof(string)))
                {
                    return typeof(string);
                }
                if (types.Count > 0)
                {
                    return FindBestType(types.ToArray());
                }

                return typeof(int);
            });
            this.Register<ForeachTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<EndTag>((tag, ctx) =>
            {
                return typeof(void);
            });
            this.Register<SetTag>((tag, ctx) =>
            {
                return typeof(void);
            });
            this.Register<CommentTag>((tag, ctx) =>
            {
                return typeof(void);
            });
            this.Register<BodyTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<BlockTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<ElseifTag>((tag, ctx) =>
            {
                var t = tag as ElseifTag;
                var types = new List<Type>();
                var hasVoid = false;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    var type = GetType(t.Children[i], ctx);
                    if (type.FullName == "System.Void")
                    {
                        hasVoid = true;
                    }
                    else
                    {
                        types.Add(type);
                    }
                }
                if (types.Count == 1)
                {
                    return types[0];
                }
                if (types.Count == 0 && hasVoid)
                {
                    return typeof(void);
                }
                return typeof(string);
            });
            this.Register<ElseTag>((tag, ctx) =>
            {
                var t = tag as ElseTag;
                var types = new List<Type>();
                var hasVoid = false;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    var type = GetType(t.Children[i], ctx);
                    if (type.FullName == "System.Void")
                    {
                        hasVoid = true;
                    }
                    else
                    {
                        types.Add(type);
                    }
                }
                if (types.Count == 1)
                {
                    return types[0];
                }
                if (types.Count == 0 && hasVoid)
                {
                    return typeof(void);
                }
                return typeof(string);
            });
            this.Register<IfTag>((tag, ctx) =>
            {
                var t = tag as IfTag;
                Type type = null;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    var cType = GetType(t.Children[i], ctx);
                    if (type == null)
                    {
                        type = cType;
                    }
                    else
                    {
                        if (cType == null || type.FullName != cType.FullName)
                        {
                            return typeof(string);
                        }
                    }
                }
                return type;
            });
            this.Register<IncludeTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<LoadTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<ForTag>((tag, ctx) =>
            {
                return typeof(string);
            });
            this.Register<IndexValueTag>((tag, ctx) =>
            {
                var t = tag as IndexValueTag;
                if (t.Parent == null)
                {
                    throw new Exception.CompileException("[IndexValueTag] : Parent cannot be null");
                }
                var parentType = GetType(t.Parent, ctx);
                if (parentType.FullName == "System.String")
                {
                    return typeof(char);
                }
                var indexType = GetType(t.Index, ctx);
                if (parentType.IsArray)
                {
                    var method = DynamicHelpers.GetMethod(parentType, "get", new Type[] { indexType });
                    if (method != null)
                    {
                        return method.ReturnType;
                    }
                }
                var m = DynamicHelpers.GetMethod(parentType, "get_Item", new Type[] { indexType });
                if (m != null)
                {
                    return m.ReturnType;
                }

                throw new Exception.CompileException($"[IndexValueTag]: \"{tag.ToSource()}\" is not defined");
            });
        }

        /// <summary>
        /// 添加一个判断方法
        /// </summary>
        /// <typeparam name="T">标签类型</typeparam>
        /// <param name="func">判断方法</param>
        public void Register<T>(Func<ITag, CompileContext, Type> func) where T : ITag
        {
            var name = typeof(T).Name;
            Register(name, func);
        }


        /// <summary>
        /// 添加一个判断方法
        /// </summary>
        /// <param name="name">标签名称</param>
        /// <param name="func">判断方法</param>
        public void Register(string name, Func<ITag, CompileContext, Type> func)
        {
            dict[name] = func;
        }

        /// <summary>
        /// 根据标签获取标签结果类型
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="ctx">编译上下文</param>
        /// <returns></returns>
        public Type GetType(ITag tag, CompileContext ctx)
        {
            return GetType(tag.GetType().Name, tag, ctx);
        }

        /// <summary>
        /// 根据标签获取标签结果类型
        /// </summary>
        /// <param name="name">标签名称</param>
        /// <param name="tag">标签</param>
        /// <param name="ctx">编译上下文</param>
        /// <returns></returns>
        public Type GetType(string name, ITag tag, CompileContext ctx)
        {
            if (dict.TryGetValue(name, out var func))
            {
                var type = func(tag, ctx);
                if (type != null)
                {
                    return type;
                }
            }
            throw new Exception.CompileException($"[{name}]:\"{tag.ToSource()}\" is not defined!");
        }

        /// <summary>
        /// 获取数组或者泛型的子类型
        /// </summary>
        /// <param name="type">父类型</param>
        /// <returns></returns>
        public static Type[] InferChildType(Type type)
        {
            if (type.IsArray)
            {
                var methd = DynamicHelpers.GetMethod(type, "Get", new Type[] { typeof(int) });
                if (methd != null)
                {
                    return new Type[] { methd.ReturnType };
                }
            }
            var gType = type.GetInterface("IEnumerable`1");
            if (gType != null)
            {
                return gType.GetGenericArguments();
            }
            //if (type.IsGenericType)
            //{
            //    return type.GetGenericArguments();
            //}
            //var types = type.GetInterfaces();
            //foreach (var t in types)
            //{
            //    if (t.IsGenericType)
            //    {
            //        return t.GetGenericArguments();
            //    }
            //}

            return new Type[] { typeof(object) };
        }

        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <param name="type">TYPE</param>
        /// <returns></returns>
        private static int GetTypeLevel(Type type)
        {
            switch (type.FullName)
            {
                case "System.Object":
                    return 13;
                case "System.String":
                    return 12;
                case "System.Boolean":
                    return 11;
                case "System.Decimal":
                    return 10;
                case "System.Double":
                    return 9;
                case "System.Single":
                    return 8;
                case "System.Int64":
                    return 7;
                case "System.UInt64":
                    return 6;
                case "System.Int32":
                    return 5;
                case "System.UInt32":
                    return 4;
                case "System.Int16":
                    return 3;
                case "System.UInt16":
                    return 2;
                case "System.SByte":
                    return 1;
                case "System.Byte":
                    return 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 是否无符号类型
        /// </summary>
        /// <param name="type">TYPE</param>
        /// <returns></returns>
        private static bool IsMinZero(Type type)
        {
            switch (type.FullName)
            {
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Byte":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 无符号类型转换成对应的有符号类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type ChangeType(Type type)
        {
            switch (type.FullName)
            {
                case "System.UInt16":
                    return typeof(Int16);
                case "System.UInt32":
                    return typeof(Int32);
                case "System.UInt64":
                    return typeof(Int64);
                case "System.SByte":
                    return typeof(Byte);
                default:
                    return type;
            }
        }

        /// <summary>
        /// 选择最优类型
        /// </summary>
        /// <param name="x">类型1</param>
        /// <param name="y">类型2</param>
        /// <returns></returns>
        public static Type ChoiceType(Type x, Type y)
        {
            if ((x.FullName == "System.Single" && (y.FullName == "System.Int64" || y.FullName == "System.UInt64"))
                || (y.FullName == "System.Single" && (x.FullName == "System.Int64" || x.FullName == "System.UInt64")))
            {
                return typeof(double);
            }
            int x1 = GetTypeLevel(x);
            int y1 = GetTypeLevel(y);
            if (IsNumber(x) && IsNumber(y))
            {
                if (x1 > y1)
                {
                    if (IsMinZero(x) && !IsMinZero(x))
                    {
                        return ChangeType(x);
                    }
                    return x;
                }
                if (x1 < y1)
                {
                    if (!IsMinZero(x) && IsMinZero(x))
                    {
                        return ChangeType(y);
                    }
                    return y;
                }
            }
            else
            {
                if (x1 < y1)
                {
                    return y;
                }
            }
            return x;
        }

        /// <summary>
        /// 查找最匹配的类型
        /// </summary>
        /// <param name="types">type array</param>
        /// <returns></returns>
        public static Type FindBestType(Type[] types)
        {
            Type type = types[0];
            for (var i = 1; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    continue;
                }
                type = ChoiceType(type, types[i]);
            }
            return type;
        }

        /// <summary>
        /// 是否基元类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive;
        }

        /// <summary>
        /// 是否是系统定义的数字类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static bool IsNumber(Type type)
        {
            var name = type.FullName;
            if (name == "System.Decimal")
            {
                return true;
            }
            if (!IsPrimitive(type))
            {
                return false;
            }
            switch (name)
            {
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                case "System.Int64":
                case "System.UInt64":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int16":
                case "System.UInt16":
                case "System.SByte":
                case "System.Byte":
                    return true;
            }
            return false;
        }
    }
}
