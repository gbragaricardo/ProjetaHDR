using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Utils
{
    internal static class TagManager
    {
        public static ElementId GetTagId(Document doc, string tagMode)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeTags)
                .WhereElementIsElementType()
                .ToElements()
                .FirstOrDefault(e => e.Name == tagMode).Id;
        }

        public static void CreateTags(Document doc, IList<Element> elementsList, ElementId tagId, IList<XYZ> insertionPoint, View activeView = null)
        {
            if (activeView == null)
                activeView = doc.ActiveView;

            for (int i = 0; i < elementsList.Count; i++)
            {
                IndependentTag.Create(doc,
                    tagId,
                    activeView.Id,
                    new Reference(elementsList[i]),
                    false, TagOrientation.Horizontal,
                    insertionPoint[i]);
            }
        }

        public static void DeleteExistingTags(Document doc, IList<Element> elementsList, ElementId tagTypeId, View activeView = null)
        {
            
            if (activeView == null)
                activeView = doc.ActiveView;

            if (elementsList == null || elementsList.Count == 0) return;

            var tagsOnView = new FilteredElementCollector(doc, activeView.Id)
                .OfClass(typeof(IndependentTag))
                .WhereElementIsNotElementType()
                .Cast<IndependentTag>()
                .ToList();

            HashSet<ElementId> pipesId = new HashSet<ElementId>(elementsList.Select(t => t.Id));

            List<ElementId> tagsToDelete = new List<ElementId>();

            foreach (var tag in tagsOnView)
            {
                if (tag.GetTypeId() == tagTypeId)
                {
                    
                    ISet<ElementId> taggedElementsId = tag.GetTaggedLocalElementIds();

                    if (taggedElementsId.Any(id => pipesId.Contains(id)))
                    {
                        tagsToDelete.Add(tag.Id);
                    }
                }
            }
            // Remove todas as tags identificadas de uma vez
            if (tagsToDelete.Count > 0)
            {
                doc.Delete(tagsToDelete);
            }
        }

    }

}
