using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextFilter.Models
{
    public class CheckboxModel : INotifyPropertyChanged
    {
        private bool runFilter;
        private bool removeDuplicates;

        public CheckboxModel(bool runFilter, bool removeDuplicates)
        {
            RunFilter = runFilter;
            RemoveDuplicates = removeDuplicates;
        }

        public bool RunFilter
        {
            get => runFilter;
            set
            {
                if (runFilter != value)
                {
                    runFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RemoveDuplicates
        {
            get => removeDuplicates;
            set
            {
                if (removeDuplicates != value)
                {
                    removeDuplicates = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
