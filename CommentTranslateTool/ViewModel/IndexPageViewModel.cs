using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit.Document;
using Workshop.Common;
using Workshop.Control;
using Workshop.Helper;
using Workshop.Model;
using Workshop.Parsers;
using Workshop.Service;

namespace Workshop.ViewModel
{
    public class IndexPageViewModel : ViewModelBase
    {
        private ICommentParser _commentParser;

        public IndexPageViewModel()
        {
            CurrentContent = new TextDocument();
            ResponseContent = new TextDocument();
            ContinueCommand = new RelayCommand(ContinueAction);
            OpenCommand = new RelayCommand(OpenAction);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboardAction);
            this.PropertyChanged += IndexPageViewModel_PropertyChanged;
            ParserProviders = new List<ParserProvider>(ParserProvider.GetAllProviders());
            this.CurrentParserProvider = this.ParserProviders.First();

        }

        private void OpenAction()
        {
            var result = ImportHelper.ImportFrom();
            if (string.IsNullOrEmpty(result.Item1) || result.Item2 == null)
            {
                return;
            }
            this.CurrentContent.Text = result.Item1;
            this.CurrentParserProvider = this.ParserProviders.First(c => c.Name == result.Item2.Name);
        }

        private void IndexPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentParserProvider))
            {
                _commentParser = CurrentParserProvider.CreateCommentParser();
            }
        }

        private void CopyToClipboardAction()
        {

            if (string.IsNullOrEmpty(ResponseContentText))
            {
                return;
            }
            Clipboard.SetText(ResponseContentText);
        }


        private async void ContinueAction()
        {
            var currentContentText = CurrentContentText;
            var task = InvokeHelper.InvokeOnUi<string>(null, () =>
            {
                string responseText;

                var settingInfo = LocalDataService.ReadObjectLocal<SettingInfo>();


                if (string.IsNullOrEmpty(currentContentText))
                {
                    responseText = "Please specify the source text.";
                }


                else if (settingInfo.IsCharLimit && currentContentText.Length > settingInfo.CharLimitCount)
                {
                    responseText = $"Only strings shorter than {settingInfo.CharLimitCount} characters are supported; your input string is " + currentContentText.Length + " characters long.";
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
                            var currentRegion = t.Item1;
                            var translateResult = DoTranslate(t.Item2).Result;
                            sb.Remove(currentRegion.Start, currentRegion.Length);
                            sb.Insert(currentRegion.Start, currentRegion.Tag.Start);
                            sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length, translateResult);
                            sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length + translateResult.Length, currentRegion.Tag.End);
                        }

                        responseText = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        responseText = ex.ToString();

                    }

                }

                return responseText;


            }, (t) =>
            {
                ResponseContent.Text = t;

            });


        }

        private async Task<string> DoTranslate(string CurrentContent)
        {
            var trans = "";
            if (CurrentContent.Contains("杨晓宇"))
            {
                Window sh = new Window();
                sh.Topmost = true;
                sh.Background = new SolidColorBrush(Color.FromArgb(255, 47, 58, 65));
                var img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/sh.jpg"));
                sh.Content = img;
                sh.ShowDialog();

            }


            Thread.Sleep(2000);
            var result = await YouDaoApiHelper.GetWordsAsync(CurrentContent);


            if (result.YouDaoTranslation.FirstTranslation.Count > 0)
            {
                trans = result.YouDaoTranslation.FirstTranslation[0];
            }

            return trans;

        }

        public string CurrentContentText => CurrentContent.Text;
        public string ResponseContentText => ResponseContent.Text;

        private TextDocument _currentContent;

        public TextDocument CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                RaisePropertyChanged(nameof(CurrentContent));
            }
        }

        private TextDocument _responseContent;

        public TextDocument ResponseContent
        {
            get { return _responseContent; }
            set
            {
                _responseContent = value;
                RaisePropertyChanged(nameof(ResponseContent));
            }
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


        public RelayCommand ContinueCommand { get; set; }
        public RelayCommand CopyToClipboardCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }

    }
}
