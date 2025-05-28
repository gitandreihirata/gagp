using UnityEngine;
using UnityEngine.InputSystem;
using GAGP.Models;
using GAGP.Modules.GamePlayAdjustments.Manager;
using GAGP.Serialization;
using System;

public class S_GAGP_GamePlayAdjustments_OneButtonController : MonoBehaviour
{
    [Header("Input System")]
    public PlayerInput playerInput;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 10f;

    public string settingsPath = "Settings/gameplay.json";

    private GamePlayAdjustmentManager manager;
    private GameplaySettings settings;

    private InputAction oneButtonAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction shootAction;
    private InputAction interactAction;

    private Action<InputAction.CallbackContext> handleOneButton;
    private Action<InputAction.CallbackContext> handleJump;
    private Action<InputAction.CallbackContext> handleAttack;
    private Action<InputAction.CallbackContext> handleShoot;
    private Action<InputAction.CallbackContext> handleInteract;

    private void Awake()
    {
        manager = new GamePlayAdjustmentManager();

        settings = SettingsSaver.Load<GameplaySettings>(Application.dataPath + "/" + settingsPath)
                   ?? new GameplaySettings();

        manager.LoadSettings(settings);
        manager.ApplyAll();

        SetupControls(settings.OneButtonModeEnabled);
    }

    public void SetOneButtonEnabled(bool enabled)
    {
        settings.OneButtonModeEnabled = enabled;
        SetupControls(enabled);
    }

    private void SetupControls(bool oneButton)
    {
        // Limpa ações anteriores
        oneButtonAction?.Disable();
        jumpAction?.Disable();
        attackAction?.Disable();
        shootAction?.Disable();
        interactAction?.Disable();

        if (handleOneButton != null && oneButtonAction != null)
            oneButtonAction.performed -= handleOneButton;
        if (handleJump != null && jumpAction != null)
            jumpAction.performed -= handleJump;
        if (handleAttack != null && attackAction != null)
            attackAction.performed -= handleAttack;
        if(handleShoot != null && shootAction != null)
            shootAction.performed -= handleShoot;
        if (handleInteract != null && interactAction != null)
            interactAction.performed -= handleInteract;

        if (oneButton)
        {
            oneButtonAction = playerInput.actions["Jump"]; // espaço
            handleOneButton = ctx => ExecuteContextualAction();
            oneButtonAction.performed += handleOneButton;
            oneButtonAction.Enable();

            Debug.Log("[GAGP] One Button ATIVADO");
        }
        else
        {
            jumpAction = playerInput.actions["Jump"];
            attackAction = playerInput.actions["Attack"];
            shootAction = playerInput.actions["Shoot"];
            interactAction = playerInput.actions["Interact"];

            handleJump = ctx => Jump();
            handleAttack = ctx => Melee();
            handleShoot = ctx => Shoot();
            handleInteract = ctx => Interact();

            jumpAction.performed += handleJump;
            attackAction.performed += handleAttack;
            shootAction.performed += handleShoot;
            interactAction.performed += handleInteract;

            jumpAction.Enable();
            attackAction.Enable();
            shootAction.Enable();
            interactAction.Enable();

            Debug.Log("[GAGP] One Button DESATIVADO – usando Jump, Attack, Interact");
        }
    }

    private void ExecuteContextualAction()
    {
        if (!settings.OneButtonModeEnabled) return;

        switch (settings.ActionName)
        {
            case "Jump": Jump(); break;
            case "Melee": Melee(); break;
            case "Shoot": Shoot(); break;
            case "Interact": Interact(); break;
            default:
                Debug.LogWarning("[GAGP] Ação desconhecida: " + settings.ActionName);
                break;
        }
    }

    private void Jump()
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            Debug.Log("[GAGP] Pulou");
        }
    }

    private void Melee()
    {
        Debug.Log("[GAGP] Ataque corpo-a-corpo");
        // adicionar dano, animação etc.
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[GAGP] Prefab ou firePoint ausente.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }

        Debug.Log("[GAGP] Projétil disparado.");
    }

    private void Interact()
    {
        Debug.Log("[GAGP] Interação acionada.");
    }

    public void SetAction(string newAction)
    {
        settings.ActionName = newAction;
    }

    public void SaveSettings()
    {
        var path = Application.dataPath + "/" + settingsPath;
        var current = manager.SaveSettings();
        SettingsSaver.Save(path, current);
    }
}
