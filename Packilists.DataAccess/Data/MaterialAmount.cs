using System.ComponentModel.DataAnnotations;

namespace Packilists.Shared.Data
{
    public class MaterialAmount
    {
        [Required(ErrorMessage = "Field should not be empty")]
        public string MaterialName { get; set; }
        [Required(ErrorMessage = "Field should not be empty")]
        public float Amount { get; set; }
        [Required(ErrorMessage = "Field should not be empty")]
        public string Unit { get; set; }
        public Material Material { get; set; }
        public int MaterialAmountId { get; set; }
        public int MaterialId { get; set; }
        public int? ItemId { get; set; }
        public int? PacklisteId { get; set; }
    }
}
