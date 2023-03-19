using System.Numerics;
using UnityEngine;
using Web3Unity.Scripts.Library.ETHEREUEM.EIP;
using UnityEngine.UI;

public class ERC20BalanceOfExample : MonoBehaviour
{
    public Text tokenBalance;
    async void Start()
    {
        string contract = "0x3E0C0447e47d49195fbE329265E330643eB42e6f";
        string account = PlayerPrefs.GetString("Account");
        string chain = "ethereum";
        string network = "goerli";
        

        BigInteger balanceOf = await ERC20.BalanceOf(chain,network,contract, account);
        Debug.Log("Balance Of: " + balanceOf);
        tokenBalance.text = "20";
    }
}
