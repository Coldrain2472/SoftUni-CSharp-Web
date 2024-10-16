namespace SoftUniBazar.Constants
{
    public static class CommonConstants
    {
        // Ad constants
        public const int AdNameMinLength = 5;
        public const int AdNameMaxLength = 25;

        public const int AdDescriptionMinLength = 15;
        public const int AdDescriptionMaxLength = 250;

        public const string DefaultDateFormat = "yyyy-MM-dd H:mm";

        // Category constants
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 15;

        // Error message
        public const string DefaultErrorMessage = "Field {0} must be between {2} and {1} characters!";
    }
}