using DewDrop.Inspector.Attributes;
using DewDrop.Internal;
using DewDrop.Utilities;
namespace Prototype; 

public class IStealMoneyFromPeopleOfSouthAmericanOrigin : Component {
	public int MoneyStolen { get; set; }
	
	public void Update () {
		Outer.Log("I stole $" + MoneyStolen + " from people of South American origin.");
	}
	[ButtonMethod("StealMoreMoney")]
	public void StealMoreMoney() {
		MoneyStolen += 100;
	}
}
