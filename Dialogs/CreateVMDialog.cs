using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class CreateVMDialog : CancelAndHelpDialog
    {
        public CreateVMDialog() : base(nameof(CreateVMDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {

            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
    }
}
