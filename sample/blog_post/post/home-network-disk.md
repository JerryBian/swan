前面我们已经装好了软路由 [Openwrt](/2020/07/home-network-openwrt.html)，作为一个路由器系统，我们访问互联网已经没有问题了。

作为一个本质上是 Linux 的操作系统，本文介绍如何挂载外部存储设备，这些设备可以是 USB，也可以是普通的硬盘。

在挂载外部硬盘之前，我们先看看系统盘的使用情况。

```shell
root@Openwrt:~# fdisk -l
Disk /dev/loop0: 464.1 MiB, 486670336 bytes, 950528 sectors
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes


Disk /dev/sda: 29.5 GiB, 31675383808 bytes, 61865984 sectors
Disk model: BIWIN SSD
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: dos
Disk identifier: 0x10fa97f8

Device     Boot  Start     End Sectors  Size Id Type
/dev/sda1  *       512  410111  409600  200M 83 Linux
/dev/sda2       410624 1434623 1024000  500M 83 Linux
```

可以看到我们的系统盘总容量为 29.5GiB，目前有两个分区 `/dev/sda1` 和 `/dev/sda2`，但是他们加起来只占了 700MB 的容量。所以，我们可以把剩下的空间创建新的分区，然后挂载使用。

```shell
root@Openwrt:~# fdisk /dev/sda

Welcome to fdisk (util-linux 2.33).
Changes will remain in memory only, until you decide to write them.
Be careful before using the write command.


Command (m for help): p
Disk /dev/sda: 29.5 GiB, 31675383808 bytes, 61865984 sectors
Disk model: BIWIN SSD
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: dos
Disk identifier: 0x10fa97f8

Device     Boot  Start     End Sectors  Size Id Type
/dev/sda1  *       512  410111  409600  200M 83 Linux
/dev/sda2       410624 1434623 1024000  500M 83 Linux
```

使用 `p` 打印出现有的分区，与前面展示的信息一致。接下来使用 `n` 新建分区，将剩下的所有空间划分为同一个新的分区，所以只需要一路回车下去即可。

```shell
Command (m for help): n
Partition type
   p   primary (2 primary, 0 extended, 2 free)
   e   extended (container for logical partitions)
Select (default p):

Using default response p.
Partition number (3,4, default 3):
First sector (410112-61865983, default 1435648):
Last sector, +/-sectors or +/-size{K,M,G,T,P} (1435648-61865983, default 61865983):

Created a new partition 3 of type 'Linux' and of size 28.8 GiB.
Partition #3 contains a ext4 signature.

Do you want to remove the signature? [Y]es/[N]o: Y

The signature will be removed by a write command.
```

目前所有的操作都是在内存之中，所以我们需要使用 `w` 命令来将新的分区写入硬盘。

```shell
Command (m for help): w
The partition table has been altered.
Syncing disks.
```

再次打印目前的所有分区，可以发现新建的 `/dev/sda3` 已经创建好了。

```shell
root@Openwrt:~# fdisk /dev/sda

Welcome to fdisk (util-linux 2.33).
Changes will remain in memory only, until you decide to write them.
Be careful before using the write command.


Command (m for help): p
Disk /dev/sda: 29.5 GiB, 31675383808 bytes, 61865984 sectors
Disk model: BIWIN SSD
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: dos
Disk identifier: 0x10fa97f8

Device     Boot   Start      End  Sectors  Size Id Type
/dev/sda1  *        512   410111   409600  200M 83 Linux
/dev/sda2        410624  1434623  1024000  500M 83 Linux
/dev/sda3       1435648 61865983 60430336 28.8G 83 Linux
```

格式化新创建的分区为 `ext4` 格式。

```shell
root@Openwrt:~# mkfs.ext4 /dev/sda3
mke2fs 1.44.5 (15-Dec-2018)
Discarding device blocks: done
Creating filesystem with 7553792 4k blocks and 1888656 inodes
Filesystem UUID: 18946374-adb4-4540-bc6f-4dd3d0ef68fb
Superblock backups stored on blocks:
	32768, 98304, 163840, 229376, 294912, 819200, 884736, 1605632, 2654208,
	4096000

Allocating group tables: done
Writing inode tables: done
Creating journal (32768 blocks): done
Writing superblocks and filesystem accounting information: done
```

接下来，我们开始挂载外部硬盘或者 USB 等设备。插入存储设备到主机接口上，USB 或者 SATA。通过 `fdisk -l` 命令来检查你是否可以看到你的这些设备，如果没有的话可以尝试重启主机试试。

我这里插入两块 3.5 寸的机械硬盘，一块通过 SATA 口连接，一块通过 USB 3.0 连接。

