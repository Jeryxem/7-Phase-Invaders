using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [field: SerializeField] public PlayerController playerController { get; private set; }
    [field: SerializeField] public TextMeshProUGUI dialogueBox { get; private set; }
    [field: SerializeField] public TextMeshProUGUI playerHP { get; private set; }
    [field: SerializeField] public TextMeshProUGUI bossHP { get; private set; }
    [field: SerializeField] public GameObject weaponSelectionUI { get; private set; }
    [field: SerializeField] public TextMeshProUGUI leftPowerUpChoice { get; private set; }
    [field: SerializeField] public TextMeshProUGUI rightPowerUpChoice { get; private set; }
    string[] powerUpChoice = {"Side Gunner", "Laser Head", "Increased Firing Rate", "HP+1", "Freeze Gun"};
    private int leftChoiceBackup, rightChoiceBackup; //temporary store selection
    [field: SerializeField] public TextMeshProUGUI phase { get; private set; }
    [field: SerializeField] public GameObject fadeoutPanel { get; private set; }

    private void Awake() {
        playerController = GameObject.Find("PlayerTest").GetComponent<PlayerController>();
        phase.text = "PHASE:1";
    }

    public void ShowDialogue(string message) {
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.text = message;

        Invoke("DisableDialogue", 1f);
    }

    public void DisableDialogue() {
        dialogueBox.gameObject.SetActive(false);
    }

    public void ShowWeaponSelectionUI(int leftChoice, int rightChoice) {
        // 1 = Side Gunner, 2 = Laser Head, 3 = Increased Firing Rate, 4 = HP+1, 5 = Freeze Gun
        leftPowerUpChoice.text = powerUpChoice[leftChoice-1];
        rightPowerUpChoice.text = powerUpChoice[rightChoice-1];

        leftChoiceBackup = leftChoice;
        rightChoiceBackup = rightChoice;

        weaponSelectionUI.SetActive(true);
    }

    public void HideWeaponSelectionUI() {
        weaponSelectionUI.SetActive(false);
    }

    public void SelectLeftPowerUp() {
        playerController.GainPowerUp(leftChoiceBackup);
    }

    public void SelectRightPowerUp() {
        playerController.GainPowerUp(rightChoiceBackup);
    }
}
