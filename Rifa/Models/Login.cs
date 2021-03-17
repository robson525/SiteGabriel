using Newtonsoft.Json;

namespace Rifa.Models
{
    public class Login
    {
        public string User { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}