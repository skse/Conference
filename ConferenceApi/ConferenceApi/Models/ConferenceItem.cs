using System.ComponentModel.DataAnnotations;

namespace ConferenceApi.Models
{
    /// <summary>
    /// Root item to be stored in database.
    /// </summary>
    public class ConferenceItem
    {
        [Key] [Required] [StringLength(5)] public string Section { get; set; }

        [Required] public InfoItem Info { get; set; }
    }
}