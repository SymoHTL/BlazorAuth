using Microsoft.AspNetCore.Identity.UI.Services;

namespace Auth.Services;

public class IdentityEmailSender(IEmailSender sender)  : IEmailSender<ApplicationUser>  {
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) {
        await sender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) {
        await sender.SendEmailAsync(email, "Reset your password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) {
        await sender.SendEmailAsync(email, "Reset your password",
            $"Please reset your password using the following code: {resetCode}");
    }
}