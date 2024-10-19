namespace DeskMarket.Constants
{
    public static class CommonConstants
    {
        // Product constants
        public const int ProductNameMinLength = 2;
        public const int ProductNameMaxLength = 60;

        public const int DescriptionMinLength = 10;
        public const int DescriptionMaxLength = 250;

        public const double PriceMinValue = 1.00;
        public const double PriceMaxValue = 3000.00;

        public const string DefaultDateFormat = "dd-MM-yyyy";

        // Category constants
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 20;

        // Error message
        public const string DefaultErrorMessage = "{0} must be between {2} and {1} characters!";
    }
}
