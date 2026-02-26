using Platformer.Mechanics;
using Platformer.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer.UI
{
    /// <summary>
    /// The MetaGameController is responsible for switching control between the high level
    /// contexts of the application, eg the Main Menu and Gameplay systems.
    /// </summary>
    public class MetaGameController : MonoBehaviour
    {
        /// <summary>
        /// The main UI object which used for the menu.
        /// </summary>
        public MainUIController mainMenu;

        /// <summary>
        /// A list of canvas objects which are used during gameplay (when the main ui is turned off)
        /// </summary>
        public Canvas[] gamePlayCanvasii;

        /// <summary>
        /// The game controller.
        /// </summary>
        public GameController gameController;

        bool showMainCanvas = false;
        private InputAction m_MenuAction;

        void OnEnable()
        {
            _ToggleMainMenu(showMainCanvas);
            m_MenuAction = InputSystem.actions.FindAction("Player/Menu");
        }

        /// <summary>
        /// Turn the main menu on or off.
        /// </summary>
        /// <param name="show"></param>
        public void ToggleMainMenu(bool show)
        {
            if (this.showMainCanvas != show)
            {
                _ToggleMainMenu(show);
            }
        }

        void _ToggleMainMenu(bool show)
        {
            if (mainMenu != null)
            {
                mainMenu.gameObject.SetActive(true);
                mainMenu.SetActivePanel(show ? 0 : 1);
            }
            Time.timeScale = show ? 0 : 1;
            if (gamePlayCanvasii != null)
            {
                foreach (var i in gamePlayCanvasii)
                {
                    if (i != null) i.gameObject.SetActive(!show);
                }
            }
            this.showMainCanvas = show;
        }

        void Update()
        {
            if (m_MenuAction != null && m_MenuAction.WasPressedThisFrame())
                ToggleMainMenu(show: !showMainCanvas);
        }

    }
}
