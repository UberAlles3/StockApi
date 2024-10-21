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

    public class SettingsSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<Setting> settings = new List<Setting>();

            foreach (XmlNode childNode in section.ChildNodes) // <token> node
            {
                Setting setting = new Setting();
                foreach (XmlAttribute attrib in childNode.Attributes)
                {
                    if (attrib.Name == "name")
                        setting.Name = attrib.Value;

                    if (attrib.Name == "value")
                        setting.Value = attrib.Value;
                }
                settings.Add(setting);
            }
            return settings;
        }
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

    public class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class SearchTerm
    {
        public string Name { get; set; }
        public string Term { get; set; }
    }

}
