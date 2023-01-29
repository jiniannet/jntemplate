using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 逻辑测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试For标签里面的逻辑运算
        /// </summary>
        [Fact]
        public void TestForAndTag()
        {
            var templateContent = @"${foreach(entity in list)}${ if (entity.Name.Length > 4 && entity.Name.Substring(0, 4) == ""File"")}yes${else}no${end}${end}";
            var template = Engine.CreateTemplate(templateContent);
            var arr = new List<Entity>();
            arr.Add(new Entity
            {
                Name = "File20190128"
            });
            arr.Add(new Entity
            {
                Name = "19"
            });
            template.Set("list", arr);
            var render = template.Render();
            Assert.Equal("yesno", render);
        }

        /// <summary>
        /// 测试AND 在条件1不满足的条件下不应该执行条件2
        /// </summary>
        [Fact]
        public void TestAndTag()
        {
            var templateContent = @"${ if (entity.Name.Length > 4 && entity.Name.Substring(0, 4) == ""File"" &&  entity.Name.EndsWith(""19""))}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("entity", new Entity
            {
                Name = "File19"
            });
            var render =  template.Render();
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// 测试逻辑优先级处理
        /// </summary>
        [Fact]
        public void TestLogicPriority()
        {
            var templateContent = @"${if(3+5>20 && 1-2<7 && 1<2) }yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("no", render);
        }


        /// <summary>
        /// 测试逻辑表达式(逻辑运算符包括 ==,!=,<,>,>=,<=,||,&&)
        /// </summary>
        [Fact]
        public void TestLogicExpression()
        {
            var templateContent = "${4<=5}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑或表达式 
        /// </summary>
        [Fact]
        public void TestLogicOrExpression()
        {
            var templateContent = "${4==5||5==5}";//如果是复杂的表达式，应优先考虑使用括号来表达优先级，比如 ${(4==5)||(5==5)} 比简单使用  ${4==5||5==5} 更好
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑与表达式
        /// </summary>
        [Fact]
        public void TestLogicAndExpression()
        {
            var templateContent = "${4==5&&5==5}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("False", render);
        }

        /// <summary>
        /// 测试IF
        /// </summary>
        [Fact]
        public void TestIf()
        {
            var templateContent = "${if(3==8)}3=8 success${elseif(3>8)}3>8 success${elseif(2<5)}2<5 success${else}null${end}";
            var template = Engine.CreateTemplate(templateContent);

            var render = template.Render();

            Assert.Equal("2<5 success", render);
        }

        /// <summary>
        /// 对象方法与逻辑运算混合判断
        /// </summary>
        [Fact]
        public void TestAdmixtureIf()
        {
            var templateContent = "$if(date >= date.AddDays(-3))yes$end"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = Engine.CreateTemplate(templateContent);
            template.Set("CreteDate", DateTime.Now);
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 简单对象NULL判断
        /// </summary>
        [Fact]
        public void TestObjectIsNull()
        {
            var templateContent = "${if(dd)}yes${else}no$end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("dd", new object());
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 算术与算术混合判断
        /// </summary>
        [Fact]
        public void TestArithmeticAndArithmetic()
        {
            var templateContent = "$if(3>2 && 5<2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent); ;
            var render = template.Render();
            Assert.Equal("no", render);
        }

        /// <summary>
        /// 对象与算术混合判断
        /// </summary>
        [Fact]
        public void TestObjectAndArithmetic()
        {
            //v1 为空 false,5<2为false，整体结果 false || false 为false
            var templateContent = "$if(v1 || 5<2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<object>("v1", null);
            var render = template.Render();
            Assert.Equal("no", render);
        }

        /// <summary>
        /// 对象判断测试
        /// </summary>
        [Fact]
        public void TestIfObject()
        {
            //单个对象的判断原则类同于JS
            //即数字0为FALSE，否则为TRUE
            //字符串空或者NULL为FALSE，否则为TRUE
            //对象在为NULL时为FALSE，否则为TRUE
            //v1 为空 false,v2等于9，数字不等于0即为true,整体结果 false || true 为true
            var templateContent = "$if(v1 || v2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("v2", 9);
            template.Set<object>("v1", null);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 测试elif
        /// </summary>
        [Fact]
        public void TestElif()
        {
            var templateContent = "${if(3>5)}3>5${elif(2==2)}2=2${else}not${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("2=2", render);
        }
        /// <summary>
        /// 测试NULL
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestNull()
        {
            var templateContent = "${if(model==null)}true${else}false${end}";
            var template = Engine.CreateTemplate("TestNull", templateContent);
            template.Set<object>("model", null);
            var render = template.Render();
            Assert.Equal("true", render);
        }
        /// <summary>
        /// 测试NULL
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestNotNull()
        {
            var templateContent = "${if(model==null)}true${else}false${end}";
            var template = Engine.CreateTemplate("TestNotNull", templateContent);
            template.Set<object>("model", new object());
            var render = template.Render();
            Assert.Equal("false", render);
        }

        /// <summary>
        /// 测试布尔不相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestNotTrue()
        {
            var templateContent = "${if(String.IsNullOrEmpty(Name)!=true)}ok${end}";
            var template = Engine.CreateTemplate("TestNotTrue", templateContent);
            template.Set("Name", "jntemplate");
            template.SetStaticType("String", typeof(string));
            var render = template.Render();
            Assert.Equal("ok", render);
        }

        /// <summary>
        /// 测试布尔相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestFuncTrue()
        {
            var templateContent = "${if(String.IsNullOrEmpty(Name)==true)}ok${end}";
            var template = Engine.CreateTemplate("TestFuncTrue", templateContent);
            template.Set<string>("Name", null);
            template.SetStaticType("String", typeof(string));
            var render = template.Render();
            Assert.Equal("ok", render);
        }

        /// <summary>
        /// 测试字符相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestStringEquals()
        {
            var templateContent = "${if(var1==var2)}yes${else}no${end}";
            var template = Engine.CreateTemplate("TestStringEquals", templateContent);
            template.Set("var1", "jntemplate");
            template.Set("var2", "jntemplate");
            template.SetStaticType("String", typeof(string));
            var render = template.Render();
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// 测试字符串不相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestStringNotEquals()
        {
            var templateContent = "${if(var1!=var2)}yes${else}no${end}";
            var template = Engine.CreateTemplate("TestStringNotEquals", templateContent);
            template.Set("var1", "jntemplate");
            template.Set("var2", "jntemplate");
            template.SetStaticType("String", typeof(string));
            var render = template.Render();
            Assert.Equal("no", render);
        }


        /// <summary>
        /// 测试枚举相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestEnumEquals()
        {
            var templateContent = "${if(enum1==enum2)}ok${end}";
            var template = Engine.CreateTemplate("TestEnumEquals", templateContent);
            template.Set("enum1", PlatformID.Win32NT);
            template.Set("enum2", PlatformID.Win32NT);
            var render = template.Render();
            Assert.Equal("ok", render);
        }

        /// <summary>
        /// 测试枚举不相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestEnumNotEquals()
        {
            var templateContent = "${if(enum1!=enum2)}ok${end}";
            var template = Engine.CreateTemplate("TestEnumNotEquals", templateContent);
            template.Set("enum1", PlatformID.Win32NT);
            template.Set("enum2", PlatformID.Unix);
            var render = template.Render();
            Assert.Equal("ok", render);
        }

#if NOTODO 

        //Struct 还不支持比较

        /// <summary>
        /// 测试结构相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestStructEquals()
        {
            //TODO Struct
            var templateContent = "${if(struct1==struct2)}ok${end}";
            var template = Engine.CreateTemplate("TestStructEquals", templateContent);
            var size = new ViewModel.Size();
            template.Set("struct1", size);
            template.Set("struct2", size);
            var render =  template.Render();
            Assert.Equal("ok", render);
        }

        /// <summary>
        /// 测试结构不相等
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void TestStructNotEquals()
        {
            var templateContent = "${if(struct1!=struct2)}ok${end}";
            var template = Engine.CreateTemplate("TestStructNotEquals", templateContent);
            template.Set("struct1", new ViewModel.Size());
            template.Set("struct2", new ViewModel.Size());
            var render = template.Render();
            Assert.Equal("ok", render);
        }
#endif

        /// <summary>
        /// 测试二个字符串比较
        /// </summary>
        [Fact]
        public void TestStrAndStr()
        {
            var templateContent = "${if(\"key\"==\"key\")}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 测试多余括号的运算
        /// </summary>
        [Fact]
        public void TestParentheses()
        {
            var templateContent = "${if((true))}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 测试括号与算术表达式组合
        /// </summary>
        [Fact]
        public void TestParenthesesAndArithmetic()
        {
            var templateContent = "${if((3+8)-4>0)}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// 测试运逻运算与括号的组合
        /// </summary>
        [Fact]
        public void TestParenthesesAndLogic()
        {
            var templateContent = "${if(id==0||(id>0 && value <10))}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("id",4);
            template.Set("value", 8);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 测试 || 与 &&的组合
        /// </summary>
        [Fact]
        public void TestOrVsAndLogic()
        {
            var templateContent = "${if(id>0 && id<10 && value<10)}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("id", 4);
            template.Set("value", 8);
            var render = template.Render();
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// fixs #I688WB
        /// </summary>
        [Fact]
        public void TestIfOnes()
        {
            var templateContent = "$if(isCollecting)${collectingMoney}${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("isCollecting", true);
            template.Set("collectingMoney", 88.8M);
            var render = template.Render(); ;
            Assert.Equal("88.8", render);
        }
    }
}
