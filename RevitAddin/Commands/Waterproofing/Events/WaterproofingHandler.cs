using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models.Enums;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels.Wrappers;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Events
{
    public class WaterproofingHandler : RevitCommandBase, IExternalEventHandler
    {
        public Action<double> OnElevationPicked { get; set; }
        public Action OnExecuteCompleted { get; set; }
        public WaterproofingAction WaterproofingAction { get; set; }
        public ElementId SelectedFloorTypeId { get; set; }
        public double FloorLevelOffset { get; set; }
        public List<WaterproofingLayerItemViewModel> WaterproofingLayers { get; set; }
        public double WaterproofingHeight { get; set; }
        public double WaterproofThickness { get; set; }

        public void Execute(UIApplication app)
        {
            InitializeContextEvent(app);

            if (Context.Doc.ActiveView.GenLevel == null)
            {
                TaskDialog.Show("Atenção", "Por favor, execute este comando em uma vista de Planta.");

                OnExecuteCompleted?.Invoke();
                return;
            }

            try
            {
                using (Transaction transaction = new Transaction(Context.Doc, "Converter Regiões em Pisos"))
                {
                    transaction.Start();
                    {

                        switch (WaterproofingAction)
                        {
                            case WaterproofingAction.PickRegions:
                                RunPickerAndGeneration(Context.UiDoc, Context.Doc);
                                break;

                            case WaterproofingAction.PickOffsetTarget:
                                RunElevationPicker(Context.UiDoc, Context.Doc);
                                break;

                            case WaterproofingAction.CreateNewType:
                                RunElevationPicker(Context.UiDoc, Context.Doc);
                                break;
                        }
                    }

                    transaction.Commit();
                }
            }
            catch { }
            finally { OnExecuteCompleted?.Invoke(); }
        }


        public string GetName() => "Waterproofing Event Handler";

        private void RunPickerAndGeneration(UIDocument uidoc, Document doc)
        {
            try
            {
                PickRegionsService pickRegionsService = new PickRegionsService();
                IList<Reference> pickedRegionReferences = pickRegionsService.Pick(Context.UiDoc);

                ElementId levelId = Context.Doc.ActiveView.GenLevel.Id;
                ElementId viewId = Context.Doc.ActiveView.Id;

                foreach (Reference regionRef in pickedRegionReferences)
                {
                    Element regionElement = Context.Doc.GetElement(regionRef);

                    FilledRegion filledRegion = regionElement as FilledRegion;

                    if (filledRegion == null)
                        continue;

                    IList<CurveLoop> regionCurves = filledRegion.GetBoundaries();

                    Floor newFloor = Floor.Create(Context.Doc, regionCurves, SelectedFloorTypeId, levelId);

                    doc.Regenerate();

                    Parameter levelOffsetParameter = newFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                    Parameter floorThicknessParameter = newFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM);
                    Parameter floorPerimeterParameter = newFloor.get_Parameter(BuiltInParameter.HOST_PERIMETER_COMPUTED);
                    Parameter floorCommentParameter = newFloor.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    Parameter waterproofHeigthParameter = newFloor.get_Parameter(new Guid("37c215c0-d09c-4564-bc77-53bba751abf6"));
                    Parameter waterproofVerticalAreaParameter = newFloor.get_Parameter(new Guid("7a888c18-3ec3-4016-adb6-babb8222c85d"));
                    Parameter waterproofThickness = newFloor.get_Parameter(new Guid("bd555174-5f28-4805-b282-1c46d71b1c5e"));

                    double perimeterInMeters = UnitUtils.ConvertFromInternalUnits(floorPerimeterParameter.AsDouble(), UnitTypeId.Meters);

                    if (waterproofHeigthParameter != null && waterproofHeigthParameter.IsReadOnly == false)
                        waterproofHeigthParameter.Set(WaterproofingHeight);

                    if (waterproofThickness != null && waterproofThickness.IsReadOnly == false)
                        waterproofThickness.Set(UnitUtils.ConvertToInternalUnits(WaterproofThickness, UnitTypeId.Millimeters));

                    if (floorCommentParameter != null && floorCommentParameter.IsReadOnly == false)
                        floorCommentParameter.Set("BANHEIRO/ÁREA MOLHADA");

                    if (waterproofVerticalAreaParameter != null && waterproofVerticalAreaParameter.IsReadOnly == false)
                        waterproofVerticalAreaParameter.Set(UnitUtils.ConvertToInternalUnits((WaterproofingHeight / 100) * perimeterInMeters, UnitTypeId.SquareMeters));

                    if (levelOffsetParameter != null && levelOffsetParameter.IsReadOnly == false)
                    {
                        double offsetEmPes = UnitUtils.ConvertToInternalUnits(FloorLevelOffset, UnitTypeId.Centimeters);

                        levelOffsetParameter.Set(offsetEmPes);
                    }

                    Context.Doc.Delete(regionElement.Id);

                    Reference floorReference = new Reference(newFloor);
                    BoundingBoxXYZ insertionBBox = newFloor.get_BoundingBox(Context.Doc.ActiveView);

                    XYZ insertionPoint = (insertionBBox.Max + insertionBBox.Min) / 2;

                    IndependentTag newTag = IndependentTag.Create(
                        doc,
                        viewId,
                        floorReference,
                        true,
                        TagMode.TM_ADDBY_CATEGORY,
                        TagOrientation.Horizontal,
                        insertionPoint);

                    FamilySymbol tagSymbol = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilySymbol))
                        .OfCategory(BuiltInCategory.OST_FloorTags)
                        .Cast<FamilySymbol>()
                        .FirstOrDefault(x => x.Name == "IMP_Tag_Padrao");

                    if (tagSymbol != null)
                    {
                        newTag.ChangeTypeId(tagSymbol.Id);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        private void RunElevationPicker(UIDocument uidoc, Document doc)
        {
            try
            {
                var filter = new ElevationSelectionFilter(doc);

                Reference pickedRef = uidoc.Selection.PickObject(
                    ObjectType.PointOnElement,
                    filter,
                    "Selecione uma face ou ponto para definir o deslocamento");

                if (pickedRef == null) return;

                double absoluteZ = 0;

                absoluteZ = pickedRef.GlobalPoint.Z;

                Level currentLevel = doc.ActiveView.GenLevel;
                double levelElevation = currentLevel.Elevation;

                double offsetInFeet = absoluteZ - levelElevation;
                double offsetInCm = Math.Round(UnitUtils.ConvertFromInternalUnits(offsetInFeet, UnitTypeId.Centimeters), 3);

                OnElevationPicked?.Invoke(offsetInCm);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
        }

        private void CreateWaterproofingFloorType(Document doc, string newFloorTypeName, List<WaterproofingLayerItemViewModel> layerItems)
        {

            try
            {
                FloorType baseFloorType = new FilteredElementCollector(doc)
                    .OfClass(typeof(FloorType))
                    .Cast<FloorType>()
                    .FirstOrDefault(ft => ft.IsFoundationSlab == false);

                if (baseFloorType == null)
                    throw new Exception("Nenhum piso base encontrado para duplicar.");

                FloorType newFloorType = baseFloorType.Duplicate(newFloorTypeName) as FloorType;

                IList<CompoundStructureLayer> newFloorLayers = new List<CompoundStructureLayer>();

                // 4. Iterar sobre a lista do usuário
                foreach (var layerItem in layerItems)
                {
                    string internalName = $"IMP_{layerItem.Name}";

                    // --- CRIAÇÃO DO MATERIAL ---
                    ElementId newMaterialId = Material.Create(doc, layerItem.Name);
                    Material newMaterial = doc.GetElement(newMaterialId) as Material;

                    // Definir a Descrição (BuiltInParameter)
                    Parameter descParam = newMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);
                    if (descParam != null && !descParam.IsReadOnly)
                    {
                        descParam.Set(layerItem.Name);
                    }

                    // Definir os Parâmetros Customizados (Multiplicadores de Área)
                    // O Revit trata parâmetros Yes/No como Inteiros (1 = True, 0 = False)
                    Parameter paramHorizontal = newMaterial.LookupParameter("IMP_Area_Horizontal"); // Substitua pelo nome exato do seu parâmetro
                    if (paramHorizontal != null && !paramHorizontal.IsReadOnly)
                    {
                        paramHorizontal.Set(layerItem.HasHorizontalArea ? 1 : 0);
                    }

                    Parameter paramVertical = newMaterial.LookupParameter("IMP_Area_Vertical"); // Substitua pelo nome exato do seu parâmetro
                    if (paramVertical != null && !paramVertical.IsReadOnly)
                    {
                        paramVertical.Set(layerItem.HasVerticalArea ? 1 : 0);
                    }

                    // --- CRIAÇÃO DA CAMADA ---
                    // O Revit trabalha internamente com Pés (Feet). Precisamos converter de mm para Pés.
                    double thicknessInFeet = UnitUtils.ConvertToInternalUnits(layerItem.Thickness, UnitTypeId.Millimeters);

                    // O Item[0] é a camada principal (Core/Structure). As demais são Acabamentos (Finish1).
                    MaterialFunctionAssignment layerFunction = MaterialFunctionAssignment.Finish1;

                    CompoundStructureLayer newLayer = new CompoundStructureLayer(thicknessInFeet, layerFunction, newMaterialId);
                    newFloorLayers.Add(newLayer);
                }

                CompoundStructure newCompoundStructure = CompoundStructure.CreateSimpleCompoundStructure(newFloorLayers);

                newCompoundStructure.SetNumberOfShellLayers(ShellLayerType.Interior, newFloorLayers.Count);

                newFloorType.SetCompoundStructure(newCompoundStructure);
            }
            catch (Exception ex)
            {
                // Aqui você pode disparar um MessageBox de erro ou passar para a UI
                throw new Exception($"Falha ao criar o tipo de impermeabilização: {ex.Message}");
            }
        }
    }
}

