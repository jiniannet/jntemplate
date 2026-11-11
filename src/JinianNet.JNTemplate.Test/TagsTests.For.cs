using JinianNet.JNTemplate.Test.Model;
using System.Text;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 循环测试（FOR,FOREARCH）
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试FOR
        /// </summary>
        [Fact]
        public void TestFor()
        {
            var templateContent = "$for(i=1;i<4;i=i+1)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            TestCreateRunDoubleEngine(templateContent,
                "123");

        }

        /// <summary>
        /// for的不同写法
        /// </summary>
        [Fact]
        public void TestForPlusPlus()
        {
            var templateContent = "$for(i=0;i<3;i++)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            TestCreateRunDoubleEngine(templateContent,
                       "012");
        }

        /// <summary>
        /// 测试Forea 遍历对象时，Foreach应优先于for
        /// </summary>
        [Fact]
        public void TestForeach()
        {
            var templateContent = "$foreach(i in list)$i$end";
            TestCreateRunDoubleEngine(templateContent,
              "70206",
              template => template.Set("list", new int[] { 7, 0, 2, 0, 6 }));
        }

        /// <summary>
        /// 测试ForIn
        /// </summary>
        [Fact]
        public void TestForIn()
        {
            var templateContent = "$for(i in list)$i$end";

            TestCreateRunDoubleEngine(templateContent,
              "70206",
              template => template.Set("list", new int[] { 7, 0, 2, 0, 6 }));
        }

        /// <summary>
        /// 测试IEnumerable<T>
        /// </summary>
        [Fact]
        public void TestForInEnumerable()
        {
            var list = (IEnumerable<int>)new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7 });
            var templateContent = "$for(i in list)$i$end";
            TestCreateRunDoubleEngine(templateContent,
              "1234567",
              template => template.Set("list", list));
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestForInDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Rows.Add(7);
            dt.Rows.Add(0);
            dt.Rows.Add(2);
            dt.Rows.Add(0);
            dt.Rows.Add(6);
            var templateContent = "$for(row in list)${row[\"id\"]}$end";

            TestCreateRunDoubleEngine(templateContent,
              "70206",
              template => template.Set("list", dt));
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestForInDataTableRows()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Rows.Add(7);
            dt.Rows.Add(0);
            dt.Rows.Add(2);
            dt.Rows.Add(0);
            dt.Rows.Add(6);
            var templateContent = "$for(row in dt.Rows)${row[\"id\"]}$end";

            TestCreateRunDoubleEngine(templateContent,
              "70206",
              template => template.Set("dt", dt));
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestRepeatFor()
        {
            var templateContent = @"<ul>$for(i=1;i<4;i++)<li>$i</li>${end}</ul>";
            var render = "";
            var engine = CreateEngine(true);
            for (int j = 0; j < 3; j++)
            {
                var template = engine.CreateTemplate(templateContent);
                render += template.Render();
            }

            Assert.Equal("<ul><li>1</li><li>2</li><li>3</li></ul><ul><li>1</li><li>2</li><li>3</li></ul><ul><li>1</li><li>2</li><li>3</li></ul>", render);

        }




        /// <summary>
        /// 综合测试
        /// </summary>
        [Fact]
        public void TestForeachAndForeach()
        {
            var templateContent = @"
$foreach(model in list)
    <div>list:${model.Id}</div>
    <ul>
    $if(model.Id == 2)
        $foreach(m in getList(model.id))
            <li>$m</li>
        $end
    $end
    </ul>
$end
";
            var text = "<div>list:1</div><ul></ul><div>list:2</div><ul><li>a2</li><li>b2</li><li>c2</li></ul><div>list:3</div><ul></ul>";
            Action<ITemplate> action = template =>
            {
                template.Set("list", new Entity[] {
                    new Entity{
                        Id=1
                    },
                    new Entity {
                        Id=2
                    },
                    new  Entity{
                        Id=3
                    }
                });
                template.Set<Func<int, string[]>>("getList", (id) =>
                {
                    return new string[] { $"a{id}", $"b{id}", $"c{id}" };
                });
            };



            TestCreateRunDoubleEngine(templateContent,
                null,
                action,
                render => Assert.Equal(text, render.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "")));
        }

    }
}
