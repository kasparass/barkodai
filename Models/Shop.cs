namespace Barkodai.Models
{
    public class Shop
    {
        public double price;
        public string name { get; set; }

        public string getLogo()
        {
            return "/images/" + this.name + ".png";
        }
    }
}