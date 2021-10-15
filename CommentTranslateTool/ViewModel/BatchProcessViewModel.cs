using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Workshop.Helper;
using Workshop.Model;
using Workshop.Parsers;
using Workshop.Service;
using FileInfo = System.IO.FileInfo;

namespace Workshop.ViewModel
{

    public class BatchProcessViewModel : ViewModelBase
    {
        private ICommentParser _commentParser;

        public BatchProcessViewModel()
        {
            OpenCommand = new RelayCommand(OpenAction);
            ClearCommand = new RelayCommand(ClearAction);
            ContinueCommand = new RelayCommand(ContinueAction);
            this.PropertyChanged += IndexPageViewModel_PropertyChanged;
            ParserProviders = new List<ParserProvider>(ParserProvider.GetAllProviders());
            this.CurrentParserProvider = this.ParserProviders.First();

        }

        private void ClearAction()
        {
            this.FileList.Clear();
        }

        private void OpenAction()
        {
            string foldPath;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            dialog.RootFolder = Environment.SpecialFolder.Programs;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = dialog.SelectedPath;
            }
            else
            {
                return;
            }

            var allfiles = new List<Model.FileInfo>();
            foreach (var ext in this.CurrentParserProvider.FileExtensionList)
            {
                allfiles.AddRange(DirFileHelper.GetFileNames(foldPath, searchPattern: "*" + ext, true).Select(c => new Model.FileInfo()
                {
                    Name = DirFileHelper.GetFileName(c),
                    Path = c,
                }));
            }
            this.FileList = new ObservableCollection<Model.FileInfo>(allfiles);


        }

        private void IndexPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentParserProvider))
            {
                _commentParser = CurrentParserProvider.CreateCommentParser();
            }
        }



        private async void ContinueAction()
        {
            var task = InvokeHelper.InvokeOnUi(null, () =>
            {

                foreach (var file in this.FileList)
                {

                    var currentContentText = DirFileHelper.ReadFile(file.Path);

                    string responseText;

                    var settingInfo = LocalDataService.ReadObjectLocal<SettingInfo>();


                    if (string.IsNullOrEmpty(currentContentText))
                    {
                        MessageBox.Show("Please specify the source text.");

                        continue;

                    }


                    else if (settingInfo.IsCharLimit && currentContentText.Length > settingInfo.CharLimitCount)
                    {
                        MessageBox.Show($"Only strings shorter than {settingInfo.CharLimitCount} characters are supported; your input string is " + currentContentText.Length + " characters long.");
                        continue;

                    }
                    else
                    {
                        try
                        {
                            var parser = _commentParser;
                            var text = string.Empty;
                            var textCollection = new List<(CommentRegion, string)>();

                            if (parser != null)
                            {
                                var regions = parser.GetCommentRegions(currentContentText).OrderByDescending(c => c.Start);
                                foreach (var r in regions)
                                {
                                    var trim = parser.GetComment(currentContentText.Substring(r.Start, r.Length)).Content.Trim();
                                    textCollection.Add((r, trim));
                                }
                            }

                            var sb = new StringBuilder(currentContentText);

                            foreach (var t in textCollection)
                            {
                                double pcent = textCollection.IndexOf(t) * 1.0 / textCollection.Count;

                                var currentRegion = t.Item1;
                                var translateResult = DoTranslate(t.Item2).Result;
                                sb.Remove(currentRegion.Start, currentRegion.Length);
                                sb.Insert(currentRegion.Start, currentRegion.Tag.Start);
                                sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length, translateResult);
                                sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length + translateResult.Length, currentRegion.Tag.End);
                                file.Progress = (int)Math.Ceiling(pcent * 100);
                            }

                            responseText = sb.ToString();
                            DirFileHelper.WriteFile(file.Path, responseText);
                            file.Progress = 100;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());

                        }

                    }

                }

            }, () =>
            {
                MessageBox.Show("已经完成所有任务");

            });


        }

        private async Task<string> DoTranslate(string CurrentContent)
        {
            var trans = "";


            Thread.Sleep(2000);
            var result = await YouDaoApiHelper.GetWordsAsync(CurrentContent);


            if (result.YouDaoTranslation.FirstTranslation.Count > 0)
            {
                trans = result.YouDaoTranslation.FirstTranslation[0];
            }

            return trans;

        }



        private ParserProvider _currentParserProvider;

        public ParserProvider CurrentParserProvider
        {
            get { return _currentParserProvider; }
            set
            {
                _currentParserProvider = value;
                RaisePropertyChanged(nameof(CurrentParserProvider));
            }
        }

        private List<ParserProvider> _parserProviders;

        public List<ParserProvider> ParserProviders
        {
            get { return _parserProviders; }
            set
            {
                _parserProviders = value;
                RaisePropertyChanged(nameof(ParserProviders));
            }
        }


        private ObservableCollection<Model.FileInfo> _fileList;

        public ObservableCollection<Model.FileInfo> FileList
        {
            get { return _fileList; }
            set
            {
                _fileList = value;
                RaisePropertyChanged(nameof(FileList));
            }
        }

        public RelayCommand ContinueCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }


    }

}
