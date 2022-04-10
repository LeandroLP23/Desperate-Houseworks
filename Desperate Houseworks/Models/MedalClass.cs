namespace Desperate_Houseworks.Models
{
    public class MedalClass
    {
        public string User { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Icon { get => GetIcon(); set => Icon = value; }
        public string Milestone { get => GetMilestone(); set => Milestone = value; }


        public MedalClass(string username, string name, int quantity)
        {
            User = username;
            Name = name;
            Quantity = quantity;
        }

        public string GetMilestone()
        {
            if (Quantity < 10)
            {
                return "/10";
            }
            else if (Quantity < 25)
            {
                return "/25";
            }
            else if (Quantity < 50)
            {
                return "/50";
            }
            else if (Quantity < 100)
            {
                return "/100";
            }
            else
            {
                return "";
            }
        }

        public string GetIcon()
        {
            if (Quantity < 10)
            {
                return "icon_grigia.png";
            }
            else if (Quantity < 25)
            {
                return "icon_bronzo.png";
            }
            else if (Quantity < 50)
            {
                return "icon_argento.png";
            }
            else if (Quantity < 100)
            {
                return "icon_oro.png";
            }
            else
            {
                return "icon_platino.png";
            }
        }

        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MedalClass m = (MedalClass)obj;

            return Name.Equals(m.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
