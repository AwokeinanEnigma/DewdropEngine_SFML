using DewDrop.Inspector.Attributes;
using DewDrop.Internal;
namespace Prototype; 

public class IStealMoneyFromPeopleOfSouthAmericanOrigin : Component {
	public int MoneyStolen { get; set; }
	[ButtonMethod("StealMoreMoney")]
	public void StealMoreMoney() {
		MoneyStolen += 100;
	}
}
