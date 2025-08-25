using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject traitUI; // 특성창 오브젝트
    private bool isPlayerInRange = false;
    private bool isUIOpen = false;

    void Update()
    {
        // 열기 (V키)
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.V) && !isUIOpen)
        {
            traitUI.SetActive(true);
            Time.timeScale = 0f;
            isUIOpen = true;
        }

        // 닫기 (ESC 또는 X 키)
        if (isUIOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X)))
        {
            traitUI.SetActive(false);
            Time.timeScale = 1f;
            isUIOpen = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}