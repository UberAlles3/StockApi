using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath; 

namespace CommonClasses
{
   public class TextFile : IEnumerable
   {
      public enum TextFileMode
      {
         InputMode,
         OutputMode,
         OutputAppend
      }

      public string FileSpec;
      public bool Existed;
      public TextFileMode Mode;
      public long FileLength;
      public bool EOF;
      public string Data;
      public long LineNumber;
      private StreamReader oSR;
      private StreamWriter oSW;

      public long Position
      {
         get 
         { 
            if(this.Mode == TextFileMode.InputMode)
               return oSR.BaseStream.Position; 
            else
               return oSW.BaseStream.Position; 
         }
         set 
         { 
            if(this.Mode == TextFileMode.InputMode)
               oSR.BaseStream.Position = value; 
            else
               oSW.BaseStream.Position = value; 
         }
      }
	
      public bool AutoFlush
      {
         get
         { return oSW.AutoFlush; }
         set 
         { oSW.AutoFlush = value; }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public TextFile()
      {
         Data = "";
         EOF = false;
         LineNumber = 0;
      }

      public bool OpenFile(string sFileSpec, TextFileMode TFM)
      {
         this.Mode = TFM;

         try
         {
            FileSpec = sFileSpec;
            this.Existed = File.Exists(FileSpec);
            if(this.Existed)
               FileLength = (new FileInfo(FileSpec).Length);
            else
               FileLength = 0;

            if(this.Mode == TextFileMode.InputMode)
            {
               if(this.Existed == true)
               {
                  oSR = new StreamReader(FileSpec);
                  MoveNext();
               }
               else 
                  throw new FileNotFoundException("File not found. " + FileSpec);
            }
            else if(this.Mode == TextFileMode.OutputMode)
            {
               oSW = new StreamWriter(FileSpec, false);
            }
            else if(this.Mode == TextFileMode.OutputAppend)
            {
               oSW = new StreamWriter(FileSpec, true);
            }
            else
            {
               throw new Exception("Input mode not found. " +  FileSpec);
            }
            return true;
         }
         catch
         {
            throw;
         }
      }

      public void MoveNext()
      {
         if (this.Mode != TextFileMode.InputMode)
         {
            throw new Exception("TextFile.MoveNext() can only be used on files opened for input.");
         }

         if (this.EOF || oSR.EndOfStream)
         {
            this.Data = "";
            return;
         }

         this.Data = oSR.ReadLine();
         this.LineNumber += 1;

         if (oSR.EndOfStream)
            this.EOF = true;
      }

      public void WriteData(string sData)
      {
         oSW.Write(sData);
      }

      public void WriteLine(string sData)
      {
         oSW.WriteLine(sData);
      }

      public void Flush()
      {
         oSW.Flush();
      }

      public void Reset()
      {
         if (this.Mode != TextFileMode.InputMode)
         {
            throw new Exception("TextFile.Reset() can only be used on files opened for input.");
         }

         if (this.FileLength == 0)
         {
            this.EOF = false;
            this.LineNumber = 0;
         }
         else
         {
            oSR.BaseStream.Seek(0, SeekOrigin.Begin);
            oSR.DiscardBufferedData(); 
            this.EOF = false;
            this.LineNumber = 0;
            MoveNext();
         }
      }

      internal void EnumReset()
      {
         oSR.BaseStream.Seek(0, SeekOrigin.Begin);
         oSR.DiscardBufferedData();
      }

      public string ReadAll()
      {
         if (this.Mode != TextFileMode.InputMode)
         {
            throw new Exception("TextFile.ReadAll() can only be used on files opened for input.");
         }

         EnumReset();
         return oSR.ReadToEnd();
      }

      public void CloseFile()
      {
         try
         {
            if (this.Mode == TextFileMode.InputMode)
               oSR.Close();
            else
               oSW.Close();
         }
         catch 
         {/* Do Nothing */}
      }

      public TextFileEnumerator GetEnumerator()
      {
         return new TextFileEnumerator(this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   public class TextFileEnumerator : IEnumerator
   {
      private TextFile TF;

      public TextFileEnumerator(TextFile tf)
      {
         this.TF = tf;
         this.TF.EnumReset();
      }

      public bool MoveNext()
      {
         TF.MoveNext();
         return !TF.EOF;
      }

      public void Reset()
      {
         TF.EnumReset();
      }

      public string Current
      {
         get
         {
            return TF.Data;
         }
      }

      object IEnumerator.Current
      {
         get
         {
            return Current;
         }
      }
   }




   //******************************************************************************************************
   /// <summary>
   ///    XMLBase - Minipulates XML Documents
   /// </summary>
   /// <example>
   /// Below is some XML for the examples
   ///<?xml version="1.0" encoding="utf-8"?>
   ///<!-- Application Configuration File -->
   ///<configuration>
   ///  <TimeInterval>2000</TimeInterval>
   ///  <Folder FolderName="D:\Pubrpts\PDF">
   ///    <MinimumFileAge>60</MinimumFileAge>
   ///  </Folder>
   ///  <Folder FolderName="C:\File2DbWatched2">
   ///    <MinimumFileAge>40</MinimumFileAge>
   ///  </Folder>
   ///</configuration>
   /// </example>
   //******************************************************************************************************
   public class XmlBase : IEnumerable
   {
      public XmlDocument oXMLDocument;
      public XPathNavigator oXPathNavigator;
      public XPathNodeIterator oXPathNodeIterator;
      public XPathNavigator CurrentElement;
      internal string LastXPath = "";
      public string CurrentName;
      public string CurrentValue;
      private string _XMLFile;
      private bool _NeedToSave = false;
      public bool EOF = false;

      public XmlBase()
      {
      }

      public XmlBase(string XMLFile)
      {
         Open(XMLFile);
      }

      public void Open(string XMLFile)
      {
         _XMLFile = XMLFile;
         oXMLDocument = new XmlDocument();
         oXMLDocument.Load(XMLFile);
         oXPathNavigator = oXMLDocument.CreateNavigator();
         // initialize interator to the first child of the root
         XPathQuery(@"/node()"); 
         oXPathNodeIterator.MoveNext();
      }

      public void CreateNewXmlFile(string Filename, string RootNodeName, string XMLComment)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<" + RootNodeName + ">" + Environment.NewLine + "</" + RootNodeName + ">");
         XmlDeclaration xmldecl;
         xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
         XmlElement root = doc.DocumentElement;
         doc.InsertBefore(xmldecl, root);
         if (XMLComment != "")
         {
            XmlComment newComment;
            newComment = doc.CreateComment(XMLComment);
            doc.InsertBefore(newComment, root);
         }
         doc.Save(Filename);
      }

      public string GetXML()
      {
         return this.oXMLDocument.OuterXml;
      }
   
      /// <summary>
      ///    AddElement - Adds and element to the root node.
      ///    After the call or when all minipulation is done, XMLBase.Save()
      ///    needs to be called to commit the changes.
      ///    Multiple calls will add the node multiple times.
      ///    Use SaveElement if you want to update the node if it exists.
      /// </summary>
      /// <example comment="Adding a node 'TestAddNode', then an attribute on the node, then an inner element">
      ///    using System.Xml;
      ///    XmlNode NewNode;
      ///    NewNode = oXMLConfig.AddElement("TestAddNode", "");
      ///    oXMLConfig.AddAttribute(NewNode, "TestAttribute", "Hello World");
      ///    oXMLConfig.AddElement(NewNode, "TestElement", "Good-Bye World");
      ///    oXMLConfig.Save();
      /// </example>
      /// <example comment="Adding nested nodes with value">
      ///    oXMLConfig.AddElement("N1/N2/N3/N4", "SomeText");
      ///    oXMLConfig.Save();
      /// </example>
      /// <param name="ElementName"></param>
      /// <param name="ElementValue"></param>
      /// <returns>XmlNode</returns>
      public System.Xml.XmlNode AddElement(string ElementName, string ElementValue)
      {
         //if (ElementName.IndexOf("/") > 0)
         //{
         //   int i = ElementName.LastIndexOf("/");
         //   string temp = ElementName.Substring(0, i);
         //   System.Xml.XmlNode n = AddElement(temp, "");
         //   i = ElementName.LastIndexOf("/");
         //   temp = ElementName.Substring(i+1);
         //   n = AddElement(n, temp, ElementValue);
         //   return n; 
         //}
         if (ElementName.IndexOf("/") > 0)
         {
            return AddElement(GetCurrentNode(), ElementName, ElementValue);
         }
         
         XmlElement newElem = oXMLDocument.CreateElement(ElementName);
         if (ElementValue != "")
         {
            newElem.InnerText = ElementValue;
         }
         _NeedToSave = true;
         return oXMLDocument.DocumentElement.AppendChild(newElem);
      
      }
      /// <summary>
      ///    AddElement - Adds and element to a node.
      ///    After the call or when all minipulation is done, XMLBase.Save()
      ///    needs to be called to commit the changes.
      /// </summary>
      /// <example>
      ///    using System.Xml;
      ///    XmlNode NewNode;
      ///    NewNode = oXMLConfig.AddElement("TestAddNode", "");
      ///    oXMLConfig.AddElement(NewNode, "TestElement", "Good-Bye World");
      ///    oXMLConfig.Save();
      /// </example>
      /// <example>
      ///    using System.Xml;
      ///    oXMLConfig.XPathQuery("descendant::Folder");
      ///    XmlNode ExistingNode = oXMLConfig.GetCurrentNode();
      ///    oXMLConfig.AddElement(ExistingNode, "TestAddNode", "SomeText");
      ///    oXMLConfig.Save();
      /// </example>
      /// <param name="XMLNode"></param>
      /// <param name="ElementName"></param>
      /// <param name="ElementValue"></param>
      /// <returns></returns>
      public System.Xml.XmlNode AddElement(System.Xml.XmlNode XMLNode, string ElementName, string ElementValue)
      {
         if (ElementName.IndexOf("/") > 0)
         {
            int i = ElementName.IndexOf("/");
            string temp = ElementName.Substring(0, i);
            System.Xml.XmlNode n = AddElement(XMLNode, temp, "");
            temp = ElementName.Substring(i + 1);
            return AddElement(n, temp, ElementValue);
         }

         XmlElement newElem2 = oXMLDocument.CreateElement(ElementName);
         try
         {
            if (ElementValue != "")
            {
               newElem2.InnerText = ElementValue;
            }
            _NeedToSave = true;
            return XMLNode.AppendChild(newElem2);
         }
         catch
         {
            return null;
         }
      }

      public void SaveElement(string ElementName, string ElementValue)
      {
         System.Xml.XmlNodeList NodeList;
         System.Xml.XmlNode Node;
         NodeList = oXMLDocument.GetElementsByTagName(ElementName);
         if (NodeList.Count < 1)
         {
            AddElement(ElementName, ElementValue);
         }
         else
         {
            Node = NodeList.Item(0);
            Node.InnerText = ElementValue;
         }
         _NeedToSave = true;
         return;
      }

      public bool AddAttribute(System.Xml.XmlNode XMLNode, string AttributeName, string AttributeValue)
      {
         XmlAttribute newElem2 = oXMLDocument.CreateAttribute(AttributeName);
         try
         {
            newElem2.Value = AttributeValue;
            XMLNode.Attributes.Append(newElem2);
            _NeedToSave = true;
         }
         catch
         {
            return false;
         }
         return true;
      }

      public void Save()
      {
         if (_NeedToSave)
         {
            oXMLDocument.Save(_XMLFile);
         }
      }

      public bool XPathQuery(string xpathexpr)
      {
         LastXPath = xpathexpr;
         oXPathNodeIterator = oXPathNavigator.Select(xpathexpr);
         if (oXPathNodeIterator.MoveNext() == true)
         {
            EOF = false;
            CurrentName = oXPathNodeIterator.Current.Name;
            CurrentValue = oXPathNodeIterator.Current.Value;
            CurrentElement = oXPathNodeIterator.Current.Clone();
            return true;
         }
         else
         {
            EOF = true;
            CurrentName = "";
            CurrentValue = "";
            return false;
         }
      }

      public bool EnumXPathQuery(string xpathexpr)
      {
         LastXPath = xpathexpr;
         oXPathNodeIterator = oXPathNavigator.Select(xpathexpr);
         return true;
      }

      public void RemoveAllNodes(string xpath)
      {
         XmlNodeList NL;
         NL = oXMLDocument.SelectNodes(xpath);
         foreach (XmlNode N in NL)
         {
            N.ParentNode.RemoveChild(N);
            _NeedToSave = true;
         }
      }

      public XmlNode GetCurrentNode()
      {
         if (oXPathNodeIterator == null)
         {

         }

         return ((System.Xml.IHasXmlNode)oXPathNodeIterator.Current).GetNode();
      }

      public bool MoveNext()
      {
         try
         {
            if (oXPathNodeIterator.MoveNext() == true)
            {
               CurrentName = oXPathNodeIterator.Current.Name;
               CurrentValue = oXPathNodeIterator.Current.Value;
               CurrentElement = oXPathNodeIterator.Current.Clone();
               return true;
            }
            else
            {
               EOF = true;
               CurrentName = "";
               CurrentValue = "";
               return false;
            }
         }
         catch 
         {
            EOF = true;
            CurrentName = "";
            CurrentValue = "";
            return false;
         }
      }

      public string GetAttribute(string AttributeName)
      {
         XPathNavigator CPClone = CurrentElement.Clone();
         if (CPClone.MoveToFirstAttribute() == false)
         {
            return "";
         }
         do
         {
            if (CPClone.Name == AttributeName)
            {
               return CPClone.Value;
            }
         } while (!(CPClone.MoveToNextAttribute() == false));
         return "";
      }

      public string GetChildElement(string ElementName)
      {
         XPathNavigator CPClone = CurrentElement.Clone();
         if (CPClone.MoveToFirstChild() == false)
         {
            return "";
         }
         do
         {
            if (CPClone.Name == ElementName)
            {
               return CPClone.Value;
            }
         } while (!(CPClone.MoveToNext() == false));
         return "";
      }
      public XMLBaseEnumerator GetEnumerator()
      {
         return new XMLBaseEnumerator(this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
//**************************************************************************************************
   /// <summary>
   /// XmlBase enumerator for nodes 
   /// </summary>
   /// <example>
   ///     oXMLConfig.XPathQuery("descendant::Folder");
   ///     foreach (XmlNode node in (XmlBase)oXMLConfig)
   ///     {
   ///        Console.WriteLine("Iterate " + oXMLConfig.GetAttribute("FolderName"));
   ///        Console.WriteLine("Iterate " + node.Attributes["FolderName"].Value);
   ///     }
   /// </example>
   public class XMLBaseEnumerator : IEnumerator
   {
      private XmlBase XMLB;

      public XMLBaseEnumerator(XmlBase xmlb)
      {
         XMLB = xmlb;
         Reset();
      }

      public bool MoveNext()
      {
         return XMLB.MoveNext(); 
      }

      public void Reset()
      {
         XMLB.EnumXPathQuery(XMLB.LastXPath);
      }

      public XmlNode Current
      {
         get
         {
            return XMLB.GetCurrentNode();
         }
      }

      object IEnumerator.Current
      {
         get
         {
            return Current;
         }
      }
   }


   public class XMLConfig : XmlBase
   {
      public XMLConfig()
      {
      }

      public XMLConfig(string XMLFile)
      {
         this.Open(XMLFile);
      }

      /// <summary>
      /// Opens an XML file. If no filename is sent then it will
      /// open the default XML configuration file which is in the format
      /// [ApplicationName].config 
      /// (i.e. if the Application is c:\test\tester.exe
      /// then the default config file will be c:\test\tester.config
      /// If the config file is not found, it will be created for you.
      /// </summary>
      /// <example>
      /// oXMLConfig.Open(); //-- If filename not specified, will default to [ApplicationName].config 
      /// </example>
      public void Open()
      {
         this.Open("");
      }
      public new void Open(string XMLFile)
      {
         if (XMLFile == "")
         {
            string AppFolder;
            string EXEName;

            AppFolder = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            EXEName = System.IO.Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);
            XMLFile = AppFolder + "\\" + EXEName + ".config";
         }

         if (System.IO.File.Exists(XMLFile) == false)
         {
            base.CreateNewXmlFile(XMLFile, "configuration", " Application Configuration File ");
         }
         base.Open(XMLFile);
      }
      /// <summary>
      /// Gets a XML configuration setting, if not found will return the default.
      /// If AddIfNotFound is set to true, the XML file will be updated with the default value.
      /// You can get a simple node or a nested node.
      /// If the node or nested node is not found, it will be added if a default is sent
      /// and AddIfNot found is set to true. You do not have to call 'Save()' if the default is added.
      /// </summary>
      /// <example>
      /// oXMLConfig.GetXMLSetting("MaxUsers", "120", true);
      /// // or
      /// oXMLConfig.GetXMLSetting("Author/Book/Name", "(No Name)", true);
      /// </example>
      /// <param name="SettingName"></param>
      /// <param name="DefaultValue"></param>
      /// <param name="AddIfNotFound"></param>
      /// <returns></returns>
      public string GetXMLSetting(string SettingName)
      {
         return GetXMLSetting(SettingName, "", true);
      }
      public string GetXMLSetting(string SettingName, string DefaultValue)
      {
         return GetXMLSetting(SettingName, DefaultValue, true);
      }
      public string GetXMLSetting(string SettingName, string DefaultValue, bool AddIfNotFound)
      {
         
         if (base.XPathQuery("descendant::" + SettingName) == false)
         {
            if (DefaultValue != "")
            {
               if (AddIfNotFound)
               {
                  base.AddElement(SettingName, DefaultValue);
                  base.Save();
               }
               return DefaultValue;
            }
            else
            {
               return "";
            }
         }
         else
         {
            return base.CurrentValue;
         }
      }

      public void SaveXMLSetting(string SettingName, string TheValue)
      {
         base.SaveElement(SettingName, TheValue);
      }
       
      // Other examples

   
   }
}
