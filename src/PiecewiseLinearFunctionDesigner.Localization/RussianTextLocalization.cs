namespace PiecewiseLinearFunctionDesigner.Localization
{
    public class RussianTextLocalization : ITextLocalization
    {
        public string AppName => "Дизайнер кривых линий";
        public string Functions => "Функции";
        public string Function => "Функция";
        public string Add => "Добавить";
        public string FunctionWithNameAlreadyAdded => "Функция с именем '{0}' уже существует";
        public string InvalidFileType => "Поддерживаются только файлы с расширением '{0}'";
        public string UnsavedChanges => "Есть несохраненные изменения.";
        public string AreYouSureYouWantToCloseActiveProject => "Уверены, что хотите закрыть текущий проект?";
    }
}