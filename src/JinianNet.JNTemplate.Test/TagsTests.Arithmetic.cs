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
            TestCreateRunDoubleEngine(templateContent, "18");
        }

        /// <summary>
        /// 测试带负数的算术表达式
        /// </summary>
        [Fact]
        public void TestNegativeExpressio()
        {
            var templateContent = "${3000-1000--200-20}";
            TestCreateRunDoubleEngine(templateContent, "2180");
        }



        /// <summary>
        /// 测试算术混合逻辑运算
        /// </summary>
        [Fact]
        public void TestMixLogic()
        {
            var templateContent = @"${10000 != 0 +  0 + 10000}";
            TestCreateRunDoubleEngine(templateContent, "False");

        }


        /// <summary>
        /// 测试加减乘除四则运算及优先级处理
        /// </summary>
        [Fact]
        public void TestArithmetic()
        {
            var templateContent = @"${10000 * 2 + 2 *  4 * 10 / 8 - 24 + 0 + 0 + 0 + 0 * 1 * 2 * 3 * 4}";
            TestCreateRunDoubleEngine(templateContent, "19986");

        }
        /// <summary>
        /// 测试复杂的计算表达式
        /// </summary>
        [Fact]
        public void TestComplicatedExpression()
        {
            var templateContent = "${(8+2)*(5+5) - ((2+8)/2)}";
            TestCreateRunDoubleEngine(templateContent, "95");
        }

    }
}
