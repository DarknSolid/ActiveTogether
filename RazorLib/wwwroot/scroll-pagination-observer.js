// Observes an element and notifies when it becomes visible in the viewport.
// Used for scroll with pagination
// Documentation: https://www.youtube.com/watch?v=n499hm7Zajo
window.Observer = {
    observer: null,
    Initialize: function (component, oberserverTargetId) {
        this.observer = new IntersectionObserver(e => {
            component.invokeMethodAsync('OnIntersection');
        });

        let element = document.getElementById(oberserverTargetId);
        if (element == null) throw new Error("Target was not found");
        this.observer.observe(element);
    }
}