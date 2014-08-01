[JNTemplate](http://www.jiniannet.com/) — Open source templating engine for .net!
==================================================

JNTemplate 是什么
--------------------------------------

极念模板引擎(JNTemplate)是一款.NET平台的解释型模板引擎，它允许用户使用简单的模板语言来定义页面及引用后台设置的对象,让前端展示与后端代码逻辑分离，同时，它也提供了一个在WebForm与Razor之外的选择！

JNTemplate具有简单，易用，高效等特点，拓展十分方便！而且JNTemplate是基于apache license 2.0协议开源的，该协议鼓励代码共享和尊重原作者的著作权，同样允许代码修改，再发布（作为开源或商业软件）, 但是必须保留作者署名。


为什么要使用JNTemplate
--------------------------------------
1. 简单易用 任何略懂 c#/java/javascript 语法的用户只需要10分钟就可以上手使用
2. 轻便高效 使用了更高效的解析机制，让引擎执行更快。
3. 自由拓展 可以十分方便的自由扩展功能，灵活性更强。
4. 免费开源 可以免费使用而无须支付任何费用，只需要在代码中保留我们的署名即可。


如何使用JNTemplate
--------------------------------------
```bash
JinianNet.JNTemplate.Template template = new JinianNet.JNTemplate.Template("hello,$name!");
template.Context.TempData["name"] = "world";
template.Render(Response.Output);
```


如何下载JNTemplate
--------------------------------------
1. [jntemplate v1.2 beta](http://down.jiniannet.com/jntemplate-v1.2-beta(1.2.0.4).zip)
2. [jntemplate v1.1](http://down.jiniannet.com/jntemplate-v1.1(1.1.34).zip)
3. [百度网盘下载](http://pan.baidu.com/s/1jGigCpo#dir/path=%2FJNTemplate%2FDLL)
4. 克隆源代码自行生成

```bash
git clone git://github.com/jquery/jquery.git
```

JNTemplate帮助
--------------------------------------
1. 开源协议：[https://github.com/jiniannet/jntemplate/blob/master/License.txt](https://github.com/jiniannet/jntemplate/blob/master/License.txt)
2. 开源主页：[https://github.com/jiniannet/jntemplate/](https://github.com/jiniannet/jntemplate/)
3. 官方网站：[http://www.jiniannet.com](http://www.jiniannet.com)

### 联系方式：
1. Email:i@jiniannet.com
2. 交流Q群:5089240 欢迎加入