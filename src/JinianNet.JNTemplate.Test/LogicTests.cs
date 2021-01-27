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
        public async Task TestForAndTag()
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
            template.Set("list",arr);
            var render = await Excute(template);
            Assert.Equal("yesno", render);
        }

        /// <summary>
        /// 测试AND 在条件1不满足的条件下不应该执行条件2
        /// </summary>
        [Fact]
        public async Task TestAndTag()
        {
            var templateContent = @"${ if (entity.Name.Length > 4 && entity.Name.Substring(0, 4) == ""File"" &&  entity.Name.EndsWith(""19""))}yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("entity",new Entity
            {
                Name = "File19"
            });
            var render = await Excute(template);
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// 测试逻辑优先级处理
        /// </summary>
        [Fact]
        public async Task TestLogicPriority()
        {
            var templateContent = @"${if(3+5>20 && 1-2<7 && 1<2) }yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            var render =await Excute(template);
            Assert.Equal("no", render);
        }


        /// <summary>
        /// 测试逻辑表达式(逻辑运算符包括 ==,!=,<,>,>=,<=,||,&&)
        /// </summary>
        [Fact]
        public async Task TestLogicExpression()
        {
            var templateContent = "${4<=5}";
            var template = Engine.CreateTemplate(templateContent);
            var render = await Excute(template);

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑或表达式 
        /// </summary>
        [Fact]
        public async Task TestLogicOrExpression()
        {
            var templateContent = "${4==5||5==5}";//如果是复杂的表达式，应优先考虑使用括号来表达优先级，比如 ${(4==5)||(5==5)} 比简单使用  ${4==5||5==5} 更好
            var template = Engine.CreateTemplate(templateContent);
            var render = await Excute(template);

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑与表达式
        /// </summary>
        [Fact]
        public async Task TestLogicAndExpression()
        {
            var templateContent = "${4==5&&5==5}";
            var template = Engine.CreateTemplate(templateContent);
            var render = await Excute(template);

            Assert.Equal("False", render);
        }

        /// <summary>
        /// 测试IF
        /// </summary>
        [Fact]
        public async Task TestIf()
        {
            var templateContent = "${if(3==8)}3=8 success${elseif(3>8)}3>8 success${elseif(2<5)}2<5 success${else}null${end}";
            var template = Engine.CreateTemplate(templateContent);

            var render = await Excute(template);

            Assert.Equal("2<5 success", render);
        }

        /// <summary>
        /// 对象方法与逻辑运算混合判断
        /// </summary>
        [Fact]
        public async Task TestAdmixtureIf()
        {
            var templateContent = "$if(date >= date.AddDays(-3))yes$end"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = Engine.CreateTemplate(templateContent);
            template.Set("CreteDate",DateTime.Now);
            template.Set("date",DateTime.Now);
            var render = await Excute(template);
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 简单对象NULL判断
        /// </summary>
        [Fact]
        public async Task TestObjectIsNull()
        {
            var templateContent = "${if(dd)}yes${else}no$end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("dd",new object());
            var render = await Excute(template);
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 算术与算术混合判断
        /// </summary>
        [Fact]
        public async Task TestArithmeticAndArithmetic()
        {
            var templateContent = "$if(3>2 && 5<2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent); ;
            var render = await Excute(template);
            Assert.Equal("no", render);
        }

        /// <summary>
        /// 对象与算术混合判断
        /// </summary>
        [Fact]
        public async Task TestObjectAndArithmetic()
        {
            //v1 为空 false,5<2为false，整体结果 false || false 为false
            var templateContent = "$if(v1 || 5<2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<object>("v1",null);
            var render = await Excute(template);
            Assert.Equal("no", render);
        }

        /// <summary>
        /// 对象判断测试
        /// </summary>
        [Fact]
        public async Task TestIfObject()
        {
            //单个对象的判断原则类同于JS
            //即数字0为FALSE，否则为TRUE
            //字符串空或者NULL为FALSE，否则为TRUE
            //对象在为NULL时为FALSE，否则为TRUE
            //v1 为空 false,v2等于9，数字不等于0即为true,整体结果 false || true 为true
            var templateContent = "$if(v1 || v2)yes${else}no${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("v2",9);
            template.Set<object>("v1", null);
            var render = await Excute(template);
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 测试elif
        /// </summary>
        [Fact]
        public async Task TestElif()
        {
            var templateContent = "${if(3>5)}3>5${elif(2==2)}2=2${else}not${end}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list",new int[] { 7, 0, 2, 0, 6 });
            var render = await Excute(template);
            Assert.Equal("2=2", render);
        }
    }
}
