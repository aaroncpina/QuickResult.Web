// Blazor JS initializer. Each prerendered page ships its own head tags between two comment
// markers. Once the app is running it renders the head itself (and keeps it current as the
// visitor navigates), so the prerendered copies are removed to avoid stale duplicates.
export function afterWebAssemblyStarted() {
    const isMarker = (node, text) => node.nodeType === Node.COMMENT_NODE && node.data === text;
    const nodes = Array.from(document.head.childNodes);
    const start = nodes.findIndex(node => isMarker(node, 'prerender:head'));
    const end = nodes.findIndex(node => isMarker(node, '/prerender:head'));

    if (start < 0 || end < start) {
        return;
    }

    nodes.slice(start, end + 1).forEach(node => node.remove());
}
