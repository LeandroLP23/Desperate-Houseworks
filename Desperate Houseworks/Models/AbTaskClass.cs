using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Desperate_Houseworks.Models
{
    /* Classe astratta riguardante le task che l'utente vuole svolgere.*/
    public abstract class AbTaskClass : INotifyPropertyChanged
    {
        private bool isTapped = false;
        public bool IsTapped
        {
            get => isTapped;
            set
            {
                if (isTapped != value)
                {
                    isTapped = value;
                    OnPropertyChanged();
                }
            }

        }

        private bool verified;
        public bool Verified
        {
            get => verified;
            set
            {
                if (verified != value)
                {
                    verified = value;
                    OnPropertyChanged();
                }
            }

        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
