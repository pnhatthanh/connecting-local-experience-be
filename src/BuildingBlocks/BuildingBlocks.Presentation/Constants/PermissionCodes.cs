namespace BuildingBlocks.Presentation.Constants
{
    public static class PermissionCodes
    {
        public const string IAM_ACCOUNT_UPDATE_STATUS = "Account.UpdateStatus";
        public const string USER_USER_VIEW_ALL = "User.ViewAll";
        public const string USER_USER_VIEW_DETAIL = "User.ViewDetail";
        public const string USER_USER_BECOME_HOST = "User.BecomeHost";
        public const string USER_HOST_VIEW_ALL = "Host.ViewAll";
        public const string USER_HOST_VIEW_DETAIL = "Host.ViewDetail";
        public const string USER_HOST_VERIFY = "Host.Verify";
        // ==================== EXPERIENCE SERVICE ====================
        // Experience Management
        public const string EXPERIENCE_EXPERIENCE_CREATE = "Experience.Create";
        public const string EXPERIENCE_EXPERIENCE_UPDATE = "Experience.Update";
        public const string EXPERIENCE_EXPERIENCE_DELETE = "Experience.Delete";
        public const string EXPERIENCE_EXPERIENCE_VIEW_ADMIN = "Experience.ViewAdmin";
        public const string EXPERIENCE_EXPERIENCE_UPDATE_STATUS = "Experience.UpdateStatus";
        // Review Management
        public const string EXPERIENCE_REVIEW_UPDATE_STATUS = "Review.UpdateStatus";
        public const string EXPERIENCE_REVIEW_VIEW_ALL = "Review.ViewAll";

        // ==================== BOOKING SERVICE ====================
        // Booking Management
        public const string BOOKING_BOOKING_CREATE = "Booking.Create";
        public const string BOOKING_BOOKING_VIEW_BY_HOST = "Booking.ViewByHost";
        public const string BOOKING_BOOKING_CANCEL = "Booking.Cancel";
        public const string BOOKING_BOOKING_CHECK_COMPLETED = "Booking.CheckCompleted";
        public const string BOOKING_BOOKING_TOGGLE_STATUS = "Booking.ToggleStatus";
    }
}
