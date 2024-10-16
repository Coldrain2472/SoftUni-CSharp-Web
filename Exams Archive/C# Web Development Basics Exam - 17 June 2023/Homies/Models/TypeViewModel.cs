namespace Homies.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Homies.Constants.CommonConstants;

    public class TypeViewModel
    {
        public int Id { get; set; }

        [StringLength(TypeNameMaxLength, MinimumLength = TypeNameMinLength, ErrorMessage = errorMessage)]
        public string Name { get; set; } = null!;
    }
}
