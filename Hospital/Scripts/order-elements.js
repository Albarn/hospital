//sorts items in containers
function orderElements(parentsSelector, dataProperty) {
    var containers = document.querySelectorAll(parentsSelector);
    for (var i = 0; i < containers.length; i++) {
        var container = containers[i];
        //get container children, sort by dataProperty
        var elements = Array.from(container.children);

        elements = elements.sort((a, b) =>
            a.dataset[dataProperty].localeCompare(b.dataset[dataProperty]));

        //clear container content, appent sorted elements
        container.innerHTML = "";
        for (var j = 0; j < elements.length; j++) {
            container.append(elements[j]);
        }
    }
}