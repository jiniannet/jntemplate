(window.webpackJsonp=window.webpackJsonp||[]).push([[16],{389:function(t,s,a){"use strict";a.r(s);var n=a(40),e=Object(n.a)({},(function(){var t=this,s=t.$createElement,a=t._self._c||s;return a("ContentSlotsDistributor",{attrs:{"slot-key":t.$parent.slotKey}},[a("h1",{attrs:{id:"自定义标签"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#自定义标签"}},[t._v("#")]),t._v(" 自定义标签")]),t._v(" "),a("p",[t._v("JNTemplate可以很方便的自定义标签，本文档假定您已经有一定的IL基本知识。自定义标签需要定义以下四个对象:")]),t._v(" "),a("ol",[a("li",[t._v("标签类，必须实现"),a("code",[t._v("ITag")]),t._v("接口。")]),t._v(" "),a("li",[t._v("标签解析委托"),a("code",[t._v("Func<TemplateParser, Nodes.TokenCollection, Nodes.ITag>")])]),t._v(" "),a("li",[t._v("标签编译委托"),a("code",[t._v("Func<ITag, CompileContext, System.Reflection.MethodInfo>")])]),t._v(" "),a("li",[t._v("标签类型推断委托"),a("code",[t._v("Func<ITag, CompileContext, Type> guessFunc)")])])]),t._v(" "),a("p",[t._v("下面将实现一个自定义标签，写法是"),a("code",[t._v("${:text}")]),t._v(",作用是直接输出冒号后的文本内容，根据该DEMO，让大家熟悉JNTemplate标签的自定义方流程。")]),t._v(" "),a("h2",{attrs:{id:"定义标签类"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#定义标签类"}},[t._v("#")]),t._v(" 定义标签类")]),t._v(" "),a("p",[t._v("定义一个类，继承自"),a("code",[t._v("ITag")]),t._v("，为了方便开发，建议同时继承自标签基类"),a("code",[t._v("Tag")]),t._v(",其它所需参数自己随意定义。")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("class")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("TestTag")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(":")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token type-list"}},[a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("Tag")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("ITag")])]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("/// <summary>")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("/// 定义一个属性Document存放冒号后的文本")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("/// </summary>")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token return-type class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")])]),t._v(" Document "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("get")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("set")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n")])])]),a("h2",{attrs:{id:"定义解析委托"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#定义解析委托"}},[t._v("#")]),t._v(" 定义解析委托")]),t._v(" "),a("p",[t._v("该委托将告诉引擎，如何解析你的标签。")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("Func"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("TemplateParser"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" Nodes"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("TokenCollection"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" Nodes"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("ITag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v(" action "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("p"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" tc"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=>")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//判断是否是我们自定义的TestTag")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("if")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("tc"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Count "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("==")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token number"}},[t._v("2")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("&&")]),t._v(" tc"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("First"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Text "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("==")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token string"}},[t._v('":"')]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n            "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("return")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("new")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token constructor-invocation class-name"}},[t._v("TestTag")]),t._v("\n            "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n                Document "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" tc"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("[")]),a("span",{pre:!0,attrs:{class:"token number"}},[t._v("1")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("]")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Text\n            "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//不是返回NULL")]),t._v("\n        "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("return")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("null")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n")])])]),a("div",{staticClass:"custom-block tip"},[a("p",{staticClass:"custom-block-title"},[t._v("TIP")]),t._v(" "),a("p",[t._v("如果当前集合不符合自定义标签条件，直接返回"),a("code",[t._v("null")]),t._v("即可。")])]),t._v(" "),a("h2",{attrs:{id:"定义编译委托"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#定义编译委托"}},[t._v("#")]),t._v(" 定义编译委托")]),t._v(" "),a("p",[t._v("该委托将告诉引擎，如何编译你定义的标签。该步骤要求有一定的IL知识。")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("Func"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("ITag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" CompileContext"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" MethodInfo"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v(" action1 "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("tag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" c"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=>")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" t "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" tag "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("as")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("TestTag")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" mb "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" c"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token generic-method"}},[a("span",{pre:!0,attrs:{class:"token function"}},[t._v("CreateReutrnMethod")]),a("span",{pre:!0,attrs:{class:"token generic class-name"}},[a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("TestTag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])])]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("typeof")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token type-expression class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")])]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//定义一个方法")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" il "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" mb"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("GetILGenerator")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//获取IL Generator")]),t._v("\n    il"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("Emit")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("OpCodes"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Ldstr"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" t"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Document"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//将Document以字符串引用推送到堆上")]),t._v("\n    il"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("Emit")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("OpCodes"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Ret"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//返回结果")]),t._v("\n    "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("return")]),t._v(" mb"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("GetBaseDefinition")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//生成方法")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n")])])]),a("h2",{attrs:{id:"定义推断委托"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#定义推断委托"}},[t._v("#")]),t._v(" 定义推断委托")]),t._v(" "),a("p",[t._v("该委托的作用是告诉引擎，该标签的返回结果是什么类型。")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[a("span",{pre:!0,attrs:{class:"token comment"}},[t._v("//TestTag 返回的是字符串类型，不需要做推断")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("Func"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("ITag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" CompileContext"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" Type"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v(" action2 "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("tag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" c"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=>")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("typeof")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token type-expression class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")])]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n")])])]),a("h2",{attrs:{id:"注册标签"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#注册标签"}},[t._v("#")]),t._v(" 注册标签")]),t._v(" "),a("p",[t._v("以上内容都定义好了，我们可以通过"),a("code",[t._v("Engine.Register<ITag>(...)")]),t._v("方法来注册我们定义的标签。")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[t._v("Engine"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token generic-method"}},[a("span",{pre:!0,attrs:{class:"token function"}},[t._v("Register")]),a("span",{pre:!0,attrs:{class:"token generic class-name"}},[a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("TestTag"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])])]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("action"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v("action1"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v("action2"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n")])])]),a("h2",{attrs:{id:"输出"}},[a("a",{staticClass:"header-anchor",attrs:{href:"#输出"}},[t._v("#")]),t._v(" 输出")]),t._v(" "),a("p",[t._v("到这一步，我们标签定义已经完成，我们可以运行下面的代码检测实际效果：")]),t._v(" "),a("div",{staticClass:"language-csharp extra-class"},[a("pre",{pre:!0,attrs:{class:"language-csharp"}},[a("code",[a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" templateContent "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" "),a("span",{pre:!0,attrs:{class:"token string"}},[t._v('"${:hello}"')]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" template "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" Engine"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("CreateTemplate")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("templateContent"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),a("span",{pre:!0,attrs:{class:"token class-name"}},[a("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("var")])]),t._v(" render "),a("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" template"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("Render")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n\nAssert"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),a("span",{pre:!0,attrs:{class:"token function"}},[t._v("Equal")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),a("span",{pre:!0,attrs:{class:"token string"}},[t._v('"hello"')]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" render"),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),a("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n")])])]),a("div",{staticClass:"custom-block warning"},[a("p",{staticClass:"custom-block-title"},[t._v("WARNING")]),t._v(" "),a("p",[t._v("注意：自定义标签为引擎的高级用法，如果您对引擎不够熟悉，我们不推荐您使用本功能，不正确的写法可能会影响整个引擎的运行速度。")])])])}),[],!1,null,null,null);s.default=e.exports}}]);