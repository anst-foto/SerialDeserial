using System.Collections;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SerialDeserial.Library;

public class ListRandom : IEnumerable<ListNode>
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    #region SerializeDeserialize

    public void Serialize(Stream s)
    {
        if (Head is null)
        {
            throw new NullReferenceException(nameof(Head));
        }

        if (s is null)
        {
            throw new NullReferenceException(nameof(s));
        }

        if (!s.CanWrite)
        {
            throw new IOException($"Stream <{nameof(s)}> does not support writing");
        }

        try
        {
            var nodes = this
                .Select((node, i) => (node, i))
                .ToDictionary(pair => pair.node, pair => pair.i);
            using var streamWriter = new StreamWriter(s);
            foreach (var (node, i) in nodes)
            {
                streamWriter.WriteLine($"{node.Data}|{nodes[node.Random]}");
            }
        }
        catch (Exception e) when (e is StackOverflowException or OutOfMemoryException)
        {
            throw new SerializationException($"Serialization error: {nameof(StackOverflowException)} or {nameof(OutOfMemoryException)}", e);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Serialization error", e);
        }
    }

    public void Deserialize(Stream s)
    {
        if (s is null)
        {
            throw new NullReferenceException(nameof(Head));
        }

        if (!s.CanRead)
        {
            throw new IOException($"Stream <{nameof(s)}> does not support reading");
        }

        try
        {
            var listNodes = new List<ListNode>();
            var randomNodes = new Dictionary<ListNode, int>();
        
            using var streamReader = new StreamReader(s);
            var line = string.Empty;
            while (!streamReader.EndOfStream)
            {
                var data = streamReader.ReadLine().Split('|');
                var node = new ListNode
                {
                    Previous = listNodes.LastOrDefault(),
                    Data = data[0]
                };

                var random = Convert.ToInt32(data[1]);
                randomNodes.Add(node, random);

                if (listNodes.Any())
                {
                    listNodes.Last().Next = node;
                }
            
                listNodes.Add(node);
            }

            Head = listNodes.FirstOrDefault();
            Tail = listNodes.LastOrDefault();
            Count = listNodes.Count;
            
            foreach (var node in this)
            {
                var random = randomNodes[node];
                node.Random = listNodes[random];
            }
        }
        catch (Exception e) when (e is StackOverflowException or OutOfMemoryException)
        {
            throw new SerializationException($"Deserialization error: {nameof(StackOverflowException)} or {nameof(OutOfMemoryException)}", e);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Deserialization error", e);
        }
    }

    #endregion
    
    
    #region IEnumerable
    
    public IEnumerator<ListNode> GetEnumerator()
    {
        var current = Head;
        while (current is not null)
        {
            yield return current;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    #endregion
}