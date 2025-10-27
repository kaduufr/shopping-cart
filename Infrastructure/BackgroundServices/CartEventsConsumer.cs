namespace Infrastructure.BackgroundServices;

public class CartEventsConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private ICartRepository _cartRepository;
    private readonly ILogger<CartEventsConsumer> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public CartEventsConsumer(IConsumer<Ignore, string> consumer, ICartRepository cartRepository, ILogger<CartEventsConsumer> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _consumer = consumer;
        _cartRepository = cartRepository;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("worker-cart-events");
        _logger.LogInformation("Esperando o serviço ser iniciado...");
        var appStarted = new TaskCompletionSource<object?>();
        _applicationLifetime.ApplicationStarted.Register(() => appStarted.TrySetResult(null));
        await appStarted.Task;

        _logger.LogInformation("Iniciando consumidor Kafka...");
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(stoppingToken);
            var eventMessage = JsonDocument.Parse(consumeResult.Message.Value);
            var eventType = eventMessage.RootElement.GetProperty("EventType").GetString();
            var data = eventMessage.RootElement.GetProperty("Data");

            switch (eventType)
            {
                case "ItemAdded":
                    var itemAddedEvent = JsonSerializer.Deserialize<CartItemAddedEvent>(data.GetRawText());
                    await HandleItemAddedAsync(itemAddedEvent);
                    break;
                case "ItemRemoved":
                    var itemRemovedEvent = JsonSerializer.Deserialize<CartItemRemovedEvent>(data.GetRawText());
                    await HandleItemRemovedAsync(itemRemovedEvent);
                    break;
            }
        }

        _consumer.Close();
    }

    private async Task HandleItemAddedAsync(CartItemAddedEvent? itemAddedEvent)
    {
        try
        {
            _logger.LogInformation("Processando evento ItemAdded para o usuário {UserEmail}", itemAddedEvent!.UserEmail);
            var cart = await _cartRepository.GetCartAsync(itemAddedEvent!.UserEmail);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemAddedEvent.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += itemAddedEvent.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem { ProductId = itemAddedEvent.ProductId, Quantity = itemAddedEvent.Quantity });
            }

            _logger.LogInformation("Atualizando carrinho para o usuário {UserEmail}", itemAddedEvent.UserEmail);
            await _cartRepository.UpdateCartAsync(cart);
            _logger.LogInformation("Carrinho atualizado com sucesso para o usuário {UserEmail}", itemAddedEvent.UserEmail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar evento ItemAdded: {Message}", e.Message);
            throw new BadHttpRequestException("Erro ao processar evento ItemAdded", e);
        }
    }

    private async Task HandleItemRemovedAsync(CartItemRemovedEvent? itemRemovedEvent)
    {
        try
        {
            var cart = await _cartRepository.GetCartAsync(itemRemovedEvent!.UserEmail);
            cart.Items.RemoveAll(i => i.ProductId == itemRemovedEvent.ProductId);
            await _cartRepository.UpdateCartAsync(cart);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar evento ItemRemoved: {Message}", e.Message);
            throw new BadHttpRequestException("Erro ao processar evento ItemRemoved", e);
        }
    }
}