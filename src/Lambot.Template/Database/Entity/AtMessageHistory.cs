using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lambot.Template.Database.Entity;

public class AtMessageHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public long MessageId { get; set; }

    [Required]
    public long GroupId { get; set; }

    [Required]
    public long UserId { get; set; }
}
