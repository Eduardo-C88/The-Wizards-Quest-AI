using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public UnityEngine.UI.Button[] hotbarButtons;
    private int selectedSlot = 0;

    void Start()
    {
        SelectSlot(selectedSlot);
        
        for (int i = 0; i < hotbarButtons.Length; i++)
        {
            int index = i;  // Local copy for closure in the lambda function
            hotbarButtons[i].onClick.AddListener(() => SelectSlot(index));
        }
    }

    void SelectSlot(int index)
    {
        // Reset all button states
        for (int i = 0; i < hotbarButtons.Length; i++)
        {
            ColorBlock cb = hotbarButtons[i].colors;
            cb.normalColor = Color.white;
            hotbarButtons[i].colors = cb;
        }

        // Highlight selected slot
        ColorBlock selectedColors = hotbarButtons[index].colors;
        selectedColors.normalColor = Color.green;
        hotbarButtons[index].colors = selectedColors;

        selectedSlot = index;
    }

    void Update()
    {
        // Keyboard input for selecting specific slots
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
    }
}