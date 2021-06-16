#region
using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Modules.Common;
using Modules.Inventory;
using Modules.Resources;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
#endregion
namespace Modules.GameFlow.Screens
{
    public class CheatMenuScreen : BaseScreen
    {

        public const string Root = "root";
        [SerializeField]
        private TextButton buttonPrefab;
        [SerializeField]
        private List<EventContainer> events = new List<EventContainer>();
        [SerializeField]
        private Button openButton;
        private readonly Dictionary<string, List<TextButton>> _tree = new Dictionary<string, List<TextButton>> {
            {
                Root, new List<TextButton>()
            }
        };
        private InventoryDataProvider _inventoryDataProvider;
        private UserDataProvider _userDataProvider;
        private ResourceSettings _resourceSettings;
        public string CurrentDir { get; private set; } = Root;


        [Inject]
        void Construct(
            InventoryDataProvider inventoryDataProvider,
            UserDataProvider userDataProvider,
            ResourceSettings resourceSettings
            )
        {
            
            _inventoryDataProvider = inventoryDataProvider;
            _userDataProvider = userDataProvider;
            _resourceSettings = resourceSettings;
        }

        private void Start()
        {
            foreach (var item in events)
                Add(item.name, item.@event.Invoke, item.path);

            Add("Close", () =>
            {
                this.SendEvent(new ChangeScreenState() {
                    Active = false
                },ScreenId);
            });
            openButton.onClick.AddListener( () => this.OnChangeScreenStateMessage(new ChangeScreenState() {
                    Active = true
            }));
            AddCheats();
        }

        protected override void OnShow()
        {
            Open(CurrentDir);
        }
        public void Open(string path = "")
        {
            if (string.IsNullOrEmpty(path)) path = Root;
            foreach (var item in _tree[CurrentDir])
                item.gameObject.SetActive(false);
            foreach (var item in _tree[path])
                item.gameObject.SetActive(true);
            CurrentDir = path;
        }

        /// <summary>
        /// without root
        /// do not check nested 
        /// </summary>
        public void RemoveDir(string path)
        {
            path = (string.IsNullOrEmpty(path) ? Root : $"{Root}/{path}").Trim('/');
            var pathParts = path.Split('/');
            var parent = string.Join("/", pathParts.Take(pathParts.Length - 1));

            var dir = _tree[path];
            foreach (var item in dir) Destroy(item.gameObject);
            _tree.Remove(path);
            _tree[parent].RemoveAll(x => x.name == pathParts.Last());
        }

        /// <summary>
        /// with root
        /// </summary>
        private void AddDir(string path)
        {
            var pathParts = path.Split('/');
            Assert.IsTrue(pathParts.Length > 1);

            var parent = string.Join("/", pathParts.Take(pathParts.Length - 1));
            var dirButton = Instantiate(buttonPrefab, Content.transform);
            dirButton.Text = pathParts.Last();
            dirButton.gameObject.name = pathParts.Last();
            dirButton.button.onClick.AddListener(() => Open(path));
            if (_tree.ContainsKey(parent) == false) AddDir(parent);
            _tree[parent].Add(dirButton);
            _tree[path] = new List<TextButton>();

            var backButton = Instantiate(buttonPrefab, Content.transform);
            backButton.name = "back";
            backButton.Text = "<";
            backButton.button.onClick.AddListener(() => Open(parent));
            _tree[path].Add(backButton);
        }
        /// <summary>
        /// without root
        /// </summary>
        public TextButton Add(string title, Action action, string path = "")
        {
            path = path.Trim('/');
            if (path == Root) path = "";
            var stringPath = (string.IsNullOrEmpty(path) ? Root : $"{Root}/{path}").Trim('/');

            if (!_tree.ContainsKey(stringPath)) AddDir(stringPath);

            var btn = Instantiate(buttonPrefab, Content.transform);
            btn.name = title;
            btn.Text = title;
            btn.button.onClick.AddListener(() => action());
            _tree[stringPath].Add(btn);
            return btn;
        }
        [Serializable]
        private class EventContainer
        {
            public string name;
            public string path;
            public UnityEvent @event;
        }
        protected override ScreenId ScreenId => ScreenId.Cheats;


        void AddCheats()
        {
            this.Add("Show Map", () =>
            {
                this.SendEvent(new ChangeScreenState() {
                    Active = true
                }, ScreenId.Map);
            });
            
            this.Add("Hide Map", () =>
            {
                this.SendEvent(new ChangeScreenState() {
                    Active = false
                }, ScreenId.Map);
            });
            
            this.Add("Show Inventory", () =>
            {
                this.SendEvent(new ChangeScreenState() {
                    Active = true
                }, ScreenId.Inventory);
            });
            
            this.Add("Hide Inventory", () =>
            {
                this.SendEvent(new ChangeScreenState() {
                    Active = false
                }, ScreenId.Inventory);
            });


            foreach (var res in _resourceSettings.data)
            {
                this.Add($"Add {res.Name}", () =>
                {
                    _userDataProvider[res.Id] ++ ;
                    _inventoryDataProvider.OnUpdate();
                },"Resources");
            }
            
        }
    }
}
