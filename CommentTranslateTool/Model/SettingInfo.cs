using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workshop.Helper;

namespace Workshop.Model
{
    public class SettingInfo : ViewModelBase
    {

        public SettingInfo()
        {
            this.TranslateTo = YouDaoApiHelper.TranslationTypes.First();
            this.TranslateFrom = YouDaoApiHelper.TranslationTypes.First();
        }

        private bool _isCharLimit;

        public bool IsCharLimit
        {
            get { return _isCharLimit; }
            set
            {
                _isCharLimit = value;
                RaisePropertyChanged(nameof(IsCharLimit));

            }
        }

        private bool _isTranslateAngleBracketElement;

        public bool IsTranslateAngleBracketElement
        {
            get { return _isTranslateAngleBracketElement; }
            set
            {
                _isTranslateAngleBracketElement = value;
                RaisePropertyChanged(nameof(IsTranslateAngleBracketElement));

            }
        }

        private int _charLimitCount;

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
    }


}