```shell
root@Openwrt:~# fdisk -l
Disk /dev/loop0: 464.1 MiB, 486670336 bytes, 950528 sectors
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes


Disk /dev/sda: 29.5 GiB, 31675383808 bytes, 61865984 sectors
Disk model: BIWIN SSD
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: dos
Disk identifier: 0x10fa97f8

Device     Boot   Start      End  Sectors  Size Id Type
/dev/sda1  *        512   410111   409600  200M 83 Linux
/dev/sda2        410624  1434623  1024000  500M 83 Linux
/dev/sda3       1435648 61865983 60430336 28.8G 83 Linux


Disk /dev/sdb: 1.8 TiB, 2000398934016 bytes, 3907029168 sectors
Disk model: ST2000NM0011
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: gpt
Disk identifier: 9660DBB5-44F0-4065-A4CD-90F7A4DEC4E9

Device     Start        End    Sectors  Size Type
/dev/sdb1   2048 3907028991 3907026944  1.8T Microsoft basic data


Disk /dev/sdc: 3.7 TiB, 4000787030016 bytes, 7814037168 sectors
Disk model: 007-2DT166
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 4096 bytes
I/O size (minimum/optimal): 4096 bytes / 4096 bytes
Disklabel type: gpt
Disk identifier: 0203E58E-D12D-46F4-9B5A-08C4A4C1EBBF

Device     Start        End    Sectors  Size Type
/dev/sdc1   2048 7814037134 7814035087  3.7T Linux filesystem
```

我们接下来开始格式化这两块硬盘，当然如果已经满足你的需求了，你可以跳过这一步。在 Linux 上面最常见的硬盘文件系统格式是 `ext4`，而在 windows 上面则是 `NTFS`。建议至少保留一个 `ext4` 硬盘，具体原因在接下来安装 Docker 的时候会说。另一块可以是 `NTFS` 格式，方便我们拔下来在 windows 上读写。

对于 1.8TiB 的 `/dev/sdb` 硬盘，格式化为 `ext4` 文件系统，我们只划分一个分区即可。

首先我们把当前硬盘所有分区都删除。

```
root@Openwrt:~# fdisk /dev/sdb

Welcome to fdisk (util-linux 2.33).
Changes will remain in memory only, until you decide to write them.
Be careful before using the write command.


Command (m for help): d
Selected partition 1
Partition 1 has been deleted.

Command (m for help): p
Disk /dev/sdb: 1.8 TiB, 2000398934016 bytes, 3907029168 sectors
Disk model: ST2000NM0011
Units: sectors of 1 * 512 = 512 bytes
Sector size (logical/physical): 512 bytes / 512 bytes
I/O size (minimum/optimal): 512 bytes / 512 bytes
Disklabel type: gpt
Disk identifier: 9660DBB5-44F0-4065-A4CD-90F7A4DEC4E9

Command (m for help): w
The partition table has been altered.
Failed to remove partition 1 from system: Resource busy

The kernel still uses the old partitions. The new table will be used at the next reboot.
Syncing disks.
```

由于该分区被其他地方引用了，所以更改需要下次重启才能生效。你也可以先将这个分区解除挂载。

```
root@Openwrt:~# block info
/dev/loop0: UUID="d92d8162-e86e-4444-afd2-c758c37bd59e" VERSION="1.12" MOUNT="/overlay" TYPE="f2fs"
/dev/sda1: UUID="57f8f4bc-abf4-655f-bf67-946fc0f9f25b" VERSION="1.0" MOUNT="/boot" TYPE="ext4"
/dev/sda2: UUID="ab1478f1-eee06834-977b2064-1ebf51f0" VERSION="4.0" MOUNT="/rom" TYPE="squashfs"
/dev/sda3: UUID="18946374-adb4-4540-bc6f-4dd3d0ef68fb" VERSION="1.0" TYPE="ext4"
/dev/sdb1: UUID="0000001800000048" MOUNT="/mnt/sdb1" TYPE="ntfs"
/dev/sdc1: UUID="fee59a6d-4de9-4f4c-aba8-8f8bb0cb12ba" LABEL="vault1" VERSION="1.0" MOUNT="/mnt/sdc1" TYPE="ext4"
root@Openwrt:~# umount /mnt/sdb1
```

用上文同样的方法创建一个全新的分区。

```
root@Openwrt:~# fdisk /dev/sdb

Welcome to fdisk (util-linux 2.33).
Changes will remain in memory only, until you decide to write them.
Be careful before using the write command.


Command (m for help): n
Partition number (1-128, default 1):
First sector (34-3907029134, default 2048):
Last sector, +/-sectors or +/-size{K,M,G,T,P} (2048-3907029134, default 3907029134):

Created a new partition 1 of type 'Linux filesystem' and of size 1.8 TiB.
Partition #1 contains a ntfs signature.

Do you want to remove the signature? [Y]es/[N]o: Y

The signature will be removed by a write command.

Command (m for help): w
The partition table has been altered.
Calling ioctl() to re-read partition table.
Syncing disks.
```
将分区格式化为 `ext4` 格式。

