using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu3Actual : System.Web.UI.Page
    {
        private static readonly List<Regex> _compiledRegexes = new List<Regex>();
        private static readonly object _lock = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Generate a large text with nested patterns
            string text = GenerateComplexText();
            
            // Compile regexes once and reuse them
            if (_compiledRegexes.Count == 0)
            {
                lock (_lock)
                {
                    if (_compiledRegexes.Count == 0)
                    {
                        InitializeRegexes();
                    }
                }
            }

            // Run multiple regex operations in parallel with more iterations
            Parallel.For(0, Environment.ProcessorCount * 2, i =>
            {
                for (int j = 0; j < 5; j++) // Run each pattern multiple times
                {
                    RunComplexRegexOperations(text);
                }
            });
        }

        private void InitializeRegexes()
        {
            // Complex regex patterns that will cause significant backtracking
            string[] patterns = new[]
            {
                @"((a+)+)+",
                @"((b+)+)+",
                @"((c+)+)+",
                @"(a|b|c)*",
                @"(a+)(b+)(c+)",
                @"(a|b|c){10,}",
                @"(a+)(b+)(c+)(d+)(e+)",
                @"(a|b|c|d|e){5,}",
                @"(a+)(b+)(c+)(d+)(e+)(f+)",
                @"(a|b|c|d|e|f){6,}",
                // Add more complex patterns
                @"((a+)+)+((b+)+)+((c+)+)+",
                @"(a|b|c|d|e|f|g|h|i|j){10,}",
                @"(a+)(b+)(c+)(d+)(e+)(f+)(g+)(h+)(i+)(j+)",
                @"((a+)+)+((b+)+)+((c+)+)+((d+)+)+((e+)+)+",
                @"(a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p){16,}"
            };

            foreach (var pattern in patterns)
            {
                _compiledRegexes.Add(new Regex(pattern, RegexOptions.Compiled));
            }
        }

        private string GenerateComplexText()
        {
            // Generate a text with nested patterns that will cause regex backtracking
            var text = new System.Text.StringBuilder();
            
            // Generate more complex patterns
            for (int i = 0; i < 2000; i++) // Increased from 1000 to 2000
            {
                text.Append("((((a+)+)+)+)+");
                text.Append("((((b+)+)+)+)+");
                text.Append("((((c+)+)+)+)+");
                text.Append("((((d+)+)+)+)+");
                text.Append("((((e+)+)+)+)+");
                text.Append("((((f+)+)+)+)+");
                text.Append("((((g+)+)+)+)+");
                text.Append("((((h+)+)+)+)+");
                text.Append("((((i+)+)+)+)+");
                text.Append("((((j+)+)+)+)+");
            }
            return text.ToString();
        }

        private void RunComplexRegexOperations(string text)
        {
            foreach (var regex in _compiledRegexes)
            {
                // Run multiple matches to increase CPU usage
                for (int i = 0; i < 3; i++)
                {
                    regex.Matches(text);
                }
            }
        }
    }
} 