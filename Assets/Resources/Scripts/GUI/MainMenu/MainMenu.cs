using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void PlayTutorial() {
        PlayMenuSound();
        SceneManager.LoadScene("Tutorial");
    }

    public void PlayLevel1() {
        PlayMenuSound();
        SceneManager.LoadScene("Level1");
    }
    
    public void PlayLevel2() {
        PlayMenuSound();
        SceneManager.LoadScene("Dungeon");
    }

    public void PlayLevel3() {
        PlayMenuSound();
        SceneManager.LoadScene("Villages1");
    }

    public void QuitGame() {
        PlayMenuSound();
        Application.Quit();
    }

    public void PlayMenuSound() {
        SoundManager.PlaySound("MenuSound");
    }
}
