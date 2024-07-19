using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public static class MessageHtmlBodies
    {
        public static string GetConfirmationBody(string link)
        {
            return $@"
            <html>
            <body>
                <p>Dear user,</p>
                <p>Please click the link below to confirm your email address:</p>
                <p><a href='{link}'>Confirm Email</a></p>
                <p>Thank you!</p>
            </body>
            </html>";
        }

        public static string GetResetPasswordBody(string link)
        {
            return $@"
            <html>
            <body>
                <p>Dear user,</p>
                <p>We received a request to reset your password. Please click the link below to set a new password:</p>
                <p><a href='{link}'>Reset password</a></p>
                <p>If you did not request a password reset you can ignore this email.</p>
            </body>
            </html>";
        }
    }
}
