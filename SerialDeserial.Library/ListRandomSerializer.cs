using System.Runtime.Serialization;

namespace SerialDeserial.Library;

public static class ListRandomSerializer
{
    public static void Serialize(Stream stream, ListRandom list)
    {
        if (list is null)
        {
            throw new NullReferenceException(nameof(list));
        }
        
        if (list.Head is null)
        {
            throw new NullReferenceException(nameof(list.Head));
        }

        if (stream is null)
        {
            throw new NullReferenceException(nameof(stream));
        }

        if (!stream.CanWrite)
        {
            throw new IOException($"Stream <{nameof(stream)}> does not support writing");
        }

        try
        {
            var nodes = list
                .Select((node, i) => (node, i))
                .ToDictionary(pair => pair.node, pair => pair.i);
            using var streamWriter = new StreamWriter(stream);
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
    
    public static ListRandom Deserialize(Stream stream)
    {
        if (stream is null)
        {
            throw new NullReferenceException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new IOException($"Stream <{nameof(stream)}> does not support reading");
        }

        try
        {
            var listNodes = new List<ListNode>();
            var randomNodes = new Dictionary<ListNode, int>();
        
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                var data = streamReader.ReadLine()?.Split('|');

                if (data is null)
                {
                    throw new NullReferenceException(nameof(data));
                }
                
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

            var list = new ListRandom()
            {
                Head = listNodes.FirstOrDefault(),
                Tail = listNodes.LastOrDefault(),
                Count = listNodes.Count
            };
            
            foreach (var node in list)
            {
                var random = randomNodes[node];
                node.Random = listNodes[random];
            }

            return list;
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
}