using System.Collections.Generic;

namespace Desperate_Houseworks.Models
{
    /* Membro della famiglia.*/
    public class FamilyMember
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public int Picture { get; set; }

        public override string ToString()
        {
            return Nickname;
        }
    }

    /* Utente.*/
    public class User : FamilyMember
    {
        public string Token { get; set; }
        public string FamilyName { get; set; }

        public User()
        {
            Username = "";
            Nickname = "";
            Token = "";
            FamilyName = null;
            Picture = -1;
        }
    }

    /* Classe per la visualizzazione degli utenti.*/
    public class FamilyMemberView : FamilyMember
    {
        public string StringPicture => FamilyMethods.MemberInt2Pic(Picture);
    }

    /* Invito per entrare in famiglia.*/
    public class JoinFamilyRequest : FamilyMember
    {
        public string FamilyName { get; set; }
        public int FamilyCode { get; set; }
    }

    public static class FamilyMethods
    {
        public static List<string> GetEveryUserPic()
        {
            List<string> lista = new List<string>();
            for (int i = 1; i < 9; i++)
            {
                lista.Add(MemberInt2Pic(i));
            }
            return lista;
        }

        public static string MemberInt2Pic(int Picture)
        {
            switch (Picture)
            {
                case 1:
                    return "icon_donna.png";
                case 2:
                    return "icon_donna2.png";
                case 3:
                    return "icon_donna3.png";
                case 4:
                    return "icon_donna4.png";
                case 5:
                    return "icon_uomo.png";
                case 6:
                    return "icon_uomo2.png";
                case 7:
                    return "icon_uomo3.png";
                case 8:
                    return "icon_uomo4.png";
                default:
                    return "icon_unknown.png";
            }
        }

        public static int MemberPic2Int(string Picture)
        {
            switch (Picture)
            {
                case "icon_donna.png":
                    return 1;
                case "icon_donna2.png":
                    return 2;
                case "icon_donna3.png":
                    return 3;
                case "icon_donna4.png":
                    return 4;
                case "icon_uomo.png":
                    return 5;
                case "icon_uomo2.png":
                    return 6;
                case "icon_uomo3.png":
                    return 7;
                case "icon_uomo4.png":
                    return 8;
                default:
                    return -1;
            }
        }
    }
}
