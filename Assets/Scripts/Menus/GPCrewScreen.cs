using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCrewScreen : GPGUIScreen
{
    public List<GPShipDesc> m_shipsData;
    public GPShipCard m_cardPrefab;
    public Transform m_cardContainer;
    public List<Sprite> m_shipTypeImages;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_shipsData.Count; i++)
        {
            GPShipCard card = Instantiate(m_cardPrefab, m_cardContainer);
            card.SetShipImage(m_shipsData[i].m_cardImage);
            card.SetName(m_shipsData[i].m_name);
            card.SetTypeImage(m_shipTypeImages[(int)m_shipsData[i].m_type]);
        }
    }

}
