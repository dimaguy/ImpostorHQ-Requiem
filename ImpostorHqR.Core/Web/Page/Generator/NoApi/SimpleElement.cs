namespace ImpostorHqR.Core.Web.Page.Generator.NoApi
{
    public class SimpleElement
    {
        public string Value { get; set; }

        public string Template { get; set; }

        public string Replacer { get; set; }

        public string Code => Template.Replace(Replacer, Value);

        public SimpleElement(string value, string replacer, string template)
        {
            this.Value = value;
            this.Replacer = replacer;
            this.Template = template;
        }
    }
}
