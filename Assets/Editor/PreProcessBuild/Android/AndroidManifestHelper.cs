#if UNITY_ANDROID
using System.Collections.Generic;
using System.Xml;

public class AndroidManifestHelper
{
    public static XmlElement SearchNode(XmlDocument doc, string path, params KeyValuePair<string, string>[] keyValues)
    {
        return SearchNode(doc.DocumentElement, path, keyValues);
    }

    public static XmlElement SearchNode(XmlElement node, string path, params KeyValuePair<string, string>[] keyValues)
    {
        if (node == null ||
            path == null ||
            string.IsNullOrEmpty(path.Trim()))
        {
            return null;
        }
        var hierarchys = path.Split('/');
        if (node.Name != hierarchys[0])
        {
            return null;
        }
        if (hierarchys.Length <= 1)
        {
            return node;
        }
        return SearchNode(node, hierarchys, 1, keyValues);
    }

    private static XmlElement SearchNode(XmlElement node, string[] hierarchys, int index, params KeyValuePair<string, string>[] keyValues)
    {
        foreach (XmlNode childNode in node.ChildNodes)
        {
            XmlElement theNode = childNode as XmlElement;
            if (theNode == null)
            {
                continue;
            }

            if (theNode.Name == hierarchys[index])
            {
                if (index == hierarchys.Length - 1)
                {
                    if (keyValues != null)
                    {
                        bool isEqual = true;
                        foreach (var keyVal in keyValues)
                        {
                            if (theNode.GetAttribute(keyVal.Key) != keyVal.Value)
                            {
                                isEqual = false;
                                break;
                            }
                        }
                        if (isEqual)
                        {
                            return theNode;
                        }
                        continue;
                    }
                    return theNode;
                }

                theNode = SearchNode(theNode, hierarchys, index + 1, keyValues);
                if (theNode != null)
                {
                    return theNode;
                }
            }
        }
        return null;
    }
}
#endif