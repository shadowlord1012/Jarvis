using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Context
{
    public sealed class AIContext
    {
        // The conversation ID is a unique identifier for the conversation. It can be used to track the conversation across multiple requests and responses.
        public string ConversationId { get; set; } = string.Empty;

        // The user input is the text that the user has entered into the chat interface. It can be used to generate a response from the AI model.
        public string UserInput { get; set; } = string.Empty;

        // The system prompt is the initial instruction or context provided to the AI model. It can be used to guide the model's responses.
        public string SystemPrompt { get; set; } = string.Empty;

        // The messages are the individual pieces of communication in the conversation. They can be used to maintain context and generate coherent responses.
        public IList<LLMMessage> Messages { get; set; }
            = new List<LLMMessage>();

        // The tools are the external functions or APIs that the AI model can use to perform specific tasks. They can be used to enhance the model's capabilities and provide more accurate responses.
        public IList<ToolDefinition> Tools { get; set; }
            = new List<ToolDefinition>();

        // The chat options are the settings that control the behavior of the AI model. They can be used to customize the model's responses and improve the user experience.
        public ChatOptions ChatOptions { get; set; }
            = new();

        // The metadata is additional information that can be associated with the conversation. It can be used to store custom data or context that may be relevant to the conversation.
        public IDictionary<string, object> Metadata { get; set; }
            = new Dictionary<string, object>();
    }
}
