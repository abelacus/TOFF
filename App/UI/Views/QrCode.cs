using QRCoder;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TOFF.UI.Views;

public class QrCode : View
{
    public QrCode(string text, bool drawBorder = false, bool isSmall = false)
    {
        
        QRCodeData qrCode = QRCodeGenerator.GenerateQrCode(text, QRCodeGenerator.ECCLevel.L);
        if (isSmall)
        {
            int qrCodeLevel = text.Length > 14 ? -4 : text.Length > 6 ? -3 : text.Length > 5 ? -2 :  -1;

            if (text.Length > 21)
            {
                throw new InvalidDataException("Micro QrCode cannot have more than 21 symbols");
            }
            
            qrCode = QRCodeGenerator.GenerateQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: qrCodeLevel);
        }
        var codePrintable = new AsciiQRCode(qrCode).GetGraphicSmall(drawBorder, true);

        Label qrCodeLabel = new Label()
        {
            Text = codePrintable
        };

        Height = (int)Math.Ceiling(codePrintable.Split("\n")[0].Length / 2f);
        Width = codePrintable.Split("\n")[0].Length;
        
        Add(qrCodeLabel);
    }
}