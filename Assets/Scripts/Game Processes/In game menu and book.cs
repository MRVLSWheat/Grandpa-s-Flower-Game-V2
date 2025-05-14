using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BookMenu : MonoBehaviour
{
    private GameObject bookUI;  // Book UI container
    private Image backgroundImage; // Background image
    private Image leftPage;     // Left page image
    private Image rightPage;    // Right page image

    private int currentPage = 0;  // Track the current page
    private bool isBookOpen = false;  // Track if the book is open

    // Public variables to adjust the positions of the pages and background
    [Header("Position Adjustments")]
    public Vector2 backgroundPosition = new Vector2(0, 0); // Position for the background
    public Vector2 leftPagePosition = new Vector2(-200, 0); // Position for the left page
    public Vector2 rightPagePosition = new Vector2(200, 0); // Position for the right page

    // List for storing left and right page sprites
    [Header("Page Sprites")]
    public List<PageSprites> pageSpritesList = new List<PageSprites>();

    // Public field to allow changing background sprite from Inspector
    [Header("Background Settings")]
    public Sprite customBackgroundSprite;  // Custom background sprite to assign in Inspector

    void Start()
    {
        // Create the book UI container
        bookUI = new GameObject("BookUI");
        bookUI.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        bookUI.AddComponent<CanvasScaler>();
        bookUI.AddComponent<GraphicRaycaster>();

        // Create the background image with adjustable position
        backgroundImage = CreateBackground("Background", backgroundPosition);

        // Create the left page (Image) with adjustable position
        leftPage = CreatePage("LeftPage", leftPagePosition);
        // Create the right page (Image) with adjustable position
        rightPage = CreatePage("RightPage", rightPagePosition);

        // Initially hide the book
        bookUI.SetActive(false);

        // If the pageSpritesList is empty, log a warning to indicate it's missing content
        if (pageSpritesList.Count == 0)
        {
            Debug.LogWarning("PageSprites list is empty. Please add pages.");
        }

        // Set the background sprite
        SetBackgroundSprite();
    }

    void Update()
    {
        // Open/close the book with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isBookOpen = !isBookOpen;
            bookUI.SetActive(isBookOpen);  // Activate the book UI
            Time.timeScale = isBookOpen ? 0 : 1;  // Freeze or unfreeze the game

            // If the book is opened, ensure it starts on the Main Menu page (index 0)
            if (isBookOpen)
            {
                currentPage = 0;  // Always start with the Main Menu page
                UpdatePages();
            }
        }

        // Change pages with Q and E keys, only if there are pages in the list
        if (isBookOpen && pageSpritesList.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchPage(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SwitchPage(1);
            }
        }
    }

    // Switch between pages
    void SwitchPage(int direction)
    {
        if (pageSpritesList.Count > 0)
        {
            currentPage = Mathf.Clamp(currentPage + direction, 0, pageSpritesList.Count - 1);

            // Update page sprites (assign sprite based on current page index)
            UpdatePages();
        }
    }

    // Update pages based on current page index
    void UpdatePages()
    {
        if (pageSpritesList.Count > 0)
        {
            var currentPageSprites = pageSpritesList[currentPage];

            // Assign the sprites directly without placeholders
            leftPage.sprite = currentPageSprites.leftPageSprite;
            rightPage.sprite = currentPageSprites.rightPageSprite;
        }
    }

    // Set the background sprite from the Inspector (if it's assigned)
    void SetBackgroundSprite()
    {
        if (customBackgroundSprite != null)
        {
            backgroundImage.sprite = customBackgroundSprite;  // Set the background sprite
        }
        else
        {
            Debug.LogWarning("No background sprite assigned.");
        }
    }

    // Create page as an Image UI element
    private Image CreatePage(string name, Vector2 position)
    {
        GameObject pageObject = new GameObject(name);
        pageObject.transform.SetParent(bookUI.transform); // Parent it to the book UI
        pageObject.AddComponent<RectTransform>().anchoredPosition = position;

        Image pageImage = pageObject.AddComponent<Image>();
        pageImage.rectTransform.sizeDelta = new Vector2(400, 600); // Set page size
        return pageImage;
    }

    // Create background image (where pages will be placed)
    private Image CreateBackground(string name, Vector2 position)
    {
        GameObject backgroundObject = new GameObject(name);
        backgroundObject.transform.SetParent(bookUI.transform); // Parent it to the book UI
        backgroundObject.AddComponent<RectTransform>().anchoredPosition = position;

        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.rectTransform.sizeDelta = new Vector2(800, 600); // Set background size
        return backgroundImage;
    }

    // Class to store left and right page sprites for each page
    [System.Serializable]
    public class PageSprites
    {
        public Sprite leftPageSprite;  // Sprite for the left page
        public Sprite rightPageSprite; // Sprite for the right page
    }
}
