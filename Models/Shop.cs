namespace Barkodai.Models
{
    public class Shop
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }

        public string getLogo()
        {
            return "/images/" + this.name + ".png";
        }
    }
}