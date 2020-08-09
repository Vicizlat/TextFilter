using TextFilter.Commands;
using TextFilter.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;

namespace TextFilter.ViewModels
{
    internal class MainViewModel
    {
        public static WaitWindow WaitWindow = new WaitWindow();
        public BackgroundWorker BackgroundWorker = new BackgroundWorker();

        public TextBoxModel TextBoxModel { get; set; }
        public CheckboxModel CheckboxModel { get; set; }
        public ProgressAndResultsModel ProgressAndResultsModel { get; set; }
        public StartButtonTextModel StartButtonTextModel { get; set; }

        public ICommand LoadSourceTextCommand { get; private set; }
        public ICommand LoadFilterTextCommand { get; private set; }
        public ICommand StartButtonCommand { get; private set; }
        public ICommand SaveLeftoverResultTextCommand { get; private set; }
        public ICommand LoadLeftoverAsSourceCommand { get; private set; }
        public ICommand SaveFilteredResultTextCommand { get; private set; }
        public ICommand LoadFilteredAsSourceCommand { get; private set; }

        public List<string> SourceList = new List<string>();
        public List<string> FilterList = new List<string>();
        public List<string> LeftoverList = new List<string>();
        public List<string> FilteredList = new List<string>();

        public bool IsWorking = false;

        public MainViewModel()
        {
            TextBoxModel = new TextBoxModel("", "", "", "");
            CheckboxModel = new CheckboxModel(true, false);
            ProgressAndResultsModel = new ProgressAndResultsModel(0, 0, 0, 0);
            StartButtonTextModel = new StartButtonTextModel("Start Filter");

            LoadSourceTextCommand = new LoadSourceTextCommand(this);
            LoadFilterTextCommand = new LoadFilterTextCommand(this);
            StartButtonCommand = new StartButtonCommand(this);
            SaveLeftoverResultTextCommand = new SaveLeftoverResultTextCommand(this);
            LoadLeftoverAsSourceCommand = new LoadLeftoverAsSourceCommand(this);
            SaveFilteredResultTextCommand = new SaveFilteredResultTextCommand(this);
            LoadFilteredAsSourceCommand = new LoadFilteredAsSourceCommand(this);
        }

        public async void StartButtonPressed()
        {
            IsWorking = true;
            StartButtonTextModel.StartButtonText = "Working, please wait...";
            WaitWindow.Show();
            AddSourceToList();

            ProgressAndResultsModel.ProgBarChange = 0;
            ProgressAndResultsModel.ProgBarMax = SourceList.Count;
            ProgressAndResultsModel.MatchCount = 0;
            ProgressAndResultsModel.NoMatchCount = 0;
            LeftoverList.Clear();
            FilteredList.Clear();
            TextBoxModel.LeftoverText = "";
            TextBoxModel.FilteredText = "";

            if (CheckboxModel.RemoveDuplicates)
            {
                RemoveDuplicatesFromSource();
            }

            if (CheckboxModel.RunFilter)
            {
                BackgroundWorker.DoWork += RunFilter;
                BackgroundWorker.RunWorkerAsync();
                BackgroundWorker.Dispose();
            }

            await HideWaitWindow(WaitWindow, BackgroundWorker);
            StartButtonTextModel.StartButtonText = "Start Filter";
            IsWorking = false;
        }

        private void RunFilter(object sender, DoWorkEventArgs e)
        {
            AddFilterToList();

            while (ProgressAndResultsModel.ProgBarChange < SourceList.Count)
            {
                string sourceLine = SourceList.ElementAt(ProgressAndResultsModel.ProgBarChange);
                CheckEachFilterLine(sourceLine);
                if (ProgressAndResultsModel.ProgBarChange == ProgressAndResultsModel.MatchCount + ProgressAndResultsModel.NoMatchCount)
                {
                    LeftoverList.Add(sourceLine);
                    ProgressAndResultsModel.NoMatchCount++;
                }
                ProgressAndResultsModel.ProgBarChange++;
            }
            StartButtonTextModel.StartButtonText = "Processing results...";
            TextBoxModel.FilteredText = string.Join("\r\n", FilteredList.ToArray());
            TextBoxModel.LeftoverText = string.Join("\r\n", LeftoverList.ToArray());
        }

