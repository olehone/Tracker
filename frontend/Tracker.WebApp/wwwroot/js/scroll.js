window.observeElement = (element, dotNetRef) => {
    new IntersectionObserver(e => {
        if (e[0].isIntersecting) {
            dotNetRef.invokeMethodAsync('ScrolledAsync');
        }
    }).observe(element);
};

window.BlazorScrollToBottom = (element) => {
    if (element) element.scrollIntoView({ behavior: 'smooth' });
};