using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ValutaShopScreen : KScreen {
    public ValutaBtn[] btns;

    public override void OnCmpEnable()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].text.text = PurchaseManager.Ins.GetLocalizedPrice(btns[i].purchaseId);
        }
    }

    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].button.onClick.RemoveAllListeners();
            int index = i;
            btns[i].button.onClick.AddListener(() =>
            {
                PurchaseManager.Ins.BuyConsumable(btns[index].purchaseId);
            });
        }
        PurchaseManager.OnPurchaseConsumable += OnPurchaseConsumable;

    }

    private void OnPurchaseConsumable(PurchaseEventArgs args)
    {
        string purchazedID = args.purchasedProduct.definition.id;

        int index = -1;

        for (int i = 0; i < btns.Length; i++)
        {
            if(btns[i].purchaseId == purchazedID)
            {
                index = i;
                break;
            }
        }

        if (!btns[index].isRuby)
            GameManager.AddMoney(btns[index].valutaCount);
        else
            GameManager.AddRuby(btns[index].valutaCount);


        DialogBox.Show("Congratulation!", "You bought " + btns[index].valutaCount + ((btns[index].isRuby) ? " ruby" : " dollars"), ()=>{ }, default, false);

        KScreenManager.Instance.ShowScreen("Shop");

    }

    [System.Serializable]
    public class ValutaBtn
    {
        public string purchaseId;
        public int valutaCount;
        public bool isRuby;
        public Button button;
        public Text text;
    }
}
