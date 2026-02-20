namespace CinemaTicketBooking.ViewModels
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }
        [DataType(DataType.Password)] 
        public string Password { get; set; } = string.Empty;
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
