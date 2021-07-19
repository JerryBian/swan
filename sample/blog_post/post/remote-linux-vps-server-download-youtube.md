现在购买一个低配的 VPS 来托管我们个人的一些小项目，已经是一件非常容易而且相当廉价的一件事情了。当前这个博客就是托管在一个入门级的国外 VPS 上面。

上个月一次偶然发现，我的 VPS 带宽基本都是浪费掉的，在一个计费周期内甚至 1% 的容量都没有用到。这些带宽每过一个月就会被重置，即使你当月没有用完也不会顺延到下个月。

![VPS bandwidth usage](../file/2020/03/vps-usage.png)

这让我想到我的一个常规需求：观看 Youtube 上面各种技术视频。

在线观看 Youtube 720P 以上的高清视频需要较高的网速支撑，当我们达不到这个条件的时候，一个很直接的方式便是离线观看：我们可以把高清版本的 Youtube 视频下载到本地，然后用视频播放器直接打开。

托管在国外的 VPS 有一个最大的好处，就是访问这些国外的网站的时候速度会飞快。所以我们的思路就是，让 VPS 帮我们下载好视频，然后我们想办法下载到本地。

### 一、配置好 SSH key

更安全也更方便的登录远程 Linux 主机的方式是通过 SSH key 而不是每次都要输入用户密码。

在本地机器创建公钥和私钥：

```sh
$ ssh-keygen -t ecdsa -b 521
```

按照提示每一步均使用默认选项即可。不建议改变密钥文件的名称或路径，对于自定义的密钥名称，每次连接均需要[显示指定](https://askubuntu.com/a/30792/322580)。

拷贝公钥到远程服务器：

```sh
$ ssh-copy-id -i ~/.ssh/id_ecdsa user@server # 指定密钥
```

输入服务器的密码之后，下次就可以直接使用 ssh user@server 来 SSH 到远程服务器了，省去了每次都输入密码的麻烦。

### 二、安装 youtube-dl

一个很强大的下载 Youtube 视频的工具是 [youtube-dl](https://github.com/ytdl-org/youtube-dl)。顺便说一句，他其实支持几乎市面上所有主流的[视频网站](https://ytdl-org.github.io/youtube-dl/supportedsites.html)，包括优酷、腾讯视频等等。

下载到服务器并且设置好权限：

```sh
$ sudo curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl
sudo chmod a+rx /usr/local/bin/youtube-dl
```

安装成功之后，我们可以测试一下下载的效果。

```sh
$ youtube-dl -o slow-the-spread.mp4 https://www.youtube.com/watch?v=-nZMeNP84gM
[youtube] -nZMeNP84gM: Downloading webpage
[download] Destination: slow-the-spread.mp4
[download] 100% of 1.37MiB in 00:00
```

`-o` 参数指定下载文件的文件名，后面直接跟着 Youtube 网页的 URL 即可。通过这种方式，我们便下载好了一个视频，存于 Linux 服务器上面。

### 三、本地调用 youtube-dl

每次都登录服务器，输入一长串这样的命令着实不方面。如果我们能够在本地执行一些简单的命令，达到同样的效果就很完美了。

我们可以使用 `ssh user@server "your remote command"` 这种方式来远程执行 shell 命令。在我们的 case 中唯一需要输入的一个参数是目标 Youtube 的 URL。

```sh
$ ssh user@server "youtube-dl -f best --all-subs  -o \"/var/www/youtube/%(title)s.%(ext)s\" $1"
```

这里我们选择清晰度最好的视频，而且包含了所有的字幕文件。同时，我们也动态的根据视频的标题和格式来指定文件名。请注意，这里统一下载视频到 `/var/www/youtube` 目录。更多的参数选项可以参考[官方文档](https://github.com/ytdl-org/youtube-dl/blob/master/README.md)。

我们可以把上述命令保存到一个 shell 文件中，每次有新的下载任务的时候可以直接调用这个文件，传入视频的地址即可。一个更好的解决方案是，将这个命令封装成一个 shell 函数，这样就可以随时调用了。

这里以 `zsh` 为例，打开 `~/.zshrc` 配置文件，在文件的最后加入新的函数。

```sh
function you-dl {
  ssh user@server "youtube-dl -f best --all-subs  -o \"/var/www/youtube/%(title)s.%(ext)s\" $1"
}
```

重新启动命令行工具后，我们就可以在本地直接调用我们定义好的函数 `you-dl`，传入目标视频的 URL。

```
$ you-dl https://www.youtube.com/watch?v=-nZMeNP84gM
```

远程下载的输出也会重定向到本地，可以很方便的查看远程命令的执行效果。

```sh
$ you-dl https://www.youtube.com/watch\?v\=-nZMeNP84gM
[youtube] -nZMeNP84gM: Downloading webpage
[info] Writing video subtitles to: /var/www/youtube/Surgeon General Social Distancing ( -30).en.vtt
[download] Destination: /var/www/youtube/Surgeon General Social Distancing ( -30).mp4
[download] 100% of 1.37MiB in 00:0090MiB/s ETA 00:00known ETA
```

### 四、配置远程访问文件

接下来要做的是，让我们方便的从服务器端把这个文件下载到本地。这样的解决方案其实有很多，我们可以搭建 FTP，或者使用 `scp`，`rsync` 这样的命令行工具来达到目的。

但其实最简单的是通过 HTTP 的形式直接下载到本地。

首先，我们要先安装 Nginx 服务器。

```sh
apt install nginx
```

我们的资源最好是需要认证用户方可访问的，[Nginx](https://docs.nginx.com/nginx/admin-guide/security-controls/configuring-http-basic-authentication/) 下面需要安装一个 Apache 的小工具才可以开启基本认证（Basic HTTP Authentication）。

```sh
# 我们需要使用 htpasswd 命令
$ apt install apache2-utils 

# 创建 password 文件，并且指定用户名为 user1，在接下来的提示中输入密码
$ htpasswd -c /var/wwww/youtube/.htpasswd user1 
```

最后，我们需要配置 Nginx 直接索引我们的下载目录。

```sh
server {
    server_name   example.com;
    root /var/www/youtube;

    location / {
        auth_basic "Show me your identidy";
        auth_basic_user_file /var/www/youtube/.htpasswd;
        autoindex on;
    }
}
```

Nginx 生效之后我们就可以直接在浏览器中看到我们下载好的内容了。

![浏览器查看列表](../file/2020/03/youtube-dl-download.png)

### 五、本地下载

我们可以直接在线观看这些视频，也可以下载这些视频到本地。这里有一个需要注意的是，我们的视频都是需要用户名密码认证的。

[FDM](https://www.freedownloadmanager.org/) 可以很好的完成我们的需求，当下载资源需要认证的时候会有额外的输入框提示我们需要输入登录账号和密码。

![FDM](../file/2020/03/fdm-basic-auth.png)

### 六、结束语

通过这种方案，我们可以充分的利用服务器的闲置资源，帮我们高速下载好高清的 Youtube 视频，我们再通过 HTTP 服务器下载到本地机器。

长远地看，我们甚至可以写一个专门的私人下载网站，输入目标视频的地址，然后内部调用相关的下载工具。这样显得更加优雅以及便捷，当然这是后话了。

另一个方面是，我们不仅可以下载各种各样的视频，我们也可以使用 `curl` 这种工具帮我们下载任意的文件，`curl` 支持各种各样的常规[协议](https://ec.haxx.se/protocols/protocols-curl)，包括 HTTP/FTP 等等。用同样的思路，我们可以将一些国内网络下载缓慢的目标源放到服务器上面去，也同时让我们的主机忙碌起来。