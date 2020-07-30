using System.Threading;
using System.Threading.Tasks;
using CoreBot;
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

        private const string LoginAsyncMsgText = "You have not logged in. Please login.";
        private const string NameStepMsgText = "Login succeeds. I have confirmed your identity based on your voice signature. What VM name would you like?";
        private const string LocationStepMsgText = "Where would you like to create the VM?";
        private const string ImageStepMsgText = "What OS do you want to install to the VM?";
        private const string DiskSizeStepMsgText = "What disk size in GB do you want?";

        public CreateVMDialog()
            : base(nameof(CreateVMDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                LoginAsync,
                NameAsync,
                LocationAsync,
                ImageAsync,
                DiskSizeAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> LoginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text(LoginAsyncMsgText, LoginAsyncMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var createVMDetails = (CreateVMDetails)stepContext.Options;

            if (createVMDetails.Name == null)
            {
                var promptMessage = MessageFactory.Text(NameStepMsgText, NameStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(createVMDetails.Name, cancellationToken);
        }

        private async Task<DialogTurnResult> LocationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var createVMDetails = (CreateVMDetails)stepContext.Options;

            createVMDetails.Name = (string)stepContext.Result;

            if (createVMDetails.Location == null)
            {
                var promptMessage = MessageFactory.Text(LocationStepMsgText, LocationStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(createVMDetails.Location, cancellationToken);
        }

        private async Task<DialogTurnResult> ImageAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var createVMDetails = (CreateVMDetails)stepContext.Options;

            createVMDetails.Location = (string)stepContext.Result;

            if (createVMDetails.Image == null)
            {
                var promptMessage = MessageFactory.Text(ImageStepMsgText, ImageStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(createVMDetails.Image, cancellationToken);
        }

        private async Task<DialogTurnResult> DiskSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var createVMDetails = (CreateVMDetails)stepContext.Options;

            createVMDetails.Image = (string)stepContext.Result;

            if (createVMDetails.DiskSize == null)
            {
                var promptMessage = MessageFactory.Text(DiskSizeStepMsgText, DiskSizeStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(createVMDetails.DiskSize, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var createVMDetails = (CreateVMDetails)stepContext.Options;

            createVMDetails.DiskSize = (string)stepContext.Result;

            var messageText = $"Please confirm, I will create a VM, name: {createVMDetails.Name}, location: {createVMDetails.Location}, OS: {createVMDetails.Image}, disk size: {createVMDetails.DiskSize}GB. Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var createVMDetails = (CreateVMDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(createVMDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
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
    }
}
