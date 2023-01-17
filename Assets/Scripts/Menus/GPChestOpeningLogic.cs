using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPChestOpeningLogic : MonoBehaviour
{
    [Header("Audio settings")]
    public AudioClip m_chestOpenSFX;

    [Header("Chest Rewards Settings")]
    public GPChestRewardWindow m_rewardWindow;
    public GPGUIScreen m_rewardScreen;

    /// <summary>
    /// Displays the chest opening screen and gives the content to the player.
    /// </summary>
    /// <param name="chestDesc"></param>
    public void OpenChest(GPStoreChestSO chestDesc)
    {
        GPGivenRewards rewards = chestDesc.OpenChest();
        m_rewardScreen.Show();
        m_rewardWindow.Show();
        m_rewardWindow.ClearContent();
        m_rewardWindow.DisplayChestImage(chestDesc);
        m_rewardWindow.DisplayCrewRewards(rewards.m_ships);
        m_rewardWindow.DisplayIconRewards(rewards.m_profileIcons);
        m_rewardWindow.DisplayDummyRewards(rewards.m_dummyParts);
        StartCoroutine(CloseRewardWindow()); // for now close reward window after 3 seconds
    }

    IEnumerator CloseRewardWindow()
    {
        yield return new WaitForSeconds(3.0f);
        m_rewardScreen.Hide();
        m_rewardWindow.Hide();
    }

    /// <summary>
    /// Opens all given chests in sequence.
    /// </summary>
    /// <param name="chests"></param>
    public void OpenChestsInSequence(List<GPStoreChestSO> chests)
    {
        StartCoroutine(IEOpenChestsInSecuence(chests));
    }

    IEnumerator IEOpenChestsInSecuence(List<GPStoreChestSO> chests)
    {
        for (int i = 0; i < chests.Count; i++)
        {
            OpenChest(chests[i]);
            yield return new WaitForSeconds(3.0f);
            if (i < chests.Count - 1) // so the last one doesn't play a sound at the end
            {
                AudioManager.Instance.Play2D(m_chestOpenSFX);
            }
        }
    }

}
