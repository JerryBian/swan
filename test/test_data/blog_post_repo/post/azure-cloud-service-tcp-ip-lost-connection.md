Last whole week, I was busying fixing a very strange bug which has been exist since the day our project launched.

### Description

We have a Microsoft Azure Cloud Service worker role served as our mobile game server which is running 24/7, and the client side is located at customers' mobile phone, usually Windows Phone and Android.

In the server side, we use a very popular framework [SuperSocket](http://www.supersocket.net/) to handle the request and send response back. However, in client side we just simply use native C# Socket API with asynchronous connect/send/receive.

Most time everything seems ok, customers can attack and play around very well. However, the lost connection problem occasionally happened, usually when the player finished the battle or other key steps (which means customers might lose resources which should not).

The code thrown an exception looks like this:

```csharp
_socket.BeginReceive(.., .., .., .., ProcessReceive, ..);
private void ProcessReceive(IAsyncResult ar)
{
	var count = _socket.EndReceive(ar)
	...
}
```

The `EndReceive` thrown a `SocketException`:

> ErrorCode=10054 Message="An existing connection was forcibly closed by the remote host"

### Reasonable Attempts

Since I did not have too much TCP/IP programming experience before, and the moment I learned the related concepts is university time.

#### I decided to start with the server code:

I put trace logs to every point which related to the socket handling and connect/disconnect, and also deployed to a new cloud service environment to ensure only my socket connection exists. I spent a lot of time here, because from the Google and MSDN, this exception happens only when server side closed the connection. I even consulted [@KerryJiang](https://github.com/kerryjiang) which is the author of SuperSocket, but I didn't get any useful information from him.

Finally, I realized the fact: server does not have any exception during the client thrown `SocketException`, and server seems still hold on the connection and no `OnClosed` event triggered before the client detected that socket error.

#### It's time to analyze the TCP/IP packets

Since I couldn't explain even a bit what caused this strange behavior, I realized I had to know more details about the socket communication. Yeah, I had to.

I spent one night to pick up the book which seems still very new, hmm, how I passed the final exam then? I quickly learned the concepts such as segment structure and the operation flags .etc. Basically, It's easy understanding for me.

Then, I started to use Wireshark to help me analyze the packet. To achieve this, I captured the socket at both client and server side, which means I need to remote to the virtual machine behind the cloud service. Here is the screen shot when the bug reproduced.

Client(port: 60035)

![](../file/2015/12/wireshark-client.png)

Server(port: 8777)

![](../file/2015/12/wireshark-server.png)

From the screen shot, we can see the last normal send from client to server is located at `Seq=409`, it sent `PSH`, `ACK` flags to server, however the server didn't captured this communication. Therefore, the client thought some error might happened, it started to resend that package with OS built-in error handling strategy. After five times failed, it realized the connection has lost that it had to reset the connection, see the red highlight line `RST`, `ACK` flag.

All this happens at client side, and the server side seems not know even a bit about this. The connection is not closed by server side SuperSocket.

#### So, how about the client side

Actually, the client side is written by Unity, which means it's not native C#/.NET code, it's Mono. The assemblies it used located at `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\3.5\Profile\Unity Full\v3.5\System.dll`(**note: this path MAYBE not correct, since I have no Unity environment at hand this moment.**), and furthermore, it's Mono 2.0. So, is it possible caused by the implementation of Mono 2.0? Since I went nowhere, I downloaded the [source code of Mono 2.0](https://github.com/mono/mono/blob/mono-2-0/mcs/class/System/System.Net.Sockets/Socket.cs), thanks to GitHub, and didn't find significant issue, it just invoke the external native DLL, nothing serious.

#### Can I reproduce it with a fresh start?

I decided to write a separate demo to reproduce this bug. To make it simple, I started to write a Console application which served as my SuperSocket server, and another one with native C# Socket APIs as my client. After about one hour, the connection seems still there, and no exception happened.

### Final attempt

I felt a little frustrated, so I tried to find more details about when this bug happens EXACTLY. I, with another tester, finally noticed this bug happens after 4-5+ minutes later if no communications between the peers.

I realized something make this happen if no communications happens after a certain time. I checked the SuperSocket configuration, the OS socket configuration.

And at Friday afternoon, I found the killer: **the azure load balancer make this happen**.

Quote from [official document](https://azure.microsoft.com/en-us/blog/new-configurable-idle-timeout-for-azure-load-balancer/):

> In its default configuration, Azure Load Balancer has an ‘idle timeout’ setting of 4 minutes. This means that if you have a period of inactivity on your tcp or http sessions for more than the timeout value, there is no guarantee to have the connection maintained between the client and your service. When the connection is closed, your client application will get an error message like “The underlying connection was closed: A connection that was expected to be kept alive was closed by the server”. Then, I modified the previous Console server to Cloud Service worker role, and deployed it again. Each 4 minutes the client has no packet transport towards the server, the connection would be closed by Azure Load Balancer. I finally hit the point, wow, wonderful weekends, right?

The fix is very straightforward,

```
<InputEndpoint name="input-endpoint-name" protocol="tcp"  port="8777" idleTimeoutInMinutes="11" />
```

The reason I set it to 11 minutes is that, SuperSocket would send the TCP-KeepAlive flag every 10 minutes in default, of course you can change it within 4 minutes also fixed this issue.

### Conclusion

Well, I am still very happy I fixed this Super-Bug which puzzled the whole team for a very long time. I also learned a lot from this experience:

*   TCP/IP is great. I am very appreciate this chance to drive me to learn TCP/IP socket programming, and also get familiar with the usage of great tool Wireshark.
*   When trying reproduce the bug, setup exactly same environment as the production, it's not wasting time. In most time, it saved your life.
*   Google/MSDN/StackOverflow is not god, it hides too much important clues if you didn't go to the right way. Don't rely on them, just make use of them.
*   Reading source code is helpful, even it's not the cause of bugs, you can make sure that it really does not, otherwise how do you know that. Besides, you can learn a lot from the popular framework anyway.