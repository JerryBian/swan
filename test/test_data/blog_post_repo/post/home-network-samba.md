家庭网络一个很重要的组成部分就是内网的文件共享，你可以很方便的在任意一台设备上通过 [smb 协议](https://en.wikipedia.org/wiki/Server_Message_Block)访问到共享的文件内容。

通常情况下，我们是在宿主操作系统中安装 Samba 软件包，然后配置对应的路径和权限。为了针对不同用户配置不同的权限，我们需要在宿主操作系统（这里就是 Openwrt）中创建对应的用户以及用户组，还要手动编辑对应的 [`smb.conf`](https://www.samba.org/samba/docs/current/man-html/smb.conf.5.html) 文件。

[前文](/2020/07/home-network-docker.html)我们已经安装好了 Docker，这帮助我们快速的部署文件共享的服务。我们不需要侵入式的修改 Openwrt 的任何配置，比如用户、权限等等，增加和修改已有的配置都是通过 `docker run` 的形式快速部署。

首先，请准备好对应的磁盘文件分区，由于是共享文件，通常需要大量的空间。

``` shell
docker run -d --name "samba" \
    -v /mnt:/mnt --network="host" dperson/samba -p \
    -w "WORKGROUP" \
    -u "test;test;1001;admin;1001" \
    -u "test2;test2;1002;family;1002" \
    -u "test3;test3;1003;family;1002" \
    -s "disk1;/mnt/disk1;no;no;no;@admin" \ # 只有管理员可以访问这个共享目录
    -s "temp;/mnt/disk1/temp;yes;no;yes" \ # 允许匿名访问
    -s "movie;/mnt/disk1/music;yes;yes;no;@family;none;test2;Shared Movie"
```

对应的镜像来自 [dperson/samba](https://hub.docker.com/r/dperson/samba)，详细的参数可以参考官方文档，这里主要解释一下这里面的各种参数。

第一个是添加用户。通过 `-u` 的参数，双引号里面依次是： 

```plaintext
同户名;
密码;
用户名 ID;
用户组;
用户组 ID
```

示例中我们创建了两个用户组：`admin` 和 `family`。`admin` 中有一个用户为 `test`，`family` 中为 `test2` 和 `test3`。

每次容器启动后，会有内置的脚本帮助我们创建对应的用户以及用户组，我们只需要写好上述配置即可。

第二个是配置共享文件。通过 `-s` 的参数，双引号里依次为： 

```plaintext
客户端看到的共享名称;
共享路径;
是否出现在客户端的网络列表（默认是 yes）;
文件夹只读（默认是 yes）;
允许匿名访问;
允许的用户名（多个用户以","分隔，默认是所有用户）;
管理员用户名（默认是 none）;
可以执行写操作的用户名;
额外展示的信息
```

上面的 `管理员用户名` 建议保持默认的 `none`。用户名那里可以填写用户组，比如 `@family` 这种形式，它下面的用户就都有对应的权限了。

需要注意的是 `可以执行写操作的用户名` 必须包含在 `允许的用户名` 之内。譬如，你这样配置就是不行的，因为 `test` 用户不在之前能够访问这个共享的用户组 `@family` 之内。

```plaintext
-s "movie;/mnt/disk1/music;yes;yes;no;@family;none;test;Shared Movie"
```

`Docker run` 运行成功后，我们就可以在其他的机器上访问到对应的共享目录了。

macOS 下面 `Finder - Go - Connect to server ...`：

![macOS SMB](../file/2020/07/smb-macos.png)

Windows 下面直接在文件管理器中输入 `\\192.168.2.1\disk1` 即可。