git 已经成为如今软件行业版本控制工具的事实上的标准，越来越多的公司采用 git 来管理软件的开发。

本文旨在列举 git 使用中的常用命令。

#### 环境设置

绑定 git 账号与用户名，

```sh
git config --global user.name "John Doe"
git config --global user.email johndoe@example.com

# 设置 VS Code 为默认编辑器， 编辑 message 的时候会自动启动实例
git config --global core.editor code

# 设置 Notepad++ 为默认编辑器
git config --global core.editor "'C:/Program Files/Notepad++/notepad++.exe' -multiInst -nosession"
```
为每一个 commit 强制添加签名，

```sh
git config --global commit.gpgsign true
git config --global user.signingkey XXXXXXXX
git config --global gpg.program "C:\Program Files (x86)\GnuPG\bin\gpg.exe"
```

如果只想对当前 project 生效，去掉 `--global`。

#### clone

以 Linux 内核源码为例，

```sh
git clone https://github.com/torvalds/linux.git
```
上述命令将会 clone 默认的分支(branch)，等同于：

```bash
git clone -b master https://github.com/torvalds/linux.git
```

如果需要 clone 制定的分支，可以使用 `-b` 选项。

如果需要指定本地目录，

```sh
git clone https://github.com/torvalds/linux.git /tmp/mylinux
```

上述命令会创建新的文件夹`mylinux`在`/tmp`路径下，源码将直接放至`mylinux`下面。

#### tag

git 允许对当前的版本设置一个 tag，这有点类似于一个 milestone 的概念，方便日后快速定位代码的版本。github 对 tag 的支持体现在，它会自动创建一个 release。

![github releases](../file/2020/01/github-releases.png)

在当前分支下面创建 tag：

```sh
git tag <tagname>
```

为这个 tag 添加说明：

```sh
git tag <tagname> -a
```

提交新创建的 tag:

```sh
git push origin <tag>
```

或者提交所有的 tag：

```sh
git push origin --tags
```

#### cache

清除本地所有 cache：

```sh
git rm -r --cached .
```

#### 修改最近一条 commit message

很多时候我们需要修改刚刚 commit 的 message，比如需要关联 issue 号。

```sh
git commit --amend
```

如果需要修改的 commit 还没有提交到仓库

```sh
git push -u origin
```

如果这个 commit 已经被 push 过了

```sh
git push -u origin --force
```