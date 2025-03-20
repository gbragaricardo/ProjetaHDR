using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ProjetaHDR.UI.ViewModels;
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
        private static readonly Guid SubSchemaGuid = new Guid("F184919C-0EB0-43C9-94EE-CF7C9B330330");

        public static Schema GetOrCreateSchema()
        {
            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema != null) return schema;

            SchemaBuilder schemaBuilder = new SchemaBuilder(SchemaGuid);
            schemaBuilder.SetSchemaName("DrenCalcStorage");

            FieldBuilder fieldFixtureItems = schemaBuilder.AddArrayField("FixtureItems", typeof(Entity));


            SchemaBuilder fixtureSchemaBuilder = new SchemaBuilder(SubSchemaGuid);
            fixtureSchemaBuilder.SetSchemaName("FixtureItemSchema");

            fixtureSchemaBuilder.AddSimpleField("Id", typeof(string));
            fixtureSchemaBuilder.AddSimpleField("InstanceElementId", typeof(ElementId));
            fixtureSchemaBuilder.AddArrayField("InputAreasIds", typeof(ElementId));
            fixtureSchemaBuilder.AddArrayField("InputFixturesIds", typeof(ElementId));

            Schema fixtureSchema = fixtureSchemaBuilder.Finish();
            fieldFixtureItems.SetSubSchemaGUID(SubSchemaGuid);

            return schemaBuilder.Finish();
        }

        public static void SaveDataToRevit(Document doc, ObservableCollection<FixtureFamilyItem> fixtures)
        {
            Schema schema = GetOrCreateSchema();
            Schema fixtureSchema = schema.GetField("FixtureItems").SubSchema;

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

                IList<Entity> fixtureEntities = new List<Entity>();
                
                foreach (FixtureFamilyItem fixture in fixtures)
                {
                    Entity fixtureEntity = new Entity(fixtureSchema);

                    fixtureEntity.Set("Id", fixture.Id);
                    fixtureEntity.Set("InstanceElementId", fixture.InstanceElementId ?? ElementId.InvalidElementId);
                    fixtureEntity.Set("InputAreasIds", (IList<ElementId>)fixture.InputAreas.Select(a => a.InstanceElementId ?? ElementId.InvalidElementId).ToList());
                    fixtureEntity.Set("InputFixturesIds", (IList<ElementId>)fixture.InputFixtureItems.Select(f => f.InstanceElementId ?? ElementId.InvalidElementId).ToList());

                    fixtureEntities.Add(fixtureEntity);
                }
                entity.Set("FixtureItems", fixtureEntities);
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

                    IList<Entity> fixtureEntities = entity.Get<IList<Entity>>("FixtureItems");

                    foreach (Entity fixtureEntity in fixtureEntities)
                    {
                        string id = fixtureEntity.Get<string>("Id");
                        ElementId instanceElementId = fixtureEntity.Get<ElementId>("InstanceElementId");
                        IList<ElementId> inputAreasIds = fixtureEntity.Get<IList<ElementId>>("InputAreasIds");
                        IList<ElementId> inputFixturesIds = fixtureEntity.Get<IList<ElementId>>("InputFixturesIds");

                        FixtureFamilyItem fixtureFamilyItem = new FixtureFamilyItem
                        {
                            Id = id,
                            InstanceElementId = instanceElementId,
                            InputAreas = new ObservableCollection<AreaFamilyItem>(inputAreasIds.Select(areaId => new AreaFamilyItem 
                            {
                                InstanceElementId = areaId 
                            }).ToList()),

                            InputFixtureItems = new ObservableCollection<FixtureFamilyItem>(inputFixturesIds.Select(fixId => new FixtureFamilyItem
                            {
                                InstanceElementId = fixId
                            }).ToList())
                        };

                        loadedFixtures.Add(fixtureFamilyItem);
                    }
                }
            }

            return loadedFixtures;
        }

    }
}
