using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextFilter.Models
{
    public class TextBoxModel : INotifyPropertyChanged
    {
        private string sourceText;
        private string filterText;
        private string leftoverText;
        private string filteredText;

        public TextBoxModel(string sourceText, string filterText, string leftoverText, string filteredText)
        {
            SourceText = sourceText;
            FilterText = filterText;
            LeftoverText = leftoverText;
            FilteredText = filteredText;
        }

        public string SourceText
        {
            get => sourceText;
            set
            {
                if (sourceText != value)
                {
                    sourceText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LeftoverText
        {
            get => leftoverText;
            set
            {
                if (leftoverText != value)
                {
                    leftoverText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FilteredText
        {
            get => filteredText;
            set
            {
                if (filteredText != value)
                {
                    filteredText = value;
                    OnPropertyChanged(nameof(FilteredText));
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