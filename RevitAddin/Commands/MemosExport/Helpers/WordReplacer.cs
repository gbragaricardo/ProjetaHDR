using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ProjetaHDR.Commands.Helpers;

namespace ProjetaHDR.Commands.MemosExport.Helpers
{
    internal class WordReplacer
    {
        internal Document _doc;
        internal ProjectInfo _projectInfo;
        internal string _exportPath;

        public WordReplacer(Document doc, string exportPath)
        {
            _doc = doc;
            _projectInfo = doc.ProjectInformation;
            _exportPath = exportPath;
        }
        internal void ProjectInfoReplaces()
        {
            using (var handler = new WordHandler(_projectInfo, _exportPath))
            {
                string titleBlock = Sheets.GetTitleBlockName(_doc);
                var consorcio = Sheets.ValidateTitleBlock(titleBlock);

                handler.OpenWordDocument();


                handler.ReplaceText("NOME DO PROJETO", "Nome do projeto");
                handler.ReplaceText("Contratante", "Nome do Contratante");
                handler.ReplaceText("Date", "Data do Projeto");
                handler.ReplaceText("TITLE", "Título do Arquivo");
                //handler.ReplaceText("City", Cidade);
                //handler.ReplaceText("State", Estado);
                handler.ReplaceText("Consorcio", consorcio);


                handler.DeleteSpecificParagraph("Reforma");

                handler.SaveAndClose();
            }
        }

    }
}
