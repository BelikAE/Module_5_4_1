using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Module_5_4_1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            string wallInfo = string.Empty;

            var walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            foreach (Element wall in walls)
            {
                double wallVolume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
                wallInfo += $"{wall.Name}\t{wallVolume}{Environment.NewLine}";
            }

            ///Предназначенный путь сохранения файла
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string csvPath = Path.Combine(desktopPath, "wallInfo.txt");
            File.WriteAllText(csvPath, wallInfo);

            //Запрос Пути сохранения
            var saveDialog = new SaveFileDialog
            {
                OverwritePrompt = true,  //Перезавписывать ли файл?
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), //Папка записи по умолчанию
                Filter = "txt files (*.txt)|*.txt", //Фильтрация типов файлов
                FileName = "wallInfo.txt", //Имя файла по умолчанию
                DefaultExt = ".txt" //Расширение файла
            };

            string selectedFilePath = string.Empty;
            if (saveDialog.ShowDialog() == DialogResult.OK) //Если сохраниние файла прошло успешно 
            {
                selectedFilePath = saveDialog.FileName;
            }

            if (string.IsNullOrEmpty(selectedFilePath)) //Если значение пустое или налл
                return Result.Cancelled;

            File.WriteAllText(selectedFilePath, wallInfo);
            return Result.Succeeded;
        }
    }
}
