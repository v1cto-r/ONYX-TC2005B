using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int startingCoins=0;
    public int maxCoins=9999;
    public int coins;

    void Awake()
    {
        if (Instance==null)
        {
            Instance=this;
        }
        else
        {
            Destroy(gameObject);
        }
        coins=startingCoins;
    }

    public void AddCoins(int amount)
    {
        coins+=amount;
        if (coins>=maxCoins)
        {
            coins=maxCoins;
        }
        //AudioManager.instance.PlayCoin(); Para cuando agregue sonidos
    }

    public int GetCoins()
    {
        return coins;
    }

    public void ResetCoins()
    {
        coins=startingCoins;
    }
}
