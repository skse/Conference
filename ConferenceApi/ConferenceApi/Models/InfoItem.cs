using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferenceApi.Models
{
    /// <summary>
    /// Subentity for ConferenceItem.
    /// </summary>
    public struct InfoItem
    {
        [Required] [StringLength(20)] public string Name { get; set; }

        [Required] [StringLength(20)] public string City { get; set; }

        [Required] [StringLength(50)] public string Location { get; set; }
    }
}