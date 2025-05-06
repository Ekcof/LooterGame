using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
	public string Condition { private set; get; }
	public bool IsEqual { private set; get; }

	/// <summary>
	/// Checks property and if it's true shows property marked this attribute
	/// </summary>
	/// <param name="condition"></param>
	public ShowIfAttribute(string condition, bool equal = true)
	{
		Condition = condition;
		IsEqual = equal;
	}
}
