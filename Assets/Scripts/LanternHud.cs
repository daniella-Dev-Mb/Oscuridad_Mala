using UnityEngine;
using UnityEngine.UI;

public class LanternHud : MonoBehaviour
{
    public PlayerController player;
    public Canvas hudCanvas;
    public Image chargeFill;
    public Text chargeLabel;
    public RectTransform pickupPrompt;
    public Text pickupLabel;
    public Vector3 promptOffset = Vector3.up;
    public string collectMessage = "Pulsa E para agarrar";
    public string fullMessage = "Linterna llena";

    float fullBarWidth;

    void Awake()
    {
        fullBarWidth = chargeFill.rectTransform.rect.width;
        chargeFill.sprite = null;
        chargeFill.type = Image.Type.Simple;
    }

    void LateUpdate()
    {
        bool visible = player != null && player.isActiveAndEnabled &&
            !player.IsDefeated && player.lantern != null;
        hudCanvas.enabled = visible;
        if (!visible) return;

        chargeFill.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fullBarWidth * Mathf.Clamp01(player.LanternCharge));
        chargeFill.color = player.lanternColor;
        chargeLabel.text = $"{player.lanternHudLabel}  {player.LanternCharge:P0}";

        FuelPickup fuel = player.NearbyFuel;
        Camera camera = player.PlayerCamera;
        bool showPrompt = fuel != null && camera != null && Time.timeScale > 0f;
        pickupPrompt.gameObject.SetActive(showPrompt);
        if (!showPrompt) return;

        Vector3 screen = camera.WorldToScreenPoint(fuel.transform.position + promptOffset);
        showPrompt = screen.z > 0f && screen.x >= 0f && screen.x <= Screen.width &&
            screen.y >= 0f && screen.y <= Screen.height;
        pickupPrompt.gameObject.SetActive(showPrompt);
        if (!showPrompt) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)hudCanvas.transform, screen, null, out Vector2 position);
        pickupPrompt.anchoredPosition = position;
        pickupLabel.text = player.LanternCharge >= 1f ? fullMessage : collectMessage;
    }
}
