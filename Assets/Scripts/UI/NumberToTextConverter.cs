using UnityEngine;
using TMPro;

public class NumberToTextConverter : MonoBehaviour {
    public void ConvertNumberToText(float number) {
        GetComponent<TextMeshProUGUI>().text = number.ToString();
    }
}