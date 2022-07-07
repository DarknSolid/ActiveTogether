// Observes an element and notifies when it becomes visible in the viewport.
// Used for scroll with pagination
// Documentation: https://www.youtube.com/watch?v=n499hm7Zajo
let previousEntryTimestamp = null;
window.Observer = {
    observer: null,
    Initialize: function (component, oberserverTargetId) {
        this.observer = new IntersectionObserver(e => {
            const entry = e[0];
            // a cheacky and bad way to check if the time between this
            // and last event is greater than 500 milliseconds to avoid double events:
            if (previousEntryTimestamp === null || entry.time > previousEntryTimestamp + 500) {
                previousEntryTimestamp = entry.time;
                component.invokeMethodAsync('OnIntersection');
            }
        });

        let element = document.getElementById(oberserverTargetId);
        if (element == null) throw new Error("Target was not found");
        this.observer.observe(element);
    }
}