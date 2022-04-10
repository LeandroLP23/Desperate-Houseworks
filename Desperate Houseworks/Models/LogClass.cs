using System;
using Xamarin.Forms;

namespace Desperate_Houseworks.Models
{
    /* Task svolta.*/
    public class LogClass : FamilyMember
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public bool Verified { get; set; }
    }

    /* Task svolta con proprietà 'grafiche'.*/
    public class LogView : LogClass
    {
        public string IconUser => FamilyMethods.MemberInt2Pic(Picture);
        public string IconTask => GestioneEnum.Cat2Icon(Category);

        public Color VerifiedColor
        {
            get
            {
                if (Verified)
                {
                    return Color.LightGreen;
                }
                else
                {
                    return new Color(1, 0, 0.15, 0.5);
                }
            }
        }
    }
}
