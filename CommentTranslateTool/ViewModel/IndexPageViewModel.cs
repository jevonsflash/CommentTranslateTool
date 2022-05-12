using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit.Document;
using Newtonsoft.Json;
using Workshop.Common;
using Workshop.Control;
using Workshop.Helper;
using Workshop.Model;
using Workshop.Parsers;
using Workshop.Service;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Workshop.ViewModel
{
    public class IndexPageViewModel : ObservableObject
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
                string responseText = string.Empty;

                var settingInfo = LocalDataService.ReadObjectLocal<SettingInfo>();


                if (string.IsNullOrEmpty(currentContentText))
                {
                    MessageBox.Show("Please specify the source text.");
                }


                else if (settingInfo.IsCharLimit && currentContentText.Length > settingInfo.CharLimitCount)
                {
                    MessageBox.Show($"Only strings shorter than {settingInfo.CharLimitCount} characters are supported; your input string is " + currentContentText.Length + " characters long.");
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
                                var trim = parser.GetComment(currentContentText.Substring(r.Start, r.Length), r.Tag).Content.Trim();
                                textCollection.Add((r, trim));
                            }
                        }

                        var sb = new StringBuilder(currentContentText);
                        Regex regex = new Regex(@"(?<=<(\w+)(>| [^>]*>)).*(?=<\/\1>)");
                        Regex regex2 = new Regex(@"^<(\w+)(>| [^>]*>)$");
                        Regex regex3 = new Regex(@"^^<\/\w+>$");

                        foreach (var t in textCollection)
                        {
                            var currentRegion = t.Item1;
                            string translateResult;
                            var contentToTranslate = t.Item2;

                            if (!settingInfo.IsTranslateAngleBracketElement)
                            {
                                var subStartIndex = 0;
                                if (regex.IsMatch(t.Item2))
                                {
                                    var matchResult = regex.Match(t.Item2);
                                    contentToTranslate = matchResult.Value;
                                    subStartIndex = matchResult.Index;
                                    if (!string.IsNullOrEmpty(contentToTranslate))
                                    {
                                        translateResult = DoTranslate(contentToTranslate).Result;
                                    }
                                    else
                                    {
                                        translateResult = contentToTranslate;
                                    }

                                    if (subStartIndex != 0)
                                    {
                                        var ssb = new StringBuilder();
                                        ssb.Append(t.Item2.Substring(0, subStartIndex));
                                        ssb.Append(translateResult);
                                        ssb.Append(t.Item2.Substring(subStartIndex + contentToTranslate.Length));
                                        translateResult = ssb.ToString();
                                    }
                                }
                                else if (regex2.IsMatch(t.Item2) || regex3.IsMatch(t.Item2))
                                {
                                    translateResult = contentToTranslate;
                                }

                                else
                                {
                                    translateResult = DoTranslate(contentToTranslate).Result;
                                }
                            }
                            else
                            {
                                translateResult = DoTranslate(contentToTranslate).Result;
                            }
                            sb.Remove(currentRegion.Start, currentRegion.Length);
                            sb.Insert(currentRegion.Start, currentRegion.Tag.Start);
                            sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length, translateResult);
                            sb.Insert(currentRegion.Start + currentRegion.Tag.Start.Length + translateResult.Length, currentRegion.Tag.End);
                        }

                        responseText = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

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
            Thread.Sleep(2000);
            var result = await YouDaoApiHelper.GetWordsAsync(CurrentContent);

            if (result.YouDaoTranslation.FirstTranslation != null)
            {
                if (result.YouDaoTranslation.FirstTranslation.Count > 0)
                {
                    trans = result.YouDaoTranslation.FirstTranslation[0];
                }
            }
            else
            {
                throw new Exception("网络错误，返回信息" + result.ResultDetail);
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
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        private TextDocument _responseContent;

        public TextDocument ResponseContent
        {
            get { return _responseContent; }
            set
            {
                _responseContent = value;
                OnPropertyChanged(nameof(ResponseContent));
            }
        }


        private ParserProvider _currentParserProvider;

        public ParserProvider CurrentParserProvider
        {
            get { return _currentParserProvider; }
            set
            {
                _currentParserProvider = value;
                OnPropertyChanged(nameof(CurrentParserProvider));
            }
        }

        private List<ParserProvider> _parserProviders;

        public List<ParserProvider> ParserProviders
        {
            get { return _parserProviders; }
            set
            {
                _parserProviders = value;
                OnPropertyChanged(nameof(ParserProviders));
            }
        }


        public RelayCommand ContinueCommand { get; set; }
        public RelayCommand CopyToClipboardCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }

    }
}
