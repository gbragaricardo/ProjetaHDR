using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using ProjetaHDR.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Services
{
    internal class FixtureStorageManager
    {
        private static readonly Guid SchemaGuid = new Guid("A3D44214-AB12-4270-9B42-C9B135EE4504");
        private static readonly Guid SubSchemaGuid = new Guid("F44CEFE6-1BD3-41FE-B651-0C782933FD9D");

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
            fixtureSchemaBuilder.AddArrayField("InputFixturesIds", typeof(string));
            fixtureSchemaBuilder.AddArrayField("OutputPipesIds", typeof(ElementId));

            Schema fixtureSchema = fixtureSchemaBuilder.Finish();
            fieldFixtureItems.SetSubSchemaGUID(SubSchemaGuid);

            return schemaBuilder.Finish();
        }

        public static void SaveDataToRevit(Document doc, ObservableCollection<FixtureFamilyItem> fixtures)
        {
            Schema schema = GetOrCreateSchema();
            Schema fixtureSchema = schema.GetField("FixtureItems").SubSchema;

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
                // FAZER UM NULL CHECK CORRETO
                fixtureEntity.Set("InputFixturesIds", (IList<string>)fixture.InputFixtureItems.Select(f => f.CorrespondentFixture != null ? f.CorrespondentFixture.Id : "").ToList());
                fixtureEntity.Set("OutputPipesIds", (IList<ElementId>)fixture.OutputPipes.Select(p => p.IsValidObject ? p.Id : null).ToList());


                fixtureEntities.Add(fixtureEntity);
            }
            entity.Set("FixtureItems", fixtureEntities);
            storage.SetEntity(entity);

               
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
                        IList<string> inputFixturesIds = fixtureEntity.Get<IList<string>>("InputFixturesIds");
                        IList<ElementId> outputPipesIds = fixtureEntity.Get<IList<ElementId>>("OutputPipesIds");


                        FixtureFamilyItem fixtureFamilyItem = new FixtureFamilyItem
                        {
                            Id = id,
                            InstanceElementId = instanceElementId,
                            InputAreas = new ObservableCollection<AreaFamilyItem>(inputAreasIds.Select(areaId => new AreaFamilyItem 
                            {
                                InstanceElementId = areaId 
                            }).ToList()),

                            InputFixtureItems = new ObservableCollection<InputFixtureItem>(inputFixturesIds.Select(fixId => new InputFixtureItem
                            {
                                Id = fixId
                            }).ToList()),

                            OutputPipes = new ObservableCollection<Pipe>(outputPipesIds
                            .Where(pId => pId != null)
                            .Select(pId => doc.GetElement(pId) as Pipe)
                            .ToList())
                        };

                        loadedFixtures.Add(fixtureFamilyItem);
                    }
                }
            }

            return loadedFixtures;
        }

    }
}
