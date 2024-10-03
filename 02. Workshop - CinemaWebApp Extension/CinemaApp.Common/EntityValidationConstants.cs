namespace CinemaApp.Common
{
    public static class EntityValidationConstants
    {
        public static class Movie
        {
            public const int TitleMinLength = 1;
            public const int TitleMaxLength = 50;

            public const int GenreMinLength = 5;
            public const int GenreMaxLength = 20;

            public const int DurationMinValue = 1;
            public const int DurationMaxValue = 999;

            public const int DirectorNameMinLength = 5;
            public const int DirectorNameMaxLength = 80;

            public const int DescriptionMinLenght = 10;
            public const int DescriptionMaxLength = 500;
        }

        public static class Cinema
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;

            public const int LocationMinLength = 2;
            public const int LocationMaxLength = 200;
        }
    }
}
