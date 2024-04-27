using System.ComponentModel.DataAnnotations;

namespace Model.Util;

public class MailSettings {
    [EmailAddress, Required] public string From{ get; set; } = null!;

    [Required] public string Host{ get; set; } = null!;
    [Required] public int Port{ get; set; }
    [Required] public string Username{ get; set; } = null!;
    [Required] public string Password{ get; set; } = null!;
}