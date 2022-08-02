using Microsoft.AspNetCore.Identity;

namespace Visionmore.Data {
    public class User : IdentityUser {
        public string Name { get; set; }
    }
}
