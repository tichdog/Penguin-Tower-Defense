**Скрипты**

**UI**
     
    LanguageSwitcher (скрипт для управления сменой языков используя dropdown)
    MainMenu (Отвечает за UI, по сути менеджер кнопок для меню и карты, работает через события)

**Камера (camera)**

    MapCameraController (Отвечает за перемещение камеры по карте, опираясь на ее размеры и платформу)

**Сохранения** 

    Что бы что то сохранить, создаем ключ в файле SaveKeys
    public const string PlayerDiamonds = "player.diamonds";
    Где то в скрипте экономики, создаем 
    [Serializable]
     public class DiamondsSaveData
     {
         public int amount;
     }
     И в сохранении\загрузке 
     private void SaveDiamonds()
    {
        GameDataManager.Instance.SetData(SaveKeys.PlayerDiamonds, new DiamondsSaveData
        {
            amount = Diamonds
        });

        GameDataManager.Instance.Save();
    }

    private void LoadDiamonds()
    {
        if (GameDataManager.Instance.TryGetData(SaveKeys.PlayerDiamonds, out DiamondsSaveData data))
            Diamonds = data.amount;
        else
            Diamonds = 0;
    }
     

**Звуки**

     Сделаны через один Mixer, в нем 2 группы Music, SFX. В настройках есть 2 бара для настройки уровня громкости (регулируют звук у своей кнопки). 
     AudioManager - основной скрипт для управления всем
