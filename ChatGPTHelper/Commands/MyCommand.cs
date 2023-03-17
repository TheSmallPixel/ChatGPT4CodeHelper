using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using OpenAI_API.Completions;

namespace ChatGPTHelper
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        private ChatGTPService _chatGPT4Service;

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs commandService)
        {
            //_package = package ?? throw new ArgumentNullException(nameof(package));
            //CommandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            _chatGPT4Service = new ChatGTPService();

            //var menuCommandID = new CommandID(new Guid("c264bd94-0a43-4d1d-944a-8e3529b1afec"), 0x0100);
            //var menuItem = new MenuCommand(Execute, menuCommandID);
            //commandService.AddCommand(menuItem);                            

            firstInteration = true;
            lineLength = 0;

            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

            docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView == null) return;

            position = docView.TextView.Caret.Position.BufferPosition.Position;
            positionStart = docView.TextView.Selection.Start.Position.Position;
            positionEnd = docView.TextView.Selection.End.Position.Position;

            selectedText = docView.TextView.Selection.StreamSelectionSpan.GetText();
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                await VS.MessageBox.ShowAsync("EXTENSION_NAME", "Please select the code.", buttons: Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK);

                return;
            }

            if (CheckIfSelectedTwoOrMoreMethods(selectedText))
            {
                await VS.MessageBox.ShowAsync("EXTENSION_NAME", "Please select one method at a time.", buttons: Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK);

                return;
            }
            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            var selection = doc?.TextView?.Selection.SelectedSpans.FirstOrDefault();
            //var dte = await _package.GetServiceAsync(typeof(SDTE)) as EnvDTE80.DTE2;
            //EnvDTE.TextSelection textSelection = dte.ActiveDocument.Selection as EnvDTE.TextSelection;
            //string currentLineText = textSelection.ActivePoint.CreateEditPoint()
            //                var selection = doc?.TextView.Selection.SelectedSpans.FirstOrDefault();
            if (!selection.HasValue) return;

            string codeSuggestion = string.Empty;
            await VS.StatusBar.ShowProgressAsync("Waiting chatGPT response", 1, 2);

            await _chatGPT4Service.GetCodeSuggestionAsync(selection.Value.GetText(), ResultHandler);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                //Some documents does not has format
                (await VS.GetServiceAsync<DTE, DTE>()).ExecuteCommand("Edit.FormatDocument", string.Empty);
            }
            catch (Exception)
            {

            }

            if (!string.IsNullOrEmpty(codeSuggestion))
            {
                //textSelection.Insert(codeSuggestion);
            }
            else
            {
                await VS.MessageBox.ShowWarningAsync("No suggestion", "Button clicked");
            }
            await VS.MessageBox.ShowWarningAsync("ChatGPTHelper", "Button clicked");
        }
        private bool CheckIfSelectedTwoOrMoreMethods(string text)
        {
            string[] words = text.Split(' ');

            return words.Count(w => w == "public" || w == "private" || w == "protected") >= 2;
        }
        protected DocumentView docView;
        private string selectedText;
        private int position;
        private int positionStart;
        private int positionEnd;
        private int lineLength;
        private bool firstInteration;
        private async void ResultHandler(int index, CompletionResult result)
        {
            try
            {
                if (firstInteration)
                {
                    await VS.StatusBar.ShowProgressAsync("Receiving chatGPT response", 2, 2);

                    position = positionStart;

                    //Erase current code
                    _ = (docView.TextBuffer?.Replace(
                        new Span(position, docView.TextView.Selection.StreamSelectionSpan.GetText().Length),
                        String.Empty));
                    //if (commandType == CommandType.Erase)
                    //{
                    //    position = positionStart;

                    //    //Erase current code
                    //    _ = (docView.TextBuffer?.Replace(
                    //        new Span(position, docView.TextView.Selection.StreamSelectionSpan.GetText().Length),
                    //        String.Empty));
                    //}
                    //else if (commandType == CommandType.InsertBefore)
                    //{
                    //    position = positionStart;

                    //    _ = (docView.TextBuffer?.Insert(position, Environment.NewLine));
                    //}
                    //else
                    //{
                    //    position = positionEnd;

                    //    _ = (docView.TextBuffer?.Insert(position, Environment.NewLine));
                    //}

                    firstInteration = false;
                }

                string resultText = result.ToString();

                docView.TextBuffer?.Insert(position, resultText);

                position += resultText.Length;

                lineLength += resultText.Length;

                //if (lineLength > 160 && typeof(TCommand) == typeof(AskAnything))
                //{
                //    lineLength = 0;
                //    MovetoNextLine();
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public static async Task InitializeAsync(AsyncPackage package)
        //{
        //    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

        //    var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        //    _ = new MyCommand(package, commandService);
        //}


    }
}
