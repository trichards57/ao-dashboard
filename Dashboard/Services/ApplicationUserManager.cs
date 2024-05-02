// -----------------------------------------------------------------------
// <copyright file="ApplicationUserManager.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Dashboard.Services;

/// <summary>
/// Application specific user manager that includes custom logic for handling SJA email addresses.
/// </summary>
public class ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
    /// <inheritdoc/>
    public override Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Task.FromResult(string.Empty);
        }

        return base.GeneratePasswordResetTokenAsync(user);
    }

    /// <inheritdoc/>
    public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "EmailDomain", Description = "@sja.org.uk email addresses are not allowed for local accounts." }));
        }

        return base.CreateAsync(user, password);
    }

    /// <inheritdoc/>
    public override Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "EmailDomain", Description = "@sja.org.uk passwords must be changed with Microsoft." }));
        }

        return base.ResetPasswordAsync(user, token, newPassword);
    }

    /// <inheritdoc/>
    public override Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string password) => throw new InvalidOperationException("Adding passwords is not supported for Microsoft accounts. Use Microsoft to manage your password.");

    /// <inheritdoc/>
    public override Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "EmailDomain", Description = "@sja.org.uk passwords must be changed with Microsoft." }));
        }

        return base.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    /// <inheritdoc/>
    public override Task<IEnumerable<string>?> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int number)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("@sja.org.uk two-factor authentication must be managed with Microsoft.");
        }

        return base.GenerateNewTwoFactorRecoveryCodesAsync(user, number);
    }

    /// <inheritdoc/>
    public override Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
    {
        if (user.Email?.EndsWith("@sja.org.uk", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("@sja.org.uk two-factor authentication must be managed with Microsoft.");
        }

        return base.GenerateTwoFactorTokenAsync(user, tokenProvider);
    }
}
