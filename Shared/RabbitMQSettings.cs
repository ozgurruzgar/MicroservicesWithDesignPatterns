namespace Shared
{
    public class RabbitMQSettings
    {
        public const string OrderSaga = "order-saga-queue";
        public const string StockRollBackMessageQueueName = "stock-rollback-queue";


        public const string StockReservedEventQueueName = "stock-reserved-queue";
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";
        public const string OrderRequestCompletedEventQueueName = "order-request-completed-queue";
        public const string OrderPaymentFailedEventQueueName = "order-request-failed-queue";
        public const string StockNotReservedEventQueueName = "order-stock-not-reserved-queue";
        public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";
        public const string PaymentStockReservedRequestQueueName = "order-stock-reserved-request-queue";
    }
}
