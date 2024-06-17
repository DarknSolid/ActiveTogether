window.LoadVideo3 = (bytes, contentType, id) => {
    if (bytes == null) {
        console.error("Bytes was null");
        return;
    }
    const blob = new Blob([bytes.buffer], { type: contentType });
    var dataUrl = window.URL.createObjectURL(blob);
    var video = document.getElementById(id);
    video.src = dataUrl;
}