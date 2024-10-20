﻿namespace SeminarHub.Constants
{
    public static class CommonConstants
    {
        // Seminar constants
        public const int TopicMinLength = 3;
        public const int TopicMaxLength = 100;

        public const int LecturerMinLength = 5;
        public const int LecturerMaxLength = 60;

        public const int DetailsMinLength = 10;
        public const int DetailsMaxLength = 500;

        public const string DefaultDateFormat = "dd/MM/yyyy HH:mm";

        public const int DurationMinValue = 30;
        public const int DurationMaxValue = 180;

        // Category constants
        public const int NameMinLength = 3;
        public const int NameMaxLength = 50;

        // Error message
        public const string DefaultErrorMessage = "Field {0} must be between {2} and {1} characters!";
    }
}
