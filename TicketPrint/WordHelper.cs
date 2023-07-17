
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;

namespace TicketPrint
{
    internal class WordHelper
    {
        private FileInfo _fileInfo;

        public WordHelper(string filename)
        {
            if (File.Exists(filename))
            {
                _fileInfo = new FileInfo(filename);
            }
            else
            {
                throw new ArgumentException("Файл не найден");
            }
        }
        Word.Application app = null;
        Word.Application printingDoc = null;

        internal string Process(Dictionary<string, string> items)
        {

            try
            {
                app = new Word.Application();
                Object file = _fileInfo.FullName;
                Object missing = Type.Missing;
                app.Documents.Open(file);

                foreach (var item in items)
                {
                    Word.Find find = GetSelection(app).Find;
                    find.Text = item.Key;
                    find.Replacement.Text = item.Value;

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replase = Word.WdReplace.wdReplaceAll;

                    find.Execute(FindText: Type.Missing,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: missing,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: wrap,
                        Format: false,
                        ReplaceWith: missing,
                        Replace: replase);
                }
                string dir = _fileInfo.DirectoryName + "\\" + DateTime.Now.ToString("dd/MM/yy");
                string fileName = DateTime.Now.ToString("HH/mm/ss") + "_" + _fileInfo.Name;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string newFileName = Path.Combine(dir, fileName);
                //сохраняем отредактированный документ
                app.ActiveDocument.SaveAs2(newFileName);
                app.ActiveDocument.Close(SaveChanges: false);
                //Возвращаем директорию откуда будет печататься файл
                return newFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                app.ActiveDocument.Close(SaveChanges: false);

            }
            finally
            {
                if (app != null)
                    app.Quit();
            }
            return null;
        }

        private static Selection GetSelection(Word.Application app)
        {
            return app.Selection;
        }

        internal bool Print(string docPath)
        {
            try
            {
                printingDoc = new Word.Application();
                printingDoc.Documents.Open(docPath);
                printingDoc.ActiveDocument.PrintOut(true);
                printingDoc.ActiveDocument.Close(SaveChanges: false);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                printingDoc.ActiveDocument.Close(SaveChanges: false);
            }
            finally
            {
                if (printingDoc != null)
                {
                    printingDoc.Quit();
                }
            }
            return false;
        }
        

    }
}
