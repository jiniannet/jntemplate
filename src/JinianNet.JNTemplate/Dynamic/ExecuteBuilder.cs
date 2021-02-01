/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteBuilder
    {

        Dictionary<string, Func<ITag, TemplateContext, object>> dict;

        /// <summary>
        /// ctor
        /// </summary>
        public ExecuteBuilder()
        {
            dict = new Dictionary<string, Func<ITag, TemplateContext, object>>(StringComparer.OrdinalIgnoreCase);
            Initialize();
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <param name="tag">TAG</param>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build(ITag tag)
        {
            return Build(tag.GetType().Name);
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build<T>() where T : ITag
        {
            return Build(typeof(T).Name);
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <param name="name">TAG NAME</param>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build(string name)
        {
            if (dict.TryGetValue(name, out var func))
            {
                return func;
            }
            throw new Exception.CompileException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// 设置Render
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="func">func</param>
        public void Register<T>(Func<ITag, TemplateContext, object> func) where T : ITag
        {
            Register(typeof(T).Name, func);
        }

        /// <summary>
        /// 设置Render
        /// </summary>
        /// <param name="name">tag name</param>
        /// <param name="func">action</param>
        public void Register(string name, Func<ITag, TemplateContext, object> func)
        {
            dict[name] = func;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="isOperator">是否操作符</param>
        /// <param name="value">标签值</param>
        /// <returns>是否是最终结果</returns>
        private bool Eval(List<object> list, bool isOperator, object value)
        {
            if (isOperator)
            {
                Operator op = (Operator)value;
                if (op == Operator.Or || op == Operator.And)
                {
                    object result;
                    bool isTrue;
                    if (list.Count > 1)
                    {
                        var stack = ExpressionEvaluator.ProcessExpression(list.ToArray());
                        result = ExpressionEvaluator.Calculate(stack);
                    }
                    else
                    {
                        result = list[0];
                    }
                    if (result is bool)
                    {
                        isTrue = (bool)result;
                    }
                    else
                    {
                        isTrue = ExpressionEvaluator.CalculateBoolean(result);
                    }
                    if (op == Operator.Or && isTrue)
                    {
                        list.Add(true);
                        return true;
                    }
                    if (op == Operator.And && !isTrue)
                    {
                        list.Add(false);
                        return true;
                    }
                    list.Clear();
                    list.Add(isTrue);
                }
                list.Add(op);
            }
            else
            {
                list.Add(value);
            }
            return false;
        } 
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            Register<VariableTag>((tag, context) =>
            {
                var t = tag as VariableTag;
                object baseValue = null;
                if (t.Parent != null)
                {
                    baseValue = Executor.Exec(t.Parent, context);
                }
                if (baseValue == null)
                {
                    return context.TempData[t.Name];
                }
                return Runtime.Actuator.CallPropertyOrField(baseValue, t.Name);
            });

            Register<ArithmeticTag>((tag, context) =>
            {
                var t = tag as ArithmeticTag;
                List<object> parameters = new List<object>();

                for (int i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt != null)
                    {
                        parameters.Add(opt.Value);
                    }
                    else
                    {
                        parameters.Add(Executor.Exec(t.Children[i], context));
                    }
                }
                var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
                return ExpressionEvaluator.Calculate(stack);
            });

            Register<BodyTag>((tag, context) =>
            {
                var t = tag as BodyTag;
                if (t.Children.Count == 0)
                {
                    return null;
                }
                if (t.Children.Count == 1)
                {
                    return Executor.Exec(t.Children[0], context);
                }
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    sb.Append(Executor.Exec(t.Children[i], context));
                }
                return sb.ToString();
            });

            Register<BooleanTag>((tag, context) =>
            {
                var t = tag as BooleanTag;
                return t.Value;
            });

            Register<CommentTag>((tag, context) =>
            {
                var t = tag as CommentTag;
                return null;
            });


            Register<ElseifTag>((tag, context) =>
            {
                var t = tag as ElseifTag;
                var condition = Executor.Exec(t.Condition, context);
                if (Utility.ToBoolean(condition))
                {
                    if (t.Children.Count == 0)
                    {
                        return null;
                    }
                    if (t.Children.Count == 1)
                    {
                        return Executor.Exec(t.Children[0], context);
                    }
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        sb.Append(Executor.Exec(t.Children[i], context));
                    }
                    return sb.ToString();
                }
                return null;
            });

            Register<ElseTag>((tag, context) =>
            {
                var t = tag as ElseTag;
                if (t.Children.Count == 0)
                {
                    return null;
                }
                if (t.Children.Count == 1)
                {
                    return Executor.Exec(t.Children[0], context);
                }
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    sb.Append(Executor.Exec(t.Children[i], context));
                }
                return sb.ToString();
            });

            Register<EndTag>((tag, context) =>
            {
                var t = tag as EndTag;
                return null;
            });

            Register<ForeachTag>((tag, context) =>
            {
                var t = tag as ForeachTag;
                if (t.Source != null)
                {
                    using (var writer = new StringWriter())
                    {
                        object value = Executor.Exec(t.Source, context);
                        var enumerable = ForeachTag.ToIEnumerable(value);
                        TemplateContext ctx;
                        if (enumerable != null)
                        {
                            var ienum = enumerable.GetEnumerator();
                            ctx = TemplateContext.CreateContext(context);
                            int i = 0;
                            while (ienum.MoveNext())
                            {
                                i++;
                                ctx.TempData.Set(t.Name, ienum.Current, ienum.Current == null ? typeof(object) : ienum.Current.GetType());
                                //为了兼容以前的用户 foreachIndex 保留
                                ctx.TempData.Set("foreachIndex", i);
                                for (int n = 0; n < t.Children.Count; n++)
                                {
                                    object result = Executor.Exec(t.Children[n], ctx);
                                    if (i == 0 && t.Children.Count == 1)
                                    {
                                        return result;
                                    }
                                    if (result != null)
                                    {
                                        writer.Write(result.ToString());
                                    }
                                }
                            }
                        }
                        return writer.ToString();
                    }
                }
                return null;
            });

            Register<ForTag>((tag, context) =>
            {
                var t = tag as ForTag;
                Executor.Exec(t.Initial, context);
                //如果标签为空，则直接为false,避免死循环以内存溢出
                bool run;

                if (t.Condition == null)
                {
                    run = false;
                }
                else
                {
                    run = Utility.ToBoolean(Executor.Exec(t.Condition, context));
                }
                using (var writer = new StringWriter())
                {
                    while (run)
                    {
                        for (int i = 0; i < t.Children.Count; i++)
                        {
                            var obj = Executor.Exec(t.Children[i], context);
                            if (obj != null)
                            {
                                writer.Write(obj.ToString());
                            }
                        }

                        if (t.Do != null)
                        {
                            //执行计算，不需要输出，比如i++
                            Executor.Exec(t.Do, context);
                        }
                        run = Utility.ToBoolean(Executor.Exec(t.Condition, context));
                    }
                    return writer.ToString();
                }
            });

            Register<FunctaionTag>((tag, context) =>
            {
                var t = tag as FunctaionTag;
                object[] args = new object[t.Children.Count];
                for (int i = 0; i < t.Children.Count; i++)
                {
                    args[i] = Executor.Exec(t.Children[i], context);
                }

                object parentValue;
                if (t.Parent == null)
                {
                    parentValue = context.TempData[t.Name];
                }
                else
                {
                    parentValue = Executor.Exec(t.Parent, context);
                }

                if (parentValue == null)
                {
                    return null;
                }
                if (t.Parent == null || (t.Parent != null && string.IsNullOrEmpty(t.Name)))
                {
                    if (parentValue is FuncHandler)
                    {
                        return (parentValue as FuncHandler)(args);
                    }
                    if (parentValue is Delegate)
                    {
                        return (parentValue as Delegate).DynamicInvoke(args);
                    }
                    return null;
                }

                var result = Runtime.Actuator.CallMethod(parentValue, t.Name, args);

                if (result != null)
                {
                    return result;
                }

                result = Runtime.Actuator.CallPropertyOrField(parentValue, t.Name);

                if (result != null && result is Delegate)
                {
                    return (result as Delegate).DynamicInvoke(args);
                }

                return null;
            });

            Register<IfTag>((tag, context) =>
            {
                var t = tag as IfTag;
                for (int i = 0; i < t.Children.Count - 1; i++)
                {
                    var c = (ElseifTag)t.Children[i];
                    if (tag == null)
                    {
                        continue;
                    }
                    if (t.Children[i] is ElseTag)
                    {
                        return Executor.Exec(t.Children[i], context);
                    }

                    var condition = Executor.Exec(c.Condition, context);
                    if (Utility.ToBoolean(condition))
                    {
                        return Executor.Exec(t.Children[i], context);
                    }
                }
                return null;
            });

            Register<IncludeTag>((tag, context) =>
            {
                var t = tag as IncludeTag;
                object path = Executor.Exec(t.Path, context);
                if (path == null)
                {
                    return null;
                }
                var res = context.Load(path.ToString()); 
                if (res != null)
                {
                    return res.Content;
                }
                return null;
            });

            Register<IndexValueTag>((tag, context) =>
            {
                var t = tag as IndexValueTag;
                object obj = Executor.Exec(t.Parent, context);
                object index = Executor.Exec(t.Index, context);
                return Runtime.Actuator.CallIndexValue(obj, index);
            });

            Register<JsonTag>((tag, context) =>
            {
                var t = tag as JsonTag;
                var result = new Dictionary<object, object>();
                foreach (var kv in t.Dict)
                {
                    var key = kv.Key == null ? null : Executor.Exec(kv.Key, context);
                    var value = kv.Value == null ? null : Executor.Exec(kv.Value, context);
                    result.Add(key, value);
                }
                return result;
            });

            Register<LayoutTag>((tag, context) =>
            {
                var t = tag as LayoutTag;
                object path = Executor.Exec(t.Path, context);
                if (path == null)
                {
                    return null;
                }
                var res = context.Load(path.ToString());
                if (res != null)
                {
                    var render = new TemplateRender();
                    render.Context = context;
                    //render.TemplateContent = res.Content;
                    var tags = render.ReadAll(res.Content);
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (tags[i] is BodyTag)
                        {
                            BodyTag body = (BodyTag)tags[i];
                            for (int j = 0; j < t.Children.Count; j++)
                            {
                                body.AddChild(t.Children[j]);
                            }
                            tags[i] = body;
                        }
                    }
                    using (System.IO.StringWriter writer = new StringWriter())
                    {
                        render.Render(writer, tags);
                        return writer.ToString();
                    }
                }
                return null;
            });

            Register<LoadTag>((tag, context) =>
            {
                var t = tag as LoadTag;
                object path = Executor.Exec(t.Path, context);
                if (path == null)
                {
                    return null;
                }
                var res = context.Load(path.ToString());
                if (res != null)
                {
                    var render = new TemplateRender();
                    render.Context = context; 
                    using (System.IO.StringWriter writer = new StringWriter())
                    {
                        render.Render(writer, render.ReadAll(res.Content));
                        return writer.ToString();
                    }
                }
                return null;
            });

            Register<LogicTag>((tag, context) =>
            {
                var t = tag as LogicTag;
                List<object> parameters = new List<object>();

                for (int i = 0; i < t.Children.Count; i++)
                {
                    bool isOperator = t.Children[i] is OperatorTag;
                    object result = Executor.Exec(t.Children[i], context);
                    if (Eval(parameters, isOperator, result))
                    {
                        return parameters[parameters.Count - 1];
                    }
                }

                var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
                return ExpressionEvaluator.Calculate(stack);
            });

            Register<NullTag>((tag, context) =>
            {
                return null;
            });

            Register<NumberTag>((tag, context) =>
            {
                var t = tag as NumberTag;
                return t.Value;
            });

            Register<OperatorTag>((tag, context) =>
            {
                var t = tag as OperatorTag;
                return t.Value;
            });

            Register<ReferenceTag>((tag, context) =>
            {
                var t = tag as ReferenceTag;
                if (t.Child != null)
                {
                    return Executor.Exec(t.Child, context);
                }
                return null;
            });

            Register<SetTag>((tag, context) =>
            {
                var t = tag as SetTag;
                object value = Executor.Exec(t.Value, context);
                if (value != null)
                {
                    context.TempData.Set(t.Name, value, value.GetType());
                }
                return null;
            });

            Register<StringTag>((tag, context) =>
            {
                var t = tag as StringTag;
                return t.Value;
            });

            Register<TextTag>((tag, context) =>
            {
                var t = tag as TextTag;
                if (context.StripWhiteSpace)
                {
                    return (t.Text ?? string.Empty).Trim();
                }
                return t.Text;
            });
        }
    }
}