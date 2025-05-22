using System;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a complex object structure
            var parent = new ParentObject();
            parent.Child = new ChildObject();
            parent.Child.GrandChild = new GrandChildObject();

            // Intentionally set a reference to null
            parent.Child.GrandChild = null;

            // This will throw a NullReferenceException
            string value = parent.Child.GrandChild.Value;
        }
    }

    public class ParentObject
    {
        public ChildObject Child { get; set; }
    }

    public class ChildObject
    {
        public GrandChildObject GrandChild { get; set; }
    }

    public class GrandChildObject
    {
        public string Value { get; set; } = "Some value";
    }
} 