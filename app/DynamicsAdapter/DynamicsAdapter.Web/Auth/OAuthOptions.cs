using System.ComponentModel.DataAnnotations;

namespace DynamicsAdapter.Web.Auth
{
    public class OAuthOptions
    {

        [Required]
        public string OAuthUrl { get; set; }

        [Required]
        public string ResourceUrl { get; set; }
        
        [Required]
        public string ClientId { get; set; }
        
        [Required]
        public string Secret { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Range(1,10)]
        public int TokenTimeout { get; set; } = 1;
    }
}