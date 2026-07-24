using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Managers
{
    public class HudEditorManager
    {

        private readonly HudLayoutManager _layout;



        public bool EditModeEnabled
        {
            get;
            private set;
        }




        public HudEditorManager(
            HudLayoutManager layout)
        {
            _layout = layout;
        }





        public void EnableEditMode()
        {
            EditModeEnabled = true;
        }





        public void DisableEditMode()
        {
            EditModeEnabled = false;
        }





        public void Toggle()
        {
            EditModeEnabled =
                !EditModeEnabled;
        }




        public void UpdatePosition(
            string panelId,
            double x,
            double y)
        {

            if (!EditModeEnabled)
                return;


            _layout.UpdatePanelPosition(
                panelId,
                x,
                y);
        }

    }
}
