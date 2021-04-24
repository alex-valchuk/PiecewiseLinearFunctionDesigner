using Prism.Commands;

namespace PiecewiseLinearFunctionDesigner.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand NewCommand { get; }
        
        CompositeCommand OpenCommand { get; }

        CompositeCommand SaveCommand { get; }
        
        CompositeCommand SaveAndExitCommand { get; }

        CompositeCommand ExitCommand { get; }
        
        CompositeCommand ChangeMadeCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand NewCommand { get; } = new CompositeCommand();
        public CompositeCommand OpenCommand { get; } = new CompositeCommand();
        public CompositeCommand SaveCommand { get; } = new CompositeCommand();
        public CompositeCommand SaveAndExitCommand { get; } = new CompositeCommand();
        public CompositeCommand ExitCommand { get; } = new CompositeCommand();
        
        public CompositeCommand ChangeMadeCommand { get; } = new CompositeCommand();
    }
}