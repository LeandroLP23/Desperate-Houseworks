using Desperate_Houseworks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desperate_Houseworks.Services
{
    internal class DesperateDB
    {
        private static string NotificationToken => App.notificationToken;
        private static readonly string urlBase = "https://vxnkf1zo3f.execute-api.eu-west-2.amazonaws.com/Beta/";

        /* Login dell'utente. */
        public static async Task<int> SignIn(string username, string password)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "login"));
                User utente = ClassGlobal.ActualUser;
                utente.Username = username;

                WebClient client = new WebClient();
                client.Headers.Add("u", username);
                client.Headers.Add("p", password);
                client.Headers.Add("tokenNotifications", NotificationToken);

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                JsonElement corpo = root.GetProperty("body");

                try
                {
                    Dictionary<string, string> dictionary = (Dictionary<string, string>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(Dictionary<String, String>));
                    dictionary.TryGetValue("Picture", out string picture);
                    dictionary.TryGetValue("Family", out string familyName);
                    dictionary.TryGetValue("Nickname", out string nickname);
                    dictionary.TryGetValue("Token", out string token);

                    utente.Picture = int.Parse(picture);
                    utente.FamilyName = familyName;
                    utente.Nickname = nickname;
                    //Inserimento nel database locale del token
                    utente.Token = token;

                    ClassGlobal.ActualUser = utente;

                    //Salvataggio utente
                    await App.DatabaseUser.SetUser(utente);
                }
                catch
                {
                }
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }

        }

        /* Login dell'utente attraverso token. */
        public static async Task<int> SignIn(string token)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "login2"));
                User utente = ClassGlobal.ActualUser;

                WebClient client = new WebClient();
                client.Headers.Add("token", token);
                client.Headers.Add("tokenNotifications", NotificationToken);

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                JsonElement corpo = root.GetProperty("body");
                try
                {
                    Dictionary<string, string> dictionary = (Dictionary<string, string>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(Dictionary<String, String>));
                    dictionary.TryGetValue("Picture", out string picture);
                    dictionary.TryGetValue("Family", out string familyName);
                    dictionary.TryGetValue("Nickname", out string nickname);

                    utente.Picture = int.Parse(picture);
                    utente.FamilyName = familyName;
                    utente.Nickname = nickname;
                }
                catch
                {
                }
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }

        }

        /* Registrazione dell'utente. */
        public static async Task<int> SignUp(string username, string password, string email)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "register"));

                var client = new WebClient();
                client.Headers.Add("u", username);
                client.Headers.Add("p", password);
                client.Headers.Add("e", email);
                int statusCode;
                string body;

                string response = await client.UploadStringTaskAsync(uri, "");

                using (JsonDocument document = JsonDocument.Parse(response))
                {
                    statusCode = document.RootElement.GetProperty("statusCode").GetInt32();
                    body = document.RootElement.GetProperty("body").GetString();
                }
                return statusCode;
            }
            catch
            {
                return 400;
            }
        }

        /* Richiesta di uscita dalla famiglia. */
        internal async static Task<int> QuitFamily(string username)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "family"));

                ClassGlobal.ActualUser.Username = username;

                WebClient client = new WebClient();
                client.Headers.Add("u", username);
                client.Headers.Add("operation", "quitFamily");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 400;
            }
        }

        /* Richiesta di ingresso in una famiglia. */
        internal static async Task<int> SendFamilyJoinRequest(string userJoining)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("u2", userJoining);
                client.Headers.Add("operation", "requestJoinFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                return root.GetProperty("statusCode").GetInt32();
            }
            catch
            {
                return 400;
            }
        }

        /* Rifiuto richiesta di ingresso in famiglia. */
        internal static async Task<int> RefuseFamilyJoinRequest(int family)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("family", family.ToString());
                client.Headers.Add("operation", "refuseJoinFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                return root.GetProperty("statusCode").GetInt32();
            }
            catch
            {
                return 400;
            }
        }

        /* Accetto richiesta di ingresso in famiglia. */
        internal async static Task<int> JoinFamily(int family)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("family", family.ToString());
                client.Headers.Add("operation", "addToFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                return root.GetProperty("statusCode").GetInt32();
            }
            catch
            {
                return 400;
            }
        }

        /* Creazione di una famiglia. */
        internal async static Task<int> CreateFamily(string familyName)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("family", familyName);
                client.Headers.Add("operation", "createFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                return root.GetProperty("statusCode").GetInt32();
            }
            catch
            {
                return 400;
            }
        }

        /* Modifica nome della famiglia. */
        internal async static Task<int> UpdateFamilyName(string newFamilyName)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("family", newFamilyName);
                client.Headers.Add("operation", "updateNameFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                return root.GetProperty("statusCode").GetInt32();
            }
            catch
            {
                return 400;
            }
        }

        /* Caricamento delle richieste di ingresso in famiglia di un utente. */
        internal static async Task<List<JoinFamilyRequest>> GetUserJoinFamilyRequestsAsync()
        {
            try
            {
                string url = string.Concat(urlBase, "task");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("operation", "getJoinRequestsFamily");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                var corpo = root.GetProperty("body");
                return (List<JoinFamilyRequest>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(List<JoinFamilyRequest>));
            }
            catch
            {
                return new List<JoinFamilyRequest>();
            }

        }

        /* Richiede la lista dei membri della famiglia dell'utente
         * (l'utente stesso è escluso) sul DBonline .*/
        internal async static Task<List<FamilyMember>> GetFamily()
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("operation", "getFamily");

                var response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                JsonElement corpo = root.GetProperty("body");
                List<FamilyMember> l = (List<FamilyMember>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(List<FamilyMember>));
                if (l == null)
                {
                    return new List<FamilyMember>();
                }
                return l;
            }
            catch
            {
                return new List<FamilyMember>();
            }
        }

        /* Richiesta sul DB online di tutte le task di un utente. */
        internal static async Task<List<UserTaskClass>> GetUserTasksAsync()
        {
            try
            {
                string url = string.Concat(urlBase, "task");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("operation", "getWeekTask");
                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                var corpo = root.GetProperty("body");
                return (List<UserTaskClass>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(List<UserTaskClass>));
            }
            catch
            {
                return null;
            }

        }

        /* Richiesta sul DBonline delle medaglie di un utente passato come parametro. */
        internal static async Task<List<MedalClass>> GetUserMedals(string user)
        {
            try
            {
                string url = string.Concat(urlBase, "medal");

                var client = new WebClient();
                client.Headers.Add("u", user);
                client.Headers.Add("operation", "getMedal");

                string response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                JsonElement corpo = root.GetProperty("body");
                return (List<MedalClass>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(List<MedalClass>));
            }
            catch
            {
                return new List<MedalClass>();
            }
        }

        /* Richiesta sul DBonline dei log di una casa
         * considerando un intervallo di tempo passato come parametro. */
        internal static async Task<List<LogView>> GetLog(DateTime start, DateTime end)
        {
            try
            {
                string url = string.Concat(urlBase, "family");

                var client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("start", start.Date.ToString("yyyy-MM-dd 00:00:00"));
                client.Headers.Add("end", end.Date.ToString("yyyy-MM-dd 23:59:59"));
                client.Headers.Add("operation", "getTasksFamily");

                var response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                JsonElement corpo = root.GetProperty("body");
                List<LogView> risultato = (List<LogView>)JsonConvert.DeserializeObject(corpo.GetString(), typeof(List<LogView>));
                if (risultato == null)
                {
                    throw new Exception();
                }
                return risultato;
            }
            catch
            {
                return new List<LogView>();
            }
        }

        /* Salvataggio sul DBonline della task passata come parametro. */
        public static async Task<int> AddUserTask(AbTaskClass t)
        {
            try
            {
                string url = string.Concat(urlBase, "task");
                string data = t.Date.ToString("yyyy-MM-dd HH:mm:ss");

                var client = new WebClient();
                client.Headers.Add("u", t.User);
                client.Headers.Add("nome", t.Name);
                client.Headers.Add("categoria", t.Category);
                client.Headers.Add("descrizione", t.Description);
                client.Headers.Add("data", data);
                client.Headers.Add("verifica", t.Verified.ToString());

                //Nel caso l'utente che invia la task è diverso da quello a cui arriverà, invio il NickName 
                //in maniera tale da creare la notifica apposita.

                if (!t.User.Equals(ClassGlobal.ActualUser.Username))
                    client.Headers.Add("u2", ClassGlobal.ActualUser.Nickname);

                client.Headers.Add("operation", "addTask");

                var response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);

                return document.RootElement.GetProperty("statusCode").GetInt32(); ;
            }
            catch
            {
                return 400;
            }
        }

        /* Rimozione sul DBonline della task passata come parametro. */
        public static async Task<int> RemoveUserTask(AbTaskClass t)
        {
            try
            {
                string url = string.Concat(urlBase, "task");
                string data = t.Date.ToString("yyyy-MM-dd HH:mm:ss");

                var client = new WebClient();
                client.Headers.Add("u", t.User);
                client.Headers.Add("nome", t.Name);
                client.Headers.Add("categoria", t.Category);
                // Non è più necessaria
                //client.Headers.Add("descrizione", t.Description);
                client.Headers.Add("data", data);
                client.Headers.Add("verifica", t.Verified.ToString());
                client.Headers.Add("operation", "removeTask");

                var response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);

                return document.RootElement.GetProperty("statusCode").GetInt32(); ;
            }
            catch
            {
                return 400;
            }
        }

        /* Aggiornamento sul DBonline della task passata come parametro in task svolta. */
        public static async Task<int> UserTaskVerifier(AbTaskClass t)
        {
            try
            {
                string url = string.Concat(urlBase, "task");
                string data = t.Date.ToString("yyyy-MM-dd HH:mm:ss");

                var client = new WebClient();
                client.Headers.Add("u", t.User);
                client.Headers.Add("nome", t.Name);
                client.Headers.Add("categoria", t.Category);
                client.Headers.Add("descrizione", t.Description);
                client.Headers.Add("data", data);
                client.Headers.Add("verifica", t.Verified.ToString());
                client.Headers.Add("operation", "updateVerTask");

                var response = await client.UploadStringTaskAsync(url, "");

                JsonDocument document = JsonDocument.Parse(response);

                return document.RootElement.GetProperty("statusCode").GetInt32(); ;
            }
            catch
            {
                return 400;
            }
        }

        /* Modifica icona dell'utente. */
        public static async Task<int> UpdateUserIcon(int icon)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "profile"));


                WebClient client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("icon", icon.ToString());
                client.Headers.Add("operation", "iconProfile");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                //Necessario solo per debug
                //JsonElement corpo = root.GetProperty("body");
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }
        }

        /* Modifica nickename dell'utente. */
        public static async Task<int> UpdateUserNickname(string nick)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "profile"));
                WebClient client = new WebClient();
                client.Headers.Add("u", ClassGlobal.ActualUser.Username);
                client.Headers.Add("name", Convert.ToBase64String(Encoding.UTF8.GetBytes(nick)));
                client.Headers.Add("operation", "nameProfile");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                //Necessario solo per debug
                //JsonElement corpo = root.GetProperty("body");
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }
        }

        /* Step 1 reset password, invio codice all'email introdotta. */
        public static async Task<int> ResetPasswordEmailSender(string email)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "profile"));
                WebClient client = new WebClient();
                client.Headers.Add("e", email);
                client.Headers.Add("operation", "resetPasswordRequestProfile");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                //Necessario solo per debug
                //JsonElement corpo = root.GetProperty("body");
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }
        }

        /* Step 2 reset password, verifica correttezza codice introdotto. */
        public static async Task<int> ResetPasswordTokenVerify(string email, string token)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "profile"));
                WebClient client = new WebClient();
                client.Headers.Add("e", email);
                client.Headers.Add("token", token);
                client.Headers.Add("operation", "checkTokenProfile");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                //Necessario solo per debug
                //JsonElement corpo = root.GetProperty("body");
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }
        }

        /* Step 3 reset password, reset della password. */
        public static async Task<int> ResetPasswordOfficial(string email, string password)
        {
            try
            {
                Uri uri = new Uri(string.Concat(urlBase, "profile"));
                WebClient client = new WebClient();
                client.Headers.Add("e", email);
                client.Headers.Add("p", password);
                client.Headers.Add("operation", "resetPasswordProfile");

                string response = await client.UploadStringTaskAsync(uri, "");

                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;
                //Necessario solo per debug
                //JsonElement corpo = root.GetProperty("body");
                int statusCode = root.GetProperty("statusCode").GetInt32();
                return statusCode;
            }
            catch
            {
                return 399;
            }
        }
    }

}