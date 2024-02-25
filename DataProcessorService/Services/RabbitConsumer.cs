using System.Text.Json;
using System.Text.Json.Nodes;
using DataProcessorService.Extensions;
using DataProcessorService.Models;
using DataProcessorService.ModuleHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DataProcessorService.Services;

public class RabbitConsumer : IDisposable
{
    private const string SensorEventName = "SensorEvent";
    private const string SensorChangeName = "SensorChange";
    private const string SensorChangeErrorName = SensorChangeName + "Error";
    private readonly ILogger<RabbitConsumer> _logger;
    
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private IModel? _channel;
    private IConnection? _connection;

    private bool _isDisposed;

    public RabbitConsumer(
        RabbitMqConnectionConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        var factory = configuration.CreateConnectionFactory();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        DeclareRoute(_channel);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Consume()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(RabbitConsumer));

        _channel!.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                _logger.LogInformation("Start consume");
                var data = ea.Body.ToArray();
                var node = JsonNode.Parse(data)
                           ?? throw new JsonException("Not valid format");

                var deviceStatuses = node.GetNode("DeviceStatus").AsArray()
                                     ?? throw new JsonException("'DeviceStatus' must be array type value");

                var modules = deviceStatuses
                    .Where(childNode => childNode is not null)
                    .Select(childNode =>
                    {
                        var moduleCategoryId = childNode!.GetNode("ModuleCategoryID").GetValue<string>();
                        if (moduleCategoryId is null)
                        {
                            throw new JsonException("'ModuleCategoryId' must be string type value");
                        }

                        var rapidControlStatus = childNode!.GetNode("RapidControlStatus").AsObject();
                        if (rapidControlStatus.Count != 1)
                        {
                            throw new ArgumentException("Unexpected rapid control status count");
                        }

                        var moduleState = rapidControlStatus.First().Value?.GetNode("ModuleState").GetValue<string>();
                        if (moduleState is null)
                        {
                            throw new JsonException("'ModuleState' must be string type value");
                        }

                        return new ModuleStatus
                        {
                            CategoryId = moduleCategoryId,
                            State = moduleState,
                        };
                    });

                await using var service = _serviceScopeFactory.CreateAsyncScope();
                var moduleHandler = service.ServiceProvider.GetRequiredService<IModuleHandler>();

                var moduleHandleTasks = modules.Select(module => moduleHandler.HandleAsync(module));

                await Task.WhenAll(moduleHandleTasks);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception not handled");

                _channel.BasicNack(ea.DeliveryTag, false, false);
                _channel.BasicPublish(
                    string.Empty,
                    SensorChangeErrorName,
                    body: ea.Body);
            }
        };

        _channel.BasicConsume(
            SensorChangeName,
            false,
            consumer);
    }

    private static void DeclareRoute(IModel channel)
    {
        channel.ExchangeDeclare(
            SensorEventName,
            ExchangeType.Fanout,
            true,
            false);

        channel.QueueDeclare(
            SensorChangeName,
            true,
            autoDelete: false,
            exclusive: false);

        channel.QueueDeclare(
            SensorChangeErrorName,
            true,
            autoDelete: false,
            exclusive: false);

        channel.QueueBind(
            SensorChangeName,
            SensorEventName,
            string.Empty);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            DisposeInstance(ref _channel);
            DisposeInstance(ref _connection);
        }


        _isDisposed = true;

        return;

        static void DisposeInstance<TDisposable>(ref TDisposable? disposable)
            where TDisposable : class, IDisposable
        {
            if (disposable is null)
            {
                return;
            }

            disposable.Dispose();
            disposable = null;
        }
    }
}