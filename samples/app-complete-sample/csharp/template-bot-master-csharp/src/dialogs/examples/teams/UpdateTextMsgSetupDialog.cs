﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Teams.TemplateBotCSharp.src.dialogs;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Update Setup Text Dialog Class. Main purpose of this class is to Setup the text message that will be update later in Bot example.
    /// </summary>
    public class UpdateTextMsgSetupDialog : ComponentDialog
    {
        protected readonly IStatePropertyAccessor<RootDialogState> _conversationState;
        public UpdateTextMsgSetupDialog(IStatePropertyAccessor<RootDialogState> conversationState) : base(nameof(UpdateTextMsgSetupDialog))
        {
            this._conversationState = conversationState;
            InitialDialogId = nameof(WaterfallDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                BeginUpdateTextMsgSetupDialogAsync,
            }));
        }

        private async Task<DialogTurnResult> BeginUpdateTextMsgSetupDialogAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stepContext == null)
            {
                throw new ArgumentNullException(nameof(stepContext));
            }

            Activity reply = stepContext.Context.Activity;
            if (reply.Attachments != null)
            {
                reply.Attachments = null;
            }

            if (reply.Entities.Count >= 1)
            {
                reply.Entities.Remove(reply.Entities[0]);
            }
            reply.Text = Strings.SetupMessagePrompt;
            reply.ReplyToId = reply.Id;
            ConnectorClient client = new ConnectorClient(new Uri(stepContext.Context.Activity.ServiceUrl), ConfigurationManager.AppSettings["MicrosoftAppId"], ConfigurationManager.AppSettings["MicrosoftAppPassword"]);
            ResourceResponse resp = await client.Conversations.ReplyToActivityAsync((Activity)reply);

            //Set the Last Dialog in Conversation Data
            var currentState = await this._conversationState.GetAsync(stepContext.Context, () => new RootDialogState());
            currentState.LastDialogKey = Strings.LastDialogSetupMessasge;
            currentState.SetUpMsgKey = resp.Id.ToString();
            await this._conversationState.SetAsync(stepContext.Context, currentState);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}