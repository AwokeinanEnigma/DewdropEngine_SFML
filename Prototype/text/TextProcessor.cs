// Decompiled with JetBrains decompiler
// Type: Mother4.Scripts.Text.TextProcessor
// Assembly: Mother4, Version=0.6.6072.35821, Culture=neutral, PublicKeyToken=null
// MVID: 75372462-4B0E-4582-8CEA-2817B7024D27
// Assembly location: D:\OddityPrototypes\Mother4 - Copy (2).exe

using DewDrop.GUI.Fonts;
using DewDrop.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace Mother4.Scripts.Text; 

internal class TextProcessor
{
    private const char NEWLINE_CHAR = '\n';
    private const char BULLET_CHAR = '@';
    private const char SPACE_CHAR = ' ';
    private const int PAUSE_CHAR_DURATION = 10;
    private const int BULLET_PAUSE_DURATION = 30;
    private const string ENCLOSING_CMD_REGEX = "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\](.*?)\\[\\/\\1\\]";
    private const string SINGLE_CMD_REGEX = "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\]";
    private const string CMD_PAUSE = "p";
    private const string CMD_CHARNAME = "cn";
    private const string CMD_TRIGGER = "t";
    private const string CMD_TRAVIS = "travis";
    private const string CMD_FLOYD = "floyd";
    private const string CMD_MERYL = "meryl";
    private const string CMD_LEO = "leo";
    private const string CMD_ZACK = "zack";
    private const string CMD_RENEE = "renee";
    private static readonly char[] _PauseChars = new[]
    {
        ',',
        '?'
    };

