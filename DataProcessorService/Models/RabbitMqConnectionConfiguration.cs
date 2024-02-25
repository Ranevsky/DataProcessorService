using RabbitMQ.Client;

namespace DataProcessorService.Models;

public class RabbitMqConnectionConfiguration
{
    public string? HostName { get; set; }
    public int? Port { get; set; }
    public string? VirtualHost { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = HostName ?? "guest",
            Port = Port ?? AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = VirtualHost ?? string.Empty,
            UserName = UserName ?? "",
            Password = Password ?? "guest",
        };
    }
}