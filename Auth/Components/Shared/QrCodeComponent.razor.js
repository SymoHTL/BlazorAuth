export function onUpdate() {
    const qrCode = document.getElementById("qrCode");
    if (!qrCode) return;
    const uri = document.getElementById("qrCodeData").getAttribute('data-url');
    const width = document.getElementById("qrCodeData").getAttribute('data-width');
    const height = document.getElementById("qrCodeData").getAttribute('data-height');
    new QRCode(qrCode,{
        text: uri,
        width: width,
        height: height
    });
}