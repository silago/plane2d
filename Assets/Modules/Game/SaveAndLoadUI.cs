using System;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
namespace Modules.Game
{
    public class SaveAndLoadUI  : MonoBehaviour
    {
        [SerializeField]
        private GameObject content;
        [SerializeField]
        private Image progress;
        [SerializeField]
        private Button newGameButton;
        [SerializeField]
        private Button continueButton;
    
        [SerializeField]
        private string sceneName;
        private SaveAndLoadProvider _saveAndLoadProvider;

        [Inject]
        public void Construct(SaveAndLoadProvider saveProvider)
        {
            _saveAndLoadProvider = saveProvider;
        }

        private void Awake()
        {
            newGameButton.onClick.AddListener(NewGame);
            continueButton.onClick.AddListener(Continue);
        }

        private void NewGame()
        {
            PlayerPrefs.DeleteAll();
            _saveAndLoadProvider.CurrentSaveName = Guid.NewGuid().ToString();
            Launch();
        }

        private void Continue() => Launch();
    
        private void Launch()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            StartCoroutine(Loading(op));
        }

        IEnumerator Loading(AsyncOperation op)
        {
            while (op.isDone == false)
            {
                yield return new WaitForEndOfFrame();
                progress.fillAmount = op.progress;
            }
        
            content.SetActive(false);
        }
    }
}