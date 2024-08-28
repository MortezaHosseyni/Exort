using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Shared.Enums.User;
using Shared.Utilities;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(70)] public string? FirstName { get; private set; }
        [MaxLength(80)] public string? LastName { get; private set; }

        [MaxLength(10)] public string? NationalCode { get; private set; }

        [Required][MaxLength(80)] public string Username { get; private set; }
        [MaxLength(100)] public string? Bio { get; private set; }
        [MaxLength(100)] public string? Address { get; private set; }

        [Required][EmailAddress] public required string Email { get; set; }
        [Required] public required bool EmailConfirmation { get; set; }
        [MaxLength(6)] public string? ResetPasswordCode { get; set; }
        public DateTime ResetPasswordExpireTime { get; set; }

        [Phone] public string? PhoneNumber { get; private set; }
        [Required] public required bool PhoneNumberConfirmation { get; set; }

        [Required][PasswordPropertyText] public required string Password { get; set; }

        [MaxLength(500)] public string? Avatar { get; private set; }

        public Dictionary<string, string>? SocialMedias { get; private set; }

        public UserGender Gender { get; private set; }
        public UserStatus Status { get; private set; }
        [MaxLength(80)] public string? StatusDescription { get; private set; }
        public DateTime Birthdate { get; set; }
        public string? RegisterIp { get; private set; }

        public DateTime LastLogin { get; set; }
        public string? LastLoginIp { get; private set; }

        public string? UserAgent { get; set; }

        public ICollection<Friend>? Friends { get; set; }

        public User(
            string? firstName,
            string? lastName,
            string? nationalCode,
            string username,
            string? bio,
            string? address,
            string? phoneNumber,
            string? avatar,
            Dictionary<string, string>? socialMedias,
            UserGender gender,
            UserStatus status,
            string? statusDescription,
            string? registerIp,
            string? lastLoginIp)
        {
            // Clarify first name
            FirstName = Sanitize.Clarify(firstName);

            // Clarify last name
            LastName = Sanitize.Clarify(lastName);

            // Clarify national code
            NationalCode = Sanitize.Clarify(nationalCode);

            // Check and clarify username
            if (!Regex.IsMatch(username, @"^[a-z][a-z0-9_]*$"))
                throw new Exception("Username is invalid.");
            Username = Sanitize.Clarify(username);

            // Clarify bio
            Bio = Sanitize.Clarify(bio);

            // Clarify address
            Address = Sanitize.Clarify(address);

            // Clarify phone number
            PhoneNumber = Sanitize.Clarify(phoneNumber);

            // Check and clarify avatar
            if (!string.IsNullOrEmpty(avatar) && !Regex.IsMatch(avatar, @"\.(png|jpg|bmp|webp)$"))
                throw new Exception("Avatar is invalid.");
            Avatar = Sanitize.Clarify(avatar);

            // Clarify social medias
            if (socialMedias != null && socialMedias.Any())
                SocialMedias = Sanitize.Clarify(socialMedias);

            // Check gender
            if (!Enum.IsDefined(typeof(UserGender), gender))
                throw new Exception("User gender is invalid.");
            Gender = gender;

            // Check status
            if (!Enum.IsDefined(typeof(UserStatus), status))
                throw new Exception("User status is invalid.");
            Status = status;

            // Clarify status description
            StatusDescription = Sanitize.Clarify(statusDescription);

            // Ip pattern
            const string ipPattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\." +
                                     @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\." +
                                     @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\." +
                                     @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$";

            // Check and clarify register ip
#if !DEBUG
            if (!string.IsNullOrEmpty(registerIp) && !Regex.IsMatch(registerIp, ipPattern))
                throw new Exception("Register ip is invalid.");
#endif
            RegisterIp = Sanitize.Clarify(registerIp);

            // Check and clarify last login ip
#if !DEBUG
            if (!string.IsNullOrEmpty(lastLoginIp) && !Regex.IsMatch(lastLoginIp, ipPattern))
                throw new Exception("Last login ip is invalid.");
#endif
            LastLoginIp = Sanitize.Clarify(lastLoginIp);

        }
    }
}
