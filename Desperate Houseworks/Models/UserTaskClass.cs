using System;
using System.Collections.Generic;

namespace Desperate_Houseworks.Models
{
    /* Classe contenente la task 'vera e propria'.
     * (Si parla delle task presenti della schermata MainTaskPage)*/
    public class UserTaskClass : AbTaskClass
    {
        //Colore della categoria
        public string Color { get; set; }

        public override bool Equals(object obj)
        {
            Type refClass = GetType();
            Type objClass = obj.GetType();
            if (refClass.Equals(objClass))
            {
                UserTaskClass o = (UserTaskClass)obj;
                return Name.Equals(o.Name)
                    && Date.Equals(o.Date)
                    && Description.Equals(o.Description);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 872520557;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Category);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Icon);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(User);
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + Verified.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Color);
            return hashCode;
        }


    }
}