        private void CheckEachFilterLine(string sourceLine)
        {
            for (int CurrentLineCheck = 0; CurrentLineCheck < FilterList.Count; CurrentLineCheck++)
            {
                string filterLine = FilterList.ElementAt(CurrentLineCheck);
                if (Regex.IsMatch(sourceLine, filterLine, RegexOptions.IgnorePatternWhitespace) ||
                    Regex.IsMatch(sourceLine, filterLine, RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(sourceLine, filterLine, RegexOptions.CultureInvariant))
                {
                    FilteredList.Add(sourceLine);
                    ProgressAndResultsModel.MatchCount++;
                    break;
                }
            }
        }

        private void RemoveDuplicatesFromSource()
        {
            StartButtonTextModel.StartButtonText = "Removing Duplicates";
            List<string> NonDuplicateList = new List<string>();
            foreach (string line in SourceList)
            {
                if (!NonDuplicateList.Contains(line))
                {
                    NonDuplicateList.Add(line);
                    ProgressAndResultsModel.MatchCount++;
                }
                ProgressAndResultsModel.ProgBarChange++;
            }
            if (!CheckboxModel.RunFilter)
            {
                TextBoxModel.FilteredText = string.Join("\r\n", NonDuplicateList.ToArray());
            }
            else
            {
                SourceList.Clear();
                foreach (string line in NonDuplicateList)
                {
                    SourceList.Add(line);
                }
            }
            NonDuplicateList.Clear();
        }

        private void AddSourceToList()
        {
            SourceList.Clear();
            int S_LineStartIndex = 0;
            while (S_LineStartIndex < TextBoxModel.SourceText.Length)
            {
                int S_LineEndIndex = TextBoxModel.SourceText.IndexOf("\r\n", S_LineStartIndex);
                if (S_LineEndIndex < 0)
                {
                    S_LineEndIndex = TextBoxModel.SourceText.Length;
                }
                string S_NewLine = TextBoxModel.SourceText.Substring(S_LineStartIndex, S_LineEndIndex - S_LineStartIndex);
                SourceList.Add(S_NewLine);
                S_LineStartIndex = S_LineEndIndex + 2;
            }
        }

        private void AddFilterToList()
        {
            FilterList.Clear();
            int F_LineStartIndex = 0;
            while (F_LineStartIndex < TextBoxModel.FilterText.Length)
            {
                int F_LineEndIndex = TextBoxModel.FilterText.IndexOf("\r\n", F_LineStartIndex);
                if (F_LineEndIndex < 0)
                {
                    F_LineEndIndex = TextBoxModel.FilterText.Length;
                }
                string F_NewLine = TextBoxModel.FilterText.Substring(F_LineStartIndex, F_LineEndIndex - F_LineStartIndex);
                FilterList.Add(F_NewLine);
                F_LineStartIndex = F_LineEndIndex + 2;
            }
        }

        public async void LoadSourceText()
        {
            WaitWindow.Show();
            OpenFileDialog LoadSourceDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (LoadSourceDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                TextBoxModel.SourceText += File.ReadAllText(LoadSourceDialog.FileName) + "\r\n";
            }
            await Task.Delay(50);
            WaitWindow.Hide();
        }

        public async void LoadFilterText()
        {
            WaitWindow.Show();
            OpenFileDialog LoadFilterDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (LoadFilterDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                TextBoxModel.FilterText += File.ReadAllText(LoadFilterDialog.FileName) + "\r\n";
            }
            await Task.Delay(50);
            WaitWindow.Hide();
        }

        public void SaveLeftoverResultText()
        {
            SaveFileDialog SaveLeftoversDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            if (SaveLeftoversDialog.ShowDialog() == true)
            {
                File.WriteAllText(SaveLeftoversDialog.FileName, TextBoxModel.LeftoverText);
            }
        }

        public async void LoadLeftoverAsSource()
        {
            WaitWindow.Show();
            BackgroundWorker.DoWork += LoadAsSource;
            BackgroundWorker.RunWorkerAsync(LeftoverList);
            await HideWaitWindow(WaitWindow, BackgroundWorker);
            BackgroundWorker.Dispose();
        }

        public void SaveFilteredResultText()
        {
            SaveFileDialog SaveFilteredDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            if (SaveFilteredDialog.ShowDialog() == true)
            {
                File.WriteAllText(SaveFilteredDialog.FileName, TextBoxModel.FilteredText);
            }
        }

        public async void LoadFilteredAsSource()
        {
            WaitWindow.Show();
            BackgroundWorker.DoWork += LoadAsSource;
            BackgroundWorker.RunWorkerAsync(FilteredList);
            await HideWaitWindow(WaitWindow, BackgroundWorker);
            BackgroundWorker.Dispose();
        }

        private void LoadAsSource(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is List<string> ToSource && ToSource != null)
            {
                TextBoxModel.SourceText = "";
                TextBoxModel.SourceText = string.Join("\r\n", ToSource.ToArray());
            }
        }

        private async Task HideWaitWindow(WaitWindow waitWindow, BackgroundWorker backgroundWorker)
        {
            if (backgroundWorker.IsBusy)
            {
                await Task.Delay(500);
                await HideWaitWindow(waitWindow, backgroundWorker);
            }
            waitWindow.Hide();
        }

        public bool CanUpdateStartButton
        {
            get
            {
                if (!IsWorking && TextBoxModel.SourceText.Length > 0)
                {
                    if (CheckboxModel.RemoveDuplicates && !CheckboxModel.RunFilter)
                    {
                        return true;
                    }
                    else if (CheckboxModel.RunFilter && TextBoxModel.FilterText.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CanUpdateLeftover
        {
            get
            {
                switch (ProgressAndResultsModel.NoMatchCount)
                {
                    case 0:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public bool CanUpdateFiltered
        {
            get
            {
                switch (ProgressAndResultsModel.MatchCount)
                {
                    case 0:
                        return false;
                    default:
                        return true;
                }
            }
        }
    }
}