using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GPDummyScreen : GPGUIScreen
{
    [Header("Screen references")]
    public GPGUIScreen m_dummySelectScreen;
    public GPDummyCustomizationScreen m_dummyCustomizingScreen;

    [Header("Dummy slots references")]
    public List<GPDummySlotCard> m_dummySlots;
    [HideInInspector]
    public GPDummySlotCard m_selectedSlot;

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

    // Start is called before the first frame update
    void Start()
    {
        m_dummyCustomizingScreen.m_customizationSlot.OnClickedEvent.AddListener(ReturnToDummySelection);
        foreach (var slot in m_dummySlots)
        {
            slot.OnClickedEvent.AddListener(OnDummyClicked);
            slot.OnToggledEvent.AddListener(OnDummyToggled);
            slot.OnNameChangedEvent.AddListener(OnDummyNameChanged);
        }
    }

    public override void Show()
    {
        base.Show();
        for (int i = 0; i < m_dummySlots.Count; i++)
        {
            var data = APIManager.Instance.PlayerData.Dummy(i).ToGPDummyData(SOManager.Instance.DummyParts);

            //m_dummySlots[i].ChangeAppearance(GPPlayerProfile.m_instance.m_dummySlots[i]);
            m_dummySlots[i].ChangeAppearance(data);
            m_dummySlots[i].SetDummyName(data.m_dummyName);
            m_dummySlots[i].m_savedData = data;
        }
        m_dummySelectScreen.Show();

        OnDummyToggled(m_dummySlots[APIManager.Instance.PlayerData.SelectedDummyIndex]);
    }

    public override void Hide()
    {
        base.Hide();
    }

    /// <summary>
    /// Called when a dummy slot is clicked.
    /// Takes the user to the customization menu for that dummy.
    /// </summary>
    /// <param name="card"></param>
    public void OnDummyClicked(GPDummySlotCard card)
    {
        m_selectedSlot = card;

        m_dummyCustomizingScreen.m_customizationSlot.ReplaceModelObject(m_selectedSlot.m_dummyModelRef);
        //Show customization screen for that dummy.
        m_dummySelectScreen.Hide();
        m_dummyCustomizingScreen.Show();
    }

    /// <summary>
    /// Called when the user clicks the toggle button of the dummy slot.
    /// Marks this dummy as the one the player is going to use in battle.
    /// </summary>
    /// <param name="card"></param>
    public void OnDummyToggled(GPDummySlotCard card)
    {
        m_selectedSlot = card;

        //find idx adn turn off/on the other toggles
        int selectedIdx = 0;
        for (int i = 0; i < m_dummySlots.Count; i++)
        {
            m_dummySlots[i].ToggleSelected(m_dummySlots[i] == m_selectedSlot);
            if (m_dummySlots[i] == m_selectedSlot)
            {
                selectedIdx = i;
            }
        }

        ChooseDummy(selectedIdx);
    }

    void OnDummyNameChanged(GPDummySlotCard card)
    {
        SaveDummyChanges(GetIdxOFSlot(card));
    }

    /// <summary>
    /// Returns to dummy slot selection screen.
    /// </summary>
    /// <param name="card"></param>
    public void ReturnToDummySelection(GPDummySlotCard card)
    {
        m_dummySelectScreen.Show();
        m_dummyCustomizingScreen.Hide();

        m_selectedSlot.ReplaceModelObject(m_dummyCustomizingScreen.m_customizationSlot.m_dummyModelRef);
        SaveDummyChanges(GetIdxOFSlot(m_selectedSlot));
    }

    public int GetIdxOFSlot(GPDummySlotCard slot)
    {
        return m_dummySlots.IndexOf(slot);
    }

    /// <summary>
    /// Save the dummy changes made in customization mode and 
    /// applies it to the dummy displayed on the slot
    /// </summary>
    public async void SaveDummyChanges(int slotIdx)
    {

        //GPPlayerProfile.m_instance.m_dummySlots[slotIdx] = m_selectedSlot.m_savedData;

        m_LoadIndicator.SetActive(true);

        await APIManager.Instance.PlayerData.SetDummy(m_dummySlots[slotIdx].m_savedData.ToDummyData(), slotIdx).Put();

        m_LoadIndicator.SetActive(false);

        //PhotonNetwork.LocalPlayer.WriteDummyKeys(GPPlayerProfile.m_instance.m_dummySlots[GPPlayerProfile.m_instance.m_currDummySlotIdx]);

    }

    async void ChooseDummy(int selectedIdx)
    {
        //GPPlayerProfile.m_instance.m_currDummySlotIdx = selectedIdx;
        //PhotonNetwork.LocalPlayer.WriteDummyKeys(GPPlayerProfile.m_instance.m_dummySlots[GPPlayerProfile.m_instance.m_currDummySlotIdx]);
        m_LoadIndicator.SetActive(true);

        await APIManager.Instance.PlayerData.SetSelectedDummyIndex(selectedIdx).Put();

        m_LoadIndicator.SetActive(false);
    }
}