```shell
root@Openwrt:~# mkfs.ext4 /dev/sdb1
mke2fs 1.44.5 (15-Dec-2018)
Creating filesystem with 488378385 4k blocks and 122101760 inodes
Filesystem UUID: bbdd1059-8218-4738-b32f-a775c8e96aa3
Superblock backups stored on blocks:
	32768, 98304, 163840, 229376, 294912, 819200, 884736, 1605632, 2654208,
	4096000, 7962624, 11239424, 20480000, 23887872, 71663616, 78675968,
	102400000, 214990848

Allocating group tables: done
Writing inode tables: done
Creating journal (262144 blocks): done
Writing superblocks and filesystem accounting information: done
```

对于另一块 3.7TiB 的 `/dev/sdc` 硬盘使用同样的操作，只不过最后一步我们需要格式化为 `NTFS` 格式。

```shell
root@Openwrt:~# mkfs.ntfs -f /dev/sdc1
Cluster size has been automatically set to 4096 bytes.
Creating NTFS volume structures.
mkntfs completed successfully. Have a nice day.
``` 

这里我们使用了 `-f` 选项来执行快速格式化，否则 3.7TiB 的硬盘将会耗时许久。

使用 `lsblk` 来查看目前的硬盘分区信息。

```shell
root@Openwrt:~# lsblk -f
NAME   FSTYPE   LABEL       UUID                                 FSAVAIL FSUSE% MOUNTPOINT
loop0  f2fs     rootfs_data d92d8162-e86e-4444-afd2-c758c37bd59e  322.5M    10% /mnt/loop0
sda
├─sda1 ext4                 57f8f4bc-abf4-655f-bf67-946fc0f9f25b  188.7M     2% /mnt/sda1
├─sda2 squashfs                                                        0   100% /rom
└─sda3 ext4                 18946374-adb4-4540-bc6f-4dd3d0ef68fb
sdb
└─sdb1 ext4                 bbdd1059-8218-4738-b32f-a775c8e96aa3
sdc
└─sdc1 ntfs                 09009C6238A12FC4
```

如果你是个强迫症患者，你也可以给这两个分区打上 label。

```shell
# ext
root@Openwrt:~# tune2fs -L vault0 /dev/sdb1
tune2fs 1.44.5 (15-Dec-2018)

# ntfs
root@Openwrt:~# ntfslabel /dev/sdc1 vault1
```

为了使用这些分区，我们现在需要将他们挂载到系统目录下面。Openwrt 自带了挂载点的页面（系统 / 挂载点），但是据我实测实在是没法用，完全摸不清他的规律。所以，这里我们使用手动的方式挂载我们新创建的三个分区。

创建 3 个挂载目录。

```shell
root@Openwrt:~# mkdir -p /mnt/sysdisk /mnt/vault0 /mnt/vault1
```

编辑 `/etc/fstab` 文件，输入以下的配置内容。这里的 UUID 可以在上文的 `lsblk -f` 中找到。

```shell
UUID=18946374-adb4-4540-bc6f-4dd3d0ef68fb /mnt/sysdisk ext4    defaults   0
UUID=bbdd1059-8218-4738-b32f-a775c8e96aa3 /mnt/vault0 ext4    defaults   0
UUID=09009C6238A12FC4 /mnt/vault1 ntfs    defaults   0
```

然后使用 `mount -a` 来挂载它们，最终的硬盘结构如下。

```shell
root@Openwrt:~# lsblk -f
NAME   FSTYPE   LABEL       UUID                                 FSAVAIL FSUSE% MOUNTPOINT
loop0  f2fs     rootfs_data d92d8162-e86e-4444-afd2-c758c37bd59e  314.7M    12% /mnt/loop0
sda
├─sda1 ext4                 57f8f4bc-abf4-655f-bf67-946fc0f9f25b  188.7M     2% /mnt/sda1
├─sda2 squashfs                                                        0   100% /rom
└─sda3 ext4                 18946374-adb4-4540-bc6f-4dd3d0ef68fb   26.8G     0% /mnt/sysdisk
sdb
└─sdb1 ext4     vault0      bbdd1059-8218-4738-b32f-a775c8e96aa3    1.7T     0% /mnt/vault0
sdc
└─sdc1 ntfs     vault1      09009C6238A12FC4                        3.7T     0% /mnt/vault1
```

别忘了把硬盘自动挂载的设置给关掉。

![openwrt](../file/2020/07/openwrt-setting.png)

当然每次重启之后，都需要手动 `mount -a`，这样虽然麻烦点，但是可以保证它们对应的 `mnt` 目录永远是固定的。

有了硬盘之后，我们就可以做很多有意思的事情了。