using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ProjetaHDR.Commands.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Services
{
    internal class FixtureStorageManager
    {
        private static readonly Guid SchemaGuid = new Guid("80F7E558-4204-4A45-95E1-43EBD580CE2E");

        public static Schema GetOrCreateSchema()
        {
            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema != null) return schema;

            SchemaBuilder schemaBuilder = new SchemaBuilder(SchemaGuid);
            schemaBuilder.SetSchemaName("DrenCalcStorage");

            schemaBuilder.AddArrayField("Ids", typeof(string));
            schemaBuilder.AddArrayField("InstanceElementIds", typeof(ElementId));

            return schemaBuilder.Finish();
        }

        public static void SaveDataToRevit(Document doc, ObservableCollection<FixtureFamilyItem> fixtures)
        {
            Schema schema = GetOrCreateSchema();

            using (Transaction transaction = new Transaction(doc, "Salvar Tabela"))
            {
                transaction.Start();

                // Verifica se já existe um DataStorage com essa Schema
                DataStorage storage = new FilteredElementCollector(doc)
                    .OfClass(typeof(DataStorage))
                    .Cast<DataStorage>()
                    .FirstOrDefault(ds => ds.GetEntity(schema).IsValid());

                Entity entity;

                if (storage == null)
                {
                    // Se não existir, cria um novo DataStorage
                    storage = DataStorage.Create(doc);
                    entity = new Entity(schema);
                }
                else
                {
                    // Se já existir, recupera o Entity associado
                    entity = storage.GetEntity(schema);
                }

                // Convertendo os dados da lista para Arrays (IList<T>)
                IList<string> ids = fixtures.Select(f => f.Id).ToList();
                IList<ElementId> elementIds = fixtures.Select(f => f.InstanceElementId ?? ElementId.InvalidElementId).ToList();

                // Armazena os Arrays dentro do Entity
                entity.Set("Ids", ids);
                entity.Set("InstanceElementIds", elementIds);

                // Salva a entidade dentro do DataStorage
                storage.SetEntity(entity);

                transaction.Commit();
            }
        }

        public static List<FixtureFamilyItem> LoadDataFromRevit(Document doc)
        {
            Schema schema = GetOrCreateSchema();
            List<FixtureFamilyItem> loadedFixtures = new List<FixtureFamilyItem>();

            // Obtém diretamente o primeiro DataStorage que contém a Schema esperada
            DataStorage storage = new FilteredElementCollector(doc)
                .OfClass(typeof(DataStorage))
                .Cast<DataStorage>()
                .FirstOrDefault(ds => ds.GetEntity(schema).IsValid());

            if (storage != null)
            {
                Entity entity = storage.GetEntity(schema);
                if (entity.IsValid())
                {
                    IList<string> ids = entity.Get<IList<string>>("Ids");
                    IList<ElementId> elementIds = entity.Get<IList<ElementId>>("InstanceElementIds");

                    for (int i = 0; i < ids.Count; i++)
                    {
                        loadedFixtures.Add(new FixtureFamilyItem
                        {
                            Id = ids[i],
                            InstanceElementId = elementIds[i]
                        });
                    }
                }
            }

            return loadedFixtures;
        }

    }
}
