using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class CreateVMDialog : CancelAndHelpDialog
    {
        private const string RegionStepMsgText = "Where would you like to create the VM?";
        private const string CoreSizeStepMsgText = "How many cores do you want for the VM?";
        private const string MemoryInGbStepMsgText = "What is the memory size (in GB) that you want for the VM?";

        public CreateVMDialog()
            : base(nameof(CreateVMDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RegionStepAsync,
                CoreSizeStepAsync,
                MemorySizeStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RegionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var creatingDetails = (CreatingDetails)stepContext.Options;

            if (creatingDetails.Region == null)
            {
                var promptMessage = MessageFactory.Text(RegionStepMsgText, RegionStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(creatingDetails.Region, cancellationToken);
        }

        private async Task<DialogTurnResult> CoreSizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var creatingDetails = (CreatingDetails)stepContext.Options;

            creatingDetails.Region = (string)stepContext.Result;

            if (creatingDetails.CoreSize == null)
            {
                var promptMessage = MessageFactory.Text(CoreSizeStepMsgText, CoreSizeStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(creatingDetails.CoreSize, cancellationToken);
        }

        private async Task<DialogTurnResult> MemorySizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var creatingDetails = (CreatingDetails)stepContext.Options;

            creatingDetails.CoreSize = (string)stepContext.Result;

            if (creatingDetails.MemorySize == null)
            {
                var promptMessage = MessageFactory.Text(MemoryInGbStepMsgText, MemoryInGbStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(creatingDetails.MemorySize, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var creatingDetails = (CreatingDetails)stepContext.Options;

            creatingDetails.MemorySize = (string)stepContext.Result;

            var messageText = $"Please confirm, I have you creating a VM in: {creatingDetails.Region} core size: {creatingDetails.CoreSize} memory size: {creatingDetails.MemorySize}. Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var creatingDetails = (CreatingDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(creatingDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
