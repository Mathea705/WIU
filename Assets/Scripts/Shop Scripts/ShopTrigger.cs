using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera shopCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject shopIcon;
    [SerializeField] private GameObject[] hideOnShopOpen;

    [SerializeField] private GameObject  dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private string  firstLine   = "Welcome, traveller!";
    [SerializeField] private string  repeatLine  = "See anything you like?";
    [SerializeField] private float  typeSpeed = 0.05f;

    [SerializeField] private GameObject shopContent;
    [SerializeField] private float cameraRotateDuration = 0.5f;
    [SerializeField] private AurumManager AurumManager;

    private bool _inRange;
    private bool _shopOpen;
    private bool _hasVisited;
    private Quaternion _shopCameraStartRotation;

    private void Start()
    {
        _shopCameraStartRotation = shopCamera.transform.localRotation;
    }

    private void Update()
    {
        if ((_inRange || _shopOpen) && Input.GetKeyDown(KeyCode.E))
        {
            if (!_shopOpen) OpenShop();
            else  CloseShop();
        }
    }

    private void OpenShop()
    {
        _shopOpen = true;
        shopIcon.SetActive(false);
        shopCamera.gameObject.SetActive(true);
        playerController.enabled = false;
        foreach (GameObject panel in hideOnShopOpen)
            panel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        dialogueBox.SetActive(true);
        shopContent.SetActive(false);
        StartCoroutine(PlayDialogue());
    }

    private void CloseShop()
    {
        _shopOpen = false;
        StopAllCoroutines();
        shopCamera.gameObject.SetActive(false);
        shopCamera.transform.localRotation = _shopCameraStartRotation;
        playerController.enabled = true;
        foreach (GameObject panel in hideOnShopOpen)
            panel.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        dialogueBox.SetActive(false);
        shopContent.SetActive(false);
    }

    private IEnumerator PlayDialogue()
    {
        dialogueText.text = "";
        var typeWait = new WaitForSeconds(typeSpeed);
        string line = _hasVisited ? repeatLine : firstLine;
         if (!_hasVisited)
        {
             AurumManager.AddAurum(200);
        }
        
        _hasVisited = true;

       
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return typeWait;
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(RotateCamera());
        dialogueBox.SetActive(false);
        shopContent.SetActive(true);
    }

    private IEnumerator RotateCamera()
    {
        Quaternion startRot = shopCamera.transform.localRotation;
        Quaternion endRot   = startRot * Quaternion.Euler(0f, 30f, 0f);
        float elapsed = 0f;
        while (elapsed < cameraRotateDuration)
        {
            elapsed += Time.deltaTime;
            shopCamera.transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / cameraRotateDuration);
            yield return null;
        }
        shopCamera.transform.localRotation = endRot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopIcon.SetActive(true);
            _inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !_shopOpen)
            shopIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopIcon.SetActive(false);
            _inRange = false;
        }
    }
}
