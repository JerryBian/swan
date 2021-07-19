
在服务器端：

```
# 下载
wget https://github.com/fatedier/frp/releases/download/v0.33.0/frp_0.33.0_linux_amd64.tar.gz
# 解压
tar zxf frp_0.33.0_linux_amd64.tar.gz
cd frp_0.33.0_linux_amd64
```

```ini
[common]
bind_addr = 127.0.0.1
bind_port = 8200
bind_udp_port = 8201
vhost_http_port = 8202

dashboard_addr = 127.0.0.1
dashboard_port = 8203
dashboard_user = jerry
dashboard_pwd = jerry-2018

authentication_method = token
token = b243b147-7d1d-4394-8d29-93301e9d8640

max_pool_count = 32
subdomain_host = laobian.me
tcp_mux = true
```

```ini
[Unit]
Description=Frp Server Service
After=network.target

[Service]
User=root
Restart=on-failure
RestartSec=5s
ExecStart=/laobian/homebox/frp/frps -c /laobian/homebox/frp/frps.ini

[Install]
WantedBy=multi-user.target
```

在客户端：

```
# 下载
wget https://github.com/fatedier/frp/releases/download/v0.33.0/frp_0.33.0_linux_amd64.tar.gz
# 解压
tar zxf frp_0.33.0_linux_amd64.tar.gz
cd frp_0.33.0_linux_amd64
```

```ini
[common]
server_addr = 97.64.22.162
server_port = 8200
token = b243b147-7d1d-4394-8d29-93301e9d8640
pool_count = 3
tcp_mux = true # same with frps

[ssh]
type = tcp
local_ip = 192.168.2.1
local_port = 22
remote_port = 8222

[openwrt]
type = http
local_port = 80
local_ip = 192.168.2.1
subdomain = openwrt

[jellyfin]
type = http
local_port = 8096
local_ip = 192.168.2.1
subdomain = jellyfin
```

```sh
cd /homebox/frp
nohup ./frpc -c ./frpc.ini &
```