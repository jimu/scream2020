// https://forum.unity.com/threads/enable-fullscreen-webgl.441174/?_ga=2.241484225.254271622.1594934385-1297830507.1584768526

var LibraryPageTool = {
    GoFullscreen: function()
    {
        var viewFullScreen = document.getElementById('#canvas');
 
        var ActivateFullscreen = function()
        {
            if (viewFullScreen.requestFullscreen) /* API spec */
            {  
                viewFullScreen.requestFullscreen();
            }
            else if (viewFullScreen.mozRequestFullScreen) /* Firefox */
            {
                viewFullScreen.mozRequestFullScreen();
            }
            else if (viewFullScreen.webkitRequestFullscreen) /* Chrome, Safari and Opera */
            {  
                viewFullScreen.webkitRequestFullscreen();
            }
            else if (viewFullScreen.msRequestFullscreen) /* IE/Edge */
            {  
                viewFullScreen.msRequestFullscreen();
            }
 
            viewFullScreen.removeEventListener('touchend', ActivateFullscreen);
            viewFullScreen.removeEventListener('mouseup', ActivateFullscreen);
        }
 
        viewFullScreen.addEventListener('touchend', ActivateFullscreen, false);
        viewFullScreen.addEventListener('mouseup', ActivateFullscreen, false);
    }
};
mergeInto(LibraryManager.library, LibraryPageTool);
