using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity;
using AlinaLib.Domain.Entity.Base;
using System.Xml;

namespace AlinaLib.Data.Implementation
{
    internal class XmlFileReader : IFileReader
    {
        private static readonly string __card = "Card";
        private static readonly string __userId = "UserId";
        private static readonly string __pan = "Pan";
        private static readonly string __expDate = "ExpDate";

        private readonly string _fullPath = string.Empty;

        #region .ctors
        public XmlFileReader(string fullPath)
        {
            _fullPath = fullPath;     
        }
        #endregion

        public IEnumerable<BaseEntity> ReadAll()
        {
            var result = new List<Card>();
            if (string.IsNullOrWhiteSpace(_fullPath)) return result;
            XmlDocument doc = new();
            using XmlTextReader xmlReader = new(_fullPath);
            while (xmlReader.Read())
            {
                if (NeedSkipNode(xmlReader)) continue;
                var node = selectCardNode(doc, xmlReader);
                var card = ParseCard(node);
                if(card.UserId.Length > 0)
                    result.Add(card);
            }
            return result;
        }

        private bool NeedSkipNode(XmlTextReader xmlReader) =>
            xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != __card;

        private XmlNode selectCardNode(XmlDocument doc, XmlTextReader xmlReader)
        {
            doc.LoadXml(xmlReader.ReadOuterXml());
            return doc.SelectSingleNode((__card))!;
        }

        private Card ParseCard(XmlNode node)
        {
            var emptyCard = new Card(string.Empty, string.Empty, string.Empty);
            string userId = string.Empty;
            if (node.Attributes!.Count > 0)
            {
                userId = node.Attributes[__userId]!.Value;
            }
            if (userId.Length < 1) return emptyCard;
            var (pan, expDate) = ParsePanAndExpDate(node.ChildNodes);
            return new Card(userId, pan, expDate);
        }

        private (string pan, string expDate) ParsePanAndExpDate(XmlNodeList nodes)
        {
            (string, string) result = (string.Empty, string.Empty);
            foreach (XmlNode node in nodes)
            {
                if (node.Name == __pan)
                {
                    result.Item1 = node.InnerText;
                }
                if (node.Name == __expDate)
                {
                    result.Item2 = node.InnerText;
                }
            }
            return result;
        }
    }
}
