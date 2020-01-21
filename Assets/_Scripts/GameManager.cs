using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	static int _money = 0;
	static bool moneyLoaded = false;

	public static int money {
		get {
			if (!moneyLoaded) {
				if (PlayerPrefs.HasKey ("money"))
					_money = PlayerPrefs.GetInt ("money");
				else {
					_money = 0;
				}

				moneyLoaded = true;
			}
			
			return _money;
		}

		private set {

			if (!moneyLoaded) {
				if (PlayerPrefs.HasKey ("money"))
					_money = PlayerPrefs.GetInt ("money");
				else {
					_money = 0;
				}
				moneyLoaded = true;
			}

			_money = value;
			PlayerPrefs.SetInt ("money", _money);
			PlayerPrefs.Save ();
		}

	}

	static int _ruby = 0;
	static bool rubyLoaded = false;

	public static int ruby {
		get {
			if (!rubyLoaded) {
				if (PlayerPrefs.HasKey ("ruby"))
					_ruby = PlayerPrefs.GetInt ("ruby");
				else
					_ruby = 0;
				rubyLoaded = true;
			}

			return _ruby;
		}

		private set {
			
			if (!rubyLoaded) {
				if (PlayerPrefs.HasKey ("ruby"))
					_ruby = PlayerPrefs.GetInt ("ruby");
			}

			_ruby = value;
			PlayerPrefs.SetInt ("ruby", _ruby);
			PlayerPrefs.Save ();
		}

	}

	public static int CurLevel {
		get {

			float xpforcurlevel = TotalXP;

			int level = 1;

			while (xpforcurlevel > NeedXPforNextLevel (level)) {

				xpforcurlevel -= NeedXPforNextLevel (level);
				level++;

			}

			return level;
		}
	}

	public static int CurXPForCurLevel {
		get {
			
			int xpforcurlevel = TotalXP;

			int level = 1;

			while (xpforcurlevel > NeedXPforNextLevel (level)) {

				xpforcurlevel -= NeedXPforNextLevel (level);
				level++;

			}

			return xpforcurlevel;
		}
	}

	static int _totalXP;

	static bool totalXPLoaded = false;

	public static int TotalXP {
		get {
			if (!totalXPLoaded) {
				if (PlayerPrefs.HasKey ("xp"))
					_totalXP = PlayerPrefs.GetInt ("xp");
				else
					_totalXP = 0;
				

				totalXPLoaded = true;

			}

			return _totalXP;

		}

		private	set {
			
			if (!totalXPLoaded) {
				if (PlayerPrefs.HasKey ("xp"))
					_totalXP = PlayerPrefs.GetInt ("xp");

				totalXPLoaded = true;
			}


			_totalXP = value;


			PlayerPrefs.SetInt ("xp", _totalXP);
			PlayerPrefs.Save ();

		}
	}

	public static GameManager Instance;

    public bool isNewGame;

	void Awake ()
	{
		Instance = this;
        if (!PlayerPrefs.HasKey("money"))
        {
            isNewGame = true;
            Database.PlayerData.levelsData.Reset();
        }

        if (Debug.isDebugBuild)
        {
            ruby = 1000;
            money = 1000000;
        }

        PurchaseManager.Ins.TryInit();
    }

	void OnApplicationPause ()
	{


/*		print ("MoneySaved");
		PlayerPrefs.SetInt ("money", money);
		PlayerPrefs.Save ();*/
	}

	public static bool Buy (int moneyCount)
	{

		int finalMoney = money - moneyCount;
    
		if (finalMoney >= 0) {
			
			EventManager.OnMoneyChangedCall (money, finalMoney);
			money = finalMoney;

			return true;
		} else {

			DialogBox.Show ("Insufficent money", "Buy money?", () => {
                KScreenManager.Instance.ShowScreen("ValutaShop");
			}, () => {
			});

			return false;
		}
	}

	public static bool BuyWithRuby (int rubyCount)
	{
		int finalRuby = ruby - rubyCount;

		print (finalRuby + "    " + rubyCount);

		if (finalRuby >= 0) {

			EventManager.OnRubyChangedCall (ruby, finalRuby);
			ruby = finalRuby;

			return true;
		} else {

			DialogBox.Show ("Insufficent ruby", "Buy ruby?", () => {
                KScreenManager.Instance.ShowScreen("ValutaShop");
            }, () => {
			});

			return false;
		}
	}

	public static void AddMoney (int addMoney)
	{
		int startMoney = money;

		money += addMoney;

		EventManager.OnMoneyChangedCall (startMoney, money);
	}

    public static void AddRuby(int addRuby)
    {
        int startRuby = ruby;

        ruby += addRuby;

        EventManager.OnRubyChangedCall(startRuby, ruby);
    }

    public static int NeedXPforNextLevel (int level)
	{
		return Mathf.Clamp (level, 0, int.MaxValue) * 30;
	}

	public static int NeedTotalXPforNextLevel (int level)
	{
		int levelTmp = 1;
		int total = 0;

		while (levelTmp <= level) {
			
			total += NeedXPforNextLevel (levelTmp);
			levelTmp++;

		}

		return total;

	}

	static void UpgradeLevel ()
	{
		print ("Upgraded " + CurLevel + "   " + CurXPForCurLevel);

	}

	public static void AddXP (int XpAmount)
	{
		if (CurXPForCurLevel + XpAmount > NeedXPforNextLevel (CurLevel)) {
			UpgradeLevel ();
		}

		CarStatController.updateXPBar = true;

		TotalXP += XpAmount;
	}

}
