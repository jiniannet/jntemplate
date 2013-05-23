/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public class Calculator
    {
        public enum LetterType
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0,
            /// <summary>
            /// 
            /// </summary>
            Operator = 1,
            /// <summary>
            /// 左圆括号
            /// </summary>
            LeftParentheses = 2,
            /// <summary>
            /// 右中括号
            /// </summary>
            RightParentheses = 3,
            /// <summary>
            /// 
            /// </summary>
            Number = 4,
            /// <summary>
            /// 
            /// </summary>
            Other = 5
        }

        private Boolean IsOperator(Char c)
        {
            if ((c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '(') || (c == ')') || (c == '%'))
                return true;
            return false;
        }

        private Boolean IsOperator(LetterType letterType)
        {
            if (letterType == LetterType.LeftParentheses || letterType == LetterType.RightParentheses || letterType == LetterType.Operator)
                return true;
            return false;
        }

        private LetterType GetLetterType(String value, Int32 index)
        {
            switch (value[index])
            {
                case '+':
                case '-':
                    if (index == 0 || IsOperator(value[index - 1]))
                    {
                        if (index < value.Length - 1 && Char.IsNumber(value[index + 1]))
                            return LetterType.Number;
                        return LetterType.Other;
                    }
                    return LetterType.Operator;
                case '*':
                case '/':
                case '%':
                    return LetterType.Operator;
                case '(':
                    return LetterType.LeftParentheses;
                case ')':
                    return LetterType.RightParentheses;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return LetterType.Number;
                case '.':
                    if (index > 0 && Char.IsNumber(value[index - 1]) && index < value.Length - 1 && Char.IsNumber(value[index + 1]))
                        return LetterType.Number;
                    return LetterType.Other;
                default:
                    return LetterType.Other;
            }
        }

        private Int32 GetPriority(Char c)
        {
            return GetPriority(c.ToString());
        }

        private Boolean IsOperator(String value)
        {
            switch (value)
            {
                case "||":
                case "|":
                case "&":
                case "&&":
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!=":
                case "+":
                case "-":
                case "*":
                case "/":
                case "(":
                case ")":
                case "%":
                    return true;
                default:
                    return false;
            }
        }

        private Int32 GetPriority(String c)
        {
            switch (c)
            {
                case "||":
                case "|":
                case "&":
                case "&&":
                    return 5;
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!=":
                    return 6;
                case "+":
                case "-":
                    return 7;
                case "%":
                case "*":
                    return 8;
                case "/":
                    return 9;
                default:
                    return 9;
            }
        }

        private Boolean GetBoolean(Double n)
        {
            return !(n == 0);
        }

        private Int32 GetInt(Boolean b)
        {
            return b ? 1 : 0;
        }

        private LetterType GetLetterType(Char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    return LetterType.Operator;
                case '(':
                    return LetterType.LeftParentheses;
                case ')':
                    return LetterType.RightParentheses;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return LetterType.Number;
                default:
                    return LetterType.Other;
            }

        }

        public Stack<Object> ProcessExpression(String value)
        {
            value = value.Replace("  ", String.Empty);
            List<Object> result = new List<Object>();
            Int32 j = 0;
            Int32 i;

            for (i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '(':
                    case ')':
                    case '%':
                        if (j < i)
                        {
                            result.Add(Double.Parse(value.Substring(j, i - j)));
                            j = i;
                        }
                        result.Add(value[i].ToString());
                        j++;
                        break;
                }
            }
            if (j < i)
            {
                result.Add(Double.Parse(value.Substring(j, i - j)));
            }
            return ProcessExpression(result.ToArray());

        }

        public Stack<Object> ProcessExpression(Object[] value)
        {
            Stack<Object> post = new Stack<Object>();
            Stack<String> stack = new Stack<String>();


            for (Int32 i = 0; i < value.Length; i++)
            {
                String fullName;
                if (value[i] != null)
                {
                    fullName = value[i].GetType().FullName;
                }
                else
                {
                    fullName = "System.String";
                    value[i] = string.Empty;
                }
                switch (fullName)
                {
                    case "System.Double":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Boolean":
                        post.Push(value[i]);
                        break;
                    case "System.String":
                        switch (value[i].ToString())
                        {
                            case "(":
                                stack.Push("(");
                                break;
                            case ")":
                                while (stack.Count > 0)
                                {
                                    if (stack.Peek() == "(")
                                    {
                                        stack.Pop();
                                        break;
                                    }
                                    else
                                        post.Push(stack.Pop());
                                }
                                break;
                            case "+":
                            case "-":
                            case "*":
                            case "%":
                            case "/":
                            case "||":
                            case "|":
                            case "&&":
                            case "&":
                            case ">":
                            case ">=":
                            case "<":
                            case "<=":
                            case "==":
                            case "!=":
                                if (stack.Count == 0)
                                {
                                    stack.Push(value[i].ToString());
                                }
                                else
                                {
                                    String eX = stack.Peek();
                                    String eY = value[i].ToString();
                                    if (GetPriority(eY) >= GetPriority(eX))
                                    {
                                        stack.Push(eY);
                                    }
                                    else
                                    {
                                        while (stack.Count > 0)
                                        {
                                            if (GetPriority(eX) > GetPriority(eY) && stack.Peek() != "(")// && stack.Peek() != '('
                                            {
                                                post.Push(stack.Pop());
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        stack.Push(eY);
                                    }
                                }
                                break;
                            default:
                                post.Push(value[i]);
                                break;
                        }
                        break;

                }
            }

            while (stack.Count > 0)
            {
                post.Push(stack.Pop());
            }

            return post;
        }

        private bool IsNumber(String fullName)
        {
            switch (fullName)
            {
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Decimal":
                    return true;
                default:
                    return false;
            }
        }

        public Object Calculate(Object x, Object y, String value)
        {
            Type tX = x.GetType();
            Type tY = y.GetType();

            if (IsNumber(tX.FullName) && IsNumber(tY.FullName))
                return Calculate(Convert.ToDouble(x), Convert.ToDouble(y), value);
            if (tX.FullName == "System.Boolean" && tY.FullName == "System.Boolean")
                return Calculate((Boolean)x, (Boolean)y, value);
            if (tX.FullName == "System.String" && tY.FullName == "System.String")
                return Calculate((String)x, (String)y, value);
            return Calculate(x.ToString(), y.ToString(), value);
            //throw new ArgumentException(String.Concat(tX.FullName, " 不能和类型 ", tY.FullName, " 进行操作"));
        }

        public Object Calculate(Boolean x, Boolean y, String value)
        {
            switch (value)
            {
                case "==":
                    return x == y;
                case "!=":
                    return x != y;
                case "||":
                    return x || y;
                case "&&":
                    return x && y;
                default:
                    throw new ArgumentException();
            }
        }

        public Object Calculate(String x, String y, String value)
        {
            switch (value)
            {
                case "==":
                    return x.Equals(y, StringComparison.OrdinalIgnoreCase);
                case "!=":
                    return !x.Equals(y, StringComparison.OrdinalIgnoreCase);
                case "+":
                    return String.Concat(x, y);
                default:
                    throw new ArgumentException();
            }
        }

        public Object Calculate(Double x, Double y, String value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                    return x == y;
                case "!=":
                    return x != y;

                //case "||":
                //    return GetBoolean(x.) || GetBoolean(y.) ? 1 : 0, 0, 0);
                //case "&&":
                //    return GetBoolean(x.) && GetBoolean(y.) ? 1 : 0, 0, 0);
                default:
                    throw new ArgumentException();
            }
        }

        public Object Calculate(Stack<Object> value)
        {
            Stack<Object> post = new Stack<Object>();
            while (value.Count > 0)
            {
                post.Push(value.Pop());
            }
            Stack<Object> stack = new Stack<Object>();

            while (post.Count > 0)
            {
                Object obj = post.Pop();
                if (IsOperator(obj.ToString()))
                {
                    Object y = stack.Pop();
                    Object x = stack.Pop();
                    stack.Push(Calculate(x, y, obj.ToString()));
                }
                else
                {
                    stack.Push(obj);
                }
            }

            return stack.Pop();
        }

        public Object Calculate(Object[] value)
        {
            Stack<Object> stack = ProcessExpression(value);

            return Calculate(stack);
        }

        public Object Calculate(String value)
        {
            Stack<Object> stack = ProcessExpression(value);

            return Calculate(stack);
        }
    }
    #region
