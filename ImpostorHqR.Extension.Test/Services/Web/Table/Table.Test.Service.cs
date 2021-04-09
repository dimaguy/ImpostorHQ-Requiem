using System;
using System.Drawing;
using System.Text;
using System.Threading;
using ImpostorHq.Extension.Api.Interface.Web;
using ImpostorHq.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHq.Extension.Test.Services.Web.Table
{
    internal class TestWebService : ISimpleWebService
    {
        private ISimplePage TestPage { get; set; }

        private readonly string[] _poem = Encoding.UTF8.GetString(Convert.FromBase64String("RG8gbm90IGdvIGdlbnRsZSBpbnRvIHRoYXQgZ29vZCBuaWdodCwKT2xkIGFnZSBzaG91bGQgYnVybiBhbmQgcmF2ZSBhdCBjbG9zZSBvZiBkYXk7ClJhZ2UsIHJhZ2UgYWdhaW5zdCB0aGUgZHlpbmcgb2YgdGhlIGxpZ2h0LgoKVGhvdWdoIHdpc2UgbWVuIGF0IHRoZWlyIGVuZCBrbm93IGRhcmsgaXMgcmlnaHQsCkJlY2F1c2UgdGhlaXIgd29yZHMgaGFkIGZvcmtlZCBubyBsaWdodG5pbmcgdGhleQpEbyBub3QgZ28gZ2VudGxlIGludG8gdGhhdCBnb29kIG5pZ2h0LgoKR29vZCBtZW4sIHRoZSBsYXN0IHdhdmUgYnksIGNyeWluZyBob3cgYnJpZ2h0ClRoZWlyIGZyYWlsIGRlZWRzIG1pZ2h0IGhhdmUgZGFuY2VkIGluIGEgZ3JlZW4gYmF5LApSYWdlLCByYWdlIGFnYWluc3QgdGhlIGR5aW5nIG9mIHRoZSBsaWdodC4KCldpbGQgbWVuIHdobyBjYXVnaHQgYW5kIHNhbmcgdGhlIHN1biBpbiBmbGlnaHQsCkFuZCBsZWFybiwgdG9vIGxhdGUsIHRoZXkgZ3JpZXZlZCBpdCBvbiBpdHMgd2F5LApEbyBub3QgZ28gZ2VudGxlIGludG8gdGhhdCBnb29kIG5pZ2h0LgoKR3JhdmUgbWVuLCBuZWFyIGRlYXRoLCB3aG8gc2VlIHdpdGggYmxpbmRpbmcgc2lnaHQKQmxpbmQgZXllcyBjb3VsZCBibGF6ZSBsaWtlIG1ldGVvcnMgYW5kIGJlIGdheSwKUmFnZSwgcmFnZSBhZ2FpbnN0IHRoZSBkeWluZyBvZiB0aGUgbGlnaHQuCgpBbmQgeW91LCBteSBmYXRoZXIsIHRoZXJlIG9uIHRoZSBzYWQgaGVpZ2h0LApDdXJzZSwgYmxlc3MsIG1lIG5vdyB3aXRoIHlvdXIgZmllcmNlIHRlYXJzLCBJIHByYXkuCkRvIG5vdCBnbyBnZW50bGUgaW50byB0aGF0IGdvb2QgbmlnaHQuClJhZ2UsIHJhZ2UgYWdhaW5zdCB0aGUgZHlpbmcgb2YgdGhlIGxpZ2h0Lg==")).Split('\n');

        private int PoemIndex { get; set; }

        public ushort UpdateInterval => 1000;

        public void Update()
        {
            lock (TestPage)
            {
                TestPage.Clear();
                TestPage.AddEntry("<h1> Video content delivery test: </h1><br>");
                TestPage.AddEntry("<br><iframe src=\"who.mp4\" width=\"100%\" height=\"300\" style=\"border:1px solid black;\">\r\n<br></iframe>");

                TestPage.AddEntry($"<br><br>It is {DateTime.Now}.");
                TestPage.AddEntry(
                    $"Thread pool threads: {ThreadPool.ThreadCount}, pending: {ThreadPool.PendingWorkItemCount}, completed: {ThreadPool.CompletedWorkItemCount}");
                if (PoemIndex < _poem.Length)
                {
                    TestPage.AddEntry(_poem[PoemIndex++]);
                }
                else PoemIndex = 0;
            }
        }

        public (string, ISimplePage) Register()
        {
            return ("testPage", TestPage = Proxy.Instance.PreInitialization.PageProvider.SimplePageProvider.ProduceTablePage(Color.Indigo, "Test Page"));
        }

    }
}
