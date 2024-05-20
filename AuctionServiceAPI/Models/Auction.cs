using System;
using System.Collections.Generic;

public class Auction
{
    public int _id { get; set; }
    public Item Item { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Bid> Bids { get; set; }
}

public class Item
{
 
}

public class Bid
{
 
}
