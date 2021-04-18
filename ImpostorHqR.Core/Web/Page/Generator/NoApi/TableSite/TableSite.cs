using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;

namespace ImpostorHqR.Core.Web.Page.Generator.NoApi.TableSite
{
    public class TableSite : IStaticPage
    {
        public string StartHtmlOriginal { get; private set; }

        public string StartHtml { get; private set; }

        public string EndHtml { get; }

        public Color Color { get; private set; }

        public string Title { get; private set; }

        public bool Update { get; private set; }

        private string Content { get; set; }

        private readonly List<SimpleElement> _elements = new List<SimpleElement>();

        private readonly object _sync = new object();

        public IEnumerable<string> Entries
        {
            get
            {
                lock (this._sync) return _elements.Select(c => c.Value);
            }
        }

        public TableSite(Color color, string title)
        {
            this.StartHtmlOriginal = TableSiteConstant.StartHtml;
            this.SetParameters(color, title);
            this.EndHtml = TableSiteConstant.EndHtml;
            this.Update = true;
        }

        public void AddEntry(string text)
        {
            lock (_sync)
            {
                this._elements.Add(new SimpleElement(text, TableSiteConstant.ReplaceInElement, TableSiteConstant.Element));
                this.Update = true;
            }
        }

        public void RemoveEntry(string text)
        {
            lock (this._sync)
            {
                var target = this._elements.FirstOrDefault(i => i.Value.Equals(text));
                if (target == null) return;
                this._elements.Remove(target);
                this.Update = true;
            }
        }

        public string GetLatest()
        {
            lock (this._sync)
            {
                if (!this.Update) return this.Content;
                using var sb = IReusableStringBuilder.Get();
                sb.Append(this.StartHtml);

                foreach (var simpleElement in _elements)
                {
                    sb.Append(simpleElement.Code);
                }

                sb.Append(this.EndHtml);
                this.Content = sb.ToString();
                return this.Content;
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                this.Update = true;
                this._elements.Clear();
            }
        }

        public void SetParameters(Color color, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }


            lock (_sync)
            {
                this.Update = true;
                this.StartHtml = this.StartHtmlOriginal
                    .Replace(TableSiteConstant.ReplaceInColor, $"rgb({color.R}, {color.G}, {color.B})".ToLower())
                    .Replace(TableSiteConstant.ReplaceInHeader, " | " + title + " |"); ;
                this.Color = color;
                this.Title = title;
            }

        }
    }
}
