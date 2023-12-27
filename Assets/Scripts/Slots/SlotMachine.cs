using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Network.Responses;

public class SlotMachine : MonoBehaviour
{
    public static int Hammers = 0;
    public Button spinButton;
    public Image slot1, slot2, slot3;

    private bool isSpinning = false;

    // Add your different slot items here
    public Sprite[] slotItems;

    // Adjust the weights for each slot item
    public float[] itemWeights;

    private float speed = 0.1f; // Initial spin speed
    private float spinSpeed = 500f; // Initial spin speed
    private float spinAcceleration = 10f; // Acceleration to slow down the spin
    private float minSpinSpeed = 5f; // Minimum spin speed
    private float spinTime = 10f; // Adjust spin time as needed
    [SerializeField] private bool _onlyHammers = false;

    void Start()
    {
        spinButton.onClick.AddListener(SpinButtonClick);
    }

    void SpinButtonClick()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinSlots());
        }
    }

    IEnumerator SpinSlots()
    {
        isSpinning = true;

        float elapsedTime = 0f;

        while (elapsedTime < spinTime)
        {
            // Change slot images rapidly to create a spinning effect
            slot1.sprite = GetRandomSlotItem();
            slot2.sprite = GetRandomSlotItem();
            slot3.sprite = GetRandomSlotItem();

            // Gradually slow down the spin speed
            spinSpeed = Mathf.Max(minSpinSpeed, spinSpeed - spinAcceleration * Time.deltaTime);

            // Rotate the slots
            float rotationAmount = spinSpeed * Time.deltaTime;
            slot1.rectTransform.Rotate(Vector3.forward * rotationAmount);
            slot2.rectTransform.Rotate(Vector3.forward * rotationAmount);
            slot3.rectTransform.Rotate(Vector3.forward * rotationAmount);

            elapsedTime++;
            yield return new WaitForSeconds(speed);
        }

        // Ensure the slots are aligned properly when spinning stops
        AlignSlots();

        // Spin finished, determine the result
        DetermineResult();

        isSpinning = false;
    }

    void AlignSlots()
    {
        // Align the slots properly after spinning stops
        slot1.rectTransform.rotation = Quaternion.identity;
        slot2.rectTransform.rotation = Quaternion.identity;
        slot3.rectTransform.rotation = Quaternion.identity;
    }

Sprite GetRandomSlotItem()
{
    float totalWeight = 0f;

    // Calculate total weight
    foreach (float weight in itemWeights)
    {
        totalWeight += weight;
    }

    // Generate a random value between 0 and total weight
    float randomValue = Random.Range(0f, totalWeight);

    // Find the slot item based on cumulative probabilities
    float cumulativeProbability = 0f;

    for (int i = 0; i < slotItems.Length; i++)
    {
        cumulativeProbability += itemWeights[i];

        if (randomValue <= cumulativeProbability)
        {
            return _onlyHammers? slotItems[1]: slotItems[i];
        }
    }

    // Fallback to a random item if something goes wrong
    return slotItems[Random.Range(0, slotItems.Length)];
}



    void DetermineResult()
    {
        // Here, you can implement logic to determine the result based on the final slot items.
        // For example, check if all three slots have the same item to declare a win.

        // Call a method and pass the items won
        DisplayResult(slot1.sprite, slot2.sprite, slot3.sprite);
    }

void DisplayResult(Sprite item1, Sprite item2, Sprite item3)
{
    int multiplier = 0;

    // Check if all three items are the same
    if (item1 == item2 && item2 == item3)
    {
        // Win even more for getting three of the same item
        multiplier = 5;
    }
    // Check if two items are the same
    else if (item1 == item2 || item2 == item3 || item1 == item3)
    {
        // Win more for getting two of the same item
        multiplier = 2;
    }
if (item1.name == "coini" || item2.name == "coini" || item3.name == "coini")
    {
        multiplier = 1;
    }
    // Check for special items (hammer or bonus)
    if (item1.name == "hammer" && item2.name == "hammer" && item3.name == "hammer")
    {
        // Call a function for hammer win
        HandleHammerWin();
    }
    else if (item1.name == "bonus" && item2.name == "bonus" && item3.name == "bonus")
    {
        // Call a function for bonus win
        HandleBonusWin();
    }
    else
    {
        if(multiplier == 0)
        return;
        // Update player coins based on the multiplier
        GameManager.Instance.UpdatePlayerCoins(100 * multiplier);
    }
}

void HandleHammerWin()
{
    // Implement the logic for winning with a hammer
    // For example, you can show a special animation or reward
    Debug.Log("You won with a hammer!");
SlotMachine.Hammers++;
    NetworkManager.GetRndPlayer(GameManager.Instance.GetLocalPlayerUsername(), OnRndPlayerGot);
}
private void OnRndPlayerGot(bool s, string r)
{
    UserDataResponse userData = JsonUtility.FromJson<UserDataResponse>(r);
    VillageManager.Instance.LoadVillage(userData.UserData.username, VerificationType.Username);
}
void HandleBonusWin()
{
    // Implement the logic for winning with a bonus
    // For example, you can activate a bonus round or reward
    Debug.Log("You won a bonus!");
}

}
