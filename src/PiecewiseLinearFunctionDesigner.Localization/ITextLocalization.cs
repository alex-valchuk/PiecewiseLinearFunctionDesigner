namespace PiecewiseLinearFunctionDesigner.Localization
{
    public interface ITextLocalization
    {
        string AppName { get; }
        string Functions { get; }
        string Function { get; }
        string FunctionName { get; }
        string FunctionParameters { get; }
        string Add { get; }
        string FunctionWithNameAlreadyAdded { get; }
        string InvalidFileType { get; }
        string UnsavedChanges { get; }
        string AreYouSureYouWantToCloseActiveProject { get; }
        string ProjectSuccessfullySaved { get; }
        string Temperature { get; }
        string AbsoluteMark { get; }
        string Graph { get; }
    }
}