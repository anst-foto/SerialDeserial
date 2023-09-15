using System.Collections;
using System.Runtime.Serialization;

namespace SerialDeserial.Library;

public class ListRandom : IEnumerable<ListNode>
{
    public ListNode? Head { get; set; }
    public ListNode? Tail { get; set; }
    public int? Count { get; set; }

    #region SerializeDeserialize

    

    

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