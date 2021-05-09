#region
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#endregion

public class Attribute
{
    public string Name;
    public string Value;
    public string Type;
}
public class Tag
{
    public Attribute[] Attributes;
    //public Dictionary<string, string> Attributes;
    public string Content;
    public string Name;
}

public class LineData
{
    public string RawText;
    public List<Tag> Tags;
    public Tag GetTag(string name) => Tags.FirstOrDefault(x => x.Name == name);
    public string Text;
}

public static class LineParser
{
    public static LineData ParseLine(string input)
    {
        var lineData = new LineData {
            RawText = input,
            Tags = new List<Tag>() 
        };
        var finalText = "";
        var rawText = input;
        var tagName = "";
        var attrsContent = "";
        var tagContent = "";
        var tagNum = 0;
        const int seek = 0;
        const int take = 1;
        const int closing = 2;
        const int attrs = 3;
        const int inside = 4; //inside the tag
        var state = seek;
        Tag tag = null;
        foreach (var c in rawText)
            switch (state)
            {
                case seek when c == '[':
                    state = take;
                    break;
                case inside when c != '[':
                    tagContent += c;
                    break;
                case seek when c != '[':
                    finalText += c;
                    break;
                case take when c == ' ':
                    state = attrs;
                    break;
                case attrs when c != ']':
                    attrsContent += c;
                    break;
                case inside when c == '[':
                    state = closing;
                    break;
                case closing when c == '/':
                    //skip
                    break;
                case closing when c != ']':
                    tagName += c;
                    break;
                //close and set and so on
                case closing when c == ']':
                    //if (tagName == tag.Name)
                    tag.Content = tagContent;
                    lineData.Tags.Add(tag);
                    tag = null;
                    tagContent = "";
                    state = seek;
                    finalText += $"$TAG_NUM_{tagNum}$";
                    tagNum++;
                    break;
                case take when c != ']' && c != '/':
                    tagName += c;
                    break;
                default:
                {
                    if ((state == take || state == attrs) && c == ']')
                    {
                        tag = new Tag {
                            Name = tagName
                        };
                        if (attrsContent != string.Empty)
                        {
                            var attrsData = attrsContent.Split(' ');
                            tag.Attributes = new Attribute[attrsData.Length];
                            for (var i =0; i< attrsData.Length; i++)
                            {
                                string[] splitResult = null;
                                foreach (var op in new[] { '=', '>', '<' })
                                    if ((splitResult = attrsData[i].Split(op)).Length > 1)
                                        tag.Attributes[i] = new Attribute() {
                                            Name = splitResult[0],
                                            Type = op.ToString(),
                                            Value = splitResult[1]
                                        };
                                tag.Attributes[i] ??= new Attribute() { Name = attrsData[i] };
                            }
                        }
                        tagName = attrsContent = "";
                        state = inside;
                    }
                    break;
                }
            }

        Dictionary<string, string> tags= lineData.Tags
            .Select((x, index) => (x, index)).ToDictionary(
            (a) => $"TAG_NUM_{a.index}",
            (a) => a.x.Content
        );
        Regex re = new Regex(@"\$(\w+)\$", RegexOptions.Compiled);
        finalText = re.Replace(finalText, match => tags[match.Groups[1].Value]);
        lineData.Text = finalText;
        return lineData;
    }
}
