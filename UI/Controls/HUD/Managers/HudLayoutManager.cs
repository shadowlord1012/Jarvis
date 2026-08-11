using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Managers
{
    public class HudLayoutManager
    {
        private readonly HudConfigurationLoader _loader;


        private HudLayoutConfiguration _layout;



        public HudLayoutManager(
            HudConfigurationLoader loader)
        {
            _loader = loader;
        }



        public async Task LoadAsync()
        {
            _layout =
                await _loader.LoadAsync();
        }



        public HudLayoutConfiguration Layout
        {
            get
            {
                return _layout;
            }
        }




        public HudPanelDefinition GetPanel(
            string id)
        {

            foreach (var panel in _layout.Panels)
            {
                if (panel.Id == id)
                    return panel;
            }


            return null;
        }





        public void UpdatePanelPosition(
            string id,
            double x,
            double y)
        {

            var panel =
                GetPanel(id);


            if (panel == null)
                return;


            panel.Position.X = x;

            panel.Position.Y = y;
        }




        public void UpdatePanelSize(
            string id,
            double width,
            double height)
        {

            var panel =
                GetPanel(id);


            if (panel == null)
                return;


            panel.Width = width;

            panel.Height = height;
        }




        public async Task SaveAsync()
        {
            await _loader.SaveAsync(
                _layout);
        }

    }
}
