namespace Homies.Constants
{
    public static class CommonConstants
    {
        // Event constants
        public const int EventNameMinLength = 5;
        public const int EventNameMaxLength = 20;
               
        public const int DescriptionMinLength = 15;
        public const int DescriptionMaxLength = 150;
              
        public const string DefaultDateFormat = "yyyy-MM-dd H:mm";

        // Type constants
        public const int TypeNameMinLength = 5;
        public const int TypeNameMaxLength = 15;

        // Error message
        public const string errorMessage = "Field {0} must be between {2} and {1} characters!";
    }
}
