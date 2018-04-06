using System;

/**
	partial condition holder.
*/
public partial class CollectedConditions
{
#pragma warning disable 414
    private static CollectedConditions c = new CollectedConditions();
#pragma warning restore 414

    public static Type[] conditions;
}