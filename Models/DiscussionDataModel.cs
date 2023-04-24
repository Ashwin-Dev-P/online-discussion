using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Discussion.Models
{
    public class DiscussionDataModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        public string Answer { get; set; }

        [ForeignKey("DiscussionId")]
        public int  DiscussionId { get; set; }
        public DiscussionModel Discussion { get; set; }

        [ForeignKey("IdentityUserId")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
