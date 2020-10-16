// https://stackoverflow.com/questions/38673629/how-to-disable-f1-key

var LibraryPreventDefaultF1 = {
    PreventDefaultF1: function()
    {
        window.addEventListener("keydown", function (e) {
            // 112 => F1, 113 => F2, ...
            if (e.keyCode >= 112 && e.keyCode <= 123) {
                e.preventDefault();
            }
        })
    }
};
mergeInto(LibraryManager.library, LibraryPreventDefaultF1);
