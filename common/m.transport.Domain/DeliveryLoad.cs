namespace m.transport.Domain
{
    public class DeliveryLoad
    {
        public DatsVehicleV5[] Vehicles { get; set; }
        public string CustomerSignatureFileName { get; set; }
        public byte[] CustomerSignatureData { get; set; }
        public string DeliverySignatureFileName { get; set; }
        public byte[] DeliverySignatureData { get; set; }
        public int DropoffLocationId { get; set; }
        public DatsRunReload[] Reload { get; set; }
    }
}