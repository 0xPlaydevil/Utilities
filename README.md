# Utilities
一些常用的小工具/小功能。Something in common use.

## 通用性好一点的

* 截图（Editor/Render2Tex.cs）

*简介*

该脚本提供了在Unity编辑器下“截图”的功能，可以指定摄像机/分辨率等，比普通的屏幕截图更加灵活强大。并且可以保存上次截图参数，以便于快速再次截图，文件名根据时间自动生成。

*使用*

将该库导入Unity后，会出现Tools菜单，里面有Render...和RenderAgain两个按钮。点击Render...即可打开相应窗口。点击RenderAgain以上次参数再次截图。

* 清理空文件夹（Editor/Utilities.cs）

*简介*

由于Git不管理目录，而Unity会为目录生成meta文件，所以当项目中存在空文件夹，或者由于分支切换等原因总是形成空文件夹时，两者就会有冲突。没有找到更好的解决办法，所以写了个清理脚本。**使用前请备份**，若因该清理功能出现数据异常的情况，本人概不负责。

*使用*

导入该库，点击菜单Tools/CleanEmptyDirectories.

* 摄像机操控（FreeCamMove.cs）

*简介*

在场景中交互式移动摄像机。一种情况是在地形基础上移动摄像机，可以实现左键拖动，右键绕中心旋转，左Alt+右键自身旋转功能。另一种情况是设置一个物体，可以围绕其旋转，可以拉近拉远。该脚本尚不完善，适应范围有限，应用场景多为地形和物体的观看。

*使用*

导入该库，将FreeCamMove挂在摄像机上，有需要可以配置移动速度/测高基准物体layer/拖动时鼠标指针等参数。

* 色彩转换（ColorCreater.cs）

*简介*

这是自己写的一个HSV颜色转RGB颜色的方法。不过后来Unity加上内置方法了，所以现在也没用了。仅供学习。

*使用*

源代码很简单，一看就会使用。



## 其它

* 修剪物体树（Editor/EditorTools.cs）; *简介*: 清理GameObject对象树中的空节点（没有组件的节点）。**谨慎使用**，后果自负; *使用*：导入该库，点击菜单Tools/TrimEmpty。
