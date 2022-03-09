using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 算术测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {


        /// <summary>
        /// 测试计算表达式
        /// </summary>
        [Fact]
        public void TestExpression()
        {
            var templateContent = "${8+2*5}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("18", render);
        }

        /// <summary>
        /// 测试带负数的算术表达式
        /// </summary>
        [Fact]
        public void TestNegativeExpressio()
        {
            var templateContent = "${3000-1000--200-20}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();;
            Assert.Equal("2180", render);
        }


        
        /// <summary>
        /// 测试算术混合逻辑运算
        /// </summary>
        [Fact]
        public void TestMixLogic()
        {
            var templateContent = @"${10000 != 0 +  0 + 10000}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();;
            Assert.Equal("False", render);

        }


        /// <summary>
        /// 测试加减乘除四则运算及优先级处理
        /// </summary>
        [Fact]
        public void TestArithmetic()
        {
            var templateContent = @"${10000 * 2 + 2 *  4 * 10 / 8 - 24 + 0 + 0 + 0 + 0 * 1 * 2 * 3 * 4}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();;

            Assert.Equal("19986", render);

        }
        /// <summary>
        /// 测试复杂的计算表达式
        /// </summary>
        [Fact]
        public void TestComplicatedExpression()
        {
            var templateContent = "${(8+2)*(5+5) - ((2+8)/2)}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();;
            Assert.Equal("95", render);
        }

    }
}