#if OLD
    public class Actuator
    {
        public enum LetterType
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0,
            /// <summary>
            /// 
            /// </summary>
            Operator = 1,
            /// <summary>
            /// 左圆括号
            /// </summary>
            LeftParentheses = 2,
            /// <summary>
            /// 右中括号
            /// </summary>
            RightParentheses = 3,
            /// <summary>
            /// 
            /// </summary>
            Number = 4,
            /// <summary>
            /// 
            /// </summary>
            Other = 5
        }

        private Boolean IsOperator(Char c)
        {
            if ((c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '(') || (c == ')'))
                return true;
            return false;
        }

        private Boolean IsOperator(LetterType letterType)
        {
            if (letterType == LetterType.LeftParentheses || letterType == LetterType.RightParentheses || letterType == LetterType.Operator)
                return true;
            return false;
        }

        private LetterType GetLetterType(String , Int32 index)
        {
            switch ([index])
            {
                case '+':
                case '-':
                    if (index == 0 || IsOperator([index - 1]))
                    {
                        if (index < .Length - 1 && Char.IsNumber([index + 1]))
                            return LetterType.Number;
                        return LetterType.Other;
                    }
                    return LetterType.Operator;
                case '*':
                case '/':
                    return LetterType.Operator;
                case '(':
                    return LetterType.LeftParentheses;
                case ')':
                    return LetterType.RightParentheses;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return LetterType.Number;
                case '.':
                    if (index > 0 && Char.IsNumber([index - 1]) && index < .Length - 1 && Char.IsNumber([index + 1]))
                        return LetterType.Number;
                    return LetterType.Other;
                default:
                    return LetterType.Other;
            }
        }

        private Int32 GetPriority(Char c)
        {
            return GetPriority(c.ToString());
        }
        
        private Int32 GetPriority(String c)
        {
            switch (c)
            {
                case "||":
                case "|":
                case "&":
                case "&&":
                    return 5;
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!=":
                    return 6;
                case "+":
                case "-":
                    return 7;
                case "*":
                    return 8;
                case "/":
                    return 9;
                default:
                    return 9;
            }
        }

        private Boolean GetBoolean(Double n)
        {
            return !(n == 0);
        }

        private Int32 GetInt(Boolean b)
        {
            return b ? 1 : 0;
        }

        private LetterType GetLetterType(Char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                    return LetterType.Operator;
                case '(':
                    return LetterType.LeftParentheses;
                case ')':
                    return LetterType.RightParentheses;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return LetterType.Number;
                default:
                    return LetterType.Other;
            }

        }

        public Stack<Element> ProcessExpression(String )
        {
             = .Replace(" ", String.Empty);
            List<Element> result = new List<Element>();
            Int32 j = 0;
            Int32 i;

            for (i = 0; i < .Length; i++)
            {
                switch ([i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '(':
                    case ')':
                        if (j < i)
                        {
                            result.Add(new NumberElement(Double.Parse(.Substring(j, i - j)), 0, 0));
                            j = i;
                        }
                        result.Add(new OperatorElement([i].ToString(), 0, 0));
                        j++;
                        break;
                }
            }
            if (j < i)
            {
                result.Add(new NumberElement(Double.Parse(.Substring(j, i - j)), 0, 0));
            }
            return ProcessExpression(result.ToArray());

        }

        public Stack<Element> ProcessExpression(Element[] )
        {
            Stack<Element> post = new Stack<Element>();
            Stack<OperatorElement> stack = new Stack<OperatorElement>();


            for (Int32 i = 0; i < .Length; i++)
            {
                switch ([i].ElementType)
                {
                    case ElementType.Number:
                    case ElementType.Boolean:
                        post.Push([i]);
                        break;

                    case ElementType.Operator:
                        OperatorElement element = [i] as OperatorElement;
                        switch (element.)
                        {
                            case "(":
                                stack.Push(element);
                                break;
                            case ")":
                                while (stack.Count > 0)
                                {
                                    if (stack.Peek(). == "(")
                                    {
                                        stack.Pop();
                                        break;
                                    }
                                    else
                                        post.Push(stack.Pop());
                                }
                                break;
                            case "+":
                            case "-":
                            case "*":
                            case "/":

                            case "||":
                            case "|":
                            case "&&":
                            case "&":
                            case ">":
                            case ">=":
                            case "<":
                            case "<=":
                            case "==":
                            case "!=":
                             

                                if (stack.Count == 0)
                                {
                                    stack.Push([i] as OperatorElement);
                                }
                                else
                                {
                                    OperatorElement eX = stack.Peek();
                                    OperatorElement eY = [i] as OperatorElement;
                                    if (GetPriority(eY.) >= GetPriority(eX.))
                                    {
                                        stack.Push(eY);
                                    }
                                    else
                                    {
                                        while (stack.Count > 0)
                                        {
                                            if (GetPriority(eX.) > GetPriority(eY.) && stack.Peek(). != "(")// && stack.Peek() != '('
                                            {
                                                post.Push(stack.Pop());
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        stack.Push(eY);
                                    }
                                }
                                break;
                            default:
                                throw new Exception(String.Concat([i].ElementType.ToString(), " Error... in Line:", [i].Line.ToString()));
                        }
                        break;
                    default:
                        throw new Exception(String.Concat([i].ElementType.ToString(), " Error... in Line:", [i].Line.ToString()));
                }
            }

            while (stack.Count > 0)
            {
                post.Push(stack.Pop());
            }

            return post;
        }

        public Element Calculate(Element x, Element y, OperatorElement o)
        {
            if(x.ElementType == ElementType.Number && y.ElementType == ElementType.Number)
                return Calculate( x as NumberElement,  y as NumberElement, o);
            if (x.ElementType == ElementType.Boolean && y.ElementType == ElementType.Boolean)
                return Calculate(x as BooleanElement, y as BooleanElement, o);
            if (x.ElementType == ElementType.Text && y.ElementType == ElementType.Text)
                return Calculate(x as TextElement, y as TextElement, o);
            throw new ArgumentException(String.Concat(x.ElementType.ToString()," 不能和类型 ",y.ElementType.ToString()," 比较"));
        }

        public Element Calculate(BooleanElement x, BooleanElement y, OperatorElement o)
        {
            switch (o.)
            {
                case "==":
                    return new BooleanElement(x. == y., 0, 0);
                case "!=":
                    return new BooleanElement(x. != y., 0, 0);
                case "||":
                    return new BooleanElement(x. || y.,0,0);
                case "&&":
                    return new BooleanElement(x. && y.,0,0);
                default:
                    throw new ArgumentException(o.);
            }
        }

        public Element Calculate(TextElement x, TextElement y, OperatorElement o)
        {
            switch (o.)
            {
                case "==":
                    return new BooleanElement(x.Text.Equals(y.Text, StringComparison.OrdinalIgnoreCase), 0, 0);
                case "!=":
                    return new BooleanElement(!x.Text.Equals(y.Text, StringComparison.OrdinalIgnoreCase), 0, 0);
                default:
                    throw new ArgumentException(o.);
            }
        }

        public Element Calculate(NumberElement x, NumberElement y, OperatorElement o)
        {
            switch (o.)
            {
                case "+":
                    return new NumberElement(x. + y., 0, 0);
                case "-":
                    return new NumberElement(x. - y., 0, 0);
                case "*":
                    return new NumberElement(x. * y., 0, 0);
                case "/":
                    return new NumberElement(x. / y., 0, 0);
                case ">=":
                    return new BooleanElement(x. >= y. , 0, 0);
                case "<=":
                    return new BooleanElement(x. <= y., 0, 0);
                case "<":
                    return new BooleanElement(x. < y. , 0, 0);
                case ">":
                    return new BooleanElement(x. > y. , 0, 0);
                case "==":
                    return new BooleanElement(x. == y., 0, 0);
                case "!=":
                    return new BooleanElement(x. != y., 0, 0);

                //case "||":
                //    return new NumberElement(GetBoolean(x.) || GetBoolean(y.) ? 1 : 0, 0, 0);
                //case "&&":
                //    return new NumberElement(GetBoolean(x.) && GetBoolean(y.) ? 1 : 0, 0, 0);
                default:
                    throw new ArgumentException(o.);
            }
        }

        public Element Calculate(Stack<Element> )
        {
            Stack<Element> post = new Stack<Element>();
            while (.Count > 0)
            {
                post.Push(.Pop());
            }
            Stack<Element> stack = new Stack<Element>();

            while (post.Count > 0)
            {
                Element element = post.Pop();
                if (element.ElementType == ElementType.Operator)
                {
                    Element y = stack.Pop();
                    Element x = stack.Pop();
                    stack.Push(Calculate(x, y, element as OperatorElement));
                }
                else if (element.ElementType == ElementType.Number || element.ElementType == ElementType.Boolean)
                {
                    stack.Push(element);
                }
                else
                {
                    throw new Exception("element error...");
                }
            }

            return stack.Pop();
        }
    }
#endif
    #endregion
}
