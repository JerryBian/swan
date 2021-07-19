最近一直在学习 WPF，好像这方面没有特别经典的书籍可以参考，光看 MSDN 又显得太过乏味。纵观 StackOverflow 上的推荐，我选择了这本 [《WPF 4 Unleashed》](https://www.amazon.com/WPF-4-Unleashed-Adam-Nathan/dp/0672331195) 书作为入门书籍，这本书的作者 [Adam Nathan](https://en.wikipedia.org/wiki/Adam_Nathan) 是微软的一名软件架构师。由于是阅读时的笔记，所以内容上可能有不严谨的地方，这在今后会做一些改进。

### Chapter 1：Why WPF, and What About Silverlight?

1\. 

WPF: Windows Presentation Foundation

2\.

WPF中的亮点：

*   大一统似的集成各种技术（Broad integration）：3D, video, speech, and rich document viewing...
*   分辨率无关（Resolution independence）：由于在矢量图形上的技术增强，WPF 程序可以不受分辨率或 DPI 改变的影响。
*   利用硬件加速（Hardware acceleration）：WPF 建立在 Direct3D 之上，所以 WPF 中的内容，不管是2D，3D，图形，还是文本，都会被转换为3D对象，充分利用硬件优势，开启 GPU 加速。
*   声明式编程（Declarative programming）：XAML。
*   丰富多样的组件和可定制化（Rich composition and customization）：要想实现很炫的动作，仅需写很少的代码，甚至不用写任何代码。

3\.

技术上来说，WPF 能够做的事情，通过之前的技术都是可以实现的。然而，由于 WPF 集成了这些技术，所以节省了很多时间和精力，开发效率大幅提高。

4\.

WPF 第一个版本是 WPF 3.0，是随着 .NET Framework 3.0 一起在 2006 年发布的。WPF 3.5 在 2007 年发布。WPF 3.5 SP1 是随着 .NET 3.5 SP1 一起在 2008 年 8 月发布的。最新版本是 WPF 4，随着 .NET Framework 4.0 一起在 2010 年发布。当然，由于本书编撰时间的原因，其实最新版本 WPF 4.5 已经发布了（[http://msdn.microsoft.com/en-us/library/bb613588.aspx](http://msdn.microsoft.com/en-us/library/bb613588.aspx)）。<del>WPF各个版本之间的差异，将单独成文。</del>

5\.

Silverlight 和 WPF 的关系：

*   Silverlight 是面向富 Web 场景的一个轻量级的 .NET Framework 版本，与之类似的是 Adobe Flash 和 Flex。
*   Silverlight 沿袭 WPF 的方式创建用户界面，并没有创建额外的技术。首先发布于 2007 年。
*   Silverlight 基本上是 WPF 的子集加上 .NET Framework 的一些基础类库（core data types, collection classes, and so on）。每一个版本都包含更多 WPF 的更多功能，但是到了最后 Silverlight 中也包含很多WPF中没有的功能。简而言之，师从 WPF，也同时自己在不断超越。

### Chapter 2：XAML Demystified

1\.

XAML 是 XML 的一种方言（dialect）。

2\.

在 WPF 和 Silverlight 中，XAML 主要用来描述用户界面（User ，interfaces），当然也描述其他东西。在 WF(Windows Workflow Foundation) 和 WCF(Windows Communication Foundation) 中，XAML 主要是用来描述活动（activities）和配置（configurations），跟用户界面不搭嘎。

3\.

XAML 的重点是使得程序员能够跟其他领域里面的专家能够更好的协同工作。在 WPF 和 Silverlight 中这些专家主要是指图形界面的设计师（graphic designers）。

4\.

基于下面的原因，即使你没有计划与图形界面设计师协同工作，你也应该学好 XAML：

*   XAML 是描述用户界面或者其他层次对象(other hierarchies of objects)的一种非常简洁的途径。
*   XAML 的使用使得前端呈现和后台逻辑分离开来，这样对于团队而言非常容易维护。
*   XAML 可以很方便的被粘贴到开发工具中，比如 Visual Studio, Expression Blend 或者其他的一些小的独立工具中，不用编译便可以看到呈现结果。
*   几乎所有的 WPF 相关的工作都采用 XAML 这门语言。

5\.

运行 `.xaml` 程序有以下几种途径：

*   直接在浏览器(IE, firefox .etc)中打开 `.xaml` 文件，当然前提是你安装了相应版本的 .NET Framework。
*   利用 XAMLPAD 2009 和 Kaxaml 这些轻量级的工具。
*   利用 Visual Studio。

早期 Windows SDK 直接内置了 XamlPad，后来没有再集成了，因为缺少相应的资源（人手，计划安排等等），幸好还有其他可选的轻量级工具：XAMLPAD2009, Kaxaml(WPF团队前成员Robby Ingebretsen开发), XamlPadX(WPF团队目前成员Lester Lobo开发), XAML Cruncher等等。

6\.

XAML 就是 XML，只不过附加了一些规则：元素，属性以及它们映射到对象，属性和它们属性的值。

XAML 只是个 .NET APIs 的技术而已，所以不应该与 HTML, SVG 以及其它 domain-specific formats/languages 进行类比。

XAML 依附于其它技术，所以应该这么称呼： WPF XAML, Silverlight XAML...脱离了这些附体毫无意义。

7\.

相关的 XAML 规范可以在这里下载PDF文档：

*   XAML Object Mapping Specification 2006 (MS-XAML): [http://go.microsoft.com/fwlink/?LinkId=130721](http://go.microsoft.com/fwlink/?LinkId=130721)
*   WPF XAML Vocabulary Specification 2006 (MS-WPFXV): [http://go.microsoft.com/fwlink/?LinkId=130722](http://go.microsoft.com/fwlink/?LinkId=130722)
*   Silverlight XAML Vocabulary Specification 2008 (MS-SLXV): [http://go.microsoft.com/fwlink/?LinkId=130707](http://go.microsoft.com/fwlink/?LinkId=130707)

8\.

XAML 中可以办到的事情，后台代码并不是全部都可以办到，主要有以下三点 <del>（这点将在后面单独成文）</del>：

*   创建完整范围的模板（Creating the full range of tmplates）。后台代码可以使用`FrameworkElementFactory` 来创建模板，但是这个方法是有所限制的。
*   可以在XAML中使用 `x:Shared="False"` 来使得WPF中元素引用一个资源字典（resource dictionary）的时候每次都会产生一个实例。这点在后台代码中无法做到。
*   资源字典中的项延迟实例化(Deferred instantiation)。这点是很重要的性能优化途径，只能通过编译XAML来实现。

9\.

XAML 和后台代码一样，也是大小写敏感的语言。

10\.

下面两段代码是完全等价的：
```xml
<Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Content="OK" Click="button_Click" />
```

```cs
System.Windows.Controls.Button b = new System.Windows.Controls.Button();
b.Click += new System.Windows.RoutedEventHandler(button_Click);
b.Content = "OK";
```

11\.

XAML 中声明的元素所映射的对象在运行时中：事件总是在属性之前处理，就如上面代码顺序一样。事实上即使像 `Name` 这样的属性(在对象初始化之后第一个被设置)也在事件绑定之后。这么做的原因是可以确保相关属性设置的时候会触发相关事件得到正确执行，这样我们就可以不用考虑属性设置的顺序的问题了。各个属性之间或者各个事件之间的顺序是按照在XAML中声明的顺序来进行的，这么做是没有任何影响的，因为“.NET设计规范”中指明：**一个类中属性的声明顺序以及附加事件的顺序是没有关系的**。

12\.

在 XAML 根节点的元素中，至少有一个 XML 命名空间来约束它自身和它的子元素。你可以自己给元素使用一个命名空间，但是必须使用不同的后缀来加以区别，比如：`xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"` 上述这个命名空间是映射到 `System.Windows.Markup` 命名空间，当然也添加了一些额外的规范给编译器或者解释器。需要注意的是，这些规范指明了 XAML 中元素各种属性，这与后台代码中的属性名称并不是完全一样的，换言之这仅仅相当于 XAML 这门语言中的关键字。

13\.

一般情况下，使用 WPF XML 命名空间 `“http://schemas.microsoft.com/winfx/2006/xaml”` 作为默认的命名空间，以及使用 XAML 语言命名空间 `“http://schemas.microsoft.com/winfx/2006/xaml”` 作为第二命名空间（当然需要加上前缀 `x`），这是一种约定俗成，就像 C# 代码都需要使用 `using System;` 一样。

截止WPF 4.0，已经有三种 WPF XML 命名空间：

*   [http://schemas.microsoft.com/winfx/2006/xaml/presentation](http://schemas.microsoft.com/winfx/2006/xaml/presentation) 这个是 WPF 3.0 的默认命名空间。
*   [http://schemas.microsoft.com/netfx/2007/xaml/presentation](http://schemas.microsoft.com/netfx/2007/xaml/presentation) 这个是 WPF 3.5 定义的命名空间。
*   [http://schemas.microsoft.com/netfx/2009/xaml/presentation](http://schemas.microsoft.com/netfx/2009/xaml/presentation) 这个是 WPF 4.0 定义的命名空间。

那么在实际中呢，推荐使用 WPF 3.0 的命名空间。因为，winfx 不仅仅包括 WPF XAML，还有 Silverlight XAML，WCF XAML 以及 WF XAML。而且这个最初版本的命名空间在各种WPF版本中都是可以生效的，其他版本的命名空间只能在集成它的WPF版本或者更高版本上才能使用。

当松散的 XAML 文件在浏览器中加载时，会运行 `PresentationHost.exe` 文件来根据命名空间来检测应该运行哪一个 .NET Framework 版本。如果是 netfx/2009 则运行 .NET Framework 4.0 否则选用 .NET Framework 3.x。

14\.

WPF 为一些常见的类型提供了类型转换（Type Converters）：`Brush`，`Color`，`FontWeight`，`Point` 等等。它们都是一些集成 `TypeConverter` 的类，比如 `ColorConverter`、`BrushConverter` 等等。你也可以为自定义的数据类型定制一个类型转换。需要注意的是，类型转换不是大小写敏感的，这与 XAML 中的关键字不一样。

实际上，编译器在处理XAML中需要类型转换的代码时声称如下的代码，这与后台实现同样的效果是不一样的：

```cs
System.Windows.Controls.Button b=new System.Controls.Button();
b.Content="OK";
b.Background=(Brush)System.CompontentModel.TypeDescriptor.GetConverter(typeof(Brush)).COnvertFromInvariantString(“White”);
```

当写错 `White` 的时候，在编译期是不会报错的，但是会在运行时抛出异常。但是借助于 Visual Studio，写错时编译期就会报错。

15\.

标记扩展(markup extensions)像类型转换一样，使得你可以扩展XAML语法。这两者都可以在运行时解析一个 `string` 类型的属性成为一个合适的对象，当然有些内置的标记扩展，出于性能的考虑是在编译期执行的。

标记扩展是从 XAML 中显式并且固定的语法中被调用的，所以如果要扩展 XAML 语法的话，标记扩展应该是首选的方法。另外，使用标记扩展可以克服你使用类型转换时没有能力转换成功的场景。

当一个属性值是写在 `{}` 中时，XAML的解析器就会把它当做标记扩展值，而不是单单一个字面量或者是一个需要类型转换的东西。比如接下来的一段代码：

```xml
<Button Background="{x:Null}" Height="{x:Static SystemParameters.IconHeight}" Content="{Binding Path=Height,RelativeSource={RelativeSource Mode=Self}}"></Button>
```

写在 `{}` 最前面的标识符比如 `x:null` `Binding` 等等都是继承至 `MarkupExtension` 类的一个子类`NullExtension`, `StaticExtension` 等等，他们都是属于 `System.Windows.Markup` 命名空间，所以前面要加上 `x`。然而，`binding` 则属于 `System.Windows.Data` 命名空间，这已经包含在 WPF XAML 默认命名空间了，所以不需要前缀 `x`。

上面代码中 `SystemParameters.IconHeight` 相当于类的初始化成员参数，`RelativeSource={RelativeSource Mode=Self}` 则相当于类的属性设置。很明显，这个属性里面的值同样的使用了一个扩展标记。

16\.

需要注意的是，如果上述 `button` 中的Content内容就是 `{Binding Path=Height,RelativeSource={RelativeSource Mode=Self}}` 这个，那么可以在前面加上空的 `{}` 进行转义：

```xml
<Button Background="{x:Null}" Height="{x:Static SystemParameters.IconHeight}" Content="{}{Binding Path=Height,RelativeSource={RelativeSource Mode=Self}}"></Button>
```

或者可以这么写：

```xml
<Button Background="{x:Null}" Height="{x:Static SystemParameters.IconHeight}">
    <Button.Content>
        {Binding Path=Height,RelativeSource={RelativeSource Mode=Self}}
    </Button.Content>
</Button>
```

17\.

WPF XAML 的默认命名空间 “http://schemas.microsoft.com/winfx/2006/xaml/presentation” 是以下 .NET 命名空间的一个集合：

```cs
System.Windows 
System.Windows.Automation 
System.Windows.Controls 
System.Controls.Primitives 
System.Windows.Data 
System.Windows.Documents 
System.Windows.Forms.Integration 
System.Windows.Ink 
System.Windows.Input 
System.Windows.Media 
System.Windows.Media.Animation 
System.Windows.Media.Effects 
System.Windows.Media.Imaging 
System.Windows.Media.Media3D 
System.Windows.Media.TextFormatting 
System.Windows.Navigation 
System.Windows.Shapes 
System.Windows.Shell
```

18\.

就像 XML 一样，XAML 必须有一个根节点元素。一个对象元素（object element）可以有三种类型的子元素：

- 一个代表内容属性的值（a value for a content property），
- 集合项（collection items），
- 一个可以被类型转换为对象元素的值（a value can be type-converted to the object element）。

19\.

WPF 中的 `Content` Property可以是任意的对象：

```xml
<button>Hello World!</button> 
<button>
    <button.content>Hello World!</button.content> 
</button>
<button>
    <rectangle width="120" height="120" fill="Red"></rectangle>
</button>
```

并不是说内容属性的名称就一定是 `Content`，比如 `ComboBox`，`ListBox` 以及 `TabControl` 的内容属性就是 `Items`。 

20\. 

XAML 允许有两种将项添加到集合中的途径：列表(Lists)和字典(Dictionaries)。 

列表是指这个集合实现了 `System.Collections.IList` 接口，比如 `System.Collections.ArrayList` 或者 WPF 定义的其他许多集合。 当然，由于 `Items` 属于内容属性，如上面一样是可以省略不写的。 

字典主要是指 `ResourceDictionary`，它实现了 `System.Collections.IDictionary` 接口，类似于操作一个哈希表（Hashtable），以键值对的形式增添索引等等。 

```xml
<stackpanel.resources>
    <resourcedictionary></resourcedictionary>
</stackpanel.resources>
```

需要注意的是，这里面讨论的 XAML 都是指 XAML 2006，所以 Key 值必须是一个字符串。因为 XAML 2006 默认不会帮你进行类型转换，这点在 XAML 2009 中有所改变。 当然还有类型转换，这点在前面已经讨论过，不予赘述。

21\. 

新版 Windows（Windows 8）中app开发已经可以使用 XAML 来进行布局，实际上 XAML 还被广泛应用于 Windows Phone 开发。所以，对于 XAML 的深入理解和灵活运用是 Windows 平台程序员必须的一件事情。

<del>上述所有特性都默认是XAML 2006，关于XAML 2009以及其它一些没有涉及到的特性都将在后面单独成文。 </del>

### Chapter 3：WPF Foundations 

1\. 

WPF中的核心类（共12个）： Core Classes form Foundaments of WPF 下面分别做以解释： 

**`Object`**:这个类没啥好说的，所有.NET类的基类，也是唯一一个并不属于 WPF Feature 的类。 

**`DispatcherObject`**:这个基类作用于那些“只希望从创建它们的线程上访问它们”的对象（the base class meant for any object that wishes to be accessed only on the thread that created it）。大部分 WPF 类都是继承自 `DispatcherObject`，所以它们都是线程不安全的。名字中的 `Dispatcher` 是来自于类似于 `Win32` 的讯息回圈（message loop）的 WPF 版本。 

**`DependencyObject`**:所有支持依赖属性的对象的基类。 

**`Freezable`**:有些对象出于性能考虑的原因，会被 frozen 成一个只读（read-only）状态的对象，这样它们可以在多线程状况下进行共享，区别于其他继承自 `DispatcherObject` 的对象。而且，它们是不可以 `unfrozen` 的，但是你可以创建它们已经 unfrozen 的副本对象。通常而言，这个类都是指图形元素（graphics primitives），比如 `Brushes`, `Pens` , and `Geometries` or `Animation` 类。 

**`Visual`**:它是那些有着 2D 虚拟外观的对象的基类。 

**`UIElement`**:它是那些拥有 “routes events, command binding, layout, and focus” 特性的2D虚拟外观对象的基类。 

**`FrameworkElement`**:这个基类增加了 “styles, data binding, reources” 以及其它的一些基于 Windows 的控件（比如 `tooltips` 和 `context menus`）常用机制的属性。 

**`Control`**:常见的控件比如 `Button`, `ListBox` 和 `StatusBar` 等等的基类。在 FrameworkElement 的基础上 `Control` 加了很多新的属性，比如 `Foreground`, `Background` 以及 `FontSize` 等等。而且它还包括完全重写这些样式的能力。 

**`Visual3D`**:它是那些自身拥有 3D 虚拟外观的对象的基类。 

**`UIElement3D`**:它是那些拥有 “routes events, command binding, and focus” 特性的 3D 虚拟外观对象的基类。 

**`ContentElement`**:与 `UIElement` 共享同样的 API 名称，行为以及 API 的内部实现等等，但是它们直接集成的基类并不相同。它并不能单独呈现行为，而是要内嵌在一个“继承自 Visual  ”的类中才能在屏幕上呈现（而`UIElement` 正好符合要求，所以它们才会极其的相似）。每一个 `ContentElement` 通常需要多个 `Visuals` 才能正确呈现（spanning lines, columns and pages）。 

**`FrameworkContentElement`**: `FrameworkElement` 的 `Content` 版本。 WPF 中的元素都是继承自`UIElement`/`FrameworkElement` 或者 `ContentElement`/`FrameworkContentElement`。而继承自 `UIElement` 还是`FrameworkElement`，或者继承自 `ContentElement` 还是 `FrameworkContentElement` 这两者之间的区别并不重要，因为 `UIElement` 的直接公共子类只有 `FrameworkElement`，`ContentElement` 的公共子类只有 `FrameworkContentElement`。 

2\. 

逻辑树 (logical tree) 是很重要的，比如 WPF 中的属性/资源设置，事件调用等都是沿着这个逻辑树向上/向下进行的。逻辑树不一定是 XAML 生成的，后台 C# 代码也可以生成逻辑树。 

视觉树(visual tree)可以看做的逻辑树的一个展开，相当于对于每一个逻辑树节点再次进行分解。当然，并不是每一个逻辑树节点都会在视觉树中出现，只有那些继承于 `System.Windows.Media.Visual和System.Windows.Media.Visual3D` 的节点对象才能出现。所以包括 `string` 类型的逻辑节点都不应该在视觉树中呈现，因为它们不能单独呈现在屏幕上。可以通过 `XamlPadX` 来查看视觉树。除非需要彻底重新设计控件或者进行一些低级别 (low-level) 的绘图工作时候需要考虑视觉树，其他时间都不需考虑。 

逻辑树一般而言除非用代码约束，否则是不会改变的，比如你不可能无端的往里面添加删除一个节点。而视觉树则根据用户使用的环境不同，比如 Windows XP 和 Windows 8 就不会完全一样。所以，应该尽量避免依赖于视觉树的代码。 可以利用 `System.Windows.LogicalTreeHelper` 和 `System.Windows.Media.VisualTreeHelper` 辅助类来操作逻辑树和视觉树。 

3\. 

WPF 内置了一种新类型的属性——依赖属性（dependence property），这种特性被广泛应用于 WPF 中的 Styling , 自动数据绑定(automatic data binding), 动画（animation）以及其它的技术。依赖属性最大的特点是它内置的“变更通知（change notification）”能力。 

4\. 

`Button` 有 111 个公共属性，其中 98 个是从 `Control` 或者它的基类，其中 89 个是依赖属性。

`Label` 有 104 个公共属性，其中 82 个是依赖属性。 

5\. 

一个依赖属性实现细节的演示： 

```cs
public class CustomButton : ButtonBase
{
    // the dependence property
    public static readonly DependencyProperty IsDefaultProperty;

    static CustomButton()
    {
        //Register the property
        CustomButton.IsDefaultProperty = DependencyProperty.Register("IsDefault", typeof(bool), typeof(CustomButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDefaultChanged)));
        ...
    }

    // a .NET property wrapper (optional)
    public bool IsDefault
    {
        get
        {
            return (bool)GetValue(CustomButton.IsDefaultProperty);
        }
        set
        {
            SetValue(CustomButton.IsDefaultProperty, value);
        }
    }

    //a property changed callback (optional)
    private static void OnIsDefaultChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ...
    }
}
```

根据设计规范，依赖属性（即是上述代码的 `IsDefaultProperty`）以 `public static` 修饰，并且加以后缀 `Property`。这么做是因为有些内部机制需要你遵循这样的规范，比如定位工具（localization tools）、加载 XAML（XAML loading）等等。 

依赖属性调用 `DependenceProperty.Register` 方法来创建。接下来，一个传统的 .NET 属性包装器 (property wrapper) 调用 `GetValue` 和 `SetValue` 方法，然而这是可选的，也就是说不一定需要实现。但是手动定义这个包装器一方面可以使代码更具可读性，调理清晰，更重要的是只有定义了这个属性包装器，你才能在 XAML 代码中设置这个属性。

这个传统的属性包装器格式是固定的，它只有在编译期时会用到，在运行时是会绕过这段代码，直接执行底层的 `GetValue` 和 `SetValue` 方法，所以不能在自己定义的这个属性包装器中写任何逻辑性的代码，因为这样在运行时会被忽略。

Visual Studio 的 Code Snippet 中有定义依赖属性这段代码片段，可以省去编码的痛苦。 

6\. 

由于依赖属性是一个静态字段（static field），所以可以很节省内存空间，这样 `Button`、`Label` 等有很多的依赖属性也不会消耗太大的内存资源。 

7\. 

依赖属性特点之一：变更通知（change notification） 

```xml
<button content="BUTTON">
    <button.style>
        <style targettype="Button">
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="true">
                    <Setter Property="Background" Value="Red"></Setter>
                </Trigger>
            </Style.Triggers>
        </style>
    </button.style>
</button>
```

上述代码实现了“鼠标移到 `Button` 上面 `Button` 背景变红色，移开恢复”的功能。这便是属性触发器（property trigger）技术的使用示例，不能够直接给 `Button` 添加触发器，必须写在 `Style` 中。需要注意的是属性触发器只能作用于依赖属性。数据触发器（data trigger）则适用于所有 .NET 属性。

还有一种触发器——事件触发器（event trigger）一般用于动画声音等等路由事件被触发的情况之下。 

8\. 

依赖属性特点之二：属性值继承（property value inheritance） 这一点在之前的逻辑树中可以体现出来，根节点设置的依赖属性会被自动附加到子节点之上。但是这只是一种尝试而已，并不是说根节点的属性一定会被子节点采用，主要基于以下两个原因会失败： 

- 有些依赖属性会选择是否执行这种继承关系：通过设置 `DependenceProperty.Register` 方法中的 `FrameworkPropertyMetadataOptions.Inherits` 标记。 
- 因为设置属性值时存在优先级，所以根节点的属性有可能因为优先级别较低会被忽略。 

9\. 

那么 WPF 是如何最终设置一个属性的值呢？一般经过下面的步骤。

10\. 

**Step 1: 选择最原始的值**

这一步骤中，WPF 会根据多个数据源的优先级选择优先级最高的那一个： 

属性系统强制转换（property system coercion）: 严格来讲，这一步并不包含在 Step 1 中（在 Step 4 中），这里列出来只是为了使得这个优先级列表完整。 

活动动画或具有Hold行为的动画（Active animations, or animations with a Hold behavior）：同样的这一步将在 Step 3 中生效。 

本地值（local value）：这一步骤中，本地值拥有最高优先级。相当于直接在 XAML 中设值或者使用实例的属性调用`SetValue` API，也包括绑定或资源。 

`TemplatedParent` 模板属性。如果元素是作为模板（`ControlTemplate` 或 `DataTemplate`）的一部分创建的，则具有 `TemplatedParent`。

在模板中，按以下优先级顺序应用： 

- 来自 `TemplatedParent` 模板的触发器 
- `TemplatedParent` 模板中的属性集 

隐式样式。只应用于 `Style` 属性。`Style` 属性是由任何样式资源通过与其类型匹配的键来填充的。该样式资源必须存在于页面或应用程序中，查找隐式样式资源不会进入到主题中。 

样式触发器。来自页面或应用程序上的样式中的触发器。 

模板触发器。来自样式中的模板或者直接应用的模板的任何触发器。 

样式 Setter。来自页面或应用程序的样式中的Setter的值。 

默认（主题）样式。在默认样式中，按以下优先级顺序应用： 

- 主题样式中的活动触发器 
- 主题样式中的 Setter 

继承。有几个依赖项属性从父元素像子元素继承值，因此不需要在应用程序中的每个元素上专门设置这些属性。 

来自依赖项属性元数据的默认值。任何给定的依赖属性都具有一个默认值。因为继承是在默认值之前检查的，所以对于继承的属性，父元素的默认值优先于子元素。 

可以通过 `DependencyPropertyHelper.GetValueSource` 辅助方法来调试检查属性值的来源，该方法只能用于辅助调试，有可能在更高版本中产用副作用： 

```cs
ValueSource vs = DependencyPropertyHelper.GetValueSource(btn, Button.ContentProperty); 
if (vs.BaseValueSource == BaseValueSource.Local) { } 
```

还可以通过空间的 `ClearValue` 方法来清空某一个属性的值： `btn.ClearValue(Button.ContentProperty); `。

11\. 

**Step 2:评估**

这一步主要是针对Step 1中的值如果是一个表达式(Expression)的话，WPF会做一定的工作将表达式准换为更具体的结果。表达式最主要出现在数据绑定(data binding)中。 

12\. 

**Step 3:动画应用**

如果一个或多个动画正在运行，而且它们有会修改或完全替换当前的值，它就有相当高的优先级会对前一步的结果进行修改替换，包括本地值。 

13\. 

**Step 4:系统强制转换**

进行到这一步的时候，系统会将前面所得到的结果传给 `CoerceValueCallback` 委托（如果有的话），基于自定义的一些逻辑返回新的值。比如 `ProgressBar` 控件会在这一步检查值是否介于 `Minimun` 和 `Maximun` 之间，并做相应处理。 

14\. 

**Step 5:验证**

这一步是将上一步的值传给 `ValidateValueCallback` 委托（如果有的话），验证通过则返回 `true`。否则返回`false`，并终止整个处理过程。 

15\. 

依赖属性特点之三：附加属性（Attached Properties） 

```xml
<StackPanel TextElement.FontSize="20">
    <Button Content="BUTTON" Name="btn"></Button>
</StackPanel>
```

其中 `StackPanel` 中的 `TextElement.FontSize` 就是附加属性，因为 `StackPanle` 本身并没有字体相关的属性。这里面的 `TextElement` 充当着依赖属性源（attached property provider）的身份。 

### Chapter 4: Sizing, Positioning, and Transforming Elements 

1\. 

<del>the main child layout properties</del>

2\. 

WPF 元素可以通过设置它的 `SizeToContent` 属性，以使得它的大小足以容得下它的内容，但是又没有多余的部分。这个属性需要手动设置，默认值为 `false`。 

3\. 

`Framework` 元素都有 `Width`, `Height` 属性，当然也有 `MinWidth`, `MinHeight`, `MaxWidth`, `MaxHeight` 属性，他们都是 `double` 类型。

默认状态下，`MinWidth` 和 `MinHeight` 值为 `0`，`MaxWidth` 和 `MaxHeight` 值为 `Double.PositiveInfinity`(在 XAML 中可以设值为 `Infinity`)。特定情况之外，一般我们都会避免显式的给元素设置尺寸等信息，我们会充分利用 panels 元素的特征。 

4\. 

对于一些复杂情况下，`FrameworkElement` 元素还有其他一些跟尺寸相关的属性，它们都是只读的。

`DesiredSize`: 从 `UIElement` 继承而来。这个属性的值在布局过程中基于前面的 `Width` 等属性通过计算而来，在 panels 内部使用。 

`RenderSize`: 从 `UIElement` 继承而来。这个元素代表了布局完成后元素的最终尺寸。由于每次布局改变的时候这个值都会改变，所以不应该在编码过程中依赖这个值，唯一例外的场景是在 `UpdateLayout` 这个事件中，但是由于性能的因素，你同样应该避免使用这个事件。 

`ActualWidth`, `ActualHeight`: 这两个属性与上面的 `RenderSize.Width` 和 `RenderSize.Height` 完全等价。M$纯粹蛋疼了。 

5\. 

`Margin` 和 `Padding` 的效果可以类比于 CSS 中的效果。在 WPF 中，所有 Framework 元素都有 `Margin` 属性，所有 `Control` 元素（包括 `Border`）都有 `Padding` 属性。它们的类型都是 `System.Windows.Thickness`，这个类型可以表示一个、两个或者四个 `double` 类型的值。 由于类型转换的缘故，在 XAML 中有很多简写方式，但是在 C# 中则必须写全，主要体现在两个值的时候： 

```xml
<Label Name="myLabel1" Margin="12" Padding="12">Ruby</Label>
<Label Name="myLabel2" Margin="10,5" Padding="10,5">Lua</Label>
<Label Name="myLabel3" Margin="4,5,6,7" Padding="4,5,6,7">JavaScript</Label>
```

```cs
myLabel1.Margin = new Thickness(12);// same with Padding
myLabel2.Margin = new Thickness(10, 5, 10, 5);// same with Padding
myLabel3.Margin = new Thickness(4, 5, 6, 7);// same with Padding
```
赋值的顺序为：`Left`, `Top`, `Right`, `Bottom`。 

6\. 

WPF 中，元素尺寸的度量单位有: `cm`, `pt`, `in`和 `px`。其中，`px` 是默认单位。这些度量单位都是设备无关的(device-independent)，或者叫 PI independent。 

7\. 

`UIElement` 的 `Visibility` 属性不是 `bool` 类型的，而是一个枚举类型：`Visible`, `Collapsed`, `Hidden`。其中 `Collapsed` 是指这个元素隐藏且不占空间；`Hidden` 是指这个元素隐藏且占空间，这就是所谓的“尸位素餐”。 

8\. 

`HorizontalAlignment` 和 `VerticalAlignment` 属性使得这个元素可以控制自身如何利用 panel 给它分配的空间。 

`HorizontalAlignment`: `Left`, `Center`, `Right`, `Stretch`。 

`VerticalAlignment`: `Top`, `Center`, `Bottom`, `Stretch`。 

`Stretch` 是两者的默认值，但是大量的控件都重写了这个默认值。这两个属性仅在父 panel 给这个子元素多余空间的时候有用。 

9\. 

与上述两个 `Alignment` 相对的是 Content Alignment: `HorizontalContentAlignment` 和 `VerticalContentAlignment`。这两者的关系就相当于 `Margin` 和 `Padding`。

同样的 Content Alignment 同样有上述对应的四个枚举值。只不过，`HorizontalContentAlignment` 的默认值是 `Left`， `VerticalContentAlignment` 的默认值是 `Top`。 

尤其需要注意的是，由于很多控件都对相关值进行了重写，所以具体情况需要具体分析才行。 

10\. `FlowDirection` 是 `FrameworkElement` 的一个控制内容流方向的一个属性，有 `LeftToRight` 和 `RightToLeft` 两个枚举值。需要注意的是，“I am Chinese”这行字即使是 `RightToLeft` 依然是 ”I am Chinese”，只不过字都偏在右边而已。这个属性在本地化中经常被使用，譬如阿拉伯语。 

11\. 

WPF 有一些内置的 2D 变换的类，继承于 `System.Windows.Media.Transform`。所有的 `FrameworkElement` 元素都有两种类型的变换： 

`LayoutTransform`: 在元素呈现之前进行变换，所以接下来呈现的元素只能排在它的后面。 

`RenderTransform`: 在所有元素呈现完成后进行变换，所以由于接下来的元素已经呈现完毕了，该元素变换后很有可能与其他元素有空间上的重叠。 

12\. 

`UIElement` 元素利用 `RenderTransformOrigin` 属性 (`System.Windows.Point` 类型) 来控制变换的原点，变换都是按照顺时针的方向进行。需要注意的是，这个原点属性仅仅适用于 `RenderTransform` 变换，因为 `LayoutTransform` 变换是完全参照父 `panel` 布局规则呈现的，不受自身控制。 

13\. 

5 个主要的 2D 变换： `RotateTransform`, `ScaleTransform`, `SkewTransform,` `TranslateTransform`, `MatrixTransform`。

14\. 

旋转变换(`RotateTransform`)，主要通过三个属性来设置：`Angle`(默认为 `0`，旋转的角度)、`CenterX`(默认为 `0`，原点的水平值)、`CenterY`(默认为 `0`，原点的纵向值)。 

```xml
<Button>
    <TextBlock RenderTransformOrigin="0.5,0.5">
        <TextBlock.RenderTransform>
            <RotateTransform Angle="25"></RotateTransform>
        </TextBlock.RenderTransform>
        Jack
    </TextBlock>
</Button>
```

15\.

缩放变换(`ScaleTransform`)，主要通过四个属性来设置：`ScaleX`(默认为 `1`，元素 `Width` 的缩放比例)，`ScaleY`(默认为 `1`，元素 `Height` 的缩放比例)，以及 `CenterX` 和 `CenterY`。缩放过后，由于 `Padding` 属于元素内部属性也会跟着缩放，而 `Margin` 则没有变化。

16\.

倾斜变换(`SkewTransform`)，主要通过四个属性来设置：`AngleX`(默认为 `0`，水平的倾斜量)，`AngleY`(默认为 `0`，垂直的倾斜量)，以及 `CenterX`和 `CenterY`。

17\.

平移变换(`TranslateTransform`)，主要通过两个属性来设置：`X`(默认为 `0`，水平偏移量)，`Y`(默认为 `0`，垂直偏移量)。这个对于 `LayoutTransform` 不起作用。

18\.

矩阵变换( `MatrixTransform` )，这个变换是用来控制自定义的变换的，所以功能最为强大，也最为繁琐。它有一个 `matrix` 属性(类型为 `System.Windows.Media.Matrix` )，代表了一个 `3X3` 的矩阵变换，其中最后一列是固定的(因为这是 2D 变换)：

```xml
<button rendertransform="1,0,0,1,10,20">Hello World!</button>
```
`MatrixTransform` 是唯一一个拥有类型转换功能的变换，你可以直接在 `RenderTransform` 直接赋值字符串。这个值依次对应于 `M11`, `M12`, `M21`, `M22`, `OffsetX`, `OffsetY`。 

19\. 

可以利用 `TransformGroup` 类(继承于 `Transform` 类)将多种变换混合起来使用，出于性能的考虑，WPF 会将多个变换计算成一个变换(效果等同于你自己计算出一个精确的 `MatrixTransform`)。 

```xml
<Button RenderTransformOrigin="0,0">
    Hello World!
    <Button.RenderTransform>
        <TransformGroup>
            <RotateTransform Angle="45"></RotateTransform>
            <RotateTransform Angle="45"></RotateTransform>
            <!--equal to rotate 90°-->
            <ScaleTransform ScaleX="1" ScaleY="5"></ScaleTransform>
            <SkewTransform AngleX="30"></SkewTransform>
        </TransformGroup>
    </Button.RenderTransform>
</Button>
```

20\.

并不是所有的Framework元素都支持变换。元素中拥有不是WPF原生内容的时候，这个元素就不支持变换。譬如，HwndHost是用来host基于GDI的内容，就不支持变换。Frame是HTML的宿主，只有当不host HTML的时候才支持变换。

### Chapter 5: Layout with Panels

1\.

`Canvas` 是最基本的 panel，一般情况下你甚至都不会用它来进行布局。可以利用附加属性，给 `canvas` 中的元素进行定位：

```xml
<Canvas>
    <Button Canvas.Left="12" Canvas.Bottom="23" Content="Hello World"></Button>
    <Button Canvas.Right="12" Canvas.Top="23" Content="Hello World"></Button>
</Canvas>
```

`Canvas` 中的元素使用 `Canvas` 附加属性不能超过两个，同一个方向上最多只能有一个存在。比如，`Canvas.Left` 和 `Canvas.Right` 同时存在的话，后者就会被忽略。

同样的，你也可以设置 `Canvas` 的 `Z` 轴来确定元素是否在一个片面内，数字越大，就叠加在越上层：

```xml
<Canvas>
    <Button Canvas.Left="12" Canvas.Bottom="23" Content="Hello World"></Button>
    <Button Canvas.Right="12" Canvas.Top="23" Content="Hello World"></Button>
</Canvas>
```

```cs
Panel.SetZIndex(btn1, 2); 
Panel.SetZIndex(btn2, -1); 
```

可以发现，只要是 `Panel` 都会有这个 `ZIndex` 属性。

在处理 2D 图像相关的时候，由于 `Canvas` 这个轻量级的 panel 有着非常高的性能优势，所以应用很广泛。

2\.

`StackPanel` 最重要的属性是 `Orientation`，他有两个枚举值：`Horizontal` 和 `Vertical`(默认值)。它没有依赖属性。

有一个特殊的 `panel`：`VirtualizingStackPanel`，它跟 `StackPanel` 极其相似，但是它会临时舍弃与屏幕显示无关的东西，以求达到最高的性能。当然，这一点仅仅在数据绑定(data binding)上起作用，因此当你要绑定大数据量的元素时候，这个 panel 是最合适的。`ListBox` 内部默认就是使用它，它也可以被应用于 `TreeView`。

3\.

`WrapPanel` 跟 `StackPanel` 很相似，只不过它会有“自动换行”的布局效果。它也没有依赖属性。它有三个主要的相关属性：

`Orientation`: `Horizontal`（默认值）和 `Vertical`。 

`ItemHeight`: 所有子元素统一的高度，超过这个高度就会被自动剪裁。 

`ItemWidth`: 所有子元素同意的宽度，超过这个宽度就会被自动剪裁。 

默认情况下，`ItemHeight` 和 `ItemWidth` 都会被设置为 `Double.NaN`，因此每列和每行的最大宽度高度都是由该列改行的最大值决定的。

一般情况下，`WrapPanel` 不被用于 `Window` 中，它一般被使用在控件内部。

4\.

`DockPanel` 是将子元素停靠在某一个方向上，有一个重要的 `Dock` 附加属性。

```xml
<dockpanel lastchildfill="False">
<textblock dockpanel.dock="Left" background="Yellow">LEFT</textblock> 
<textblock dockpanel.dock="Top" background="Red">TOP</textblock> 
<textblock dockpanel.dock="Right" background="AliceBlue">RIGHT</textblock> 
<textblock dockpanel.dock="Bottom" background="Azure">BOTTOM</textblock>
</dockpanel>
```

5\.

`Grid` 是应用最广泛的 panel，Visual Studio 和 Expression Blend 都会默认使用 `Grid`，它的效果类似于 HTML 中的 `table`，更有意思的是 WPF 还真有 `table` 这个类，然而它不是 `Panel`，甚至都不是一个 `UIElement`，它是一个为了显示文档内容的 `FrameworkContentElement`。

```xml
<grid>
    <textblock grid.column="1" grid.row="0" grid.rowspan="2">cool experiments!</textblock>
</grid> 
```

以上就是一个典型的 `Grid` 布局的示例。先定义列和行，然后放置元素，并且利用附加属性指定元素的位置。如同HTML一样，一个元素也可以跨越多行多列。 

有时候为了方便调试布局，你可以设值 `ShowGridLines=”True”` 来显示网格线。 

`RowDefinition` 和 `ColumnDefinition` 的 `Height`、`Width` 属性值不是 `double` 类型的，而是 `System.Windows.GridLength` 类型，它有三种度量单位：绝对尺寸(12,22等等)、自动(Auto)以及比例。 

```cs
rd1.Height = new GridLength(2, GridUnitType.Star); 
rd2.Height = new GridLength(3, GridUnitType.Star); 
rd3.Height = new GridLength(30, GridUnitType.Pixel); 
rd4.Height = new GridLength(0, GridUnitType.Auto);//first param would be ignored. 
```

6\. 

`Grid` 配合 `GridSplitter` 就可以使得用户手动平移行或者列的大小。就像 `Grid` 中其它子元素一样，给 `GridSplitter` 指定行和列的位置，即可使用，也可以使用 `Grid.RowSpan` 和 `Grid.ColumnSpan`。

有时候我们希望在某一行或者某一列改变（比如拖动 `GridSplitter`）的时候，另一些与之大小一样的行或者列也能跟着改变，这样我们就用到 `RowDefinitions` 和 `ColumnDefinitions` 的 `SharedSizeGroup` 属性。由于这个属性是全局生效的，也就是说不一定在当前 `Grid` 中同名的生效，整个页面中都会生效，所以出于性能等其他特定场景的考虑，我们会给需要生效的 `Grid` 设置 `IsSharedSizeScope` 属性： 

```xml
<grid issharedsizescope="True">
    <textblock grid.column="0" background="Red">1</textblock> 
    <textblock grid.column="1" background="Brown">2</textblock> 
    <textblock grid.column="2" background="Green">3</textblock>
</grid>
```

7\.

`Grid` 可以实现 `Canvas`、`StackPanel`、`DockPanel` 的功能，只有 `WrapPanel` 没法实现，但是根据应用场景的不同，我们应该选择最合适的那个 panel，不能靠一个 panel 搞定所有场景。

8\.

有时候 `panel` 中的内容大小超过了 `panel` 预留的空间，这时候就会出现内容溢出(Content Overflow)的情况，有 5 种策略解决这个问题：`Clipping`, `Scrolling`, `Scaling`, `Wrapping`, `Trimming`。

9\.

`Clipping` 是指超过的部分就直接截掉，它出现在 `RenderTransforms` 生效之前：

```xml
<grid cliptobounds="True"></grid>
```

10\.

`Scrolling` 就是如果出现内容溢出了，以滚动条的形式呈现。你只需要把 `panel` 放在 `System.Windows.Controls.ScrollViewer` 控件中就行了。它有 `HorizontalScrollBarVisibility` 和 `VerticalScrollBarVisibility` 属性，这两个属性有四种枚举值：

**`Visible`**:无论内容是否溢出，滚动条都可见。 

**`Auto`**:当内容溢出时，滚动条可见，否则就隐藏。 

**`Hidden`**: 无论内容是否已出，滚动条都实际存在，但是不可见。 

**`Disabled`**: 不仅内容不可见，实际上都不存在。 

`HorizontalScrollBarVisibility` 的默认值是 `Visible`，`VerticalScrollBarVisibility` 的默认值是 `Auto`。

```xml
<scrollviewer horizontalscrollbarvisibility="Auto" verticalscrollbarvisibility="Visible">
    <stackpanel>
        <button>Hello</button> 
        <button>Hello</button> 
        <button>Hello</button> 
        <button>Hello</button>
    </stackpanel>
</scrollviewer>
```

11\.

**`Scaling`**: 是指利用 `System.Windows.Controls.Viewbox` 来使得 `panel` 中的元素尺寸能够动态的缩放。默认情况下，`Viewbox` 会拉伸宽度和高度来适应预留的空间。同时，它也有一个 `Stretch` 属性方便我们进一步的控制元素的边框大小，它有四个枚举值：

**`None`**: 跟没用 `Viewbox` 一样，元素边框没有缩放。 

**`Fill`**: 元素的尺寸会被设置成 `Viewbox` 的尺寸以完全的填充 `Viewbox`，之前预定义的元素尺寸将失效，所以它们原先的纵横比将被破坏。 

**`Uniform`**: 保持原先的纵横比，然后尽量的填充 `Viewbox`，所以它不会被截断，但是有可能 `Viewbox` 会有尚未使用的预留空间（默认值）。 

**`UniformToFill`**: 保持原先的纵横比，然后完全的填充 `Viewbox`，所以它有可能被截断，但是 `Viewbox` 中没有尚未使用的预留空间了。 

还有一个 `StretchDirection` 属性可以让我们控制内容缩放：

**`UpOnly`**: 元素只能放大，最小不能小于它的最初值（元素本身的属性最初值）。 

**`DownOnly`**: 元素只能缩小，最大不能大于它的最初值。 

**`Both`**: 元素既能放大也能缩小，这也就不受这个 `StretchDirection` 属性的影响了，一切将听从 `Stretch` 属性的设置影响。否则 `UpOnly` 和 `DownOnly` 的影响力将破坏 `Stretch` 设置值的效果。

```xml
<viewbox stretchdirection="UpOnly" stretch="Fill">
    <button width="120" height="20">Hello</button>
</viewbox>
```