using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lambot.Template.Database.Entity;

public class FastLearningRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }


    [Required]
    public string Question { get; set; } = default!;

    [Required]
    public string Answer { get; set; } = default!;

    [Required]
    public long GroupId { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public DateTimeOffset Time { get; set; }
}