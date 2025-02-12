using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signOutButton; // Add Sign Out button reference here

    private bool rememberMe = false;

    public async void Start()
    {
        await InitializeUnityServices();
        LoadSavedCredentials();
        loginButton.interactable = true; // Enable login button after Unity Services are initialized

        signOutButton.onClick.AddListener(SignOut); // Hook up the SignOut button
        signOutButton.gameObject.SetActive(false); // Hide sign-out button initially
    }

    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        await UnityServices.InitializeAsync();
    }

    public void LoginButtonPressed()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();
        if (username.Length < 3 || password.Length < 6)
        {
            statusText.text = "Username must be 3+ chars & Password 6+ chars!";
            return;
        }

        loginButton.interactable = false; // Disable button to prevent spam clicking
        Login(username, password);
    }

    public async void SignupButtonPressed()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (username.Length < 3 || password.Length < 6)
        {
            statusText.text = "Username must be 3+ chars & Password 6+ chars!";
            return;
        }

        try
        {
            loadingIndicator.SetActive(true);
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            statusText.text = "Signup successful! Now login.";
        }
        catch (AuthenticationException e)
        {
            statusText.text = "Signup failed: " + e.Message;
        }
        finally
        {
            loadingIndicator.SetActive(false);
        }
    }

    private async void Login(string username, string password)
    {
        try
        {
            loadingIndicator.SetActive(true);
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            statusText.text = "Login successful! Loading Main Menu...";

            if (rememberMe)
            {
                PlayerPrefs.SetString("SavedUsername", username);
                PlayerPrefs.SetString("SavedPassword", password);
                PlayerPrefs.SetInt("RememberMe", 1);
            }
            else
            {
                PlayerPrefs.DeleteKey("SavedUsername");
                PlayerPrefs.DeleteKey("SavedPassword");
                PlayerPrefs.SetInt("RememberMe", 0);
            }
            PlayerPrefs.Save();

            await SavePlayerName(username);
            Invoke(nameof(LoadMainMenu), 2f); // Load Main Menu after 2 seconds
        }
        catch (AuthenticationException e)
        {
            statusText.text = "Login failed: " + e.Message;
            loginButton.interactable = true; // Re-enable login button
        }
        finally
        {
            loadingIndicator.SetActive(false);
        }
    }

    private void LoadMainMenu()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void ToggleRememberMe(bool isOn)
    {
        rememberMe = isOn;
    }

    private void LoadSavedCredentials()
    {
        if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
        {
            usernameInput.text = PlayerPrefs.GetString("SavedUsername", "");
            passwordInput.text = PlayerPrefs.GetString("SavedPassword", "");
            rememberMeToggle.isOn = true;
        }
    }

    private async Task SavePlayerName(string playerName)
    {
        try
        {
            var data = new Dictionary<string, object> { { "PlayerName", playerName } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Player name saved to Cloud Save.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save name: " + e.Message);
        }
    }

    // Sign out method to clear session and player preferences
    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        PlayerPrefs.DeleteKey("SavedUsername");
        PlayerPrefs.DeleteKey("SavedPassword");
        PlayerPrefs.SetInt("RememberMe", 0);
        PlayerPrefs.Save();

        statusText.text = "You have been signed out.";
        signOutButton.gameObject.SetActive(false); // Hide the SignOut button after sign-out
        loginButton.gameObject.SetActive(true); // Show the login button again

        // Optionally, you can redirect to the login screen
        SceneManager.LoadScene("LoginScene"); // Change "LoginScene" to your login scene
    }
}
