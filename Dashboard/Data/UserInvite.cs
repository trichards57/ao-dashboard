using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dashboard.Data;

public class UserInvite
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(254)]
    public required string Email { get; set; }

    public string? RoleId { get; set; }

    [NotNull]
    public IdentityRole? Role { get; set; }

    public DateTimeOffset Created { get; set; }

    public string? CreatedById { get; set; }

    [NotNull]
    public ApplicationUser? CreatedBy { get; set; }

    public DateTimeOffset? Accepted { get; set; }
}
