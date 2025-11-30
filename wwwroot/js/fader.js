window.fader = {
    init(root, dotnet) {
        let dragging = false;

        const thumb = root.querySelector(".fader-thumb");
        if (!thumb) return;

        const getPercent = (clientY) => {
            const rect = root.getBoundingClientRect();
            const y = clientY - rect.top;
            const percent = 100 - (y / rect.height) * 100;
            return Math.min(100, Math.max(0, percent));
        };

        const onMove = (e) => {
            if (!dragging) return;   
            e.preventDefault();
            const clientY = e.touches ? e.touches[0].clientY : e.clientY;
            const percent = getPercent(clientY);
            dotnet.invokeMethodAsync("OnDrag", percent);
        };

        const endDrag = () => {
            dragging = false;
            window.removeEventListener("mousemove", onMove);
            window.removeEventListener("mouseup", endDrag);
            window.removeEventListener("touchmove", onMove);
            window.removeEventListener("touchend", endDrag);
        };

        const startDrag = (e) => {
            dragging = true;
            e.preventDefault();

            const clientY = e.touches ? e.touches[0].clientY : e.clientY;
            const percent = getPercent(clientY);
            dotnet.invokeMethodAsync("OnDrag", percent);

            window.addEventListener("mousemove", onMove);
            window.addEventListener("mouseup", endDrag);

            window.addEventListener("touchmove", onMove, { passive: false });
            window.addEventListener("touchend", endDrag);
        };

        thumb.addEventListener("mousedown", startDrag);
        thumb.addEventListener("touchstart", startDrag, { passive: false });
    }
};
