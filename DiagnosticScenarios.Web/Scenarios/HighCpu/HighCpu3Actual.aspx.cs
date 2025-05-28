using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu3Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Generate a large text with nested patterns
            string text = GenerateComplexText();
            
            // Run multiple regex operations in parallel
            Parallel.For(0, 10, i =>
            {
                RunComplexRegexOperations(text);
            });
        }

        private string GenerateComplexText()
        {
            // Generate a text with nested patterns that will cause regex backtracking
            var text = new System.Text.StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                text.Append("((((a+)+)+)+)+");
                text.Append("((((b+)+)+)+)+");
                text.Append("((((c+)+)+)+)+");
            }
            return text.ToString();
        }

        private void RunComplexRegexOperations(string text)
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
                @"(a|b|c|d|e|f){6,}"
            };

            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern, RegexOptions.Compiled);
                regex.Matches(text);
            }
        }
    }
} 