using SerialDeserial.Library;

var first = new ListNode();
first.Previous = null;
first.Data = "first";

var second = new ListNode();
second.Previous = first;
second.Data = "second";

first.Next = second;

var third = new ListNode();
third.Previous = second;
third.Data = "third";
third.Next = null;

second.Next = third;

first.Random = third;
second.Random = third;
third.Random = third;

var list = new ListRandom();
list.Head = first;
list.Tail = third;
list.Count = 3;

using (var stream = new FileStream("stram.dat", FileMode.Create, FileAccess.Write))
{
    ListRandomSerializer.Serialize(stream, list);
}

using (var stream = new FileStream("stram.dat", FileMode.Open, FileAccess.Read))
{
    var list2 = ListRandomSerializer.Deserialize(stream);
    
    using (var s = new FileStream("stram_2.dat", FileMode.Create, FileAccess.Write))
    {
        ListRandomSerializer.Serialize(s, list2);
    }
}
