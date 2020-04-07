namespace Packilists.Shared.Data
{
    public class MaterialAmount
    {
        public string MaterialName { get; set; }
        public float Amount { get; set; }
        public string Unit { get; set; }
        public Material Material { get; set; }
        public int MaterialAmountId { get; set; }
        public int MaterialId { get; set; }
        public int? ItemId { get; set; }
        public int? PacklisteId { get; set; }
    }
}
