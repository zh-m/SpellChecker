using SpellChecker_Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpellChecker_Demo
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ICommand onCorrectPNCommand;
        private ICommand onCorrectDeletionsCommand;

        private string initialText = "снякк";
        private string initialTextWithDeletion = "снякк";
        private ObservableCollection<string> spellCheckedValues;
        private ObservableCollection<string> spellCheckedValuesWithDeletions;

        public string InitialText
        {
            get
            {
                return this.initialText;
            }
            set
            {
                if (this.initialText != value)
                {
                    this.initialText = value;
                    NotifyPropertyChanged("InitialText");
                }
            }
        }

        public string InitialTextWithDeletion
        {
            get
            {
                return this.initialTextWithDeletion;
            }
            set
            {
                if (this.initialTextWithDeletion != value)
                {
                    this.initialTextWithDeletion = value;
                    NotifyPropertyChanged("InitialTextWithDeletion");
                }
            }
        }

        public ObservableCollection<string> SpellCheckedValues
        {
            get
            {
                if (this.spellCheckedValues == null)
                {
                    this.spellCheckedValues = new ObservableCollection<string>();
                }
                return this.spellCheckedValues;
            }
        }

        public ObservableCollection<string> SpellCheckedValuesWithDeletions
        {
            get
            {
                if (this.spellCheckedValuesWithDeletions == null)
                {
                    this.spellCheckedValuesWithDeletions = new ObservableCollection<string>();
                }
                return this.spellCheckedValuesWithDeletions;
            }
        }

        public ICommand OnCorrectPNCommand
        {
            get
            {
                return this.onCorrectPNCommand;
            }
            set
            {
                this.onCorrectPNCommand = value;
            }
        }

        public ICommand OnCorrectDeletionsCommand
        {
            get
            {
                return this.onCorrectDeletionsCommand;
            }
            set
            {
                this.onCorrectDeletionsCommand = value;
            }
        }

        public ViewModel()
        {
            OnCorrectPNCommand = new DelegateCommand(CorrectPN, param => true);
            OnCorrectDeletionsCommand = new DelegateCommand(CorrectDeletions, param => true);
        }

        private void CorrectPN(object obj)
        {
            CorrectWithSpellCheck_PN(this.initialText);
        }

        private void CorrectDeletions(object obj)
        {
            CorrectWithSpellCheck_Deletion(this.initialTextWithDeletion);
        }

        private void CorrectWithSpellCheck_Deletion(string p)
        {
            var spellChecker = new SpellCheck_Deletions.SpellChecker();

            this.SpellCheckedValuesWithDeletions.Clear();
            spellChecker.Correct(this.InitialTextWithDeletion, "bg", 3).ForEach(i => this.SpellCheckedValuesWithDeletions.Add(i));
        }

        private void CorrectWithSpellCheck_PN(string p)
        {
            var spellChecker = new SpellCheck_PN.SpellChecker();
            this.SpellCheckedValues.Clear();
            spellChecker.Correct(this.InitialText, "bg").ForEach(i => this.SpellCheckedValues.Add(i));
        }

        public void InvalidateSpellCheckedValues()
        {
            this.SpellCheckedValues.Clear();
        }

        public void InvalidateSpellCheckedValuesWithDeletions()
        {
            this.SpellCheckedValuesWithDeletions.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
