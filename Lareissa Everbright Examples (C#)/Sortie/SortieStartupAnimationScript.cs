using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieStartupAnimationScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public GameObject equipmentButtonPanelReference;
    public GameObject enemyPreviewReference;
    public Image backgroundReference;
    public Sprite backgroundOpenedSprite;
    public GameObject selectionCounterReference;

    public float chestOpenDelay = 1;
    public float equipmentRevealDelay = 0.06f;

    private AudioManagerScript audioManagerReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        
        // Go ahead and start the coroutine!
        StartCoroutine(SortieStartupAnimationCoroutine());
	}
	
	private IEnumerator SortieStartupAnimationCoroutine()
    {
        // Wait a bit first
        yield return new WaitForEndOfFrame();

        audioManagerReference = FindObjectOfType<AudioManagerScript>();

        // Need to first turn everything off
        enemyPreviewReference.SetActive(false);
        selectionCounterReference.SetActive(false);

        // Then wait for delay before opening chest
        yield return new WaitForSeconds(chestOpenDelay);

        // Play sfx
        audioManagerReference.PlayUISFX("ChestOpen");

        // Wait for 0.3 secs for the chest to open
        yield return new WaitForSeconds(0.31f);

        // Open the chest
        backgroundReference.sprite = backgroundOpenedSprite;

        // Play particles
        backgroundReference.GetComponent<ParticleSystem>().Play();

        // Start revealing all of the equipment
        for (int i = 0; i < equipmentButtonPanelReference.transform.childCount; i++)
        {
            equipmentButtonPanelReference.transform.GetChild(i).GetComponent<Animator>().Play("EquipmentButtonFadeIn");

            // Play reveal sfx
            audioManagerReference.PlayUISFX("EquipmentReveal");

            // Wait until next equipment ready to reveal
            yield return new WaitForSeconds(equipmentRevealDelay);
        }

        // Finally reveal the enemy preview
        enemyPreviewReference.SetActive(true);
        selectionCounterReference.SetActive(true);

        // Murder thyself
        Destroy(gameObject);
    }
}
