using UnityEngine;

public class Log
{
    public string Author { get; private set; }
    public string Content { get; private set; }
    public string Key { get; private set; }
    public PersonID AuthorID { get; private set; }
    public string MessageID { get; private set; }

    public Log(string author, string content, string key, PersonID authorID, string messageID)
    {
        Author = author;
        Content = content;
        Key = key;
        AuthorID = authorID;
        MessageID = messageID;
    }

}
