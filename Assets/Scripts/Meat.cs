public class Meat : GrabObject
{
	public Bun.MeatType meatType = Bun.MeatType.Raw;

	public Bun.MeatType GetMeatType()
	{
		return meatType;
	}
}
