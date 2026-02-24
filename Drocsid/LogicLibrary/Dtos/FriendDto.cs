using System.ComponentModel.DataAnnotations;

namespace LogicLibrary.Dtos
{
    public class FriendDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int User1Id { get; set; }

        [Required]
        public int User2Id { get; set; }
    }
}
