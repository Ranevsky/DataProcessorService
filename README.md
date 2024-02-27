# Data Processor Service

## Запуск проекта

Для запуска этого сервиса необходимо иметь
[**.NET 8**](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) и [**RabbitMQ**](https://www.rabbitmq.com/docs/download).

Запуск проекта:
```sh
dotnet run -c Release --project DataProcessorService
```

## Конфигурация

Для изменения конфигурации перейдите в файл **appsettings.json**.

> [!NOTE]
> Предоставленные данные являются тестовыми и должны быть заменены на ваши.

### Логирование

```json
"Logging": {
  "LogLevel": {
    "Default": "Information"
  }
}
```

Вы можете настроить уровень логирования; они идут в порядке возрастания:

Trace - Debug - Information - Warning - Error - Critical - None

### Подключение к **RabbitMq**

Убедитесь, что данные подключения соответствуют вашей настройке **RabbitMQ**; предоставленные данные являются конфигурацией по умолчанию.

```json
"Rabbit": {
  "HostName": "localhost",
  "Port": 5672,
  "VirtualHost": "/",
  "UserName": "guest",
  "Password": "guest"
}
```


### Подключение к базе данных

Этот сервис использует базу данных **SQLite**.

Введите строку подключения здесь:

```json
"ConnectionString": {
  "SQLite": "Data Source=./../../../../Sensors.db"
}
```

## Примечание
Файл **Sensors.db** представлен лишь в качестве результата выполнения программы.
