using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshop.Model
{
    public class SettingInfo : ViewModelBase
    {

        public SettingInfo()
        {
            var defaulTranslationTypes = new List<YouDaoTranslationType>(
            )
            {
                new YouDaoTranslationType()
                {
                    Name = "自动检测",
                    Type = "auto"
                },
                new YouDaoTranslationType()
                {
                    Name = "英文",
                    Type = "en"
                },
                new YouDaoTranslationType()
                {
                    Name = "中文",
                    Type = "zh-CHS"
                },
                new YouDaoTranslationType()
                {
                    Name = "日文",
                    Type = "ja"
                },
                new YouDaoTranslationType()
                {
                    Name = "俄文",
                    Type = "ru"
                },
                new YouDaoTranslationType()
                {
                    Name = "法文",
                    Type = "fr"
                },

            };
            this.TranslateTypes = new List<YouDaoTranslationType>(defaulTranslationTypes);
            this.TranslateTo = this.TranslateTypes.First();
            this.TranslateFrom = this.TranslateTypes.First();
        }

        public bool _isCharLimit;

        public bool IsCharLimit
        {
            get { return _isCharLimit; }
            set
            {
                _isCharLimit = value;
                RaisePropertyChanged(nameof(IsCharLimit));

            }
        }

        public int _charLimitCount;

        public int CharLimitCount
        {
            get { return _charLimitCount; }
            set
            {
                _charLimitCount = value;
                RaisePropertyChanged(nameof(CharLimitCount));

            }
        }
        private YouDaoTranslationType _translateTo;

        public YouDaoTranslationType TranslateTo
        {
            get { return _translateTo; }
            set
            {
                _translateTo = value;
                RaisePropertyChanged(nameof(TranslateTo));

            }
        }


        private YouDaoTranslationType _translateFrom;

        public YouDaoTranslationType TranslateFrom
        {
            get { return _translateFrom; }
            set
            {
                _translateFrom = value;
                RaisePropertyChanged(nameof(TranslateFrom));

            }
        }

        private List<YouDaoTranslationType> _translateTypes;

        public List<YouDaoTranslationType> TranslateTypes
        {
            get { return _translateTypes; }
            set
            {
                _translateTypes = value;
                RaisePropertyChanged(nameof(TranslateTypes));

            }
        }
    }
}
