// Send the message when Enter is pressed
function handleKeyDown(event) {
    if (event.key === "Enter") {
        askAI();
    }
}

// Fill the input with a suggested question and send it
function askSuggestedQuestion(question) {
    document.getElementById("question").value = question;
    askAI();
}

async function askAI() {
    const input = document.getElementById("question");
    const question = input.value.trim();

    if (!question) {
        return;
    }

    const chatArea = document.getElementById("chatArea");

    // Show the user's message
    const userMessage = document.createElement("div");

    userMessage.className = "message user-message";

    userMessage.innerHTML = `
        <div class="message-bubble">
            ${escapeHtml(question)}
        </div>
    `;

    chatArea.appendChild(userMessage);

    input.value = "";

    // Show a temporary loading message
    const loadingMessage = document.createElement("div");

    loadingMessage.className =
        "message assistant-message loading-message";

    loadingMessage.innerHTML = `
        <div class="message-icon">
            <i data-lucide="bot"></i>
        </div>

        <div class="message-content">
            <span class="thinking-text">
                Thinking
            </span>

            <span class="thinking-dots">
                <span>.</span>
                <span>.</span>
                <span>.</span>
            </span>
        </div>
    `;

    chatArea.appendChild(loadingMessage);

    lucide.createIcons();

    chatArea.scrollTop = chatArea.scrollHeight;

    try {
        // Send the question to the backend
        const response = await fetch("/AI/Ask", {
            method: "POST",

            headers: {
                "Content-Type":
                    "application/x-www-form-urlencoded"
            },

            body:
                `question=${encodeURIComponent(question)}`
        });

        if (!response.ok) {
            throw new Error("Request failed");
        }

        const data = await response.json();

        loadingMessage.remove();

        // Display the AI response
        const assistantMessage = document.createElement("div");

        assistantMessage.className =
            "message assistant-message";

        assistantMessage.innerHTML = `
            <div class="message-icon">
                <i data-lucide="bot"></i>
            </div>

            <div class="message-bubble ai-response">
                ${marked.parse(data.answer)}
            </div>
        `;

        chatArea.appendChild(assistantMessage);

        lucide.createIcons();

    } catch (error) {
        loadingMessage.remove();

        // Show an error if the request fails
        const errorMessage = document.createElement("div");

        errorMessage.className =
            "message assistant-message";

        errorMessage.innerHTML = `
            <div class="message-icon">
                <i data-lucide="bot"></i>
            </div>

            <div class="message-bubble">
                Something went wrong.
                Please try again.
            </div>
        `;

        chatArea.appendChild(errorMessage);

        lucide.createIcons();
    }

    chatArea.scrollTop = chatArea.scrollHeight;
}

function escapeHtml(text) {
    const div = document.createElement("div");

    div.textContent = text;

    return div.innerHTML;
}