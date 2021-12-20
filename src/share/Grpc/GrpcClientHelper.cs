using System.Collections.Concurrent;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace Laobian.Share.Grpc;

public static class GrpcClientHelper
{
    private static readonly ConcurrentDictionary<string, GrpcChannel> Channels;

    static GrpcClientHelper()
    {
        Channels = new ConcurrentDictionary<string, GrpcChannel>();
    }

    public static T CreateClient<T>(string address) where T : class
    {
        var channel = Channels.GetOrAdd(address, s =>
        {
            var c = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                MaxRetryAttempts = 3,
                MaxSendMessageSize = 1024 * 1024 * 20
            });

            return c;
        });

        return channel.CreateGrpcService<T>();
    }
}