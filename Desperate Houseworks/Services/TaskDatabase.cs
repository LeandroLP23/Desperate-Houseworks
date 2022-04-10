using Desperate_Houseworks.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Desperate_Houseworks.Services
{

    public class TaskDatabase
    {
        private readonly SQLiteAsyncConnection database;

        public TaskDatabase(string dbPath)
        {
            //Creazione delle due tabelle del DB locale
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<GenericTaskClass>().Wait();
            database.CreateTableAsync<UserTaskClass>().Wait();
        }

        /* Richiede la lista delle GenericTask.*/
        public async Task<List<GenericTaskClass>> GetGenericTasksAsync()
        {
            return await database.Table<GenericTaskClass>().ToListAsync();
        }

        /* Richiede la lista delle GenericTask ordinate per categoria.*/
        public async Task<List<GenericTaskClass>> GetGenericTasksOrderedByCategoryAsync()
        {
            return await database.QueryAsync<GenericTaskClass>("SELECT * FROM GenericTaskClass ORDER BY category");
        }

        public async Task<GenericTaskClass> GetTaskAsync(int id)
        {
            return await database.Table<GenericTaskClass>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        /* Aggiunta della GenericTask passata come parametro al DBlocale */
        public async Task<int> SaveGenericTaskAsync(GenericTaskClass GenTask)
        {
            if (GenTask.ID != 0)
            {
                return await database.UpdateAsync(GenTask);
            }
            else
            {
                return await database.InsertAsync(GenTask);
            }
        }

        /* Aggiunta della UserTask passata come parametro al DBlocale */
        public async Task<int> AddUserTaskAsync(UserTaskClass UserTask)
        {
            if (UserTask.ID != 0)
            {
                return await database.UpdateAsync(UserTask);
            }
            else
            {
                return await database.InsertAsync(UserTask);
            }
        }

        /* Rimozione della GenericTask passata come parametro da DBlocale*/
        public async Task<int> DeleteGenericTaskAsync(GenericTaskClass GenTask)
        {
            return await database.DeleteAsync(GenTask);
        }

        /* Rimozione della UserTask passata come parametro da DBlocale*/
        public async Task<int> DeleteUserTaskAsync(UserTaskClass UserTask)
        {
            return await database.DeleteAsync(UserTask);
        }

        /* Rimozione di tutte le GenericTask da DBlocale*/
        public async void DeleteGenericTaskAll()
        {
            await database.DeleteAllAsync<GenericTaskClass>();
        }

        /* Rimozione di tutte le UserTask da DBlocale*/
        public async void DeleteAllUserTaskClass()
        {
            await database.DeleteAllAsync<UserTaskClass>();
        }

        /* Richiede le task dal DBonline, se ce ne sono, 
         * le aggiunge al DBlocale dopo averlo svuotato */
        public async Task<bool> SyncUserTaskClass()
        {
            List<UserTaskClass> lista = await DesperateDB.GetUserTasksAsync();

            //C'è stato un problema e le task non sono state sincronizzate
            if (lista == null)
            {
                return false;
            }

            //Lista inizializzata
            DeleteAllUserTaskClass();
            foreach (var item in lista)
            {
                await AddUserTaskAsync(item);
            }

            return true;
        }

        /* Inizializza il DBlocale delle GenericTask*/
        public async void InitGenericTaskDb()
        {
            int dbTaskCounter = await database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM GenericTaskClass");


            //Se non ho elementi nel DBlocale, li genero
            if (dbTaskCounter == 0)
            {
                //Array di stringhe contenente i nomi delle task recuperate da Enum
                string[] ens = Enum.GetNames(typeof(EnumTask));

                foreach (string en in ens)
                {
                    //Salvo categoria per ritrovare l'icona della task
                    string cat = GestioneEnum.Name2Cat(en);

                    //Creo la task
                    var GenTask = new GenericTaskClass()
                    {
                        Name = en,
                        Category = cat,
                        Icon = GestioneEnum.Cat2Icon(cat),
                    };

                    //Inserisco nel DBlocal
                    await database.InsertAsync(GenTask);
                }
            }
        }

        /* Recupero le UserTask dal DBlocale per spostarle sulla collezione 
         * delle UserTask */
        public async Task<ObservableCollection<UserTaskClass>> RecoveryFromUserTaskDb()
        {

            int dbTaskCounter = await database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM UserTaskClass");

            //Se ho task su DBlocale le metto in lista
            if (dbTaskCounter != 0)
            {
                //Recupero la lista di UserTask
                List<UserTaskClass> ts = await database.Table<UserTaskClass>().ToListAsync();

                //Ritorno la collezione
                return new ObservableCollection<UserTaskClass>(ts);
            }
            return new ObservableCollection<UserTaskClass>();
        }
    }

    public class UserDatabase
    {
        private readonly SQLiteAsyncConnection database;

        public UserDatabase(string dbPath)
        {
            //Creazione della tabella nel DB locale
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<User>().Wait();
        }

        public async Task<int> SetUser(User u)
        {

            int dbTaskCounter = await database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM User");

            if (!(dbTaskCounter == 0))
            {
                await database.DeleteAllAsync<User>();
            }
            return await database.InsertAsync(u);

        }

        /* Se questo è il primo Login dell'utente, ritornerà null, in quanto 
         * non è presente un'utente a priori; nel caso opposto, ritornerà l'utente stesso.*/
        public async Task<User> GetUser()
        {
            List<User> utente = await database.QueryAsync<User>("SELECT * FROM User");
            if (utente.Count != 0)
            {
                return utente[0];
            }
            else
            {
                return new User();
            }
        }

        /* Rimozione utente del DBlocale e da ClassGlobal.*/
        internal async Task ClearUsers()
        {
            await database.DeleteAllAsync<User>();
            ClassGlobal.ActualUser = new User();
        }
    }
}
