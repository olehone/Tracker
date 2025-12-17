using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Entities.Queries;

public class SearchUsersByUsernamePartQuery()
{
    [Required]
    [MinLength(3)]
    public required string Username { get; set; }
    [Required]
    public required int Page { get; set; }
    [Required]
    public required int AmountInPage { get; set; }
}
