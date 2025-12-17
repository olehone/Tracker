using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Entities.Queries;

public class GetUserByIdQuery
{
    [Required]
    public Guid Id { get; set; }
}
