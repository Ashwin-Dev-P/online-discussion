using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Discussion.Models
{
    public class DiscussionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        public string Title { get; set; }

        [ForeignKey("IdentityUserId")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }


    }
}
