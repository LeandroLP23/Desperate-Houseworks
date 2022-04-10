using Desperate_Houseworks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Desperate_Houseworks.Services
{
    internal class ClassGlobal
    {
        public static User ActualUser;
        public static bool isLogged;

        public static string descrizioneVuota = "Non hai particolari istruzioni per questo compito.\n" +
                        "Fallo come meglio credi!";

        public static List<FamilyMember> UserList4Picker = new List<FamilyMember>();

        /* Recupero membri della famiglia.*/
        public static async Task GettingUsers()
        {
            if (isLogged)
            {

                //Aggiungo il resto dei familiari
                UserList4Picker = await DesperateDB.GetFamily();

                //Aggiungo il nome dell'utente stesso nella lista
                UserList4Picker.Insert(0, ActualUser);

            }
            else
            {
                //Svuoto la lista degli utenti della casa
                UserList4Picker.Clear();

                //Non hai gli elementi della famiglia, è presente solo lui
                UserList4Picker.Add(ActualUser);
            }
        }

        /* Animazione shacking dell'elemento. */
        internal static async void Shaking_Animation(VisualElement elemento)
        {
            uint timeout = 50;

            await elemento.TranslateTo(-15, 0, timeout);

            await elemento.TranslateTo(15, 0, timeout);

            await elemento.TranslateTo(-10, 0, timeout);

            await elemento.TranslateTo(10, 0, timeout);

            await elemento.TranslateTo(-5, 0, timeout);

            await elemento.TranslateTo(5, 0, timeout);

            elemento.TranslationX = 0;
        }
    }
}