    public static TextBlock Process (FontData font, string text, int frameWidth) {
        // List to hold each line of the text
        List<string> linesList = new List<string>();

        // Dictionary to track which lines have bullet points
        Dictionary<int, bool> bulletLines = new Dictionary<int, bool>();

        // List to hold text commands
        List<ITextCommand> textCommands = new List<ITextCommand>();

        // StringBuilder to manipulate the text
        StringBuilder textBuilder = new StringBuilder(text ?? "").Replace("\r", "");

        // Find and replace all enclosing commands in the text
        MatchCollection enclosingCommandMatches = Regex.Matches(textBuilder.ToString(), ENCLOSING_CMD_REGEX);
        int commandLengthOffset = 0;
        foreach (Match match in enclosingCommandMatches) {
            string replacementText = match.Groups[3].Value;
            textBuilder = textBuilder.Remove(match.Index, match.Length);
            textBuilder = textBuilder.Insert(match.Index, replacementText);
        }
        // Find and process all single commands in the text
        foreach (Match match in Regex.Matches(textBuilder.ToString(), SINGLE_CMD_REGEX)) {
            string commandName = match.Groups[1].Value;
            string commandParamsStr = match.Groups[2].Value;
            string[] commandParams = commandParamsStr.Split(',');

            for (int index = 0; index < commandParams.Length; ++index)
                commandParams[index] = commandParams[index].Trim();

            int commandPosition = match.Index - commandLengthOffset;
            OffsetCommandPositions(textCommands, commandPosition, match.Length);
            textBuilder = textBuilder.Remove(commandPosition, match.Length);
            commandLengthOffset += match.Length;

            switch (commandName) {
            case "p":
                int pauseDuration;
                int.TryParse(commandParamsStr, out pauseDuration);
                textCommands.Add(new TextPause(commandPosition, pauseDuration));
                continue;
            case "t":
                int eventId;
                int.TryParse(commandParams[0], out eventId);
                string[] eventArgs = new string[commandParams.Length - 1];
                Array.Copy(commandParams, 1, eventArgs, 0, eventArgs.Length);
                textCommands.Add(new TextTrigger(commandPosition, eventId, eventArgs));
                continue;
            default:
                continue;
            }
        }
        // Create a new SFML text object with the processed text
        SFML.Graphics.Text sfmlText = new SFML.Graphics.Text(textBuilder.ToString().Replace('@'.ToString(), string.Empty).Replace('\n'.ToString(), string.Empty), font.Font, font.Size);

        float accumulatedWidth = 0.0f;
        float previousCharPositionX = sfmlText.FindCharacterPos(0U).X;
        int lastSpaceIndex = 0;
        int currentLineStartIndex = 0;
        int currentLineEndIndex = 0;
        bool newLineNeeded = false;
        bool bulletPointActive = false;

        bool splitDueToWidth = false;
        // Process each character in the text, breaking lines where necessary
        for (int index = 0; index < textBuilder.Length; ++index) {
            char currentChar = textBuilder[index];
            bool newLine = false;
            if (_PauseChars.Contains(currentChar)) {
                textCommands.Add(new TextPause(index + 1, 5));
            } else {
                switch (currentChar) {
                case '\n':
                    currentLineEndIndex = index;
                    textBuilder = textBuilder.Remove(index, 1);
                    OffsetCommandPositions(textCommands, index, -1);
                    --index;
                    newLine = true;
                    newLineNeeded = true;
                    break;
                case ' ':
                    lastSpaceIndex = index;
                    break;
                case '@':
                    currentLineEndIndex = index;
                    textBuilder = textBuilder.Remove(index, 1);
                    OffsetCommandPositions(textCommands, index, -1);
                    --index;
                    newLineNeeded = index > currentLineStartIndex;
                    bulletPointActive = true;
                    if (bulletPointActive) {
                        textCommands.Add(new TextWait(index)); 
                    }
                    bulletLines.Add(linesList.Count + (newLineNeeded ? 1 : 0), true);
                    continue;
                }
            }
            float currentCharPositionX = sfmlText.FindCharacterPos((uint)index).X ;
            float charWidth = (currentCharPositionX - previousCharPositionX) + .1f;
            accumulatedWidth += charWidth;
            previousCharPositionX = currentCharPositionX;
            float savedAccumulatedWidth = accumulatedWidth;
            if (accumulatedWidth > (double)frameWidth) {
                currentLineEndIndex = lastSpaceIndex; 
                newLineNeeded = true;
            } 
            if (newLineNeeded) {
                string lineStr = textBuilder.ToString(currentLineStartIndex, currentLineEndIndex - currentLineStartIndex);
                //get the end of lineStr relative to the original text
                Outer.Log(lineStr);
                int endOfLineStr = currentLineStartIndex + lineStr.Length;
                // get the last TextWait command in the line
                TextWait? lastTextWait = null;
                if (textCommands.Count > 0) {
                    TextWait textWait = textCommands.OfType<TextWait>().LastOrDefault()!;

                    if (textWait?.Position <= endOfLineStr) {
                        lastTextWait = textWait;
                    }
                }
                if (lastTextWait != null) {
                    lastTextWait.Position -= 1;
                }
                linesList.Add(lineStr);
                accumulatedWidth = 0.0f;
                int indexOffset = 0;
                int length = textBuilder.Length;
                while (currentLineEndIndex + indexOffset < length && textBuilder[currentLineEndIndex + indexOffset] == ' ')
                    ++indexOffset;
                currentLineStartIndex = currentLineEndIndex + indexOffset;
                newLineNeeded = false;
            }
            splitDueToWidth  = savedAccumulatedWidth > (double)frameWidth;
        }
        string remainingStr = textBuilder.ToString(currentLineStartIndex, textBuilder.Length - currentLineStartIndex);
        if (remainingStr.Length > 0)
            linesList.Add(remainingStr);
        List<TextLine> textLines = new List<TextLine>();
        int commandStartPosition = 0;
        for (int index = 0; index < linesList.Count; ++index) {
            bool containsBullet = false;
            if (bulletLines.TryGetValue(index, out bool value))
                containsBullet = value;

            string lineText = linesList[index];
            textLines.Add(new TextLine(
                containsBullet,
                GetCommandRange(textCommands, commandStartPosition, commandStartPosition + lineText.Length),
                lineText));
            commandStartPosition += lineText.Length;
        }
        return new TextBlock(textLines);
    }

    private static ITextCommand[] GetCommandRange(
        List<ITextCommand> commands,
        int startPosition,
        int endPosition)
    {
        List<ITextCommand> textCommandList = new List<ITextCommand>();
        foreach (ITextCommand command in commands)
        {
            if (command.Position >= startPosition && command.Position < endPosition)
                textCommandList.Add(command);
        }
        return textCommandList.ToArray();
    }

    private static void OffsetCommandPositions(
        List<ITextCommand> commands,
        int afterPosition,
        int offset)
    {
        foreach (ITextCommand command in commands)
        {
            if (command.Position > afterPosition)
                command.Position += offset;
        }
    }
}