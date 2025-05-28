using UnityEngine;
using System.Collections;
using TMPro;

public class S_GAGP_Subtitles_Typer : MonoBehaviour
{
    public TextMeshProUGUI textField; // O TextMeshProUGUI onde o texto será "digitado"

    // Este método será chamado pelo S_GAGP_Subtitles_Controller
    public IEnumerator TypeText(string fullText, float delayPerChar)
    {
        if (textField == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Typer: textField (TextMeshProUGUI) não está atribuído!");
            yield break; // Interrompe a coroutine se não houver campo de texto
        }

        textField.text = ""; // Limpa o texto anterior
        foreach (char c in fullText)
        {
            textField.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }
    }
}