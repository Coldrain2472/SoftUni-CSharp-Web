namespace Homies.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Homies.Constants.CommonConstants;

    public class EventAddViewModel
    {
        [Required]
        [StringLength(EventNameMaxLength, MinimumLength = EventNameMinLength, ErrorMessage = errorMessage)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = errorMessage)]
        public string Description { get; set; } = null!;

        [Required]
        public string Start {  get; set; } = null!;

        [Required]
        public string End { get; set; } = null!;

        [Required]
        public int TypeId { get; set; }

        public ICollection<TypeViewModel> Types {  get; set; } = new HashSet<TypeViewModel>();
    }
}
