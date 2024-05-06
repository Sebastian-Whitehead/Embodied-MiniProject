using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLMUnity;

namespace LLMUnitySamples
{
    public class ChatBot : MonoBehaviour
    {
        // Variables declaration
        public Transform chatContainer;
        public Color playerColor = new Color32(81, 164, 81, 255);
        public Color aiColor = new Color32(29, 29, 73, 255);
        public Color fontColor = Color.white;
        public Font font;
        public int fontSize = 16;
        public int bubbleWidth = 600;
        public LLMClient llm;
        public float textPadding = 10f;
        public float bubbleSpacing = 10f;
        public Sprite sprite;

        private InputBubble inputBubble;
        private List<Bubble> chatBubbles = new List<Bubble>();
        private bool blockInput = true;
        private BubbleUI playerUI, aiUI;
        private bool warmUpDone = false;
        private int lastBubbleOutsideFOV = -1;

        private UdpSocket udpSocket;

        // Start method
        void Start()
        {
            // Set default font if not specified
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Initialize player and AI bubble UI
            playerUI = new BubbleUI
            {
                sprite = sprite,
                font = font,
                fontSize = fontSize,
                fontColor = fontColor,
                bubbleColor = playerColor,
                bottomPosition = 0,
                leftPosition = 0,
                textPadding = textPadding,
                bubbleOffset = bubbleSpacing,
                bubbleWidth = bubbleWidth,
                bubbleHeight = -1
            };
            aiUI = playerUI;
            aiUI.bubbleColor = aiColor;
            aiUI.leftPosition = 1;

            // Initialize input bubble
            inputBubble = new InputBubble(chatContainer, playerUI, "InputBubble", "Loading...", 4);
            inputBubble.AddSubmitListener(onInputFieldSubmit);
            inputBubble.AddValueChangedListener(onValueChanged);
            inputBubble.setInteractable(false);
            _ = llm.Warmup(WarmUpCallback);

            // Initialize UDP socket
            udpSocket = this.GetComponent<UdpSocket>();
        }

        // Method to handle input field submission
        void onInputFieldSubmit(string newText)
        {
            // Activate input field
            inputBubble.ActivateInputField();

            // Check for conditions to block input
            if (blockInput || newText.Trim() == "" || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                StartCoroutine(BlockInteraction());
                return;
            }
            blockInput = true;

            // Get message from input bubble
            string message = inputBubble.GetText().Replace("\v", "\n");

            // Create player and AI bubbles
            Bubble playerBubble = new Bubble(chatContainer, playerUI, "PlayerBubble", message);
            Bubble aiBubble = new Bubble(chatContainer, aiUI, "AIBubble", "...");
            chatBubbles.Add(playerBubble);
            chatBubbles.Add(aiBubble);
            playerBubble.OnResize(UpdateBubblePositions);
            aiBubble.OnResize(UpdateBubblePositions);

            // Format message and send to AI
            message = messageFormatter(message);
            Task chatTask = llm.Chat(message, aiBubble.SetText, AllowInput);
            inputBubble.SetText("");
        }

        // CUSTOM FUNCTION TO APPEND THE DETECTED EMOTION TO THE SUBMITTED MESSAGE
        public string messageFormatter(string message)
        {
            message += "<" + udpSocket.lastRecieved + ">";
            Debug.Log("<" + udpSocket.lastRecieved + ">");
            return message;
        }

        // Warm-up callback method
        public void WarmUpCallback()
        {
            warmUpDone = true;
            inputBubble.SetPlaceHolderText("Message me");
            AllowInput();
        }

        // Allow input method
        public void AllowInput()
        {
            blockInput = false;
            inputBubble.ReActivateInputField();
        }

        // Cancel requests method
        public void CancelRequests()
        {
            llm.CancelRequests();
            AllowInput();
        }

        // Coroutine to block interaction
        IEnumerator<string> BlockInteraction()
        {
            inputBubble.setInteractable(false);
            yield return null;
            inputBubble.setInteractable(true);
            inputBubble.MoveTextEnd();
        }

        // On value changed method
        void onValueChanged(string newText)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                if (inputBubble.GetText().Trim() == "")
                    inputBubble.SetText("");
            }
        }

        // Update bubble positions method
        public void UpdateBubblePositions()
        {
            float y = inputBubble.GetSize().y + inputBubble.GetRectTransform().offsetMin.y + bubbleSpacing;
            float containerHeight = chatContainer.GetComponent<RectTransform>().rect.height;
            for (int i = chatBubbles.Count - 1; i >= 0; i--)
            {
                Bubble bubble = chatBubbles[i];
                RectTransform childRect = bubble.GetRectTransform();
                childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x, y);

                if (y > containerHeight && lastBubbleOutsideFOV == -1)
                {
                    lastBubbleOutsideFOV = i;
                }
                y += bubble.GetSize().y + bubbleSpacing;
            }
        }

        // Update method
        void Update()
        {
            if (!inputBubble.inputFocused() && warmUpDone)
            {
                inputBubble.ActivateInputField();
                StartCoroutine(BlockInteraction());
            }
            if (lastBubbleOutsideFOV != -1)
            {
                for (int i = 0; i <= lastBubbleOutsideFOV; i++)
                {
                    chatBubbles[i].Destroy();
                }
                chatBubbles.RemoveRange(0, lastBubbleOutsideFOV + 1);
                lastBubbleOutsideFOV = -1;
            }
        }

        // Exit game method
        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }
    }
}
