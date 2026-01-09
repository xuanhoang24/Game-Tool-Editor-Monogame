using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Editor.GUI
{
    /// <summary>
    /// Simple Lua syntax highlighter for RichTextBox
    /// </summary>
    public static class LUAHighlighter
    {
        // Simple color scheme
        private static readonly Color KeywordColor = Color.Blue;
        private static readonly Color CommentColor = Color.Green;
        private static readonly Color StringColor = Color.Red;
        private static readonly Color DefaultColor = Color.Black;

        // LUA keywords
        private static readonly string[] Keywords = {
            "function", "end", "if", "then", "else", "elseif", "while", "do", 
            "for", "in", "local", "return", "break", "true", "false", "nul"
        };

        // Apply LUA syntax highlighting to a RichTextBox
        public static void ApplyHighlighting(RichTextBox textBox)
        {
            if (textBox == null || string.IsNullOrEmpty(textBox.Text)) return;

            // Store current selection
            int selStart = textBox.SelectionStart;
            int selLength = textBox.SelectionLength;

            // Reset all text to default color
            textBox.SelectAll();
            textBox.SelectionColor = DefaultColor;

            // Highlight comments (-- comment)
            HighlightPattern(textBox, @"--.*$", CommentColor, RegexOptions.Multiline);

            // Highlight strings ("string" and 'string')
            HighlightPattern(textBox, @"""[^""]*""", StringColor);
            HighlightPattern(textBox, @"'[^']*'", StringColor);

            // Highlight keywords
            foreach (string keyword in Keywords)
            {
                HighlightPattern(textBox, @"\b" + keyword + @"\b", KeywordColor);
            }

            // Restore selection
            textBox.SelectionStart = selStart;
            textBox.SelectionLength = selLength;
        }

        private static void HighlightPattern(RichTextBox textBox, string pattern, Color color, RegexOptions options = RegexOptions.None)
        {
            Regex regex = new Regex(pattern, options);
            foreach (Match match in regex.Matches(textBox.Text))
            {
                textBox.Select(match.Index, match.Length);
                textBox.SelectionColor = color;
            }
        }

        // Configure a RichTextBox for Lua editing
        public static void ConfigureForLua(RichTextBox textBox)
        {
            textBox.Font = new Font("Consolas", 10);
            textBox.WordWrap = false;
            textBox.AcceptsTab = true;
        }
    }
}