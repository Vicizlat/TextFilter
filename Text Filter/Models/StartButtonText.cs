using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextFilter.Models
{
    public class StartButtonTextModel : INotifyPropertyChanged
    {
        private string startButtonText;
        public StartButtonTextModel(string startButtonText)
        {
            StartButtonText = startButtonText;
        }
        public string StartButtonText
        {
            get => startButtonText;
            set
            {
                if (startButtonText != value)
                {
                    startButtonText = value;
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
