using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JinianNet.JNTemplate.Test
{
    [TestClass]
    public class TemplateTests
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestExpression()
        {
            var templateContent = "$set(n=868)\r\n${\"result:\"+n}";
            var template = new Template(templateContent);

            var render = template.Render();

            Assert.AreEqual("\r\nresult:868", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestExpression1()
        {
            var templateContent = "${8+2*5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("18", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestExpression2()
        {
            var templateContent = "${(8+2)*5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("50", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestExpression3()
        {
            var templateContent = "${4==5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("False", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestIf()
        {
            var templateContent = "${if(3==8)}3=8 success${elseif(3>8)}3>8 success${elseif(2<5)}2<5 success${else}null${end}";
            var template = new Template(templateContent);

            var render = template.Render();

            Assert.AreEqual("2<5 success", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestFor()
        {
            var templateContent = "${123}";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("123", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestForeach()
        {
            var templateContent = "$foreach(i in list)$i$end";
            var template = new Template(templateContent);
            template.Set("list", new int[]{7,0,2,0,6});
            var render = template.Render();
            Assert.AreEqual("70206", render);
        }

    }
  
}