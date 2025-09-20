using System;
using UnityEngine;

public class Order 
{
    Vector3 pickUp;
    Vector3 issue;
    readonly int distance;
    readonly int price;
    int tips;
    readonly OrderType type;
    TimeSpan time;
    TimeSpan timeRemaining;

    const float finePercent = 0.4f;


    public Vector3 PickUp => pickUp;
    public Vector3 Issue => issue;
    public int Distance => distance;
    public int Price => price;
    public int Tips { get => Convert.ToInt32(tips * TimeRemaining / Time);  set { if (value > 0) tips = value; } }
    public OrderType Type => type;
    public TimeSpan Time => time;
    public TimeSpan TimeRemaining { get => timeRemaining; set { timeRemaining = value.TotalSeconds >= 0 ? value : TimeSpan.Zero; } }
    public int Total => price + tips - (IsLate ? Fine : 0);
    public int Fine => Mathf.RoundToInt(price * finePercent);
    public bool IsLate { get; set; } = false;

    public Order(Vector3 pickUp, Vector3 issue, float distance, float price, float tips, OrderType type)
    {
        this.pickUp = pickUp;
        this.issue = issue;
        this.distance = Mathf.RoundToInt(distance);
        this.price = Mathf.RoundToInt(price);
        this.tips = Mathf.RoundToInt(tips);
        this.type = type;
    }

    public Order(Vector3 pickUp, Vector3 issue, float distance, float price, float tips, OrderType type, TimeSpan time) : 
        this(pickUp, issue, distance, price, tips, type)
    {
        this.time = time;
        timeRemaining = time;
    }
}
