using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using EntreLaunch.DataAnnotations;
using EntreLaunch.Interfaces;

namespace EntreLaunch.Entities;

[Table("link")]
[Index(nameof(Uid), IsUnique = true)]
[SupportsChangeLog]
public class Link : SharedData
{
    [Required]
    [Searchable]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Searchable]
    public string Uid { get; set; } = string.Empty;

    public virtual BaseEntity SharedData { get; set; } = new BaseEntity();

    [Required]
    [Searchable]
    public string Destination { get; set; } = string.Empty;
}
