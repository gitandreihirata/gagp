using UnityEngine;

public class S_GAGP_GamePlayAdjustments_TriggerSetter : MonoBehaviour
{
    public string newActionName = "Jump";

    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<S_GAGP_GamePlayAdjustments_OneButtonController>();
        if (controller != null)
        {
            controller.SetAction(newActionName);
            Debug.Log("[GAGP Trigger] A��o alterada para: " + newActionName);
        }
    }

    // Opcional: reseta a a��o ao sair
    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponent<S_GAGP_GamePlayAdjustments_OneButtonController>();
        if (controller != null)
        {
            controller.SetAction("None");
            Debug.Log("[GAGP Trigger] A��o resetada para: None");
        }
    }
}
