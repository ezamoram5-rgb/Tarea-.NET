const instances = new Map();
export function show(id) {
    const element = document.getElementById(id);
    const trigger = document.activeElement;
    const modal = bootstrap.Modal.getOrCreateInstance(element, { backdrop: "static", keyboard: false });
    element.addEventListener("hidden.bs.modal", () => {
        if (trigger?.isConnected) trigger.focus();
    }, { once: true });
    instances.set(id, modal);
    modal.show();
}
export function hide(id) {
    instances.get(id)?.hide();
}

