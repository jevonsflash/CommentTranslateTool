using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Workshop.Model;

namespace Workshop.Helper
{
    public class ImportHelper
    {
        private static string _fileName = "";
        private static string _excelFilesXlsxXls = "所有文件|*.*";


        public static (string,ParserProvider) ImportFrom()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonHelper.DesktopPath;
            openFileDialog.Filter = _excelFilesXlsxXls;
            openFileDialog.FileName = _fileName;
            openFileDialog.AddExtension = true;
            openFileDialog.RestoreDirectory = true;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filePath = openFileDialog.FileName;
                return ImportFromPath(filePath);
            }
            else
            {
                return default;
            }
        }


        public static (string, ParserProvider) ImportFromPath(string filePath)
        {

            ParserProvider output = default;

            var data1 = string.Empty;
            try
            {
                data1 = DirFileHelper.ReadFile(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(filePath + " 此文件正被其他程序占用");
                return (data1, output);
            }

            try
            {
                foreach (var parserProvider in ParserProvider.GetAllProviders())
                {
                    var currentAllowExtension = parserProvider.FileExtensionList;
                    foreach (var extension in currentAllowExtension)
                    {
                        if (filePath.EndsWith(extension))
                        {
                            output = parserProvider;
                            break;
                        }
                    }
                }

                if (output == null)
                {
                    MessageBox.Show(filePath + " 不支持的文件格式");
                    return (data1, output); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(filePath + " 格式错误");
            }

            return (data1, output);
        }


    }
}
