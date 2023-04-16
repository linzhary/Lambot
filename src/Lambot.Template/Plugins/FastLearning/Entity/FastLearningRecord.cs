using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lambot.Template.Plugins.FastLearning.Entity;

[PrimaryKey(nameof(Question), nameof(GroupId), nameof(UserId))]
public class FastLearningRecord
{
    [Required]
    public string Question { get; set; }

    [Required]
    public string Answer { get; set; }

    [Required]
    public long GroupId { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public DateTimeOffset Time { get; set; }
}