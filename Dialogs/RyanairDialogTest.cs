using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;


namespace Ryanair.Echobot
{
    [Serializable]
    public class RyanairTest : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "Flights")
            {
                PromptDialog.Confirm(
                    context,
                    AfterFlightsAsync,
                    "Do you want to start looking for flights?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterFlightsAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Flight search.");
            }
            else
            {
                await context.PostAsync("Did not search for flights.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}
