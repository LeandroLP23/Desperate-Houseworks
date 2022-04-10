using Desperate_Houseworks.Services;
using System;
using System.Collections.Generic;

namespace Desperate_Houseworks.Models
{
    public enum EnumTask
    {
        Lavatrice,
        Lavastoviglie,
        Apparecchiare,
        Pranzo,
        Cena,
        Piante,
        Passeggiata,
        Spesa,
        Pulizie,
        Letto,
    };

    public static class GestioneEnum
    {
        public static string Name2Cat(string s)
        {
            switch (s)
            {
                case "Lavatrice": return "Bagno";
                case "Lavastoviglie": return "Cucina";
                case "Apparecchiare": return "Cucina";
                case "Pranzo": return "Cucina";
                case "Cena": return "Cucina";
                case "Spesa": return "Cucina";
                case "Piante": return "Esterni";
                case "Passeggiata": return "Esterni";
                case "Pulizie": return "Casa";
                case "Letto": return "Camera";

                default: return "Altro";
            }
        }
        public static string Name2Desc(string s)
        {
            switch (s)
            {
                case "Lavatrice":
                    return "Lavatrice di colorati, usa l'acchiappacolore.";
                case "Lavastoviglie":
                    return "Caricare la lavastoviglie, sciacquare un poco i piatti prima di caricarla.";
                case "Apparecchiare":
                    return "Pulire e apparecchiare la tavola.";
                case "Pranzo":
                    return "Prepara il pranzo per tutti, siamo affamati.";
                case "Cena":
                    return "Prepara la cena per tutti, cucina ciò che preferisci.";
                case "Piante":
                    return "Annaffia bene le piante, controlla anche le erbacce.";
                case "Passeggiata":
                    return "Fai fare un giro al cane, ricordati le bustine.";
                case "Spesa":
                    return "La lista della spesa è vuota. Spero tu abbia una buona memoria!";
                case "Pulizie":
                    return "Passare l'aspirapolvere e lo straccio.";
                case "Letto":
                    return "Rifai il letto.";

                default:
                    return ClassGlobal.descrizioneVuota;
            }
        }

        public static string Cat2Icon(string s)
        {
            switch (s)
            {
                case "Cucina": return "icon_cucina.png";
                case "Bagno": return "icon_bagno.png";
                case "Esterni": return "icon_esterni.png";
                case "Casa": return "icon_casa.png";
                case "Camera": return "icon_bedroom.png";

                default: return "icon_altro.png";
            }
        }

        public static string Cat2Color(string s)
        {
            switch (s)
            {
                case "Cucina": return "#c29bfc";
                case "Bagno": return "#94d6fe";
                case "Esterni": return "#6fd9a5";
                case "Casa": return "#fa9959";
                case "Camera": return "#f7cae2";

                default: return "#fadb59";
            }
        }

        public static List<string> Cat2IList()
        {
            HashSet<string> category = new HashSet<string>();

            //array di stringhe contenente i nomi delle task degli enum
            string[] ens = Enum.GetNames(typeof(EnumTask));

            foreach (string en in ens)
            {
                //salvo la categoria 
                string cat = Name2Cat(en);
                category.Add(cat);
            }
            List<string> appoggio = new List<string>(category)
            {
                "Altro"
            };
            return appoggio;
        }
    }
}
