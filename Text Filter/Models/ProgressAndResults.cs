using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextFilter.Models
{
    public class ProgressAndResultsModel : INotifyPropertyChanged
    {
        private int progBarChange;
        private int progBarMax;
        private int matchCount;
        private int noMatchCount;

        public ProgressAndResultsModel(int progBarChange, int progBarMax, int matchCount, int noMatchCount)
        {
            ProgBarChange = progBarChange;
            ProgBarMax = progBarMax;
            MatchCount = matchCount;
            NoMatchCount = noMatchCount;
        }

        public int ProgBarChange
        {
            get => progBarChange;
            set
            {
                if (progBarChange != value)
                {
                    progBarChange = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProgBarMax
        {
            get => progBarMax;
            set
            {
                if (progBarMax != value)
                {
                    progBarMax = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MatchCount
        {
            get => matchCount;
            set
            {
                if (matchCount != value)
                {
                    matchCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int NoMatchCount
        {
            get => noMatchCount;
            set
            {
                if (noMatchCount != value)
                {
                    noMatchCount = value;
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
