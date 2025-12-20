namespace DeliverySystem.Order
{
    public class StandardOrder : Order
{
        private decimal baseTotal;

        public StandardOrder(int orderId) : base(orderId)
        {
        }

        public StandardOrder(decimal baseTotal, int orderId): base(orderId)
        {
            this.baseTotal = baseTotal;
        }

        public StandardOrder(object orderId1, int orderId) : base(orderId)
        {}

        public override void CalculateBaseTotal()
    {
        baseTotal = Items.Sum(i => i.GetPrice());
    }
}
}