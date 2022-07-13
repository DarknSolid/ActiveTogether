// Observes an element and notifies when it becomes visible in the viewport.
// Used for scroll with pagination
// Documentation: https://www.youtube.com/watch?v=n499hm7Zajo
let previousEntryTimestamp = null;
window.Observer = {
    observer: null,
    components: new Map(),

    Add: function (component, observerTargetId) {

        if (this.observer == null) {
            //console.log("Observer was initialized");
            this.observer = new IntersectionObserver(entries => {

                // console.log("Intersection(s) happened: ");

                entries.forEach(entry => {

                    //console.log("entry: ", entry.target.id)

                    const component = this.components.get(entry.target.id);
                    // a cheacky and bad way to check if the time between this
                    // and last event is greater than 500 milliseconds to avoid double events:
                    if (previousEntryTimestamp === null || entry.time > previousEntryTimestamp + 500) {
                        previousEntryTimestamp = entry.time;
                        component.invokeMethodAsync('OnIntersection');
                    }
                })
            });
        }

        let element = document.getElementById(observerTargetId);
        if (element == null) throw new Error("Target was not found");

        this.components.set(observerTargetId, component);
        //console.log("New component set:")
        //console.log("observerTargetId: ", observerTargetId)
        this.observer.observe(element);
    }
}