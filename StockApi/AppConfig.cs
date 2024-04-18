using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;

namespace StockApi
{
    class AppConfig
    {
    }

    public class SearchTokensSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<SearchTerm> searchTerms = new List<SearchTerm>();

            foreach (XmlNode childNode in section.ChildNodes) // <token> node
            {
                SearchTerm searchTerm = new SearchTerm();
                foreach (XmlAttribute attrib in childNode.Attributes)
                {
                    if (attrib.Name == "name")
                        searchTerm.Name = attrib.Value;

                    if (attrib.Name == "term")
                        searchTerm.Term = attrib.Value;
                }
                searchTerms.Add(searchTerm);
            }
            return searchTerms;
        }
    }

    public class SearchTerm
    {
        public string Name { get; set; }
        public string Term { get; set; }
    }
}
